using Godot;
using System;

public partial class Bullet : Node3D {
	[Export] public float BulletSpeed { get; set; } = 10f;

	public float MaxDistance { get; set; } = 500f;

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
		if (_distanceTraveled >= MaxDistance) {
			QueueFree();
		}
	}

	private void OnBulletCollisionBodyEntered(Node3D body) {
		if (body is Player) {
			EventBus.Instance.EmitPlayerDied();
		}
		if(body is GridMap) {
			return;
		}
		
		QueueFree();
	}

	public void Fire(Vector3 direction) {
		_direction = direction;
		_travel    = true;
	}
}
