using Godot;


public partial class SettingsMenu : Control {
	
	[Export] private Slider _masterSlider;
	[Export] private Slider _sfxSlider;
	[Export] private CheckBox _fullscreenBox;
	[Export] private OptionButton _resDropdown;
	[Export] private CheckBox _ssaoButton;
	[Export] private CheckBox _ssilButton;
	[Export] private CheckBox _ssrButton;
	
	[Signal]
	public delegate void ClosedEventHandler();


	public override void _Ready() {
		_masterSlider.Value          = SettingsManager.Instance.MasterVolume;
		_sfxSlider.Value             = SettingsManager.Instance.SfxVolume;
		_fullscreenBox.ButtonPressed = SettingsManager.Instance.Fullscreen;
		_ssaoButton.ButtonPressed    = SettingsManager.Instance.Ssao;
		_ssilButton.ButtonPressed    = SettingsManager.Instance.Ssil;
		_ssrButton.ButtonPressed    = SettingsManager.Instance.Ssr;
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
				SettingsManager.Instance.ResolutionHeight  = 1080;
				SettingsManager.Instance.ResolutionWidth = 1920;
				SettingsManager.Instance.ApplySettings();
				break;
			case 1:
				SettingsManager.Instance.ResolutionHeight = 1440;
				SettingsManager.Instance.ResolutionWidth  = 2560;
				SettingsManager.Instance.ApplySettings();
				break;
			case 2:
				SettingsManager.Instance.ResolutionHeight = 720;
				SettingsManager.Instance.ResolutionWidth  = 1080;
				SettingsManager.Instance.ApplySettings();
				break;
		}
	}

	private void _on_close_button_pressed() {
		this.Visible = false;
		EmitSignal(SignalName.Closed);
	}

	private void _on_save_button_pressed() {
		SettingsManager.Instance.SaveSettings();
		EmitSignal(SignalName.Closed);
	}
	
	public void start_anim() 
	{
				Vector2 startPos = this.Position;
        
        		this.Modulate = new Color(1,1,1,0);
        		this.Position = startPos + new Vector2(0, -200);
        
        		var tween = GetTree()
        		           .CreateTween()
        		           .SetParallel()
        		           .SetEase(Tween.EaseType.Out)
        		           .SetTrans(Tween.TransitionType.Cubic);
        
        		tween.TweenProperty(this, "position:y", startPos.Y, 1.0f);
        		tween.TweenProperty(this, "modulate:a", 1.0f, 1.0f);
	}
}
