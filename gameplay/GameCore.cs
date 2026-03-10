using Godot;

public partial class GameCore : Node {

	private float _startTime;
	private float _finishTime;
	 
	// TODO: display in HUD (together with time to beat (if known)
	public float TimeSeconds => (_finishTime==0 ? (Time.GetTicksMsec() - _startTime) : (_finishTime - _startTime)) / 1000.0f;
	
	public override void _Ready() {
		var bus = EventBus.Instance;
		
		bus.FinishReached += OnFinished;
		
		// TODO: show a count down (3 to GO) before starting
		bus.EmitLevelStarted();
		_startTime = Time.GetTicksMsec();
	}
	
	private void OnFinished() {
		_finishTime = Time.GetTicksMsec();
		// TODO: fade out and show highscore list
	}
}
