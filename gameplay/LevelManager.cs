using Godot;
using Manufaktur.gameplay.levels;

public partial class LevelManager : Node
{

	public static LevelManager Instance { get; private set; }

	[Export]
	public Godot.Collections.Array<LevelData> AvailableLevels { get; set; } = new();

	public override void _EnterTree()
	{

		if (Instance == null) {
			Instance = this;
		} else {
			QueueFree();
		}
	}
}
