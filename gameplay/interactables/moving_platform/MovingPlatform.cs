using Godot;

namespace Manufaktur.gameplay.interactables.moving_platform;

public partial class MovingPlatform : Triggerable {

	[ExportGroup("Stops")]
	[Export] public Node3D EndCoordinate;

	[ExportGroup("Motion")]
	[Export(PropertyHint.Range, "0.05,30.0,0.05")] public float TravelTimeSeconds = 1.5f;
	[Export] public float StartDelay = 0.0f;
	[Export] public Tween.TransitionType Transition = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType Ease = Tween.EaseType.InOut;

	[ExportGroup("Options")]
	[Export] public bool Looping = false;
	[Export] public bool OneWayOnly = false;  
	[Export] public bool StartAtEnd = false;  
	[Export] public bool Triggerable = true;
	[Export] public bool StartEnabled = false;

	private AnimatableBody3D _platform;
	private Tween _tween;
	private Vector3 _lastPosition;
	private Vector3 _spawnPosition;

	private enum State { AtStop1, AtStop2, Moving }
	private State _state;

	public override void _Ready() {
		base._Ready();

		_platform = GetNodeOrNull<AnimatableBody3D>("Body");
		if (_platform == null) {
			GD.PushError($"{Name}: Body not found or not an AnimatableBody3D.");
			return;
		}

		_spawnPosition = _platform.GlobalPosition;

		_platform.GlobalPosition = StartAtEnd ? EndCoordinate.GlobalPosition : _platform.GlobalPosition;
		_lastPosition            = _platform.GlobalPosition;
		_state                   = StartAtEnd ? State.AtStop2 : State.AtStop1;

		if (Looping && StartEnabled) {
			if (StartDelay > 0) {
				GetTree().CreateTimer(StartDelay).Timeout += StartLooping;
			} else {
				StartLooping();
			}
		}
	}

	public override void Trigger() {
		if (!Triggerable) {
			return;
		}

		if (Looping) {
			ToggleLooping();
			return;
		}

		ActivateOnce();
	}

	private void ActivateOnce() {
		if (_platform == null) {
			return;
		}
		if (_state == State.Moving) {
			return;
		}

		Vector3 target;

		if (OneWayOnly) {
			if (_state == State.AtStop2) {
				return;
			}
			target = EndCoordinate.GlobalPosition;
		} else {
			target = (_state == State.AtStop2) ? _spawnPosition : EndCoordinate.GlobalPosition;
		}

		if (StartDelay > 0) {
			GetTree().CreateTimer(StartDelay).Timeout += () => MoveTo(target);
		} else {
			MoveTo(target);
		}
	}

	private void MoveTo(Vector3 targetLocalPos) {
		_tween?.Kill();

		_tween = CreateTween();
		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);

		_state = State.Moving;

		_tween.TweenProperty(_platform, "global_position", targetLocalPos, TravelTimeSeconds);

		_tween.Finished += () => {
			if (targetLocalPos.IsEqualApprox(_spawnPosition)) {
				_state = State.AtStop1;
			} else if (targetLocalPos.IsEqualApprox(EndCoordinate.GlobalPosition)) {
				_state = State.AtStop2;
			}

			_platform.ConstantLinearVelocity = Vector3.Zero;
		};
	}

	private void StartLooping() {
		if (_platform == null) {
			return;
		}

		_tween?.Kill();

		_tween = CreateTween();
		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);
		_tween.SetLoops();

		Vector3 firstTarget = (_state == State.AtStop2) ? _spawnPosition : EndCoordinate.GlobalPosition;

		_state = State.Moving;

		_tween.TweenProperty(_platform, "global_position", firstTarget, TravelTimeSeconds);
		_tween.TweenProperty(_platform, "global_position", firstTarget.IsEqualApprox(EndCoordinate.GlobalPosition) ? _spawnPosition : EndCoordinate.GlobalPosition, TravelTimeSeconds);
	}

	private void StopLooping() {
		_tween?.Kill();
		_tween = null;

		if (_platform != null) {
			_platform.ConstantLinearVelocity = Vector3.Zero;
		}

		if (_platform.GlobalPosition.IsEqualApprox(_spawnPosition)) {
			_state = State.AtStop1;
		} else if (_platform.GlobalPosition.IsEqualApprox(EndCoordinate.GlobalPosition)) {
			_state = State.AtStop2;
		} else {
			_state = State.Moving;
		}
	}

	private void ToggleLooping() {
		if (_tween != null && _tween.IsRunning()) {
			StopLooping();
		} else {
			StartLooping();
		}
	}
}
