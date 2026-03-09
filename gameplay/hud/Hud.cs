using Godot;
using System;
using Manufaktur.gameplay;

public partial class Hud : CanvasLayer {
	[Export] private PackedScene _highScoreRow;
	
	[Export] private RichTextLabel _labelCurrentTime;
	[Export] private RichTextLabel _labelBestTime;
	[Export] private Control _highscorePanel;

	private HighscoreList.Entry _newEntry;
	private TextEdit _newEntryNameEdit;

	public void ShowHighscore(HighscoreList highscores, HighscoreList.Entry newEntry) {
		_newEntry = newEntry;
		
		var prevHighscoreRow = _highscorePanel.GetNode<Control>("%HighscoreHeader");
		var position        = 1;
		foreach (var entry in highscores.Entries) {
			var row = (Control) _highScoreRow.Instantiate();
			row.GetNode<Label>("Number").Text     = position + ".";
			position++;
			row.GetNode<Label>("Time").Text       = FormatTime(entry.Time);
			if (entry == newEntry) {
				var edit = row.GetNode<TextEdit>("Name/Edit");
				_newEntryNameEdit                        =  edit;
				row.GetNode<Label>("Name/Label").Visible =  false;
					edit.Visible                         =  true;
					edit.Text                            =  entry.Name;
					edit.TextChanged                     += OnRowChange;
			} else {
				row.GetNode<Label>("Name/Label").Text = entry.Name;
			}
			prevHighscoreRow.AddSibling(row);
			prevHighscoreRow = row;
		}
		
		_highscorePanel.Show();
	}

	private void OnRowChange() {
		if(_newEntry != null && _newEntryNameEdit != null)
			_newEntry.Name = _newEntryNameEdit.Text;
	}

	public void OnExit() {
		OnRowChange();
		GetTree().Paused = false;
		
		// TODO: return to menu
	}

	public void OnRetry() {
		OnRowChange();
		GetTree().Paused = false;
		
		var error = GetTree().ReloadCurrentScene();
		if(error != Error.Ok) {
			GD.PrintErr(error);
		}
	}

	public void OnNext() {
		OnRowChange();
		GetTree().Paused = false;
		// TODO: load next level
	}

	public void SetCurrentTime(float currentTime) {
		_labelCurrentTime.Text = FormatTime(currentTime);
	}

	public void SetBestTime(float bestTime) {
		_labelBestTime.Text = FormatTime(bestTime);
	}

	private static string FormatTime(float currentTime) {
		return TimeSpan.FromSeconds(currentTime).ToString(@"m\:ss\.fff");
	}
}
