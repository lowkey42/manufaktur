using Godot;

namespace Manufaktur.gameplay.interactables.wall;

public partial class Wall : Triggerable {
	[Export] public bool StartDisabled = false;

	[Export] private AudioStreamPlayer3D _plopSoundPlayer;

private Node3D _mesh;
	private StaticBody3D _collider;

	private bool _isDisabled;

	public override void _Ready() {
		base._Ready();

		_mesh = GetNodeOrNull<Node3D>("Body/Mesh");
		_collider = GetNodeOrNull<StaticBody3D>("Body");

		_isDisabled = StartDisabled;
		ApplyState();
	}

	public override void Trigger() {
		_isDisabled = !_isDisabled;
		
		ApplyState();
		
		if(_isDisabled)
		{
			_plopSoundPlayer.Play();
		}
	}

	private void ApplyState() {
		// Visual
		if (_mesh != null) {
			_mesh.Visible = !_isDisabled;
		}

		// Collision
		if (_collider != null) {
			uint collisionlayer = _isDisabled ? (uint) GameHelpers.CollisionLayers.Noclip : (uint) GameHelpers.CollisionLayers.Interactables;
			
			_collider.CollisionLayer = collisionlayer;
		}
	}
}
