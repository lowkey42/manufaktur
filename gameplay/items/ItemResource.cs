using Godot;
using System;

[GlobalClass]
public abstract partial class ItemResource : Resource {
	[Export] public Texture2D Icon { get; set; }
	[Export] public Mesh Mesh { get; set; }

	public abstract void Use(Player player);
}
