using Godot;
using System;

public partial class DashItem : DurationItemResource {
	[Export]
	public float DashStrength { get; set; } = 75.0f;

	public override void Use(Player player) {
		Vector3 direction = player.GetFacingDirection();

		Vector3 dashVector = direction.Normalized() * DashStrength;

		player.Velocity = dashVector;
	}
}
