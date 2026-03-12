using Godot;
using Manufaktur.gameplay;

namespace Manufaktur.gameplay.interactables.button;

public partial class ButtonScript : Trigger {
	[Export] public bool TriggerOnEnter { get; set; } = true;
	[Export] public bool TriggerOnExit { get; set; } = false;

	[Export] private AudioStreamPlayer3D _pressSoundPlayer;
	
	private Area3D _area;
	private AnimationPlayer _player;

	public override void _Ready() {
		_area = GetNode<Area3D>("TriggerArea");
		_area.BodyEntered += OnAreaBodyEntered;
		_area.BodyExited += OnAreaBodyExited;

		_player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void OnAreaBodyEntered(Node3D body) {
		_player.Play("Press");
		_pressSoundPlayer.Play();

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
