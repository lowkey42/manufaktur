using Godot;
using System;



public partial class LevelSelection : Control
{
	[Signal]
	public delegate void ClosedEventHandler();
	
	[Export]
	public PackedScene Level1;
	[Export]
	public PackedScene Level2;
	[Export]
	public PackedScene Level3;


	public void _on_close_button_pressed() {
		SettingsManager.Instance.SaveSettings();
		this.Visible = false;
		
		EmitSignal(SignalName.Closed);
	}

	public async void _on_load_level_1_button_pressed() {
		await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToPacked(Level1);
		
	}
	public async void _on_load_level_2_button_pressed() {
		await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToPacked(Level2);
		
	}	public async void _on_load_level_3_button_pressed() {
		await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToPacked(Level3);
		
	}
}
