using Godot;
using System;

public partial class BallInit : Node3D
{
	[Export]
	RigidBody3D physicsBody3D;

	[Export]
	Vector3 initialPush;

	[Export]
	float initialPushMultiplier;

	Random random = new Random();

	public bool scored2 = false;
	public bool scored1 = false;
	public bool penalized = false;

	public void resetBall()
	{
		physicsBody3D.LinearVelocity = Vector3.Zero;
		physicsBody3D.AngularVelocity = Vector3.Zero;

		physicsBody3D.Position = Vector3.Zero;

		physicsBody3D.ApplyImpulse(initialPush * (float)(random.NextDouble() * 2 - 1) * initialPushMultiplier);


	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resetBall();
		// Engine.TimeScale = 5;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (physicsBody3D.LinearVelocity.Length() <= 0.01)
		{
			resetBall();
			penalized = true;
		}

		if (physicsBody3D.Position.X < -0.567f && physicsBody3D.Position.Z > -0.1f && physicsBody3D.Position.Z < 0.1f)
		{
			ScoreGoal(true);
		}

		if (physicsBody3D.Position.X > 0.567f && physicsBody3D.Position.Z > -0.1f && physicsBody3D.Position.Z < 0.1f)
		{
			ScoreGoal(false);
		}
	}

	public void ScoreGoal(bool player1)
	{
		scored1 = player1;
		scored2 = !player1;

		resetBall();
	}

	public float getX(bool isPlayer1)
	{
		return physicsBody3D.Position.X * 2f * (isPlayer1 ? -1f : 1f);
	}

	public float getZ(bool isPlayer1)
	{
		return physicsBody3D.Position.Z * 3 * (isPlayer1 ? 1f : -1f);
	}

	public float getXSpeed(bool isPlayer1)
	{
		return physicsBody3D.LinearVelocity.X * (isPlayer1 ? -1f : 1f);
	}

	public float getZSpeed(bool isPlayer1)
	{
		return physicsBody3D.LinearVelocity.Z * (isPlayer1 ? 1f : -1f);
	}
}
