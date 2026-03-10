using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
	private Stack<ItemResource> InventoryCollection { get; set; } = new();

	public void CollectItem(ItemResource item) {
		InventoryCollection.Push(item);
	}

	public ItemResource UseItem() {
		if(InventoryCollection.TryPop(out var item)) {
			return item;
		}

		return null;
	}

	public IEnumerable<ItemResource> GetItems() { return InventoryCollection; }

	public void Clear() { InventoryCollection.Clear(); }
}
