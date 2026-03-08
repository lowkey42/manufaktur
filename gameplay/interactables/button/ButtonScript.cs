using System;
using System.Xml.Linq;
using Godot;

public partial class ButtonScript : Node3D {

	[Signal]
	public delegate void ButtonPressedEventHandler();

	public void _on_area_3d_body_entered(Node3D body) {
		if(body.)
		EmitSignal(SignalName.ButtonPressed);
		GD.Print($"Body entered!");
	}

	public void _on_area_3d_body_exited(Node3D body) {
		GD.Print($"Body exited!");
	}
}
