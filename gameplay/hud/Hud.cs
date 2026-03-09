using Godot;
using System;

public partial class Hud : CanvasLayer
{

	[Export] private RichTextLabel _labelCurrentTime;
	[Export] private RichTextLabel _labelBestTime;

	public void SetCurrentTime(float currentTime) {
		_labelCurrentTime.Text = FormatTime(currentTime);
	}
	
	public void SetBestTime(float bestTime) {
		_labelBestTime.Text = FormatTime(bestTime);
	}

	private static string FormatTime(float currentTime) {
		return TimeSpan.FromSeconds(currentTime).ToString(@"m\:ss\.fff");
	}
}
