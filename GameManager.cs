using Godot;
using System;

public partial class GameManager : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public Label scoreLabel;
	[Export]
	public ProgressBar playerBar;
	int score = 0;
	public override void _Ready()
    {
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void addScore()
	{
		score += 10;
		scoreLabel.Text = "Score: " + score.ToString();
    }
}
