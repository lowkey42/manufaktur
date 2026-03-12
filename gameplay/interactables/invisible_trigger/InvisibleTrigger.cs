using Godot;
using Manufaktur.gameplay;
using System;

public partial class InvisibleTrigger : Trigger
{
	[Export] public bool TriggerOnEnter { get; set; } = true;
	[Export] public bool TriggerOnExit { get; set; } = false;

	private Area3D _area;
	private AnimationPlayer _player;

	public override void _Ready() {
		_area = GetNode<Area3D>("TriggerArea");
		_area.BodyEntered += OnAreaBodyEntered;
		_area.BodyExited += OnAreaBodyExited;
	}

	public void OnAreaBodyEntered(Node3D body) {
		_player.Play("Press");

		if (TriggerOnEnter) {
			FireTrigger();
		}
	}

	public void OnAreaBodyExited(Node3D body) {
		_player.PlayBackwards("Press");

		if (TriggerOnExit) {
			FireTrigger();
		}
	}
}
