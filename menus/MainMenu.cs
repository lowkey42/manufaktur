using Godot;
using System;

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
		
		_levelSelectionMenu.Closed    += OnMenuClosed;
		_creditsMenu.Closed           += OnMenuClosed;
		_settingsMenu.Closed          += OnMenuClosed;
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
	
	private void OnMenuClosed()
	{
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
	
	private void _on_close_button_button_pressed() {
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
