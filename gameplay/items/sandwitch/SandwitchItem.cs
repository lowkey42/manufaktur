using Godot;

public partial class SandwitchItem : DurationItemResource
{
	[Export] public PackedScene ThrowableScene { get; set; } =
		GD.Load<PackedScene>("res://gameplay/items/sandwitch/SandwitchThrowable.tscn");

	[Export] public float SpawnForwardOffset { get; set; } = 0.8f;
	[Export] public float SpawnUpOffset { get; set; } = 1.2f;

	[ExportGroup("Throw")]
	[Export] public float ThrowSpeed { get; set; } = 16.0f;
	[Export] public float ThrowArcUpwardSpeed { get; set; } = 4.0f;

	public override void Use(Player player)
	{
		if (ThrowableScene?.Instantiate() is not SandwitchThrowable throwable)
			return;

		var spawnPosition = player.GlobalPosition
		                  + player.GetFacingDirection().Normalized() * SpawnForwardOffset
		                  + Vector3.Up * SpawnUpOffset;

		var parent = player.GetTree().CurrentScene ?? player.GetTree().Root;
		parent.AddChild(throwable);
		throwable.GlobalPosition = spawnPosition;
		
		GD.Print($"Duration: {Duration}");

		throwable.Duration            = Duration;
		throwable.ThrowSpeed          = ThrowSpeed;
		throwable.ThrowArcUpwardSpeed = ThrowArcUpwardSpeed;
		throwable.Launch(player.GetFacingDirection(), player.Velocity);
	}
}
