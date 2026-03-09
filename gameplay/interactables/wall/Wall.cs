using Godot;

namespace Manufaktur.gameplay.interactables.wall;

public partial class Wall : Triggerable {
	[Export] public bool StartDisabled = false;

	private Node3D _mesh;
	private CollisionObject3D _collider;

	private bool _isDisabled;

	public override void _Ready() {
		base._Ready();

		_mesh = GetNodeOrNull<Node3D>("Body/Mesh");
		_collider = GetNodeOrNull<CollisionObject3D>("Body/Collider");

		_isDisabled = StartDisabled;
		ApplyState();
	}

	public override void Trigger() {
		_isDisabled = !_isDisabled;
		ApplyState();
	}

	private void ApplyState() {
		// Visual
		if (_mesh != null)
			_mesh.Visible = !_isDisabled;

		// Collision
		if (_collider != null)
			_collider.ProcessMode = _isDisabled ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
	}
}
