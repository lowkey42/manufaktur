using Godot;
using System;

public partial class SettingsManager : Node
{
	public static SettingsManager Instance { get; private set; }

	private ConfigFile _config = new ConfigFile();
	private const string _savePath = "user://settings.cfg";
	
	public bool Fullscreen { get; set; } = false;
	public float MasterVolume { get; set; } = 0.5f;
	public float SfxVolume { get; set; } = 0.5f;
	
	public int ResolutionHeight { get; set; } = 1920;
	public int ResolutionWidth { get; set; } = 1080;

	public bool Ssao { get; set; } = false;
	
	public bool Ssr { get; set; } = false;
	
	public bool Ssil { get; set; } = false;

	public override void _Ready()
	{
		Instance = this;
		LoadSettings();
		
	}

	public void LoadSettings()
	{
		var err = _config.Load(_savePath);

		if (err != Error.Ok) {
			GD.Print("Keine Konfigurationsdatei gefunden. Verwende Standardwerte.");
			return;
		}
		Fullscreen       = (bool)_config.GetValue("video", "fullscreen", false);
		MasterVolume     = (float)_config.GetValue("audio", "master_vol", 50.0f);
		SfxVolume     = (float)_config.GetValue("audio", "sfx_vol", 50.0f);
		Ssr              = (bool)_config.GetValue("video", "ssr", false);
		Ssao             = (bool)_config.GetValue("video", "ssao", false);
		Ssil             = (bool)_config.GetValue("video", "ssil", false);
		ResolutionHeight = (int)_config.GetValue("video", "ResolutionHeight", 1920);
		ResolutionWidth = (int)_config.GetValue("video", "ResolutionWidth", 1080);
		SaveSettings();
		ApplySettings();
	}

	public void SaveSettings()
	{
		_config.SetValue("video", "fullscreen", Fullscreen);
		_config.SetValue("audio", "master_vol", MasterVolume);
		_config.SetValue("audio", "sfx_vol", SfxVolume);
		_config.SetValue("video", "ssr", Ssr);
		_config.SetValue("video", "ssao", Ssao);
		_config.SetValue("video", "ssil", Ssil);
		_config.SetValue("video", "ResolutionHeight", ResolutionHeight);
		_config.SetValue("video", "ResolutionWidth", ResolutionWidth);
		
		_config.Save(_savePath);
	}

	public void ApplySettings() {
		DisplayServer.WindowSetMode(Fullscreen
			                            ? DisplayServer.WindowMode.Fullscreen
			                            : DisplayServer.WindowMode.Windowed);

		
		int busIndex = AudioServer.GetBusIndex("Master");
		AudioServer.SetBusVolumeDb(busIndex, (MasterVolume));
		
		UpdateWorldEnvironment();
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
