using Godot;

public partial class ItemDurationContainer : Control {

	public override void _Ready() {
		EventBus.Instance.InventoryItemUsed += OnInventoryItemUsed;
	}

	public void OnInventoryItemUsed(ItemResource item, ItemResource[] items) {
		if (item is not DurationItemResource durationItem) return;
		
		var progressBar = new TextureProgressBar();
		
		progressBar.TextureProgress  = item.Texture;
		progressBar.NinePatchStretch = true;

		progressBar.Value = 100;
		progressBar.FillMode = (int)TextureProgressBar.FillModeEnum.CounterClockwise;
		
		progressBar.CustomMinimumSize = new Vector2(64, 96);
		progressBar.Size = new Vector2(64, 96);
		progressBar.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		progressBar.SizeFlagsVertical = SizeFlags.ExpandFill;

		var tween = progressBar.CreateTween();
		tween.TweenProperty(progressBar, "value", 0, durationItem.Duration);

		this.AddChild(progressBar);
		this.MoveChild(progressBar, 0);

		var timer = GetTree().CreateTimer(durationItem.Duration);
		timer.Timeout += progressBar.QueueFree;
	}
}
