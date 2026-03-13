using Godot;
using System;

public partial class CreditsMenu : Control
{
	[Signal]
	public delegate void ClosedEventHandler();

	[Export] private VideoStreamPlayer _video;

	private int _musicVolume;

	public void OnVisibilityChanged() {
		if (Visible) {
			var radio = Radio.Instance;
			_musicVolume = radio.GetMusicVolume();
			radio.SetMusicVolume(0);
			_video.Play();
			
		} else {
			_video.Stop();
			Radio.Instance.SetMusicVolume(_musicVolume);
		}
	}
	
	private void _on_close_button_pressed() {
		EmitSignal(SignalName.Closed);
	}
}
