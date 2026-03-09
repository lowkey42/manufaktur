using Godot;
using Manufaktur.gameplay;
using System;

public partial class ItemSpawer : Triggerable {
	[Export] public ItemResource item { get; set; }
	[Export] public bool SpawnOnTrigger { get; set; } = true;
	[Export] public bool AnimateMesh { get; set; } = true;
	[Export] public float FloatHeight { get; set; } = 0.3f;
	[Export] public float FloatDuration { get; set; } = 2.0f;
	[Export] public float RotationDuration { get; set; } = 4.0f;

	private Node3D itemNode;
	private MeshInstance3D meshInstance;

	private bool spawned = false;

	public override void _Ready() {
		base._Ready();

		itemNode = GetNodeOrNull<Node3D>("Item");

		var collectionArea = GetNodeOrNull<Area3D>("Item/CollectionArea");
		collectionArea.BodyEntered += OnCollectionAreaBodyEntered;

		if (!SpawnOnTrigger) {
			SpawnItem();
		}
	}

	private void SpawnItem() {
		if (!spawned) {
			spawned = true;

			if (item != null) {
				meshInstance = new MeshInstance3D();
				meshInstance.Mesh = item.Mesh;

				itemNode.AddChild(meshInstance);

				if (AnimateMesh) {
					AnimateSpawnedItem(meshInstance);
				}
			}
		}
	}

	private void DespawnItem() {
		if (spawned) {
			spawned = false;
			if (meshInstance != null) {
				meshInstance.QueueFree();
				meshInstance = null;
			}
		}
	}

	private void AnimateSpawnedItem(Node3D mesh) {
		var floatTween = CreateTween();
		floatTween.SetLoops();
		floatTween.SetTrans(Tween.TransitionType.Sine);
		floatTween.SetEase(Tween.EaseType.InOut);
		floatTween.TweenProperty(mesh, "position:y", FloatHeight, FloatDuration / 2);
		floatTween.TweenProperty(mesh, "position:y", 0, FloatDuration / 2);

		var rotationTween = CreateTween();
		rotationTween.SetLoops();
		rotationTween.TweenProperty(mesh, "rotation:y", Mathf.Tau, RotationDuration).From(0);
	}

	private void OnCollectionAreaBodyEntered(Node3D body) {
		if (body is Player player) {
			player.Inventory.CollectItem(item);
			DespawnItem();
		}
	}

	public override void Trigger() {
		if (SpawnOnTrigger) {
			SpawnItem();
		}
	}
}
