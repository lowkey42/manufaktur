using Godot;
using Manufaktur.gameplay;

public partial class ButtonScript : Trigger {
	[Export] public bool TriggerOnEnter { get; set; } = true;
	[Export] public bool TriggerOnExit { get; set; } = false;

	private Area3D _area;
	private AnimationPlayer _player;

	public override void _Ready() {
		_area = GetNode<Area3D>("Area3D");
		_area.BodyEntered += OnAreaBodyEntered;
		_area.BodyExited += OnAreaBodyExited;

		_player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void OnAreaBodyEntered(Node3D body) {
		_player.Play("Press");
		GD.Print("Button pressed");

		if (TriggerOnEnter) {
			FireTrigger();
		}
	}

	public void OnAreaBodyExited(Node3D body) {
		_player.PlayBackwards("Press");
		GD.Print("Button released");

		if (TriggerOnExit) {
			FireTrigger();
		}
	}
}
