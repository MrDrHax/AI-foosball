import models
import fastapi
from pydantic import BaseModel
import numpy as np

from fastapi import BackgroundTasks

model = models.FoosballModel()
loss = models.PPO(model)

app = fastapi.FastAPI()

states = []
actions = []
rewards = []
values = []

class Body(BaseModel):
    data: list[list[list[list[float]]]]
    reward: float

def processData():
    loss.update(states, actions, rewards, values)



@app.post('/')
def getNextUpdate(body: Body, backgroundTask: BackgroundTasks):
    player = np.array(body.data[0])
    other = np.array(body.data[1])
    ball = np.array(body.data[2])
    matrix = np.hstack((player, other, ball))
    actionToTake, value = model([matrix, ]) # make the data pretty

    states.append(matrix)
    actions.append(actionToTake)
    rewards.append(body.reward)
    values.append(value)

    # TODO ver si se puede
    if (len(states) >= loss.batch_size):
        backgroundTask.add_task(processData)

    return actionToTake
