using Godot;
using System;

public partial class Level : Node {
	[Export] public float TimeMinimum { get; private set; }
	[Export] public float TimeBronze { get; private set; }
	[Export] public float TimeSilver { get; private set; }
	[Export] public float TimeGold { get; private set; }
	[Export] public float TimePlatinum { get; private set; }

	[Export] public Json MinimumTimeGhost { get; private set; }

	[Export(PropertyHint.File, "*.tscn")] public string NextLevelScene { get; private set; }
}
