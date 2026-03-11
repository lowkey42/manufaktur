using Godot;


public partial class MainMenu : Control {
	private SettingsMenu _settingsMenu;
	private LevelSelection _levelSelectionMenu;
	private Control _mainMenu;
	private CreditsMenu _creditsMenu;
	private HighScoreMenu _highScoreMenu;
	

	public override void _Ready()
	{
		_settingsMenu = GetNode<SettingsMenu>("%SettingsPanel");
		_levelSelectionMenu = GetNode<LevelSelection>("%LevelSelection");
		_mainMenu = GetNode<Control>("%MainMenuPanel");
		_creditsMenu = GetNode<CreditsMenu>("%CreditsMenuPanel");
		_highScoreMenu = GetNode<HighScoreMenu>("%HighScorePanel");
		
		_levelSelectionMenu.Closed    += CloseLevelSelection;
		_creditsMenu.Closed           += CloseAnimCredits;
		_settingsMenu.Closed          += CloseSettings;
		_highScoreMenu.Closed         += OnHighScoreClosed;
		_levelSelectionMenu.ScoreOpen += OnScoreOpened;

		ShowMenu("Main");

	}
	
	private void AnimMenu()
	{
		Vector2 startPos = _mainMenu.Position;

		_mainMenu.Modulate = new Color(1,1,1,0);
		_mainMenu.Position = startPos + new Vector2(0, -200);

		var tween = GetTree()
		           .CreateTween()
		           .SetParallel()
		           .SetEase(Tween.EaseType.Out)
		           .SetTrans(Tween.TransitionType.Cubic);

		tween.TweenProperty(_mainMenu, "position:y", startPos.Y, 1.0f);
		tween.TweenProperty(_mainMenu, "modulate:a", 1.0f, 1.0f);
		AnimateButtons();
	}
	
	
	private void ShowMenu(string menuName)
	{
		_settingsMenu.Visible       = menuName == "Settings";
		_levelSelectionMenu.Visible = menuName == "LevelSelection";
		_mainMenu.Visible           = menuName == "Main";
		_creditsMenu.Visible        = menuName == "Credits";
		_highScoreMenu.Visible      = menuName == "HighScore";
		AnimMenu();
	}
	
	public void OpenSettings()
	{
		_mainMenu.Visible = false;
		
		_settingsMenu.Visible = true;
		Vector2 finalPos = _settingsMenu.Position; 
    
		_settingsMenu.Modulate = new Color(1, 1, 1, 0);
		_settingsMenu.Position = finalPos + new Vector2(0, 300);


		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    
		tween.TweenProperty(_settingsMenu, "position:y", finalPos.Y, 1f);
		tween.TweenProperty(_settingsMenu, "modulate:a", 1.0f, 1f);
	}

	private void CloseSettings() {
		Vector2 startPos = _settingsMenu.Position;
		float   targetY  = startPos.Y + 300f;

		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);


		tween.TweenProperty(_settingsMenu, "position:y", targetY, 1f);
		tween.TweenProperty(_settingsMenu, "modulate:a", 0.0f, 1f);


		tween.Chain().TweenCallback(Callable.From(() => {
			_settingsMenu.Visible = false;

			_settingsMenu.Position = startPos;

			ShowMenu("Main");
		}));
	}



	public void OpenLevelSelection()
	{
		_mainMenu.Visible = false;
		
		_levelSelectionMenu.Visible = true;
		Vector2 finalPos = _levelSelectionMenu.Position; 
    
		_levelSelectionMenu.Modulate = new Color(1, 1, 1, 0);
		_levelSelectionMenu.Position = finalPos + new Vector2(0, -300);


		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    
		tween.TweenProperty(_levelSelectionMenu, "position:y", finalPos.Y, 1f);
		tween.TweenProperty(_levelSelectionMenu, "modulate:a", 1.0f, 1f);
	}
	
	private void CloseLevelSelection()
	{
		Vector2 startPos = _levelSelectionMenu.Position;
		float   targetY  = startPos.Y - 300f;

		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    

		tween.TweenProperty(_levelSelectionMenu, "position:y", targetY, 1f);
		tween.TweenProperty(_levelSelectionMenu, "modulate:a", 0.0f, 1f);
    

		tween.Chain().TweenCallback(Callable.From(() => {
			_levelSelectionMenu.Visible = false;
			
			_levelSelectionMenu.Position = startPos; 
			
			ShowMenu("Main"); 
		}));
	}
	
	private void CloseAnimCredits()
	{
		Vector2 startPos = _creditsMenu.Position;
		float   targetY  = startPos.Y - 300f;

		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    

		tween.TweenProperty(_creditsMenu, "position:y", targetY, 1f);
		tween.TweenProperty(_creditsMenu, "modulate:a", 0.0f, 1f);
    

		tween.Chain().TweenCallback(Callable.From(() => {
			_creditsMenu.Visible = false;
			
			_creditsMenu.Position = startPos; 
			
			ShowMenu("Main"); 
		}));
	}
	
	public void OpenCredits()
	{
		_mainMenu.Visible = false;
		
		_creditsMenu.Visible = true;
		Vector2 finalPos = _creditsMenu.Position; 
    
		_creditsMenu.Modulate = new Color(1, 1, 1, 0);
		
		_creditsMenu.Position = finalPos + new Vector2(0, 300); 
		
		var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    
		tween.TweenProperty(_creditsMenu, "position:y", finalPos.Y, 1f);
		tween.TweenProperty(_creditsMenu, "modulate:a", 1.0f, 1f);
	}
	

	
	
	private void OnMenuClosed()
	{
		_levelSelectionMenu.Visible = false;
		_highScoreMenu.Visible      = false;
		_settingsMenu.Visible       = false;
		_creditsMenu.Visible      = false;
		ShowMenu("Main");
	}

	private void OnScoreOpened(string path) {
		ShowMenu("HighScore");
		_highScoreMenu._load_score(path);
	}
	
	private void OnHighScoreClosed()
	{
		ShowMenu("LevelSelection");
	}
	
	private void _on_close_button_pressed() {
		GetTree().Quit();
	}
	
	private void AnimateButtons()
	{
		var container = GetNode<VBoxContainer>("%MainMenuPanel/VBoxContainer");
		var tween     = GetTree().CreateTween();


		foreach (Node child in container.GetChildren())
		{
			if (child is Control button)
			{
				button.Scale    = new Vector2(0.5f, 0.5f);
				button.Modulate = new Color(1,1,1,0);

				tween.TweenInterval(0.08f);

				tween.SetParallel();
				tween.TweenProperty(button, "scale", Vector2.One, 0.25f);
				tween.TweenProperty(button, "modulate:a", 1.0f, 0.25f);
				tween.SetParallel(false);
			}
		}
	}
	
}
