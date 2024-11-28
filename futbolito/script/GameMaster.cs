using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
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

	private static System.Net.Http.HttpClient requester = new();

	public override void _Ready()
	{
	}

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
		var matrix1_3 = new float[][] { new float[] { ball.getX(true), ball.getZ(true), ball.getXSpeed(true), ball.getZSpeed(true) } };


		// create json
		string toSend = JsonSerializer.Serialize(
			new Dictionary<string, object>() {
				{ "data", new float[][][] { matrix1_1, matrix1_2, matrix1_3 } },
				{ "reward", reward1V },
			}
		);

		var request = requester.PostAsync("http://127.0.0.1:8000/1", new StringContent(toSend, Encoding.UTF8, "application/json"));

		request.Wait();

		HttpResponseMessage httpResponse = request.Result;

		// Ensure the request was successful
		httpResponse.EnsureSuccessStatusCode();

		var decoder = httpResponse.Content.ReadAsStringAsync();

		decoder.Wait();

		string response = decoder.Result;

		// get response
		var decodedResponse = JsonSerializer.Deserialize<float[][][]>(response);

		player1.movement_1 = decodedResponse[0][0][0];
		player1.rotation_1 = decodedResponse[0][0][1];
		player1.movement_2 = decodedResponse[0][1][0];
		player1.rotation_2 = decodedResponse[0][1][1];
		player1.movement_3 = decodedResponse[0][2][0];
		player1.rotation_3 = decodedResponse[0][2][1];
		player1.movement_4 = decodedResponse[0][3][0];
		player1.rotation_4 = decodedResponse[0][3][1];


		// player 1
		var matrix2_1 = player2.getPlayerMatrix();
		var matrix2_2 = player2.getOtherMatrix(player1.characters);
		var matrix2_3 = new float[][] { new float[] { ball.getX(false), ball.getZ(false), ball.getXSpeed(false), ball.getZSpeed(false) } };


		// create json
		toSend = JsonSerializer.Serialize(
			new Dictionary<string, object>() {
				{ "data", new float[][][] { matrix2_1, matrix2_2, matrix2_3 } },
				{ "reward", reward2V },
			}
		);

		request = requester.PostAsync("http://127.0.0.1:8000/2", new StringContent(toSend, Encoding.UTF8, "application/json"));

		request.Wait();

		httpResponse = request.Result;

		// Ensure the request was successful
		httpResponse.EnsureSuccessStatusCode();

		decoder = httpResponse.Content.ReadAsStringAsync();

		decoder.Wait();

		response = decoder.Result;

		// get response
		decodedResponse = JsonSerializer.Deserialize<float[][][]>(response);

		player2.movement_1 = decodedResponse[0][0][0];
		player2.rotation_1 = decodedResponse[0][0][1];
		player2.movement_2 = decodedResponse[0][1][0];
		player2.rotation_2 = decodedResponse[0][1][1];
		player2.movement_3 = decodedResponse[0][2][0];
		player2.rotation_3 = decodedResponse[0][2][1];
		player2.movement_4 = decodedResponse[0][3][0];
		player2.rotation_4 = decodedResponse[0][3][1];
	}
}
