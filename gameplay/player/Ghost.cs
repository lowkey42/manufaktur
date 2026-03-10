using System.Collections.Generic;
using Godot;
using Manufaktur.gameplay.player;

public partial class Ghost : Node3D {

	[Export] private float _knockbackVelocity = 400;
	
	private PlayerPath _path;
	private float _time;
	private bool _started;
	private readonly List<Player> _closePlayers = [];

	public void Start() {
		_started = true;
	}
	
	public void SetPath(PlayerPath path) {
		_path = path;
	}

	public void KnockbackPlayer(Node3D player) {
		if (player is Player p) {
			p.Velocity = (p.GlobalPosition - GlobalPosition).Normalized() * _knockbackVelocity;
		}
	}
	public void WarnPlayer(Node3D player) {
		if (player is Player p) {
			// TODO: set shader variable to pulsing visual
		}
	}

	public override void _Process(double delta) {
		if (!_started)
			return;

		var previousTime = _time;
		_time += (float) delta;
		_path.SampleGhost(_time, previousTime, this);
	}

}
