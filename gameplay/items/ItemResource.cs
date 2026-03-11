using Godot;

public abstract partial class ItemResource : Resource {
	[Export] public Texture2D Texture { get; set; }

	public abstract void Use(Player player);
}
