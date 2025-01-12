using Godot;
using System;

public enum LookingDirection
{
	Right,
	Left,
	Up,
	Down
}

public partial class Player : Area2D
{
	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	public Vector2 ScreenSize; // Size of the game window.

	[Signal]
	public delegate void HitEventHandler();

	[Signal]
	public delegate void ShootEventHandler(LookingDirection lookingDirection);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}

		if (Input.IsActionJustPressed("shoot"))
		{
			EmitSignal(SignalName.Shoot, (int)GetLookingDirection(animatedSprite2D, animatedSprite2D.FlipV, animatedSprite2D.FlipH));
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			// See the note below about the following boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
	}

	public void Start(Vector2 position)
	{
		Position = position;
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "walk";
		animatedSprite2D.FlipV = false;
		animatedSprite2D.FlipH = false;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

	// We also specified this function name in PascalCase in the editor's connection window.
	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	private LookingDirection GetLookingDirection(AnimatedSprite2D animatedSprite2D, bool flipV, bool flipH)
	{
		if (animatedSprite2D.Animation == "walk")
		{
			if (flipH)
			{
				return LookingDirection.Left;
			}
			else
			{
				return LookingDirection.Right;
			}

		}
		else if (animatedSprite2D.Animation == "up")
		{
			if (flipV)
			{
				return LookingDirection.Down;
			}
			else
			{
				return LookingDirection.Up;
			}

		}
		else
		{
			throw new ArgumentException($"Unhandled animation: {animatedSprite2D.Animation}");
		}
	}

}
