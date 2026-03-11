using Godot;

public abstract partial class DurationItemResource : ItemResource {
	[Export]
	public float Duration { get; set; } = 5f;
}
