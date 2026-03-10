using Godot;
using System;

[GlobalClass]
public partial class SuperjumpItem : DurationItemResource {
	[Export]
	public float JumpStrength { get; set; } = 20f;

	public override void Use(Player player) {
		var previousJumpHeight = player.GetJumpHeight();

		player.SetJumpHeight(JumpStrength);

		var timer = player.GetTree().CreateTimer(Duration);
		timer.Timeout += () => player.SetJumpHeight(previousJumpHeight);
	}
}
