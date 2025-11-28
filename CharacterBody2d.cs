using Godot;
using System;
using System.Threading.Tasks;

public partial class CharacterBody2d : CharacterBody2D
{
    const float Speed = 700;
    const float Acceleration = 3000;

    [Export] public PackedScene BulletScene;
    [Export] public Node2D gunPoint;
    [Export] public Node2D muzzle;

    [Export] public AudioStreamPlayer2D gunShot;
    [Export] public PackedScene shotgunParticle;
    // RECOIL SETTINGS
    // Reverted to original value (1200f) as requested
    [Export] public float ShotgunKickbackStrength = 1200f; 
    [Export] public float RecoilRecoverySpeed = 20f; // How fast you recover control

    const float interval = 0.1f;
    float timeSinceLastBullet = 0f;

    // Separate vector to store the impact so input doesn't overwrite it immediately
    Vector2 recoilVelocity = Vector2.Zero; 

    GameManager gm;

    Sprite2D gunSprite;
    Sprite2D characterSprite;
    bool isFacingRight = true;
    
    // Variables to remember the original size set in the Editor
    Vector2 charOriginalScale;
    
    public override void _Ready()
    {
        gunSprite = GetNode<Sprite2D>("gun");
        characterSprite = GetNode<Sprite2D>("Sprite2D");
        gm = GetNode<GameManager>("/root/Game");
        // SAVE the scale you set in the editor so we don't make it huge
        charOriginalScale = characterSprite.Scale;

        DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
    }

    public override void _PhysicsProcess(double delta)
    {
        handleFacing(); // Handle flipping here
        handleMovement((float)delta);
        MoveAndSlide();
        shoot(delta);
        
    }

    public override void _Process(double delta)
    {
        handleGunRotate();
    }

    public void handleFacing()
    {
        Vector2 mousePosition = GetGlobalMousePosition();

        // We check if mouse is to the left or right of the player
        if (mousePosition.X < GlobalPosition.X)
        {
            // Mouse is Left: Set X to negative original size
            characterSprite.Scale = new Vector2(-Mathf.Abs(charOriginalScale.X), charOriginalScale.Y);
            gunSprite.Scale = new Vector2(-0.1f, gunSprite.Scale.Y);
            isFacingRight = false;
        }
        else
        {
            // Mouse is Right: Set X to positive original size
            characterSprite.Scale = new Vector2(Mathf.Abs(charOriginalScale.X), charOriginalScale.Y);
            gunSprite.Scale = new Vector2(0.1f, gunSprite.Scale.Y);
            isFacingRight = true;
        }
    }

    public void handleGunRotate()
    {
        Vector2 mousePos = GetGlobalMousePosition();

        // Direction from gun to mouse
        Vector2 direction = mousePos - gunSprite.GlobalPosition;

        // Compute angle in radians
        float angle = Mathf.Atan2(direction.Y, direction.X);

        // Apply rotation
        gunSprite.Rotation = angle;

        // Fix rotation if character is facing left
        if (!isFacingRight)
        {
            gunSprite.Rotation += Mathf.Pi;
        }
    }

    public void handleMovement(float delta)
    {
        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
        
        // 1. Calculate the desired velocity from input (max speed in the input direction)
        Vector2 desiredVelocity = inputDirection * Speed;

        // 2. Separate the current movement velocity from the transient recoil velocity.
        // This is crucial for mixing player input and external forces correctly.
        // If Velocity = Movement + Recoil, then Movement = Velocity - Recoil.
        Vector2 currentMovementVelocity = Velocity - recoilVelocity;

        // 3. Accelerate the movement component towards the desired input speed.
        // This acts as both acceleration (when moving) and friction (when standing still).
        Vector2 newMovementVelocity = currentMovementVelocity.MoveToward(desiredVelocity, Acceleration * delta);

        // 4. Handle Recoil Decay
        // Smoothly bring the external recoil force back to zero over time.
        recoilVelocity = recoilVelocity.Lerp(Vector2.Zero, RecoilRecoverySpeed * delta);

        // 5. Combine the new, player-controlled movement + the decaying recoil force.
        // The player input is now smoothly blended, allowing the recoil to be effective even while moving.
        Velocity = newMovementVelocity + recoilVelocity;
    }

    public async Task shoot(double delta)
    {   
        // Increment time since last shot regardless of input state to track cooldown
        timeSinceLastBullet += (float)delta;
        
        bool isFiring = false;

        // --- 1. Single-Shot (Shotgun) Logic: Only fires on the frame the button is pressed ---
        if (gm.selectedGunIndex == 3 && Input.IsActionJustPressed("shoot"))
        {
            isFiring = true;
            var particle = (Node2D)shotgunParticle.Instantiate();
            particle.GlobalPosition = gunPoint.GlobalPosition;
            particle.GlobalRotation = gunPoint.GlobalRotation;
            GetTree().CurrentScene.AddChild(particle);
            GpuParticles2D particle2D = particle.GetNode<GpuParticles2D>("GPUParticles2D");
            particle2D.Emitting = true;
            var timer = GetTree().CreateTimer(particle2D.Lifetime + 0.5f);
            timer.Timeout += () => particle.QueueFree();
        }   
        // --- 2. Auto/Semi-Auto Logic: Fires repeatedly while the button is held (for all other guns) ---
        else if (gm.selectedGunIndex != 3 && Input.IsActionPressed("shoot"))
        {
            isFiring = true;
        }
        
        // Perform fire action if an input trigger was met AND the weapon cooldown has passed
        if (isFiring && timeSinceLastBullet >= interval)
        {
            // Apply recoil only to the shotgun (gun index 3)
            muzzle.Visible = !muzzle.Visible;
            gunShot.Play();
            if(gm.selectedGunIndex == 3)
            {
                // FIX: Calculate the direction directly from the gun's global rotation (the bullet's direction).
                // This ensures the recoil is perfectly opposite the barrel, regardless of character flip state.
                Vector2 shootDir = -Vector2.Right.Rotated(gunSprite.GlobalRotation);
                
                // Apply recoil in the OPPOSITE direction
                Velocity = Vector2.Zero;
                recoilVelocity = -shootDir * ShotgunKickbackStrength;
            }
            else
            {
               
                
                var bullet = (Node2D)BulletScene.Instantiate();
                
                bullet.GlobalPosition = gunPoint.GlobalPosition;
                bullet.GlobalRotation = gunSprite.GlobalRotation;
                
                GetTree().CurrentScene.AddChild(bullet);
                timeSinceLastBullet = 0;

                // Simple cleanup
                await ToSignal(GetTree().CreateTimer(1f), SceneTreeTimer.SignalName.Timeout);
                if (IsInstanceValid(bullet)) bullet.QueueFree();
            }

            // Common firing routine
          
        }
        // Only turn off the muzzle flash when the key is released
        else if (Input.IsActionJustReleased("shoot"))
        {
            muzzle.Visible = false;
        }
    }
}