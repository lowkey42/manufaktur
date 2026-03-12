using Godot;
using System;

public partial class AnimatedButton : Button {
	[Export] public SoundPool HoverSounds;	
	

	private Tween tween;
	public override void _Ready() {
		PivotOffset      =  Size * new Vector2(0.5f, 1.0f);
		ButtonDown       += _onButton_Down;
		ButtonUp         += _onButton_Up;
		MouseExited      += _onMouse_exited;
		MouseEntered     += _onMouse_entered;
		CallDeferred(nameof(SetPivot));

	}
	
	private void    SetPivot()
	{
		PivotOffset = Size * new Vector2(0.5f, 1.0f);
	}

	private void _onMouse_entered() {
		PivotOffset = Size * new Vector2(0.0f, 0.5f);
		if (tween != null) {
			tween.Kill();
		}
		tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.25f);

		Radio.Instance.PlayRandomSound(HoverSounds.pool, Radio.Instance.UiSfxBus);
	}

	private void _onMouse_exited() {
		PivotOffset = Size * new Vector2(0.0f, 0.5f);
		if (tween != null) {
			tween.Kill();
		}
		tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(this, "scale", Vector2.One, 0.25f);
	}

	private void _onButton_Down() {
		SetPivot();
		if (tween != null) {
			tween.Kill();
		}
		Scale = new Vector2(0.9f, 0.9f);
	}

	private void _onButton_Up() {
		SetPivot();
		tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.TweenProperty(this, "scale", Vector2.One, 0.25f);
	}
	
}
