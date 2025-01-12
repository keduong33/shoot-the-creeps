using Godot;
using System;

public partial class Bullet : Area2D
{

	private CollisionShape2D _rightCollisionShape;
	private CollisionShape2D _leftCollisionShape;
	private CollisionShape2D _upCollisionShape;
	private CollisionShape2D _downCollisionShape;

	private LookingDirection _direction;

	[Export]
	public int speed = 500;

	private AnimatedSprite2D _animatedSprite2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rightCollisionShape = GetNode<CollisionShape2D>("RightCollisionShape");
		_leftCollisionShape = GetNode<CollisionShape2D>("LeftCollisionShape");
		_upCollisionShape = GetNode<CollisionShape2D>("UpCollisionShape");
		_downCollisionShape = GetNode<CollisionShape2D>("DownCollisionShape");
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch (_direction)
		{
			case LookingDirection.Right:
				Position += new Vector2(speed, 0) * (float)delta;
				break;
			case LookingDirection.Left:
				Position += new Vector2(-speed, 0) * (float)delta;
				break;
			case LookingDirection.Up:
				Position += new Vector2(0, -speed) * (float)delta;
				break;
			case LookingDirection.Down:
				Position += new Vector2(0, speed) * (float)delta;
				break;
			default:
				break;
		}
	}

	public void SetPosition(Vector2 playerPosition, LookingDirection lookingDirection)
	{
		var position = playerPosition;

		switch (lookingDirection)
		{
			case LookingDirection.Right:
				_direction = LookingDirection.Right;
				position.X += 60;
				break;
			case LookingDirection.Left:
				_direction = LookingDirection.Left;
				position.X -= 60;
				break;
			case LookingDirection.Up:
				_direction = LookingDirection.Up;
				position.Y -= 60;
				break;
			case LookingDirection.Down:
				_direction = LookingDirection.Down;
				position.Y += 60;
				break;
		}
		Position = position;
	}

	public void SetDirection(LookingDirection lookingDirection)
	{
		_rightCollisionShape.Disabled = true;
		_leftCollisionShape.Disabled = true;
		_upCollisionShape.Disabled = true;
		_downCollisionShape.Disabled = true;

		switch (lookingDirection)
		{
			case LookingDirection.Right:
				_animatedSprite2D.Rotate(Mathf.DegToRad(0));
				_rightCollisionShape.Disabled = false;
				break;
			case LookingDirection.Left:
				_animatedSprite2D.Rotate(Mathf.DegToRad(180));
				_leftCollisionShape.Disabled = false;
				break;
			case LookingDirection.Up:
				_animatedSprite2D.Rotate(Mathf.DegToRad(-90));
				_upCollisionShape.Disabled = false;
				break;
			case LookingDirection.Down:
				_animatedSprite2D.Rotate(Mathf.DegToRad(90));
				_downCollisionShape.Disabled = false;
				break;
		}
	}

	public void OnMobHit(Node2D mob)
	{
		mob.QueueFree();
		QueueFree();
	}


}
