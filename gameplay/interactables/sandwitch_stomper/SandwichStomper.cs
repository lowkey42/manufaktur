using Godot;
using System;

public partial class SandwichStomper : Node3D
{
	[Export] private Node3D _stomper;
	
	[ExportGroup("Motion")]
	[Export(PropertyHint.Range, "0.05,30.0,0.05")] public float TravelTimeSeconds = 1.5f;
	[Export] public float StartDelay = 0.0f;
	[Export] public Tween.TransitionType Transition = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType Ease = Tween.EaseType.InOut;

	public override void _Ready() {
		var _tween = CreateTween();
		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Transition);
		_tween.SetEase(Ease);
		_tween.SetLoops();
		
		_tween.TweenProperty(_stomper, "position:y", 9.3, TravelTimeSeconds);
		_tween.TweenProperty(_stomper, "position:y", 0, TravelTimeSeconds);
	}
}
