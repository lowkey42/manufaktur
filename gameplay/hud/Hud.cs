using Godot;
using System;
using System.Globalization;
using Manufaktur.gameplay;

public partial class Hud : CanvasLayer {
	[Export(PropertyHint.File, "*.tscn")] private string _mainMenuScene;

	[Export] private PackedScene _highScoreRow;

	[Export] private RichTextLabel _labelCurrentTime;
	[Export] private RichTextLabel _labelBestTime;
	[Export] private Control _highscorePanel;
	[Export] private Button _nextButton;
	[Export] private Button _retryButton;

	[Export] private Label _labelCountdown;

	private HighscoreList _highscores;
	private HighscoreList.Entry _newEntry;
	private TextEdit _newEntryNameEdit;
	private int _lastCountdownValue = -1;
	private string _nextLevelScene;

	public void ShowHighscore(HighscoreList highscores, HighscoreList.Entry newEntry, string nextLevelScene) {
		_highscores     = highscores;
		_newEntry       = newEntry;
		_nextLevelScene = nextLevelScene;

		var prevHighscoreRow = _highscorePanel.GetNode<Control>("%HighscoreHeader");
		var position         = 1;
		foreach (var entry in highscores.Entries) {
			var row = (Control) _highScoreRow.Instantiate();
			row.GetNode<Label>("Number").Text = position + ".";
			position++;
			row.GetNode<Label>("Time").Text = FormatTime(entry.Time);
			if (entry == newEntry) {
				var edit = row.GetNode<TextEdit>("Name/Edit");
				_newEntryNameEdit                        =  edit;
				row.GetNode<Label>("Name/Label").Visible =  false;
				edit.Visible                             =  true;
				edit.Text                                =  entry.Name;
				edit.TextChanged                         += OnRowChange;
			} else {
				row.GetNode<Label>("Name/Label").Text = entry.Name;
			}

			prevHighscoreRow.AddSibling(row);
			prevHighscoreRow = row;
		}

		_highscorePanel.Show();
		_nextButton.Disabled = !highscores.IsMinimumTimeReached() || nextLevelScene == null || nextLevelScene.Length == 0;

		if (_nextButton.Disabled) {
			_retryButton.GrabFocus();
		} else {
			_nextButton.GrabFocus();
		}
	}

	public override void _Process(double delta) {
		if (_highscorePanel.Visible) {
			Input.SetMouseMode(Input.MouseModeEnum.Visible);
		}
	}

	private void OnRowChange() {
		if (_newEntry != null && _newEntryNameEdit != null) {
			_newEntry.Name = _newEntryNameEdit.Text;
		}
	}

	public void OnReset() {
		_newEntry = null;
		_highscores.ResetProgress();
		OnRetry();
	}

	public void OnExit() {
		OnRowChange();
		GetTree().Paused = false;

		var error = GetTree().ChangeSceneToFile(_mainMenuScene);
		if (error != Error.Ok) {
			GD.PrintErr(error);
		}
	}

	public void OnRetry() {
		OnRowChange();
		GetTree().Paused = false;

		var error = GetTree().ReloadCurrentScene();
		if (error != Error.Ok) {
			GD.PrintErr(error);
		}
	}

	public void OnNext() {
		OnRowChange();
		GetTree().Paused = false;

		var error = GetTree().ChangeSceneToFile(_nextLevelScene);
		if (error != Error.Ok) {
			GD.PrintErr(error);
		}
	}

	public void SetCurrentTime(float currentTime) {
		_labelCurrentTime.Text = FormatTime(currentTime);
	}

	public void SetBestTime(float bestTime) {
		_labelBestTime.Text = FormatTime(bestTime);
	}

	public void SetCountdown(int countdown) {
		if (_lastCountdownValue == countdown) {
			return;
		}

		_lastCountdownValue = countdown;

		if (countdown > 0) {
			_labelCountdown.Text = countdown.ToString(CultureInfo.InvariantCulture);

			const float jumpHeight = 50.0f;
			const float duration   = 0.7f;

			var tween       = GetTree().CreateTween().SetParallel(true);
			var originalPos = _labelCountdown.Position;
			var posTween    = GetTree().CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			posTween.TweenProperty(_labelCountdown, "position:y", originalPos.Y - jumpHeight, duration / 2);
			posTween.SetEase(Tween.EaseType.In);
			posTween.TweenProperty(_labelCountdown, "position:y", originalPos.Y, duration / 2);
			tween.TweenProperty(_labelCountdown, "scale", new Vector2(0.8f, 1.3f), duration * 0.2f);         // Stretch Up
			tween.Chain().TweenProperty(_labelCountdown, "scale", new Vector2(1.0f, 1.0f), duration * 0.3f); // Normal at peak
			tween.Chain().TweenProperty(_labelCountdown, "scale", new Vector2(1.4f, 0.6f), duration * 0.2f)  // Impact Squash
			     .SetTrans(Tween.TransitionType.Bounce);
			tween.Chain().TweenProperty(_labelCountdown, "scale", new Vector2(1.0f, 1.0f), duration * 0.1f); // Reset
		} else {
			_labelCountdown.Text = "GO";
			GetTree().CreateTween().TweenProperty(_labelCountdown, "modulate:a", 0.0f, 0.5f)
			         .SetTrans(Tween.TransitionType.Linear)
			         .SetEase(Tween.EaseType.In);
		}
	}

	private static string FormatTime(float currentTime) {
		return TimeSpan.FromSeconds(currentTime).ToString(@"m\:ss\.fff");
	}
	
	
}
