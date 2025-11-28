using Godot;
using System;
using System.Threading.Tasks;

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
	public AnimatedSprite2D enemySprite;
	[Export]
	public PackedScene pointLabel;
	Area2D enemyArea;
	float speed = 300f;
	double health = 100;
	bool isDead = false;
	float timeSinceLastBullet = 0.4f;
	float interval = 0.4f;
	Color originalColor;
	Vector2 enemyOriginalScale;
	public override void _Ready()
    {
		Player = GetNode<Node2D>("/root/Game/Player");
		GameManager = GetNode<GameManager>("/root/Game");
		enemyArea = GetNode<Area2D>("Area2D");
		enemyOriginalScale = enemySprite.Scale;
		enemyArea.AreaEntered += _on_area_2d_area_entered;
		originalColor = enemySprite.Modulate;
		enemySprite.Play("Walk");
		if(Player == null)
        {
			GD.Print("null");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(isDead) return;
		
		

		handleFacing();
		handleMovement(delta);
		MoveAndSlide();


	}
	
	public async Task shootOnTime(double delta)
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
			await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
			bullet.QueueFree();

        }
    }

	public void handleMovement(double delta)
    {
        float distance = (Player.GlobalPosition - GlobalPosition).Length();

		if (distance > 300)
		{
			Vector2 direction = (Player.Position - Position).Normalized(); // normalize because we dont want length of the vector to affect our speed
			Velocity = direction * speed;
			enemySprite.Play("Walk");
		}
		else
		{
			Velocity = Vector2.Zero;
			enemySprite.Play("Idle");
			shootOnTime(delta);
		}
    }

	public void handleFacing()
    {
        if(Player.GlobalPosition.X > GlobalPosition.X)
        {
            enemySprite.Scale = new Vector2(enemyOriginalScale.X,enemyOriginalScale.Y);
        }
        else
        {
            enemySprite.Scale = new Vector2(-enemyOriginalScale.X,enemyOriginalScale.Y);
        }
    }
	// enemy is hit here
	public async void _on_area_2d_area_entered(Area2D area)
	{
		//check if bullet enters not player
		
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
				enemySprite.Play("Dead");
				//enemySprite.Texture = GD.Load<Texture2D>("res://death_9.png");
				enemySprite.Centered = true;
				var label = (PointLabel)pointLabel.Instantiate();
				label.show(GlobalPosition, "+10");
				GetTree().CurrentScene.AddChild(label);
				healthBar.Visible = false;
				
				// Fade out enemy slowly
				await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);
				var fadeTween = CreateTween();
				fadeTween.TweenProperty(enemySprite, "modulate:a", 0.0f, 1.0f);
				await ToSignal(fadeTween, Tween.SignalName.Finished);
				
				GameManager.addScore();
				QueueFree();
				
			}
			area.QueueFree();//bullet get destroyed here
		}

	}
	
}
