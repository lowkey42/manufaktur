using System;
using Godot;
using Manufaktur.gameplay;

public partial class GameCore : Node {
	[Export] private int _startDelay = 3;

	[Export] private int _ghostHeadStart = 2;

	[Export] private PackedScene _ghostScene;

	[Export] private Hud _hud;

	[Export] private Player _player;

	[Export] private GhostTracker _ghostTracker;

	private HighscoreList _highscoreList;

	private bool _started = false;
	private float _startTime;
	private float _finishTime;
	private float _time;

	private Ghost _ghost = null;

	public float TimeSeconds => _time;

	public override void _Ready() {
		_highscoreList = new HighscoreList(GetParentOrNull<Node>());

		var bus = EventBus.Instance;
		bus.FinishReached += OnFinished;
		bus.PlayerDied    += OnPlayerDied;

		var spawnPoints = GetTree().GetNodesInGroup("spawn_point");
		if (spawnPoints.Count > 0) {
			if (spawnPoints[Random.Shared.Next(0, spawnPoints.Count)] is Node3D sp) {
				_player.GlobalPosition = sp.GlobalPosition;
			}
		}

		var highscoreEntries = _highscoreList.Entries;
		if (highscoreEntries.Length > 0)
			_hud.SetBestTime(highscoreEntries[0].Time);

		foreach (var entry in highscoreEntries) {
			var ghostPath = entry.LoadGhost();
			if (ghostPath == null)
				continue;

			_ghost = (Ghost) _ghostScene.Instantiate();
			_ghost.SetPath(ghostPath);
			AddChild(_ghost, true);
			break;
		}

		StartCountdown();
	}

	private async void StartCountdown() {
		var bus = EventBus.Instance;

		_player.ProcessMode = ProcessModeEnum.Disabled;

		for (var i = 0; i < _startDelay; i++) {
			var timeLeft = _startDelay - i;
			if (timeLeft <= _ghostHeadStart)
				_ghost?.Start();

			_hud.SetCountdown(timeLeft);
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
		}

		_player.ProcessMode = ProcessModeEnum.Inherit;
		_hud.SetCountdown(0);

		bus.EmitLevelStarted();
		_started = true;
	}

	public override void _ExitTree() {
		_highscoreList.Save();
		EventBus.Instance.FinishReached -= OnFinished;
		EventBus.Instance.PlayerDied    -= OnPlayerDied;
	}

	public override void _Process(double delta) {
		if (_finishTime == 0 && _started) {
			_time += (float) delta;
		}

		_hud.SetCurrentTime(_time);
		_ghostTracker.SetCurrentTime(_time);

		if (Input.IsActionJustReleased("reset")) {
			EventBus.Instance.EmitPlayerDied();
		}
	}

	private void OnFinished() {
		_finishTime = _time;

		var entry = _highscoreList.AddHighscoreEntry("Manu", _finishTime);
		if (entry != null) {
			_ghostTracker.Path.Save(entry.GetUserGhostPath());
		}

		GetTree().Paused = true;
		_hud.ShowHighscore(_highscoreList, entry, (GetParentOrNull<Node>() as Level)?.NextLevelScene);
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}

	private async void OnPlayerDied() {
		// TODO: play animation/particle-effect and wait for 1-2 seconds
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");

		var error = GetTree().ReloadCurrentScene();
		if (error != Error.Ok) {
			GD.PrintErr(error);
		}
	}
}
