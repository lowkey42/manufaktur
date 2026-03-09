using Godot;

public partial class DebugResetRigidBody : Node3D {
	[Export] public float HeightAboveButton = 2.0f;
	
	private Node3D Button;
	private RigidBody3D Player;

	public override void _Ready() {
		base._Ready();
		Button = GetNode<Node3D>("button");
		Player = GetNode<RigidBody3D>("player");
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event.IsActionPressed("ui_accept")) // default: Space / Enter
		{
			ResetAboveButton();
			GetViewport().SetInputAsHandled();
		}
	}

	private void ResetAboveButton() {
		Player.Position = Button.Position + new Vector3(0, HeightAboveButton, 0);
	}
}
