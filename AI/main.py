import models
import fastapi
from pydantic import BaseModel
import numpy as np

import torch

from fastapi import BackgroundTasks

model = models.FoosballModel()
# model = torch.load("saves/9787b26b-fdbd-4f62-acea-77b91880aaa0/11223", weights_only=False)
loss = models.PPO(model)

model2 = models.FoosballModel()
# model2 = torch.load(
#     "saves/10589532-fdc5-459c-ae83-810bef35156c/11223", weights_only=False)
loss2 = models.PPO(model2)

app = fastapi.FastAPI()

states = []
actions = []
rewards = []
values = []

states2 = []
actions2 = []
rewards2 = []
values2 = []


class Body(BaseModel):
    data: list[list[list[float]]]
    reward: float


def processData():
    global states, actions, rewards, values

    loss.update(states, actions, rewards, values)

    states = []
    actions = []
    rewards = []
    values = []


def processData2():
    global states2, actions2, rewards2, values2

    loss2.update(states2, actions2, rewards2, values2)

    states2 = []
    actions2 = []
    rewards2 = []
    values2 = []


@app.post('/1')
def getNextUpdate(body: Body, backgroundTask: BackgroundTasks):
    player = np.array(body.data[0], dtype=np.float32)  # 2x4
    other = np.array(body.data[1], dtype=np.float32)  # 2x4
    ball = np.array(body.data[2], dtype=np.float32).reshape((4, 1))  # 1x4
    matrix = np.array([[np.hstack((player, other, ball, ball)),], ])  # 6x4
    actionToTake, value = model(torch.tensor(matrix))  # make the data pretty

    states.append(np.squeeze(matrix, axis=0))
    actions.append(np.squeeze(actionToTake.detach().numpy(), axis=0))
    rewards.append(float(body.reward))
    values.append(value)

    # TODO ver si se puede
    if (len(states) >= loss.batch_size):
        # backgroundTask.add_task(processData)
        processData()

    return np.clip(actionToTake.detach().numpy(), -1, 1).tolist()


@app.post('/2')
def getNextUpdate(body: Body, backgroundTask: BackgroundTasks):
    player = np.array(body.data[0], dtype=np.float32)  # 2x4
    other = np.array(body.data[1], dtype=np.float32)  # 2x4
    ball = np.array(body.data[2], dtype=np.float32).reshape((4, 1))  # 1x4
    matrix = np.array([[np.hstack((player, other, ball, ball)),], ])  # 6x4
    actionToTake, value = model2(torch.tensor(matrix))  # make the data pretty

    states2.append(np.squeeze(matrix, axis=0))
    actions2.append(np.squeeze(actionToTake.detach().numpy(), axis=0))
    rewards2.append(float(body.reward))
    values2.append(value)

    # TODO ver si se puede
    if (len(states2) >= loss2.batch_size):
        # backgroundTask.add_task(processData)
        processData2()

    return np.clip(actionToTake.detach().numpy(), -1, 1).tolist()
