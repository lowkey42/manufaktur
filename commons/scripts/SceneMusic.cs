using Godot;

[GlobalClass]
public partial class SceneMusic : Node
{
	[Export]
	public AudioStream Music { get; set; }
	public override void _Ready()
	{
		Radio.Instance.PlayMusic(Music);
	}
}
