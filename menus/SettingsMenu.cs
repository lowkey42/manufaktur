using Godot;

public partial class SettingsMenu : Control 
{
    // Keine [Export] mehr nötig, wir holen das sicher per Code
    private HSlider _masterSlider;
    private HSlider _sfxSlider;
    private CheckBox _fullscreenBox;
    private OptionButton _resDropdown;
    private CheckBox _ssaoButton;
    private CheckBox _ssilButton;
    private CheckBox _ssrButton;
    
    private PanelContainer _panel; // Für die Animation

    [Signal]
    public delegate void ClosedEventHandler();

    public override void _Ready() 
    {

       _masterSlider  = GetNode<HSlider>("%Master_Slider");
       _sfxSlider     = GetNode<HSlider>("%SFX_Slider");
       _fullscreenBox = GetNode<CheckBox>("%Fullscreen_CheckBox");
       _resDropdown   = GetNode<OptionButton>("%Resolution_Dropdown");
       _ssaoButton    = GetNode<CheckBox>("%SSAO_CheckBox");
       _ssilButton    = GetNode<CheckBox>("%SSIL_CheckBox");
       _ssrButton     = GetNode<CheckBox>("%SSR_CheckBox");
       _panel         = GetNode<PanelContainer>("PanelContainer"); // Das sichtbare Fenster


       _masterSlider.Value          = SettingsManager.Instance.MasterVolume;
       _sfxSlider.Value             = SettingsManager.Instance.SfxVolume;
       _fullscreenBox.ButtonPressed = SettingsManager.Instance.Fullscreen;
       _ssaoButton.ButtonPressed    = SettingsManager.Instance.Ssao;
       _ssilButton.ButtonPressed    = SettingsManager.Instance.Ssil;
       _ssrButton.ButtonPressed     = SettingsManager.Instance.Ssr;
       
       switch (SettingsManager.Instance.ResolutionWidth) {
          case 1920:
             _resDropdown.Selected = 0;
             break;
          case 2560:
             _resDropdown.Selected = 1;
             break;
          case 1280:
             _resDropdown.Selected = 2;
             break;
       }
    }

    private void _on_ssao_check_box_toggled(bool toggledOn) {
       SettingsManager.Instance.Ssao = toggledOn;
       SettingsManager.Instance.ApplySettings();
    }

    private void _on_ssr_check_box_toggled(bool toggledOn) {
       SettingsManager.Instance.Ssr = toggledOn;
       SettingsManager.Instance.ApplySettings();
    }

    private void _on_ssil_check_box_toggled(bool toggledOn) {
       SettingsManager.Instance.Ssil = toggledOn;
       SettingsManager.Instance.ApplySettings();
    }


    private void _on_fullscreen_check_box_toggled(bool toggledOn) {
       SettingsManager.Instance.Fullscreen = toggledOn;
       SettingsManager.Instance.ApplySettings();
    }

    private void _on_master_slider_value_changed(float value) {
       SettingsManager.Instance.MasterVolume = value;
       SettingsManager.Instance.ApplySettings();
    }
    
    private void _on_sfx_slider_value_changed(float value) {
       SettingsManager.Instance.SfxVolume = value;
       SettingsManager.Instance.ApplySettings();
    }

    private void _on_resolution_dropdown_item_selected(int index) {
       switch (index) {
          case 0:
             SettingsManager.Instance.ResolutionWidth  = 1920;
             SettingsManager.Instance.ResolutionHeight = 1080;
             SettingsManager.Instance.ApplySettings();
             break;
          case 1:
             SettingsManager.Instance.ResolutionWidth  = 2560;
             SettingsManager.Instance.ResolutionHeight = 1440;
             SettingsManager.Instance.ApplySettings();
             break;
          case 2:
             SettingsManager.Instance.ResolutionWidth  = 1280;
             SettingsManager.Instance.ResolutionHeight = 720;
             SettingsManager.Instance.ApplySettings();
             break;
       }
    }

    private void _on_close_button_pressed() {
       EmitSignal(SignalName.Closed);
    }

    private void _on_save_button_pressed() {
       SettingsManager.Instance.SaveSettings();
       EmitSignal(SignalName.Closed);
    }
    
    public void start_anim() 
    {

        if (_panel == null) return;

        Vector2 startPos = _panel.Position;
        
        _panel.Modulate = new Color(1, 1, 1, 0);
        _panel.Position = startPos + new Vector2(0, -200);
        
        var tween = GetTree()
                    .CreateTween()
                    .SetParallel()
                    .SetEase(Tween.EaseType.Out)
                    .SetTrans(Tween.TransitionType.Cubic);
        
        tween.TweenProperty(_panel, "position:y", startPos.Y, 0.8f);
        tween.TweenProperty(_panel, "modulate:a", 1.0f, 0.5f);
    }
}
