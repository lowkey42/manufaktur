using Godot;
using System;

[GlobalClass]
public partial class BoostItem : ItemResource {
	[Export]
	public float SpeedMultiplier { get; set; } = 1.5f;

	public override void Use(Player player) {
		//todo continue here tomorrow
		//player.
	}
}
