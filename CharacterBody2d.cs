using Godot;
using System;

public partial class CharacterBody2d : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	const float Speed = 700;
	const float Acceleration = 3000;
	[Export]
	public PackedScene BulletScene;
	Node2D gunSprite;
	public override void _Ready()
	{
		GD.Print("leoo");
		gunSprite = GetNode<Node2D>("gun");
		DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{


		// Call the built-in MoveAndSlide() method.
		handleMovement((float)delta);
		MoveAndSlide();
		shoot();
		
	}

	public override void _Process(double delta)
	{
		handleGunRotate();
	}




	public void handleMovement(float delta)
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

		if (inputDirection != Vector2.Zero)
		{
			// Set the built-in Velocity property based on input and speed.
			Velocity = inputDirection * Speed;
		}
		else
		{
			// Use MoveToward to smoothly stop the character.
			Velocity = Velocity.MoveToward(Vector2.Zero, Acceleration * delta);
		}
	}

	public void handleGunRotate()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = mousePosition - GlobalPosition;
		gunSprite.Rotation = direction.Angle();
	}
	
	public void shoot()
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			var bullet = (Bullet)BulletScene.Instantiate();
			bullet.Position = GlobalPosition;
			bullet.Rotation = gunSprite.Rotation;
			GetTree().CurrentScene.AddChild(bullet);
		}
	}
}
