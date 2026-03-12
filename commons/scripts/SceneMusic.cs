using Godot;

[GlobalClass]
public partial class SceneMusic : Node
{
	[Export]
	public AudioStream Music { get; set; }
	public override void _Ready()
	{
		if (Music == null) {
			GD.PrintErr("No scene music selected.");
			return;
		}

		Radio.Instance.PlayMusic(Music);
	}
}
