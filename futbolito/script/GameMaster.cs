using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class GameMaster : Node
{
	[Export]
	TurnMannager player1;
	[Export]
	TurnMannager player2;

	[Export]
	BallInit ball;

	[Export]
	Label reward1;
	[Export]
	Label reward2;

	List<float[][]> player1Actions_1 = new(); // player data
	List<float[][]> player1Actions_2 = new(); // other data
	List<float[][]> player1Actions_3 = new(); // ball data

	List<float[][]> player2Actions_1 = new();
	List<float[][]> player2Actions_2 = new();

	float calculateReward(float ballPosX, float ballPosZ, float ballSpeedX, float ballSpeedZ, bool scored, bool failed)
	{
		float reward = 0f;

		reward += ballPosX * 0.5f; // make the ball be on the other's side. Kinda important

		if (reward > 0)
			reward += (1 - Math.Abs(ballPosZ)) * 0.4f; // try and keep the ball center. Not as important
		else
			reward -= (1 - Math.Abs(ballPosZ)) * 0.4f; // try and keep the ball to the sides if on one's side. Not as important

		reward += ballSpeedX * 6; // the most important thing is that the ball goes the direction of the other player! The faster the better. The most important

		if (scored)
		{
			// absolute chad if it can score tho
			reward += 10f;
		}

		if (failed)
		{
			reward -= 10;
		}

		return reward;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		float reward1V = calculateReward(ball.getX(true), ball.getZ(true), ball.getXSpeed(true), ball.getZSpeed(true), ball.scored1, ball.penalized || ball.scored2);
		float reward2V = calculateReward(ball.getX(false), ball.getZ(false), ball.getXSpeed(false), ball.getZSpeed(false), ball.scored2, ball.penalized || ball.scored1);

		reward1.Text = reward1V.ToString();
		reward2.Text = reward2V.ToString();

		// reset flags
		ball.scored1 = false;
		ball.scored2 = false;
		ball.penalized = false;

		// ask for next movements

		// player 1
		var matrix1_1 = player1.getPlayerMatrix();
		var matrix1_2 = player1.getOtherMatrix(player2.characters);
		var matrix1_3 = new float[][] { new float[] { ball.getX(true), ball.getZ(true) }, new float[] { ball.getXSpeed(true), ball.getZSpeed(true) } };

		player1Actions_1.Add(matrix1_1);
		player1Actions_2.Add(matrix1_2);
		player1Actions_3.Add(matrix1_3);

		// create json
		string toSend = JsonSerializer.Serialize(new float[][][] { matrix1_1, matrix1_2, matrix1_3 });

		// TODO send data
		string response = "[[0,0], [0,0], [0,0], [0,0]]";

		// get response
		var decodedResponse = JsonSerializer.Deserialize<float[][]>(response);

		player1.movement_1 = decodedResponse[0][0];
		player1.rotation_1 = decodedResponse[0][1];
		player1.movement_2 = decodedResponse[1][0];
		player1.rotation_2 = decodedResponse[1][1];
		player1.movement_3 = decodedResponse[2][0];
		player1.rotation_3 = decodedResponse[2][1];
		player1.movement_4 = decodedResponse[3][0];
		player1.rotation_4 = decodedResponse[3][1];

	}
}
