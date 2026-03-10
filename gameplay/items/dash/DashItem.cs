using Godot;
using System;

[GlobalClass]
public partial class DashItem : ItemResource
{
	[Export]
	public float DashStrength { get; set; } = 500f;

	public override void Use(Player player) {
		Vector3 direction = player.GetFacingDirection();

		Vector3 dashVector = direction.Normalized() * DashStrength;

		player.Velocity += dashVector;
	}
}
