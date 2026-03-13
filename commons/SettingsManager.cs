using Godot;
using System;

public partial class SettingsManager : Node
{
    public static SettingsManager Instance { get; private set; }

    private ConfigFile _config = new ConfigFile();
    private const string _savePath = "user://settings.cfg";
    
    public bool Fullscreen { get; set; } = false;
    public float MasterVolume { get; set; } = 50f;
    public float MusicVolume { get; set; } = 50f;
    public float SfxVolume { get; set; } = 50f;
    public float UiVolume { get; set; } = 50f;
    public int ResolutionHeight { get; set; } = 1080;
    public int ResolutionWidth { get; set; } = 1920;
    public bool Ssao { get; set; } = false;
    public bool Ssr { get; set; } = false;
    public bool Ssil { get; set; } = false;


    private struct SettingsState
    {
        public bool Fullscreen;
        public float MasterVolume;
        public float MusicVolume;
        public float SfxVolume;
        public float UiVolume;
        public int ResolutionHeight;
        public int ResolutionWidth;
        public bool Ssao;
        public bool Ssr;
        public bool Ssil;
    }

    private SettingsState _lastAppliedState;
    private bool _isFirstApply = true; 

    public override void _Ready()
    {
       Instance = this;
       LoadSettings();
    }

    public void LoadSettings()
    {
       var err = _config.Load(_savePath);

       Fullscreen       = (bool)_config.GetValue("video", "fullscreen", true);
       MasterVolume     = (float)_config.GetValue("audio", "master_vol", 50.0f);
       MusicVolume      = (float)_config.GetValue("audio", "music_vol", 50.0f);
       SfxVolume        = (float)_config.GetValue("audio", "sfx_vol", 50.0f);
       UiVolume         = (float)_config.GetValue("audio", "ui_vol", 50.0f);
       Ssr              = (bool)_config.GetValue("video", "ssr", true);
       Ssao             = (bool)_config.GetValue("video", "ssao", true);
       Ssil             = (bool)_config.GetValue("video", "ssil", true);
       

       ResolutionHeight = (int)_config.GetValue("video", "ResolutionHeight", 1080);
       ResolutionWidth  = (int)_config.GetValue("video", "ResolutionWidth", 1920);
       
       SaveSettings();
       ApplySettings();
    }

    public void SaveSettings()
    {
       _config.SetValue("video", "fullscreen", Fullscreen);
       _config.SetValue("audio", "master_vol", MasterVolume);
       _config.SetValue("audio", "music_vol", MusicVolume);
       _config.SetValue("audio", "sfx_vol", SfxVolume);
       _config.SetValue("audio", "ui_vol", UiVolume);
       _config.SetValue("video", "ssr", Ssr);
       _config.SetValue("video", "ssao", Ssao);
       _config.SetValue("video", "ssil", Ssil);
       _config.SetValue("video", "ResolutionHeight", ResolutionHeight);
       _config.SetValue("video", "ResolutionWidth", ResolutionWidth);
       
       _config.Save(_savePath);
    }

    public void ApplySettings() 
    {

       bool videoModeChanged = _isFirstApply || _lastAppliedState.Fullscreen != Fullscreen;
       if (videoModeChanged) {
           DisplayServer.WindowSetMode(Fullscreen 
               ? DisplayServer.WindowMode.Fullscreen 
               : DisplayServer.WindowMode.Windowed);
       }

       bool resolutionChanged = _isFirstApply || 
                                _lastAppliedState.ResolutionWidth != ResolutionWidth || 
                                _lastAppliedState.ResolutionHeight != ResolutionHeight;
                                

       if ((resolutionChanged || videoModeChanged) && !Fullscreen) {
           DisplayServer.WindowSetSize(new Vector2I(ResolutionWidth, ResolutionHeight));
       }
       

       if (_isFirstApply || Math.Abs(_lastAppliedState.MasterVolume - MasterVolume) > 0.01f)
           AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(MasterVolume / 100f));
           
       if (_isFirstApply || Math.Abs(_lastAppliedState.MusicVolume - MusicVolume) > 0.01f)
           AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), Mathf.LinearToDb(MusicVolume / 100f));
           
       if (_isFirstApply || Math.Abs(_lastAppliedState.UiVolume - UiVolume) > 0.01f)
           AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("UI"), Mathf.LinearToDb(UiVolume / 100f) - 15f);
           
       if (_isFirstApply || Math.Abs(_lastAppliedState.SfxVolume - SfxVolume) > 0.01f)
           AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Sound Effects"), Mathf.LinearToDb(SfxVolume / 100f));


       bool envChanged = _isFirstApply || 
                         _lastAppliedState.Ssao != Ssao || 
                         _lastAppliedState.Ssr != Ssr || 
                         _lastAppliedState.Ssil != Ssil;
                         
       if (envChanged) {
           UpdateWorldEnvironment();
       }


       SaveCurrentStateAsApplied();
    }
    

    private void SaveCurrentStateAsApplied()
    {
        _lastAppliedState = new SettingsState
        {
            Fullscreen = Fullscreen,
            MasterVolume = MasterVolume,
            MusicVolume = MusicVolume,
            SfxVolume = SfxVolume,
            UiVolume = UiVolume,
            ResolutionHeight = ResolutionHeight,
            ResolutionWidth = ResolutionWidth,
            Ssao = Ssao,
            Ssr = Ssr,
            Ssil = Ssil
        };
        _isFirstApply = false; 
    }

    public void UpdateWorldEnvironment()
    {
       var worldEnv = GetTree().Root.FindChild("WorldEnvironment", true, false) as WorldEnvironment;
    
       if (worldEnv != null && worldEnv.Environment != null)
       {
          worldEnv.Environment.SsaoEnabled = Ssao;
          worldEnv.Environment.SsrEnabled  = Ssr;
          worldEnv.Environment.SsilEnabled = Ssil;
       }
    }
}
