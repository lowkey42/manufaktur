using Godot;
using System;

public partial class AnimatedButton : Button {
	private Tween tween;
	public override void _Ready() {
		PivotOffset =  Size * new Vector2(0.5f, 1.0f);
		ButtonDown  += _onButton_Down;
		ButtonUp    += _onButton_Up;
		CallDeferred(nameof(SetPivot));

	}
	
	private void    SetPivot()
	{
		PivotOffset = Size * new Vector2(0.5f, 1.0f);
	}

	private void _onButton_Down() {
		if (tween != null)
			tween.Kill();
		Scale = new Vector2(0.9f, 0.9f);
	}

	private void _onButton_Up() {
		tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(this, "scale", Vector2.One, 0.25f);
	}
	
}
