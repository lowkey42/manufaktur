using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Node
{
	private Stack<ItemResource> InventoryCollection { get; set; } = new();

	public void CollectItem(ItemResource item) {
		InventoryCollection.Push(item);
		EventBus.Instance.EmitInventoryItemCollected(item, InventoryCollection.ToArray());
	}

	public ItemResource UseItem(Player player) {
		if(InventoryCollection.TryPop(out var item)) {
			item.Use(player);
			EventBus.Instance.EmitInventoryItemUsed(item, InventoryCollection.ToArray());
			return item;
		}

		return null;
	}
}
