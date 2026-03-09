using Godot;
using System;

public partial class MainMenu : Control {
	private SettingsMenu _settingsMenu;
	private LevelSelection _levelSelectionMenu;
	private Control _mainMenu;
	private CreditsMenu _creditsMenu;
	

	public override void _Ready()
	{
		_settingsMenu = GetNode<SettingsMenu>("%SettingsPanel");
		_levelSelectionMenu = GetNode<LevelSelection>("%LevelSelection");
		_mainMenu = GetNode<Control>("%MainMenuPanel");
		_creditsMenu = GetNode<CreditsMenu>("%CreditsMenuPanel");
		
		_levelSelectionMenu.Closed += OnMenuClosed;
		_creditsMenu.Closed        += OnMenuClosed;
		_settingsMenu.Closed       += OnMenuClosed;

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
	}

	public void _on_start_button_button_down() {
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = true;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
	}
	public void _on_settings_button_button_down() {
		_settingsMenu.Visible       = true;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = false;
	}
	
	public void _on_exit_button_button_down() {
		GetTree().Quit();

	}

	public void _on_credits_button_pressed() {
		_settingsMenu.Visible       = false;
		_levelSelectionMenu.Visible = false;
		_mainMenu.Visible           = false;
		_creditsMenu.Visible        = true;
	}
	
}
