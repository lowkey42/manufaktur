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
	
	private void ShowMenu(string menuName)
	{
		_settingsMenu.Visible       = menuName == "Settings";
		_levelSelectionMenu.Visible = menuName == "LevelSelection";
		_mainMenu.Visible           = menuName == "Main";
		_creditsMenu.Visible        = menuName == "Credits";
		_highScoreMenu.Visible      = menuName == "HighScore";
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
	
	private void _on_exit_button_button_pressed() {
		GetTree().Quit();
	}
	
}
