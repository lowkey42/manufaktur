using Godot;
using System;
using System.ComponentModel.Design;

public partial class SandwichStomper : Node3D {
	[ExportGroup("Motion")]
	[Export(PropertyHint.Range, "0.05,30.0,0.05")] public float TravelTimeSeconds = 1.5f;
	[Export] public Tween.TransitionType Transition = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType Ease = Tween.EaseType.InOut;

	private Node3D _stomper;
	private Area3D _stomperContactArea;

	private bool isLethal = false;

	public override void _Ready() {
		_stomper            = GetNode<Node3D>("Stomper");
		_stomperContactArea = GetNode<Area3D>("Stomper/StomperContactArea");

		lastPosition = _stomper.Position;

		_stomperContactArea.BodyEntered += OnStomperContactAreaBodyEntered;

		var _tween = CreateTween();
		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);
		_tween.SetLoops();

		_tween.TweenProperty(_stomper, "position:y", 9.3, TravelTimeSeconds);
		_tween.TweenProperty(_stomper, "position:y", 0, TravelTimeSeconds);
	}

	private Vector3 lastPosition;

	public override void _PhysicsProcess(double delta) {
		var stomperVelocity = lastPosition - _stomper.Position;
		
		if (stomperVelocity.Y > 0) {
			isLethal = true;
		} else {
			isLethal = false;
		}
		
		lastPosition = _stomper.Position;
	}

	public void OnStomperContactAreaBodyEntered(Node3D body) {
		if (body is Player player) {
			if (isLethal && player.IsOnFloor()) {
				EventBus.Instance.EmitPlayerDied();
			}
		}
	}
}
