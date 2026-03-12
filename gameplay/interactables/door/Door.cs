using Godot;
using System;

namespace Manufaktur.gameplay.interactables.door;

public partial class Door : Triggerable
{
	AnimationPlayer _animationPlayer;

	[Export] bool DoorOpen { get; set; } = false;
	[Export] private AudioStreamPlayer3D _openSoundPlayer;
	[Export] private AudioStreamPlayer3D _closeSoundPlayer;

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
			_closeSoundPlayer.Play();
			DoorOpen = false;
		} else {
			_animationPlayer.Play("Open");
			_openSoundPlayer.Play();
			DoorOpen = true;
		}
	}

	public override void Trigger() {
		this.ToggleDoor();
	}
}
