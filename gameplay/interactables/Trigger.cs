using Godot;

namespace Manufaktur.gameplay;

public abstract partial class Trigger : Node3D {
	[Export] public TriggerChannel Channel { get; set; }
	[Export] public bool OnlyTriggerOnce { get; set; } = true;

	private bool fired = false;

	public virtual void FireTrigger() {
		if (OnlyTriggerOnce && fired) { return; }

		fired = true;

		GD.Print($"Firing trigger on channel {Channel}");

		string groupName = $"triggerable_{Channel}";
		var triggerables = GetTree().GetNodesInGroup(groupName);

		foreach (Node node in triggerables) {
			if (node is Triggerable triggerable) {
				triggerable.Trigger();
			}
		}
	}
}
