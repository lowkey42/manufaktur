using Godot;
using System;

public partial class Turret : Node3D {
	[Export] public float TimeBetweenBursts { get; set; } = 0.8f;
	[Export] public int BurstBulletCount { get; set; } = 5;
	[Export] public float TimeBetweenBurstBullets { get; set; } = 0.07f;

	[Export] public float MaxBulletTravelDistance { get; set; }= 100f;

	[Export] public PackedScene BulletTemplate { get; set; }
	[Export] private AudioStreamPlayer3D _fireSoundPlayer;

	private RayCast3D _fireDirection;
	private Node3D _bulletOrigin;
	private Timer _fireTimer;

	public override void _Ready() {
		_fireTimer     = GetNode<Timer>("FireTimer");
		_bulletOrigin  = GetNode<Node3D>("Origin/BulletOrigin");
		_fireDirection = GetNode<RayCast3D>("Origin/BulletOrigin/BulletDirection");

		_fireTimer.OneShot =  true;
		_fireTimer.Timeout += OnFireTimerTimeout;
		_fireTimer.Start(TimeBetweenBursts);
	}

	private int _shotsLeftInBurst = 0;

	private void OnFireTimerTimeout() {

		if (_shotsLeftInBurst <= 0)
			_shotsLeftInBurst = Mathf.Max(1, BurstBulletCount);

		FireOneBullet();
		_shotsLeftInBurst--;

		float nextDelay = (_shotsLeftInBurst > 0)
			                  ? Mathf.Max(0.01f, TimeBetweenBurstBullets)
			                  : Mathf.Max(0.01f, TimeBetweenBursts);

		_fireTimer.Start(nextDelay);
	}

	private void FireOneBullet() {
		var nextBullet = BulletTemplate?.Instantiate<Bullet>();

		if (nextBullet == null) {
			GD.PushWarning("BulletTemplate is not assigned or is not a Bullet scene.");
			return;
		}
		
		nextBullet.MaxDistance = MaxBulletTravelDistance;

		AddChild(nextBullet);
		nextBullet.GlobalTransform = _bulletOrigin.GlobalTransform;

		var direction = (_fireDirection.GlobalTransform.Basis * _fireDirection.TargetPosition).Normalized();
		nextBullet.Fire(direction);
		
		_fireSoundPlayer.Play();
	}
}
