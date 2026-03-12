using System.Collections.Generic;
using Godot;
using Manufaktur.gameplay.player;

public partial class Ghost : Node3D {
	[Export] private float _knockbackVelocity = 400;

	[Export] private MeshInstance3D _mesh;
	[Export] private GpuParticles3D _particles;
	[Export] private CollisionShape3D _body;
	[Export] private AudioStreamPlayer3D _knockbackSound;

	private PlayerPath _path;
	private float _time;
	private bool _started;
	private Vector3? _startPosition;
	private readonly List<Player> _closePlayers = [];

	public void Start() {
		_started = true;
	}

	public void SetPath(PlayerPath path) {
		_path = path;
	}

	public void KnockbackPlayer(Node3D player) {
		if (player is Player p && _time < _path.TotalTime - 0.5f && (_startPosition == null || (GlobalPosition - _startPosition.Value).Length() > 4f)) {
			var knockback = (p.GlobalPosition - GlobalPosition).Normalized() * _knockbackVelocity;
			knockback.Y *= 0.25f;
			p.Velocity  =  knockback;
			
			if(_knockbackSound != null)
				_knockbackSound.Play();
		}
	}

	public void StartWarnPlayer(Node3D player) {
		if (player is Player p && _time > _path.TotalTime - 0.5f) {
			// TODO: set shader variable to pulsing visual
		}
	}

	public void StopWarnPlayer(Node3D player) {
		if (player is Player p) {
			// TODO: set shader variable to pulsing visual
		}
	}

	public override void _Process(double delta) {
		if (!_started) {
			return;
		}

		var previousTime = _time;
		_time += (float) delta;
		_path.SampleGhost(_time, previousTime, this);
		
		if(_startPosition == null && GlobalPosition.Length() > 0.01f)
			_startPosition = GlobalPosition;

		if (_time > _path.TotalTime) {
			if (_mesh != null)
				_mesh.Visible = false;

			if (_particles != null) {
				_particles.Lifetime = 4.0;
				_particles.Emitting = false;
			}

			if (_body != null)
				_body.Disabled = true;
		}
	}
}
