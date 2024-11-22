using Godot;
using System;

public partial class CharController : Node
{
	[Export]
	RigidBody3D physicsBody3D;

	[Export]
	float torque;
	[Export]
	float movement;

	[Export]
	Label rotation;
	[Export]
	Label position;

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// if (physicsBody3D.)
		position.Text = getPos().ToString();
		rotation.Text = getTorque().ToString();
	}



	public void applyForces(float axis, float rotation, Vector3 facing){
		physicsBody3D.ApplyTorque(facing * torque * rotation);
		physicsBody3D.ApplyForce(facing * movement * axis);
	}

	public float getTorque()
	{
		return physicsBody3D.RotationDegrees.Z / 180 ;
	}

	public float getPos(){
		return physicsBody3D.Position.Z * 3;
	}
}
