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
	[Export]
	TextureRect gun1;
	[Export]
	TextureRect gun2;
	[Export]
	TextureRect gun3;
	TextureRect selectedGun;
	Node2D player;
	public int selectedGunIndex;
	public override void _Ready()
    {
            gun1.Modulate = Colors.Red;
			selectedGun = gun1;
			selectedGunIndex = 1;
			player = GetNode<Node2D>("Player");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("select_gun_1"))
		{
			SelectGun(gun1);
			selectedGunIndex = 1;
		}
		if (Input.IsActionPressed("select_gun_2"))
		{
			SelectGun(gun2);
			selectedGunIndex = 2;
		}
		if (Input.IsActionPressed("select_gun_3"))
		{
			SelectGun(gun3);
			selectedGunIndex = 3;
		}

		if (Input.IsActionJustPressed("switch_gun"))
		{ 	
			selectedGunIndex++;
			if (selectedGunIndex > 3)
				selectedGunIndex = 1;
			
			switch (selectedGunIndex)
			{
				case 1:
					SelectGun(gun1);
					break;
				case 2:
					SelectGun(gun2);
					break;
				case 3:
					SelectGun(gun3);
					break;
			}
		}
	}

	private void SelectGun(TextureRect gun)
	{
		gun1.Modulate = Colors.White;
		gun2.Modulate = Colors.White;
		gun3.Modulate = Colors.White;
		gun.Modulate = new Color(0.8f, 0.2f, 0.2f);
		Sprite2D gunSprite = player.GetNode<Sprite2D>("gun");
		gunSprite.Texture = gun.Texture;
		selectedGun = gun;
	}

	public void addScore()
	{
		score += 10;
		scoreLabel.Text = "Score: " + score.ToString();
    }

	static public void test()
    {
        GD.Print("lol");
    }
}
