import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
import numpy as np
from uuid import uuid4
import os


class FoosballModel(nn.Module):
    def __init__(self):
        super(FoosballModel, self).__init__()

        # First convolutional layer: input (4x6) -> output (2x3)
        self.conv1 = nn.Conv2d(in_channels=1, out_channels=32, kernel_size=(
            2, 2), stride=(2, 2), padding=0)  # Output: (16, 2, 3)

        # Fully connected layers
        # Flattened size: 32 * 2 * 3 = 192
        self.fc1 = nn.Linear(32 * 2 * 3, 128)
        self.fc2 = nn.Linear(128, 64)
        self.fc3 = nn.Linear(64, 4 * 2)  # Final output size: 4 * 2

        # Flattened size: 32 * 2 * 3
        self.fc_value = nn.Linear(32 * 2 * 3, 128)
        # Output a single value for the state
        self.fc_value_out = nn.Linear(128, 1)

    def forward(self, x):
        # Input shape: (batch_size, 1, 4, 6)
        x = F.relu(self.conv1(x))  # Output shape: (batch_size, 32, 2, 3)

        # Flatten the output for the fully connected layers
        x = x.view(x.size(0), -1)  # Flatten to (batch_size, 32 * 2 * 2)

        # estimate values
        value = F.relu(self.fc_value(x))  # Output shape: (batch_size, 128)
        value = self.fc_value_out(value)   # Output shape: (batch_size, 1)

        # finish movement
        x = F.relu(self.fc1(x))     # Output shape: (batch_size, 128)
        x = F.relu(self.fc2(x))     # Output shape: (batch_size, 64)
        x = self.fc3(x)             # Output shape: (batch_size, 8)

        return x.view(-1, 4, 2), value     # Reshape to (batch_size, 4, 2)


class PPO:
    def __init__(self, model, learning_rate=3e-4, gamma=0.99, epsilon=0.2, epochs=10, batch_size=32):
        self.model = model
        self.optimizer = optim.Adam(self.model.parameters(), lr=learning_rate)
        self.gamma = gamma
        self.epsilon = epsilon
        self.epochs = epochs
        self.batch_size = batch_size

        self.counter = 0
        self.runID = uuid4()

        if not os.path.exists(f'saves/{self.runID}/'):
            os.makedirs(f'saves/{self.runID}/')

    def compute_advantages(self, rewards, values):
        # Compute advantages using Generalized Advantage Estimation (GAE)
        advantages = np.zeros_like(rewards)
        returns = np.zeros_like(rewards)
        running_add = 0
        for t in reversed(range(len(rewards))):
            running_add = rewards[t] + self.gamma * running_add
            returns[t] = running_add
            advantages[t] = returns[t] - values[t]
        return advantages, returns

    def update(self, states, actions, rewards, values):
        # Convert to tensors
        states = torch.FloatTensor(states)
        actions = torch.FloatTensor(actions)
        rewards = torch.FloatTensor(rewards)
        values = torch.FloatTensor(values)

        # Compute advantages and returns
        advantages, returns = self.compute_advantages(
            rewards.numpy(), values.numpy())

        # Convert advantages and returns to tensors
        advantages = torch.FloatTensor(advantages)
        returns = torch.FloatTensor(returns)

        # Normalize advantages
        advantages = (advantages - advantages.mean()) / \
            (advantages.std() + 1e-8)

        # Training loop
        for _ in range(self.epochs):
            # Shuffle data
            indices = np.arange(len(states))
            np.random.shuffle(indices)

            for start in range(0, len(states), self.batch_size):
                end = start + self.batch_size
                batch_indices = indices[start:end]

                # Get batch data
                batch_states = states[batch_indices]
                batch_actions = actions[batch_indices]
                batch_advantages = advantages[batch_indices]
                batch_returns = returns[batch_indices]

                # Get current policy and value estimates
                current_values = self.model(batch_states)
                current_action_probs = self.model(batch_states)

                # Calculate the ratio (pi_theta / pi_theta_old)
                ratio = current_action_probs / (batch_actions + 1e-8)

                # Calculate surrogate loss
                surrogate_loss = ratio * batch_advantages
                clipped_surrogate_loss = torch.clamp(
                    ratio, 1 - self.epsilon, 1 + self.epsilon) * batch_advantages

                # Total loss
                loss = -torch.min(surrogate_loss, clipped_surrogate_loss).mean() + \
                    F.mse_loss(current_values, batch_returns)

                # Update model
                self.optimizer.zero_grad()
                loss.backward()
                self.optimizer.step()

        torch.save(self.model, f'saves/{self.runID}/{self.counter}')
        self.counter += 1
