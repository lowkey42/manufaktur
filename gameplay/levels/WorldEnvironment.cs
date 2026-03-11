using Godot;
using System;

public partial class WorldEnvironment : Godot.WorldEnvironment
{

	public override void _Ready() {
		SettingsManager.Instance.UpdateWorldEnvironment();
	}
	
}
