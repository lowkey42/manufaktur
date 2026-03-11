using Godot;
using System;

public partial class Turret : Node3D {
	[Export] public float FireRate { get; set; } = 1f;
	[Export] public PackedScene BulletTemplate { get; set; }

	private RayCast3D _fireDirection;
	private Node3D _bulletOrigin;
	private Timer _fireTimer;

	public override void _Ready() {
		_fireTimer = GetNode<Timer>("FireTimer");
		_bulletOrigin  = GetNode<Node3D>("Origin/BulletOrigin");
		_fireDirection = GetNode<RayCast3D>("Origin/BulletOrigin/BulletDirection");
		
		_fireTimer.WaitTime = 1f / FireRate;
		_fireTimer.Timeout += OnFireTimerTimeout;
	}

	private void OnFireTimerTimeout() {
		var nextBullet = BulletTemplate.Instantiate() as Bullet;
		
		AddChild(nextBullet);
		nextBullet.GlobalTransform = _bulletOrigin.GlobalTransform;
		
		// Transform the raycast's local target position to global space using its rotation
		var direction = (_fireDirection.GlobalTransform.Basis * _fireDirection.TargetPosition).Normalized();
    
		nextBullet.Fire(direction);
	}
}
