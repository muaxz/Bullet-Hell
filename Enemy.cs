using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Called when the node enters the scene tree for the first time.
	public Node2D Player;
	public GameManager GameManager;
	[Export]
	public ProgressBar healthBar;
	[Export]
	public PackedScene enemyBullet;
	[Export]
	public Sprite2D enemySprite;
	[Export]
	public PackedScene pointLabel;
	Area2D enemyArea;
	float speed = 300f;
	double health = 100;
	bool isDead = false;
	float timeSinceLastBullet = 0.4f;
	float interval = 0.4f;
	Color originalColor;
	public override void _Ready()
    {
		Player = GetNode<Node2D>("../Player");
		GameManager = GetNode<GameManager>("../../Game");
		enemyArea = GetNode<Area2D>("./Area2D");
		enemyArea.AreaEntered += _on_area_2d_area_entered;
		originalColor = enemySprite.Modulate;
		if(Player == null)
        {
			GD.Print("null");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		float distance = (Player.GlobalPosition - GlobalPosition).Length();

		if (distance > 300)
		{
			Vector2 direction = (Player.Position - Position).Normalized(); // normalize because we dont want length of the vector to affect our speed
			Velocity = direction * speed;

		}
		else
		{
			Velocity = Vector2.Zero;
			shootOnTime(delta);
		}

		MoveAndSlide();


	}
	
	public void shootOnTime(double delta)
    {
		timeSinceLastBullet += (float)delta;
		if(timeSinceLastBullet >= interval)
		{
			EnemyBullet bullet = (EnemyBullet)enemyBullet.Instantiate();
			bullet.GlobalPosition = GlobalPosition;
			Vector2 direction = Player.GlobalPosition - GlobalPosition;
			bullet.Rotation = direction.Angle();
			GetTree().CurrentScene.AddChild(bullet);
			timeSinceLastBullet = 0;
        }
    }
	// enemy is hit here
	public void _on_area_2d_area_entered(Area2D area)
	{
		//check if bullet enters not player
		GD.Print("tocuhed");
		if (GameManager != null && !isDead)
		{
			health -= 30;
			healthBar.Value = health;
			enemySprite.Modulate = Colors.Red;
			var tween = CreateTween();
			tween.TweenProperty(enemySprite, "modulate", originalColor, 0.2f);
			if (health <= 0)
			{	
				isDead = true;
				QueueFree();
				GameManager.addScore();
				var label = (PointLabel)pointLabel.Instantiate();
				label.show(GlobalPosition, "+10");
				GetTree().CurrentScene.AddChild(label);
			}
			area.QueueFree();
		}

	}
	
}
