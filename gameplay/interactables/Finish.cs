using Godot;

[GlobalClass]
public partial class Finish : Node3D
{

	public void OnEnter(Node3D node) {
		if (node is Player)
			EventBus.Instance.EmitFinishReached();
	}
	
}
