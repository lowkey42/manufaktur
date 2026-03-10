using Godot;
using System;

[GlobalClass]
public abstract partial class ItemResource : Resource {
	[Export] public Texture2D Texture { get; set; }

	public abstract void Use(Player player);
}

[GlobalClass]
public abstract partial class DurationItemResource : ItemResource {
	[Export]
	public float Duration { get; set; } = 5f;
}
