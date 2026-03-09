using System;
using System.Diagnostics;
using System.Xml.Linq;
using Godot;

public partial class ButtonScript : Node3D {

	[Signal]
	public delegate void ButtonPressedEventHandler();

	private Area3D _area;
	private AnimationPlayer _player;

	public override void _Ready() {
		_area = GetNode<Area3D>("Area3D");
		_area.BodyEntered += OnAreaBodyEntered;
		_area.BodyExited += OnAreaBodyExited;

		_player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void OnAreaBodyEntered(Node3D body) {
		EmitSignal(SignalName.ButtonPressed);
		_player.Play("Press");
		GD.Print($"Body entered!");
	}

	public void OnAreaBodyExited(Node3D body) {
		_player.PlayBackwards("Press");
		GD.Print($"Body exited!");
	}
}
