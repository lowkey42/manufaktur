using Godot;
using System;
using Manufaktur.gameplay;

public partial class HighScoreMenu : Control
{


	[Signal]
	public delegate void ClosedEventHandler();
	
	
	private void _on_close_button_pressed() {
		EmitSignal(SignalName.Closed);
	}
	
	

	public void _load_score(string levelName)
	{
		var list    = new HighscoreList(levelName);
		var entries = list.EntriesLocal;

		
		var scoreListContainer = GetNodeOrNull<VBoxContainer>("%ScoreListContainer");
    
		if (scoreListContainer == null) {
			GD.PrintErr("Konnte %ScoreListContainer nicht finden!");
			return;
		}

		for (int i = 0; i < 9; i++)
		{

			var scoreNode = scoreListContainer.GetNodeOrNull<HBoxContainer>($"Score_{i + 1}");
        
			if (scoreNode == null) {
				continue;
			}


			var nameLabel  = scoreNode.GetNodeOrNull<RichTextLabel>("RichTextLabel");
			var scoreLabel = scoreNode.GetNodeOrNull<RichTextLabel>("RichTextLabel2");


			if (nameLabel == null || scoreLabel == null) {
				GD.PrintErr($"Score_{i + 1} fehlen die RichTextLabels!");
				continue;
			}


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
