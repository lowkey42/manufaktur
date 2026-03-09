using Godot;
using Manufaktur.gameplay.player;

public partial class GhostTracker : Node
{
	
	[Export] private Player _player;
	
	[Export] private float _minSampleTimespan = 0.1f;
	
	[Export] private float _minSampleDistance = 0.5f;

	private float _currentTime;

	public PlayerPath Path { get; } = new();

	public override void _Ready() {
		_player.PlayerTeleported += OnTeleported;
	}

	public void SetCurrentTime(float time) {
		_currentTime = time;
	}

	private void OnTeleported(Vector3 origin, Vector3 destination) {
		Path.AddTeleportSample(_currentTime, _player, origin, destination);
	}

	public override void _Process(double delta) {
		Path.AddSample(_currentTime, _player, _minSampleDistance, _minSampleTimespan);
	}
}
