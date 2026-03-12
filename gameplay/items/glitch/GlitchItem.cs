using Godot;
using Manufaktur;

public partial class GlitchItem : DurationItemResource
{
	private SceneTreeTimer _timer;

	public override void Use(Player player) {
		player.CollisionMask -= (int)GameHelpers.CollisionLayers.Glitched;

		_timer = player.GetTree().CreateTimer(Duration);
		
		_timer.Timeout += () => {
			//check for if player has been disposed
			player.CollisionMask += (int)GameHelpers.CollisionLayers.Glitched;
		};
	}
}
