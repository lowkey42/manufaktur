using Godot;

namespace Manufaktur;

public static class GdExtensions {
	public static float RandfRange(float lower, float upper) {
		if (lower > upper) {
			(lower, upper) = (upper, lower);
		}

		return lower + (upper - lower) * GD.Randf();
	}
}
