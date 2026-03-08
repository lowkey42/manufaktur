using Godot;

public partial class Main : Node
{
	[Export]
	public PackedScene TargetScene;

	// Change scene after a 1 second delay
	public override async void _Ready()
	{
		await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

		if (TargetScene == null) {
			GD.PrintErr("TargetScene is not set in the Inspector!");
			return;
		}

		GetTree().ChangeSceneToPacked(TargetScene);
	}
	
}
