using Godot;
using Manufaktur.gameplay;

public partial class LevelSelection : Control
{
    [Signal]
    public delegate void ClosedEventHandler();

    [Signal]
    public delegate void ScoreOpenEventHandler(string levelName);


    private VBoxContainer _rightPanelContent; 
    private RichTextLabel _currentNameLabel;
    private TextureRect _currentImage;
    private RichTextLabel _currentScoreLabel;
    
    private Button _startLevelButton;
    private Button _highScoreButton;


    private string _currentScenePath = "";
    private string _currentHighscoreKey = "";
    private Tween _rightPanelTween; 

    public override void _Ready()
    {

        _rightPanelContent = GetNode<VBoxContainer>("%RightPanelContent");
        
        _currentNameLabel  = GetNode<RichTextLabel>("%CurrentNameLabel");
        _currentImage      = GetNode<TextureRect>("%CurrentLevelImage");
        _currentScoreLabel = GetNode<RichTextLabel>("%CurrentScoreLabel");
        
        _startLevelButton  = GetNode<Button>("%StartLevelButton");
        _highScoreButton   = GetNode<Button>("%HighScoreButton");


        _startLevelButton.Pressed += OnStartPressed;
        _highScoreButton.Pressed  += OnHighscorePressed;


        var btn1 = GetNode<Button>("%Level1Button");
        var btn2 = GetNode<Button>("%Level2Button");
        var btn3 = GetNode<Button>("%Level3Button");
        var btn4 = GetNode<Button>("%Level4Button");

        Texture2D defaultImg       = GD.Load<Texture2D>("res://commons/textures/placeholder.png");
        Texture2D beachlevelimg    = GD.Load<Texture2D>("res://commons/textures/beachlevel01_preview.png");
        Texture2D beachlevel_2_img = GD.Load<Texture2D>("res://commons/textures/beachlevel02_preview.png");
        Texture2D factorylevel_1_img = GD.Load<Texture2D>("res://commons/textures/factorylevel01_preview.png");
        Texture2D factorylevel_2_img = GD.Load<Texture2D>("res://commons/textures/factorylevel02_preview.png");

        btn1.Pressed += () => SelectLevel("Beach Level 1", "res://menus/story_menu_real.tscn", "baselevel", beachlevelimg);
        btn2.Pressed += () => SelectLevel("Beach Level 2", "res://gameplay/levels/BeachLevel02.tscn", "BeachLevel02", beachlevel_2_img);
        btn3.Pressed += () => SelectLevel("Factory Level 1", "res://gameplay/levels/FactoryLevel.tscn", "FactoryLevel", factorylevel_1_img);
        btn4.Pressed += () => SelectLevel("Factory Level 2", "res://gameplay/levels/FactoryLevel02.tscn", "FactoryLevel2", factorylevel_2_img);


        CallDeferred(MethodName.SelectLevel, "baselevel", "res://menus/story_menu_real.tscn", "baselevel", beachlevelimg);
    }
    
    private void SelectLevel(string levelName, string scenePath, string highscoreKey, Texture2D image)
    {

        _currentScenePath = scenePath;
        _currentHighscoreKey = highscoreKey;

        _currentNameLabel.Text = levelName;
        _currentImage.Texture = image;

        var topScore = HighscoreList.LoadTopEntry(highscoreKey);
        _currentScoreLabel.Text = topScore != null ? $"{topScore.Name} - {topScore.Time}" : "Noch kein Highscore";


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
