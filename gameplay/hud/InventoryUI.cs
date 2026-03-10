using Godot;
using System;

public partial class InventoryUI : Control
{
	private VBoxContainer _itemsContainer;

	public override void _Ready() {
		EventBus.Instance.InventoryItemCollected += OnInventoryItemChanged;
		EventBus.Instance.InventoryItemUsed += OnInventoryItemChanged;

		_itemsContainer = GetNode<VBoxContainer>("ItemsContainer");
	}

	private void OnInventoryItemChanged(ItemResource item, ItemResource[] items) {
		foreach (Node child in _itemsContainer.GetChildren()) {
			child.QueueFree();
		}

		if (items != null) {
			foreach (ItemResource iterItem in items) {
				var itemTexture = new TextureRect();
				itemTexture.Texture = iterItem.Icon;
				_itemsContainer.AddChild(itemTexture);
			}
		}
	}
}
