using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	public Node2D Player;
	public GameManager GameManager;
	float speed = 300f;
	public override void _Ready()
    {
		Player = GetNode<Node2D>("../Player");
		GameManager = GetNode<GameManager>("../../Game");
		if(Player == null)
        {
			GD.Print("null");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = (Player.Position - Position).Normalized();
		Velocity = direction * speed;
		MoveAndSlide();
		
	
	}
	
	public void _on_area_2d_area_entered(Area2D area)
	{
		if(GameManager != null)
		{
			GD.Print("sdasdad");
			GameManager.addScore();
        }
	    area.QueueFree();
		QueueFree();
		
    }
}
