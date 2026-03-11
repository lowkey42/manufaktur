using Godot;

namespace Manufaktur.gameplay.interactables.moving_platform;

public partial class MovingPlatform : Triggerable {

	[ExportGroup("Stops (local space)")]
	[Export] public Vector3 Stop1 = new Vector3(0, 0, 0);
	[Export] public Vector3 Stop2 = new Vector3(0, 5, 0);

	[ExportGroup("Motion")]
	[Export(PropertyHint.Range, "0.05,30.0,0.05")] public float TravelTimeSeconds = 1.5f;
	[Export] public Tween.TransitionType Transition = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType Ease = Tween.EaseType.InOut;

	[ExportGroup("Options")]
	[Export] public bool Triggerable = true;
	[Export] public bool Looping = false;
	[Export] public bool StartEnabled = false;
	[Export] public bool OneWayOnly = false;  
	[Export] public bool StartAtEnd = false;

	private AnimatableBody3D _platform;
	private Tween _tween;
	private Vector3 _lastPosition;

	private enum State { AtStop1, AtStop2, Moving }
	private State _state;

	public override void _Ready() {
		base._Ready();

		_platform = GetNodeOrNull<AnimatableBody3D>("Body");
		if (_platform == null) {
			GD.PushError($"{Name}: Body not found or not an AnimatableBody3D.");
			return;
		}

		_platform.Position = StartAtEnd ? Stop2 : Stop1;
		_lastPosition = _platform.GlobalPosition;
		_state = StartAtEnd ? State.AtStop2 : State.AtStop1;

		if (Looping && StartEnabled) {
			StartLooping();
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
			target = Stop2;
		} else {
			target = (_state == State.AtStop2) ? Stop1 : Stop2;
		}

		MoveTo(target);
	}

	private void MoveTo(Vector3 targetLocalPos) {
		_tween?.Kill();

		_tween = CreateTween();
		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);

		_state = State.Moving;

		_tween.TweenProperty(_platform, "position", targetLocalPos, TravelTimeSeconds);

		_tween.Finished += () => {
			if (targetLocalPos.IsEqualApprox(Stop1)) {
				_state = State.AtStop1;
			} else if (targetLocalPos.IsEqualApprox(Stop2)) {
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
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);
		_tween.SetLoops();

		Vector3 firstTarget = (_state == State.AtStop2) ? Stop1 : Stop2;

		_state = State.Moving;

		_tween.TweenProperty(_platform, "position", firstTarget, TravelTimeSeconds);
		_tween.TweenProperty(_platform, "position", firstTarget.IsEqualApprox(Stop2) ? Stop1 : Stop2, TravelTimeSeconds);
	}

	private void StopLooping() {
		_tween?.Kill();
		_tween = null;

		if (_platform != null) {
			_platform.ConstantLinearVelocity = Vector3.Zero;
		}

		if (_platform.Position.IsEqualApprox(Stop1)) {
			_state = State.AtStop1;
		} else if (_platform.Position.IsEqualApprox(Stop2)) {
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
