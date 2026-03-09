using Godot;

namespace Manufaktur.gameplay.interactables.wall;

public partial class Wall : Triggerable {

	[ExportGroup("Nodes")]
	[Export] public NodePath MeshPath = "Body/Mesh";
	[Export] public NodePath ColliderPath = "Body/Collider";

	[ExportGroup("Options")]
	[Export] public bool StartDisabled = false;

	private Node3D _visual3D;
	private CollisionObject3D _collider;

	private bool _isDisabled;

	public override void _Ready() {
		base._Ready();

		_visual3D = GetNodeOrNull<Node3D>(MeshPath);
		_collider = GetNodeOrNull<CollisionObject3D>(ColliderPath);

		_isDisabled = StartDisabled;
		ApplyState();
	}

	public override void Trigger() {
		_isDisabled = !_isDisabled;
		ApplyState();
	}

	private void ApplyState() {
		// Visual
		if (_visual3D != null)
			_visual3D.Visible = !_isDisabled;

		// Collision
		if (_collider != null)
			_collider.ProcessMode = _isDisabled ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
	}
}
