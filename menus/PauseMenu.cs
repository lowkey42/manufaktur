using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	public override void _Input(InputEvent e) {
		if (e.IsActionPressed("pause")) {
			this.Visible = !this.Visible;
			if (this.Visible) {
				OnPauseButtonPressed();

			} else {
				OnResumeButtonPressed();

			}

		}
	}

	private void OnPauseButtonPressed() {
		this.Visible     = true;
		GetTree().Paused = true;
		Input.MouseMode  = Input.MouseModeEnum.Visible;
	}

	private void OnResumeButtonPressed() {
		this.Visible = false;
		GetTree().Paused = false;
		Input.MouseMode  = Input.MouseModeEnum.Captured;
	}

	private void OnResetButtonPressed() {

		GetTree().Paused = false;
		var error = GetTree().ReloadCurrentScene();
		if (error != Error.Ok) {
			GD.PrintErr(error);
		}
		Input.MouseMode  = Input.MouseModeEnum.Captured;
	}

	private void OnExitButtonPressed() {
		
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://menus/main_menu.tscn");
	}

}
