using Godot;
using System;
using System.Diagnostics;

public partial class Goal : RigidBody3D
{
	[Export]
	BallInit ball;
	[Export]
	bool player1;

	public override void _Ready()
	{
		this.Connect("body_entered", new Callable(this, nameof(Score)));
	}

    public void Score(){
		ball.ScoreGoal( player1 );
		Debug.WriteLine( "Scored!!!" );
	}
}
