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
       

       _levelSelectionMenu.Closed    += () => CloseSubMenu(_levelSelectionMenu, -300f, OpenMainMenu);
       _creditsMenu.Closed           += () => CloseSubMenu(_creditsMenu, -300f, OpenMainMenu);
       _settingsMenu.Closed          += () => CloseSubMenu(_settingsMenu, 300f, OpenMainMenu);
       

       _highScoreMenu.Closed         += () => CloseSubMenu(_highScoreMenu, 300f, OpenLevelSelection);
       

       _levelSelectionMenu.ScoreOpen += OnScoreOpened;


       ShowMenu("Main");
    }

    private void OpenSubMenu(Control menu, float yOffset)
    {
       _mainMenu.Visible = false; 
       menu.Visible = true;
       
       Vector2 finalPos = menu.Position; 

       var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
       
       tween.TweenProperty(menu, "position:y", finalPos.Y, 1f).From(finalPos.Y + yOffset);
       tween.TweenProperty(menu, "modulate:a", 1.0f, 1f).From(0.0f);
    }

    private void CloseSubMenu(Control menu, float yOffset, Action onComplete)
    {
       Vector2 startPos = menu.Position;
       float   targetY  = startPos.Y + yOffset;

       var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

       tween.TweenProperty(menu, "position:y", targetY, 1f);
       tween.TweenProperty(menu, "modulate:a", 0.0f, 1f);

       tween.Chain().TweenCallback(Callable.From(() => {
          menu.Visible = false;
          menu.Position = startPos; 
          
          onComplete?.Invoke(); 
       }));
    }



    public void OpenSettings()       => OpenSubMenu(_settingsMenu, 300f);
    public void OpenLevelSelection() => OpenSubMenu(_levelSelectionMenu, -300f);
    public void OpenCredits()        => OpenSubMenu(_creditsMenu, 300f);
    
    private void OpenMainMenu()      => ShowMenu("Main");


    private void OnScoreOpened(string path) 
    {

       CloseSubMenu(_levelSelectionMenu, -300f, () => {
           _highScoreMenu._load_score(path);
           OpenSubMenu(_highScoreMenu, 300f);
       });
    }
	

    private void AnimMenu()
    {
       Vector2 startPos = _mainMenu.Position;

       var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

       tween.TweenProperty(_mainMenu, "position:y", startPos.Y, 1.0f).From(startPos.Y - 200f);
       tween.TweenProperty(_mainMenu, "modulate:a", 1.0f, 1.0f).From(0.0f);
       AnimateButtons();
    }
    

    private void ShowMenu(string menuName)
    {
       _settingsMenu.Visible       = menuName == "Settings";
       _levelSelectionMenu.Visible = menuName == "LevelSelection";
       _mainMenu.Visible           = menuName == "Main";
       _creditsMenu.Visible        = menuName == "Credits";
       _highScoreMenu.Visible      = menuName == "HighScore";
       
       if (menuName == "Main") 
       {
           AnimMenu();
       }
    }
    
    private void _on_close_button_pressed() 
    {
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
			    button.Modulate = new Color(1, 1, 1, 0);


			    tween.TweenInterval(0.08f);
			    
			    tween.SetParallel();
			    tween.TweenProperty(button, "scale", Vector2.One, 0.25f);
			    tween.TweenProperty(button, "modulate:a", 1.0f, 0.25f);
			    
			    tween.SetParallel(false);
		    }
	    }
    }
}
