using Godot;
using System;

public partial class SpikeTrap : Node3D
{
	public void OnCollision(Node3D other) {
		if (other is Player) {
			EventBus.Instance.EmitPlayerDied();
		}
	}
}
