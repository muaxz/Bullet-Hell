using Godot;
using System;

public partial class Spawner : Node2D
{
	[Export]
	public PackedScene enemy;
	
	[Export]
	public float spawnInterval = 1.0f; // Time between spawns
	
	[Export]
	public Vector2 spawnAreaSize = new Vector2(400, 600); // Spawn area dimensions
	
	private Timer spawnTimer;
	private Random random = new Random();

	public override void _Ready()
	{
		// Create and configure timer
		spawnTimer = new Timer();
		spawnTimer.WaitTime = spawnInterval;
		spawnTimer.Timeout += SpawnEnemy;
		spawnTimer.Autostart = true;
		AddChild(spawnTimer);
	}

	private void SpawnEnemy()
	{
		if (enemy != null)
		{
			// Create enemy instance
			Node2D enemyInstance = (Enemy)enemy.Instantiate<Node2D>();

			// Set random position
			Vector2 randomPosition = new Vector2(
				(float)(random.NextDouble() * spawnAreaSize.X - spawnAreaSize.X / 2),
				(float)(random.NextDouble() * spawnAreaSize.Y - spawnAreaSize.Y / 2)
			);
			
			enemyInstance.Position = Position + randomPosition;
			
			// Add to scene
			GetParent().AddChild(enemyInstance);
		}
	}

	public override void _Process(double delta)
	{
	}
}
