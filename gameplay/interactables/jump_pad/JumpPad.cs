using Godot;
using System;

public partial class JumpPad : Node3D {
	private Area3D _area;
	
	[Export] public float JumpForce { get; set; } = 65.0f;
	[Export] private AudioStreamPlayer3D _boingSoundPlayer;

	public override void _Ready() {
		_area = GetNodeOrNull<Area3D>("JumpPadArea");

		if (_area != null) {
			_area.BodyEntered += OnBodyEntered;
		}
	}

	private void OnBodyEntered(Node3D body) {
		if (body is Player player) {
			player.Velocity = new Vector3(player.Velocity.X, JumpForce, player.Velocity.Z);
			_boingSoundPlayer.Play();
		}
	}
}
