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
       

       _levelSelectionMenu.Closed += () => CloseSubMenu(_levelSelectionMenu, -1000f, OpenMainMenu); 
       _creditsMenu.Closed        += () => CloseSubMenu(_creditsMenu, -1000f, OpenMainMenu);
       _settingsMenu.Closed       += () => CloseSubMenu(_settingsMenu, 1000f, OpenMainMenu);
       _highScoreMenu.Closed      += () => CloseSubMenu(_highScoreMenu, 1000f, OpenLevelSelection);
       

       _levelSelectionMenu.ScoreOpen += OnScoreOpened;


       ShowMenu("Main");
    }
	

    private void OpenSubMenu(Control menu, float yOffset) {

	    menu.Visible = true;
   
	    Vector2 menuFinalPos = menu.Position; 
	    Vector2 mainStartPos = _mainMenu.Position;
    
	    var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
    

	    tween.TweenProperty(menu, "position:y", menuFinalPos.Y, 0.8f).From(menuFinalPos.Y + yOffset);
	    tween.TweenProperty(menu, "modulate:a", 1.0f, 0.3f).From(0.0f);


	    tween.TweenProperty(_mainMenu, "position:y", mainStartPos.Y + 1000f, 1f);
	    tween.TweenProperty(_mainMenu, "modulate:a", 0.0f, 0.5f);


	    tween.Chain().TweenCallback(Callable.From(() => {
		    _mainMenu.Visible  = false;
		    _mainMenu.Position = mainStartPos; 
		    _mainMenu.Modulate = new Color(1, 1, 1, 1);
	    }));
    }

    private void CloseSubMenu(Control menu, float yOffset, Action onComplete)
    {
	    Vector2 startPos = menu.Position;
	    float   targetY  = startPos.Y + yOffset;
	    
	    var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Sine);

	    tween.TweenProperty(menu, "position:y", targetY, 0.8f);
	    tween.TweenProperty(menu, "modulate:a", 0.0f, 0.3f);

	    tween.Chain().TweenCallback(Callable.From(() => {
		    menu.Visible  = false;
		    menu.Position = startPos; 
		    onComplete?.Invoke(); 
	    }));
    }
    

    public void OpenSettings()       => OpenSubMenu(_settingsMenu, 1000f);       
    public void OpenLevelSelection() => OpenSubMenu(_levelSelectionMenu, -1000f);
    public void OpenCredits()        => OpenSubMenu(_creditsMenu, 1000f);
    
    private void OpenMainMenu()      => ShowMenu("Main");


    private void OnScoreOpened(string path) 
    {
       CloseSubMenu(_levelSelectionMenu, -300f, () => {
           _highScoreMenu._load_score(path);
           OpenSubMenu(_highScoreMenu, 300f);
       });
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
    private void AnimMenu()
    {
	    Vector2 startPos = _mainMenu.Position;

	    var tween = GetTree().CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);


	    tween.TweenProperty(_mainMenu, "position:y", startPos.Y, 0.6f).From(startPos.Y + 1000f);
	    tween.TweenProperty(_mainMenu, "modulate:a", 1.0f, 0.4f).From(0.0f);
       
	    AnimateButtons();
    }

    private void AnimateButtons()
    {
	    var container = GetNode<VBoxContainer>("%MainMenuPanel/VBoxContainer");
	    var tween     = GetTree().CreateTween();

	    foreach (Node child in container.GetChildren())
	    {
		    if (child is Control button)
		    {
			    
			    button.Modulate = new Color(1, 1, 1, 0);

			    tween.TweenInterval(0.05f);
              
			    
			    tween.TweenProperty(button, "modulate:a", 1.0f, 0.3f)
			         .SetTrans(Tween.TransitionType.Sine)
			         .SetEase(Tween.EaseType.Out);
		    }
	    }
    }
}
