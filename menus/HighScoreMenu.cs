using Godot;
using System;
using Manufaktur.gameplay;

public partial class HighScoreMenu : Control
{
	[Signal]
	public delegate void ClosedEventHandler();
	

	private HighscoreList _currentList;
	private bool _showingGlobal = false;

	public override void _Ready()
	{

		var btnLocal = GetNodeOrNull<Button>("%LocalButton");
		var btnGlobal = GetNodeOrNull<Button>("%GlobalButton");

		if (btnLocal != null) 
			btnLocal.Pressed += ShowLocalScores;
			
		if (btnGlobal != null) 
			btnGlobal.Pressed += ShowGlobalScores;
	}

	private void _on_close_button_pressed() 
	{
	   EmitSignal(SignalName.Closed);
	}

	public void _load_score(string levelName)
	{
		_currentList = new HighscoreList(levelName);
		

		ShowLocalScores();
	}


	private void ShowLocalScores()
	{
		if (_currentList == null) return;
		_showingGlobal = false;
		UpdateUI(_currentList.EntriesLocal);
	}

	private async void ShowGlobalScores()
	{
		if (_currentList == null || _showingGlobal) return;
		_showingGlobal = true;
		
		UpdateUI([]);

		var scores = await _currentList.GetGlobalScores();
		if (_showingGlobal) UpdateUI(scores);
	}


	private void UpdateUI(HighscoreList.Entry[] entries)
	{
		var scoreListContainer = GetNodeOrNull<VBoxContainer>("%ScoreListContainer");
		
		if (scoreListContainer == null) {
			GD.PrintErr("Konnte %ScoreListContainer nicht finden!");
			return;
		}

		for (int i = 0; i < 9; i++)
		{
			var scoreNode = scoreListContainer.GetNodeOrNull<HBoxContainer>($"Score_{i + 1}");
			if (scoreNode == null) continue;

			var nameLabel  = scoreNode.GetNodeOrNull<RichTextLabel>("RichTextLabel");
			var scoreLabel = scoreNode.GetNodeOrNull<RichTextLabel>("RichTextLabel2");

			if (nameLabel == null || scoreLabel == null) continue;

			if (i < entries.Length)
			{
				var entry = entries[i];
				nameLabel.Text  = entry.Name;
				scoreLabel.Text = $"{entry.Time:0.000}s";
			}
			else
			{
				nameLabel.Text  = "-";
				scoreLabel.Text = "-";
			}
		}
	}
}
