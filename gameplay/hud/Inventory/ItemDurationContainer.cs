using Godot;
using System;

public partial class ItemDuration : Control {
	[Export]
	public TextureProgressBar progressBarTemplate;

	public override void _Ready() {
		EventBus.Instance.InventoryItemUsed += OnInventoryItemUsed;
	}

	public void OnInventoryItemUsed(ItemResource item, ItemResource[] items) {
		if (item is DurationItemResource durationItem) {
			var durationIndicator = new ItemDurationIndicator() {
				Duration = durationItem.Duration,
				Texture = durationItem.Texture
			};

			AddChild(durationIndicator);
		}
	}
}
