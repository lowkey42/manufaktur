using Godot;

public partial class InventoryUi : Control {
	[Export] public float CardWidth { get; set; } = 100f;
	[Export] public float CardHeight { get; set; } = 150f;
	[Export] public float CardOverlap { get; set; } = 50f;
	[Export] public float CardRotationStep { get; set; } = 5f;
	[Export] public float CardVerticalOffset { get; set; } = 10f;

	public override void _Ready() {
		EventBus.Instance.InventoryItemCollected += OnInventoryItemChanged;
		EventBus.Instance.InventoryItemUsed += OnInventoryItemChanged;
	}

	private void OnInventoryItemChanged(ItemResource item, ItemResource[] items) {
		foreach (var child in this.GetChildren()) {
			child.QueueFree();
		}

		if (items == null)
		{ return; }
		
		// Calculate starting position to center the fan
		var totalWidth = (items.Length - 1) * CardOverlap + CardWidth;
		var startX     = -totalWidth / 2;
			
		for (var i = 0; i < items.Length; i++) {
			var itemTexture = new TextureRect();
			itemTexture.Texture           = items[i].Texture;
			itemTexture.CustomMinimumSize = new Vector2(CardWidth, CardHeight);
			itemTexture.ExpandMode        = TextureRect.ExpandModeEnum.FitWidth;
			itemTexture.StretchMode       = TextureRect.StretchModeEnum.KeepAspectCentered;
			itemTexture.PivotOffset       = new Vector2(CardWidth / 2, CardHeight);

			// Position cards with overlap (first card on top)
			var xOffset = startX + i * CardOverlap;
			var yOffset = -CardHeight + Mathf.Abs(i - items.Length / 2f) * CardVerticalOffset;
			itemTexture.Position = new Vector2(xOffset, yOffset);

			// Rotate cards for fan effect
			var rotation = (i - items.Length / 2f) * CardRotationStep;
			itemTexture.RotationDegrees = rotation;

			// Z-index: first item (index 0) should be on top
			itemTexture.ZIndex = items.Length - i;

			this.AddChild(itemTexture);
		}
	}
}
