using Manufaktur;

public partial class GlitchItem : DurationItemResource
{
	public override void Use(Player player) {
		player.CollisionMask -= (int)GameHelpers.CollisionLayers.Glitched;
		
		player.GetTree().CreateTimer(Duration).Timeout += () => {
			player.CollisionMask += (int)GameHelpers.CollisionLayers.Glitched;
		};
	}
}
