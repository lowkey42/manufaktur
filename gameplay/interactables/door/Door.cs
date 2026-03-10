using Godot;
using System;

namespace Manufaktur.gameplay.interactables.door;

public partial class Door : Triggerable
{
	AnimationPlayer _animationPlayer;

	[Export] bool DoorOpen { get; set; } = false;

	public override void _Ready() {
		base._Ready();
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		if(DoorOpen) {
			_animationPlayer.Play("Open");
		} else {
			_animationPlayer.PlayBackwards("Open");
		}
	}

	private void ToggleDoor() {
		if (DoorOpen) {
			_animationPlayer.PlayBackwards("Open");
			DoorOpen = false;
		} else {
			_animationPlayer.Play("Open");
			DoorOpen = true;
		}
	}

	public override void Trigger() {
		this.ToggleDoor();
	}
}
