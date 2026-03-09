using Godot;
using System.Collections.Generic;
using Manufaktur.gameplay.player;

public partial class Ghost : Node3D {

	private PlayerPath _path;
	private float _time;
	private bool _started;

	public void Start() {
		_started = true;
	}
	
	public void SetPath(PlayerPath path) {
		_path = path;
	}

	public override void _Process(double delta) {
		if (!_started)
			return;

		var previousTime = _time;
		_time += (float) delta;
		_path.SampleGhost(_time, previousTime, this);
	}
}
