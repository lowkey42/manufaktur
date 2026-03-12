using Godot;

public partial class InventoryUi : Control {
	[Export] public float CardWidth { get; set; } = 150f;
	[Export] public float CardHeight { get; set; } = 225f;
	[Export] public float CardOverlap { get; set; } = 25f;
	[Export] public float CardRotationStep { get; set; } = 5f;
	[Export] public float CardVerticalOffset { get; set; } = 10f;

	[Export] private AudioStreamPlayer _itemCollectedPlayer;
	[Export] private AudioStreamPlayer _itemUsedPlayer;

	public override void _Ready() {
		EventBus.Instance.InventoryItemCollected += OnInventoryItemChanged;
		EventBus.Instance.InventoryItemCollected += OnInventoryItemCollected;
		EventBus.Instance.InventoryItemUsed      += OnInventoryItemChanged;
		EventBus.Instance.InventoryItemUsed	  += OnInventoryItemUsed;
	}
	
	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
		{
			EventBus.Instance.InventoryItemCollected -= OnInventoryItemChanged;
			EventBus.Instance.InventoryItemCollected -= OnInventoryItemCollected;
			EventBus.Instance.InventoryItemUsed      -= OnInventoryItemChanged;
			EventBus.Instance.InventoryItemUsed      -= OnInventoryItemUsed;
		}
	}


	private void OnInventoryItemCollected(ItemResource item, ItemResource[] items) {
		_itemCollectedPlayer.Play();
	}

	private void OnInventoryItemUsed(ItemResource item, ItemResource[] items) {
		_itemUsedPlayer.Stream = item.UseSounds;
		_itemUsedPlayer.Play();
	}

	private void OnInventoryItemChanged(ItemResource item, ItemResource[] items) {
		foreach (var child in this.GetChildren()) {
			if (child is TextureRect textureRect) {
				child.QueueFree();
			}
		}

		if (items == null) { return; }

		// Calculate starting position to center the fan
		var totalWidth = (items.Length - 1) * CardOverlap + CardWidth;
		var startX     = -totalWidth / 2;

		for (var i = 0; i < items.Length; i++) {
			var itemTexture = new TextureRect();
			itemTexture.Texture           = items[i].HandTexture;
			itemTexture.TextureFilter     = TextureFilterEnum.LinearWithMipmapsAnisotropic;
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
