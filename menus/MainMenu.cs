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

		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = true;
		_creditsMenu.Visible        = false;
	}
	
	private void OnMenuClosed()
	{
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = true;
		_creditsMenu.Visible        = false;
		_highScoreMenu.Visible      = false;
	}

	private void OnScoreOpened(string path) {
		_highScoreMenu.Visible      = true;
		_levelSelectionMenu.Visible = false;
		GD.Print(path);
		
		_highScoreMenu._load_score(path);
	}
	
	private void OnHighScoreClosed()
	{
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = true;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
		_highScoreMenu.Visible      = false;
	}

	public void _on_start_button_pressed() {
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = true;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
		_highScoreMenu.Visible      = false;
	}
	public void _on_settings_button_pressed() {
		_settingsMenu.Visible       = true;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
		_highScoreMenu.Visible      = false;
	}
	
	public void _on_exit_button_button_pressed() {
		GetTree().Quit();

	}
	
	public void _on_high_score_button_pressed() {
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
		_highScoreMenu.Visible      = true;
	}

	public void _on_credits_button_pressed() {
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = true;
		_highScoreMenu.Visible      = false;
	}
	
}
