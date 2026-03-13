using Godot;
using System;

public partial class CreditsMenu : Control
{
	[Signal]
	public delegate void ClosedEventHandler();

	
	private void _on_close_button_pressed() {
		EmitSignal(SignalName.Closed);
	}
}
