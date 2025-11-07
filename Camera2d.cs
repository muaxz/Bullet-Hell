using Godot;
using System;

public partial class Camera2d : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Node2D Player;
	public override void _Ready()
    {
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
    {
		Position = Player.GlobalPosition;
    }
}
