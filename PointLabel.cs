using Godot;
using System;

public partial class PointLabel : Label
{
	// Called when the node enters the scene tree for the first time.
	[Export] public float floatDistance = 50f;
    [Export] public float duration = 1f;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void show(Vector2 startPos,string value)
    {
		Position = startPos;
		this.Text = value;

		var tween = CreateTween();
        tween.TweenProperty(this, "position:y", Position.Y - floatDistance, duration);
        tween.TweenProperty(this, "modulate:a", 0f, duration);

        // Remove node after animation
        tween.Finished += () => QueueFree();

    }
}
