using System.Reflection.Emit;
using Godot;
using Manufaktur.gameplay;
using Manufaktur.gameplay.levels;

public partial class LevelSelection : Control
{
	[Signal]
	public delegate void ClosedEventHandler();

	[Signal]
	public delegate void ScoreOpenEventHandler(string levelName);

	[Export]
	public Control iHateNodePathsBecuaseTheyAreWonkyAsHell;
	[Export]
	public Control iHateNodePathsBecuaseTheyAreWonkyAsHell2ElectricBenedict;



	public Godot.Collections.Array<LevelData> AvailableLevels { get; set; } = new();

	private VBoxContainer _rightPanelContent; 
	private RichTextLabel _currentNameLabel;
	private TextureRect _currentImage;
	private RichTextLabel _currentScoreLabel;
	
	private Button _startLevelButton;
	private Button _highScoreButton;

	private string _currentScenePath = "";
	private string _currentHighscoreKey = "";
	private Tween _rightPanelTween; 

	public override void _Ready() {
		AvailableLevels = LevelManager.Instance.AvailableLevels;
		
		_rightPanelContent = GetNode<VBoxContainer>("%RightPanelContent");
	
		_currentNameLabel  = GetNode<RichTextLabel>("%CurrentNameLabel");
		_currentImage      = GetNode<TextureRect>("%CurrentLevelImage");
		_currentScoreLabel = GetNode<RichTextLabel>("%CurrentScoreLabel");
	
		_startLevelButton = GetNode<Button>("%StartLevelButton");
		_highScoreButton  = GetNode<Button>("%HighScoreButton");

		_startLevelButton.Pressed += OnStartPressed;
		_highScoreButton.Pressed  += OnHighscorePressed;

		var buttonContainer = GetNode<Control>("%LevelListContainer");


		foreach (Node child in buttonContainer.GetChildren())
		{
			child.QueueFree();
		}


		var buttonScene = GD.Load<PackedScene>("res://menus/LevelSelectionButton.tscn");
		var i           = 1;
		foreach (var level in AvailableLevels)
		{
			if (level == null) continue;
	
			var btn = buttonScene.Instantiate<Button>();

			if (level.UniqueListName?.Length > 0)
			{
				btn.Text = level.UniqueListName;
			} else {
				btn.Text = "Level " + i;
				i++;
			}

			var fn = () => SelectLevel(level.LevelName, level.ScenePath, level.HighscoreKey, level.PreviewImage, level.HighscoreKey.Length > 0);
			btn.Pressed += fn;

			buttonContainer.AddChild(btn);
		}

		if (AvailableLevels.Count > 0)
		{
			var firstLevel = AvailableLevels[0];
			CallDeferred(MethodName.SelectLevel, firstLevel.LevelName, firstLevel.ScenePath, firstLevel.HighscoreKey, firstLevel.PreviewImage, firstLevel.HighscoreKey.Length > 0);
		}
	}

	private void SelectLevel(string levelName, string scenePath, string highscoreKey, Texture2D image, bool hasHighscore = true)
	{
		_currentScenePath = scenePath;
		_currentHighscoreKey = highscoreKey;

		_currentNameLabel.Text = levelName;
		_currentImage.Texture = image;

		if (hasHighscore) {
			var topScore = HighscoreList.LoadTopEntry(highscoreKey);
			_currentScoreLabel.Text = topScore != null ? $"{topScore.Name} - {topScore.Time}" : "Noch kein Highscore";
			iHateNodePathsBecuaseTheyAreWonkyAsHell.Show();
			iHateNodePathsBecuaseTheyAreWonkyAsHell2ElectricBenedict.Show();
		} else {
			iHateNodePathsBecuaseTheyAreWonkyAsHell.Hide();
			iHateNodePathsBecuaseTheyAreWonkyAsHell2ElectricBenedict.Hide();
		}

		AnimateRightPanel();
	}

	private void AnimateRightPanel()
	{
		_rightPanelTween?.Kill();

		_rightPanelContent.PivotOffset = _rightPanelContent.Size / 2f; 
		_rightPanelContent.Modulate = new Color(1, 1, 1, 0); 
		_rightPanelContent.Scale = new Vector2(0.95f, 0.95f); 

		_rightPanelTween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
		
		_rightPanelTween.TweenProperty(_rightPanelContent, "modulate:a", 1.0f, 0.25f);
		_rightPanelTween.TweenProperty(_rightPanelContent, "scale", Vector2.One, 0.25f);
	}

	private async void OnStartPressed()
	{
		if (string.IsNullOrEmpty(_currentScenePath)) return;
		
		await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile(_currentScenePath);
	}

	private void OnHighscorePressed()
	{
		if (string.IsNullOrEmpty(_currentHighscoreKey)) return;
		EmitSignal(SignalName.ScoreOpen, _currentHighscoreKey);
	}

	public void _on_close_button_pressed() 
	{
	   EmitSignal(SignalName.Closed);
	}
}
