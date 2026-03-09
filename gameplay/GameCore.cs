using System;
using Godot;

public partial class GameCore : Node {

	[Export] private Hud _hud;

	[Export] private Player _player;
	
	private float _startTime;
	private float _finishTime;
	
	public float TimeSeconds => (_finishTime==0 ? (Time.GetTicksMsec() - _startTime) : (_finishTime - _startTime)) / 1000.0f;
	
	public override void _Ready() {
		var bus = EventBus.Instance;
		bus.FinishReached += OnFinished;
		
		var spawnPoints = GetTree().GetNodesInGroup("spawn_point");
		if (spawnPoints.Count > 0) {
			if (spawnPoints[Random.Shared.Next(0, spawnPoints.Count)] is Node3D sp) {
				_player.GlobalPosition = sp.GlobalPosition;
			}
		}
		
		// TODO: show a count down (3 to GO) before starting
		bus.EmitLevelStarted();
		_startTime = Time.GetTicksMsec();
	}

	public override void _Process(double delta) {
		_hud.SetCurrentTime(TimeSeconds);
		_hud.SetBestTime(3232.424242f); // TODO
	}

	private void OnFinished() {
		_finishTime = Time.GetTicksMsec();
		// TODO: fade out and show highscore list
	}
}
