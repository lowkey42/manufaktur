using Godot;
using Manufaktur.gameplay;


public partial class LevelSelection : Control
{
	[Signal]
	public delegate void ClosedEventHandler();

	[Signal]
	public delegate void ScoreOpenEventHandler(string levelName);


	private RichTextLabel _level1Label;
	private RichTextLabel _level2Label;
	private RichTextLabel _level3Label;

	public void _on_close_button_pressed() {
		EmitSignal(SignalName.Closed);
	}
	

	public override void _Ready()
	{
		LoadScores();
	}
	
	private void LoadScores()
	{
		_level1Label = GetNode<RichTextLabel>("%Level1ScoreLabel");
		_level2Label = GetNode<RichTextLabel>("%Level2ScoreLabel");
		_level3Label = GetNode<RichTextLabel>("%Level3ScoreLabel");
		
		var level1 = HighscoreList.LoadTopEntry("level01");
		var level2 = HighscoreList.LoadTopEntry("level01");
		var level3 = HighscoreList.LoadTopEntry("level01");

		_level1Label.Text = $"{level1.Name} - {level1.Time}";
		_level2Label.Text = $"{level2.Name} - {level2.Time}";
		_level3Label.Text = $"{level3.Name} - {level3.Time}";
	}

	public void LoadScore(string path) {
		EmitSignal(SignalName.ScoreOpen, path);
	}
	
	public async void LoadLevel(string path)
	{
		await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile(path);
	}
}
