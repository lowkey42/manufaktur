using Godot;
using System;

public partial class Bullet : Node3D {
	[Export] public float BulletSpeed { get; set; } = 10f;

	private float _maxDistance = 100f;
	private float _distanceTraveled = 0f;

	private Vector3 _direction;
	private bool _travel = false;

	private Area3D _bulletCollision;

	public override void _Ready() {
		_bulletCollision             =  GetNode<Area3D>("Area3D");
		_bulletCollision.BodyEntered += OnBulletCollisionBodyEntered;
	}

	public override void _Process(double delta) {
		if (!_travel) {
			return;
		}

		var movement = _direction * (float) delta * BulletSpeed;
		GlobalPosition    += movement;
		_distanceTraveled += movement.Length();

		// Destroy after max distance
		if (_distanceTraveled >= _maxDistance) {
			QueueFree();
		}
	}

	private void OnBulletCollisionBodyEntered(Node3D body) {
		if (body is Player) {
			EventBus.Instance.EmitPlayerDied();
		}

		QueueFree();
	}

	public void Fire(Vector3 direction) {
		_direction = direction;
		_travel    = true;
	}
}
