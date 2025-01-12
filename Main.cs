using Godot;
using System;

public partial class Main : Node
{
	// Don't forget to rebuild the project so the editor knows about the new export variable.

	[Export]
	public PackedScene MobScene { get; set; }

	[Export]
	public int ShootingPenalty = 1;

	[Export]
	public PackedScene BulletScene { get; set; }

	private int _score;

	private Player _player;
	private Marker2D _startPosition;

	private double _timePassed;

	private bool _gameStarted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_startPosition = GetNode<Marker2D>("StartPosition");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();
		GetNode<Hud>("Hud").ShowGameOver();
		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
	}

	public void NewGame()
	{
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		_score = 0;

		_player.Start(_startPosition.Position);
		GetNode<Timer>("StartTimer").Start();
		GetNode<AudioStreamPlayer>("Music").Play();

		var hud = GetNode<Hud>("Hud");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		_gameStarted = true;
	}

	private void OnScoreTimerTimeout()
	{
		_score++;
		GetNode<Hud>("Hud").UpdateScore(_score);
	}

	private void OnStartTimerTimeout()
	{
		_timePassed += GetNode<Timer>("StartTimer").WaitTime;
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		// Set the mob's direction perpendicular to the path direction.
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;

		// Add some randomness to the direction.
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Choose the velocity.
		// var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		var velocity = new Vector2((float)(150 + _timePassed / 5 * 20), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	public void OnPlayerShoot(LookingDirection lookingDirection)
	{
		if (!_gameStarted || _score < ShootingPenalty)
		{
			return;
		}

		Bullet bullet = BulletScene.Instantiate<Bullet>();
		AddChild(bullet);
		bullet.SetPosition(_player.Position, lookingDirection);
		bullet.SetDirection(lookingDirection);
		_score -= ShootingPenalty;
		GetNode<Hud>("Hud").UpdateScore(_score);
	}
}
