using Godot;

public abstract partial class ItemResource : Resource {
	[Export] public Texture2D HandTexture { get; set; }
	[Export] public Texture2D SpawnerTexture { get; set; }

	public abstract void Use(Player player);
}
