using System;
using Godot;
using Manufaktur.gameplay;

public partial class GameCore : Node {
	
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
		_highscoreList = new HighscoreList(GetParentOrNull<Node>()?.GetName() ?? "unknown"); 
		
		var bus = EventBus.Instance;
		bus.FinishReached += OnFinished;
		
		var spawnPoints = GetTree().GetNodesInGroup("spawn_point");
		if (spawnPoints.Count > 0) {
			if (spawnPoints[Random.Shared.Next(0, spawnPoints.Count)] is Node3D sp) {
				_player.GlobalPosition = sp.GlobalPosition;
			}
		}

		var highscoreEntries = _highscoreList.Entries;
		if(highscoreEntries.Length > 0)
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

		// TODO: show a count down (3 to GO) before starting
		// TODO: disable player collider until start, so they aren't pushed out of the start zone
		bus.EmitLevelStarted();
		_started   = true;
		_ghost?.Start();
	}

	public override void _ExitTree() {
		_highscoreList.Save();
		EventBus.Instance.FinishReached -= OnFinished;
	}

	public override void _Process(double delta) {
		if (_finishTime == 0 && _started) {
			_time += (float) delta;
		}
		
		_hud.SetCurrentTime(_time);
		_ghostTracker.SetCurrentTime(_time);

		if (Input.IsActionJustReleased("reset")) {
			var error = GetTree().ReloadCurrentScene();
			if(error != Error.Ok) {
				GD.PrintErr(error);
			}
		}
	}

	private void OnFinished() {
		_finishTime = _time;

		var entry = _highscoreList.AddHighscoreEntry("Manu", _finishTime);
		if (entry != null) {
			_ghostTracker.Path.Save(entry.GetUserGhostPath());
		}

		GetTree().Paused = true;
		_hud.ShowHighscore(_highscoreList, entry);
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}
}
