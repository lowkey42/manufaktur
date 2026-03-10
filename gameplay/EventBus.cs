using Godot;

public partial class EventBus : Node {
	public static EventBus Instance { get; private set; }

	[Signal]
	public delegate void LevelStartedEventHandler();

	[Signal]
	public delegate void LevelCompletedEventHandler(float time);

	[Signal]
	public delegate void FinishReachedEventHandler();

	[Signal]
	public delegate void PlayerDiedEventHandler();

	[Signal]
	public delegate void InventoryItemCollectedEventHandler(ItemResource item, ItemResource[] items);

	[Signal]
	public delegate void InventoryItemUsedEventHandler(ItemResource item, ItemResource[] items);

	public override void _Ready() {
		Instance = this;
	}

	public void EmitLevelStarted() {
		EmitSignalLevelStarted();
	}
	public void EmitLevelCompleted(float time) {
		EmitSignalLevelCompleted(time);
	}
	public void EmitFinishReached() {
		EmitSignalFinishReached();
	}
	public void EmitPlayerDied() {
		EmitSignalPlayerDied();
	}
	public void EmitInventoryItemCollected(ItemResource item, ItemResource[] items) {
		EmitSignalInventoryItemCollected(item, items);
	}
	public void EmitInventoryItemUsed(ItemResource item, ItemResource[] items) {
		EmitSignalInventoryItemUsed(item, items);
	}

}
