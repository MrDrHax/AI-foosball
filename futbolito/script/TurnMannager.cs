using Godot;
using System;
using System.Collections.Generic;

public partial class TurnMannager : Node
{
	[Export]
	public CharController[] characters;

	[Export]
	bool player1;

	[Export]
	Label movement;
	[Export]
	Label rotation;


	[Export]
	Label currentMovement_1;
	[Export]
	Label currentRotation_1;
	[Export]
	Label currentMovement_2;
	[Export]
	Label currentRotation_2;
	[Export]
	Label currentMovement_3;
	[Export]
	Label currentRotation_3;
	[Export]
	Label currentMovement_4;
	[Export]
	Label currentRotation_4;

	public float movement_1 = 0;
	public float movement_2 = 0;
	public float movement_3 = 0;
	public float movement_4 = 0;
	public float rotation_1 = 0;
	public float rotation_2 = 0;
	public float rotation_3 = 0;
	public float rotation_4 = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		string baseAction;
		if (player1)
		{
			baseAction = "1_";
		}
		else
		{
			baseAction = "2_";
		}


		if (@event.IsActionPressed($"{baseAction}move_1_1"))
		{
			movement_1 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}move_1_2"))
		{
			movement_1 = -1;
		}
		else
		{
			movement_1 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}move_2_1"))
		{
			movement_2 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}move_2_2"))
		{
			movement_2 = -1;
		}
		else
		{
			movement_2 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}move_3_1"))
		{
			movement_3 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}move_3_2"))
		{
			movement_3 = -1;
		}
		else
		{
			movement_3 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}move_4_1"))
		{
			movement_4 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}move_4_2"))
		{
			movement_4 = -1;
		}
		else
		{
			movement_4 = 0;
		}


		if (@event.IsActionPressed($"{baseAction}rotation_1_1"))
		{
			rotation_1 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}rotation_1_2"))
		{
			rotation_1 = -1;
		}
		else
		{
			rotation_1 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}rotation_2_1"))
		{
			rotation_2 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}rotation_2_2"))
		{
			rotation_2 = -1;
		}
		else
		{
			rotation_2 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}rotation_3_1"))
		{
			rotation_3 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}rotation_3_2"))
		{
			rotation_3 = -1;
		}
		else
		{
			rotation_3 = 0;
		}
		if (@event.IsActionPressed($"{baseAction}rotation_4_1"))
		{
			rotation_4 = 1;
		}
		else if (@event.IsActionPressed($"{baseAction}rotation_4_2"))
		{
			rotation_4 = -1;
		}
		else
		{
			rotation_4 = 0;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (player1)
		{
			characters[0].applyForces(movement_1, rotation_1, Vector3.Forward);
			characters[1].applyForces(movement_2, rotation_2, Vector3.Forward);
			characters[2].applyForces(movement_3, rotation_3, Vector3.Forward);
			characters[3].applyForces(movement_4, rotation_4, Vector3.Forward);
		}
		else
		{

			characters[0].applyForces(movement_1, rotation_1, Vector3.Back);
			characters[1].applyForces(movement_2, rotation_2, Vector3.Back);
			characters[2].applyForces(movement_3, rotation_3, Vector3.Back);
			characters[3].applyForces(movement_4, rotation_4, Vector3.Back);
		}

		currentMovement_1.Text = movement_1.ToString();
		currentRotation_1.Text = rotation_1.ToString();
		currentMovement_2.Text = movement_2.ToString();
		currentRotation_2.Text = rotation_2.ToString();
		currentMovement_3.Text = movement_3.ToString();
		currentRotation_3.Text = rotation_3.ToString();
		currentMovement_4.Text = movement_4.ToString();
		currentRotation_4.Text = rotation_4.ToString();
	}


	public float[][] getPlayerMatrix()
	{
		float multiplier = player1 ? 1 : -1;

		return new float[][] {
			new float[]{characters[0].getPos() * multiplier, characters[0].getTorque() * multiplier},
			new float[]{characters[1].getPos() * multiplier, characters[1].getTorque() * multiplier},
			new float[]{characters[2].getPos() * multiplier, characters[2].getTorque() * multiplier},
			new float[]{characters[3].getPos() * multiplier, characters[3].getTorque() * multiplier},
		};
	}

	public float[][] getOtherMatrix(CharController[] charactersOther)
	{
		float multiplier = player1 ? 1 : -1;

		return new float[][] {
			new float[]{charactersOther[0].getPos() * multiplier, charactersOther[0].getTorque() * multiplier},
			new float[]{charactersOther[1].getPos() * multiplier, charactersOther[1].getTorque() * multiplier},
			new float[]{charactersOther[2].getPos() * multiplier, charactersOther[2].getTorque() * multiplier},
			new float[]{charactersOther[3].getPos() * multiplier, charactersOther[3].getTorque() * multiplier},
		};
	}

	// public float getRewards(float ballPosX, float ballPosZ, float ballSpeedX, float ballSpeedZ)
	// {

	// }
}
