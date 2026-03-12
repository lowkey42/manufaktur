using Godot;

public abstract partial class ItemResource : Resource {
	[Export] public Texture2D HandTexture { get; set; }
	[Export] public Texture2D SpawnerTexture { get; set; }
	[Export] public AudioStreamRandomizer UseSounds { get; set; }

	public virtual void Use(Player player) {
		player.ItemUseSoundPlayer.Stream = UseSounds;
		player.ItemUseSoundPlayer.Play();
	}
}
