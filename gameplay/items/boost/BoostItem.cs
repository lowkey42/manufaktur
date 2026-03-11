using Godot;
using System;

public partial class BoostItem : ItemResource {
	[Export]
	public float SpeedMultiplier { get; set; } = 2f;

	public override void Use(Player player) {
		player.Velocity *= SpeedMultiplier;
	}
}
