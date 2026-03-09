using Godot;

[GlobalClass]
public partial class Finish : Node3D
{

	public void OnEnter(Node3D node) {
		EventBus.Instance.EmitFinishReached();
	}
	
}
