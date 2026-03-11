using System;

namespace Manufaktur;

public static class GameHelpers {
	[Flags]
	public enum CollisionLayers {
		Environment = 1 << 0,
		Player = 1 << 1,
		Interactables = 1 << 2,
		Turret = 1 << 3,
		Glitched = 1 << 4,
	}	
}
