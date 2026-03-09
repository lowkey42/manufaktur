using Godot;

namespace Manufaktur.gameplay.triggerables;

public abstract partial class Triggerable : Node3D {
	[Export] public TriggerChannel Channel { get; set; }

	public override void _Ready() {
		base._Ready();
		AddToGroup($"triggerable_{Channel}");
	}

	public abstract void Trigger();
}
