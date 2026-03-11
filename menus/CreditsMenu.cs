using Godot;
using System;

public partial class CreditsMenu : Control
{
	[Signal]
	public delegate void ClosedEventHandler();
	private ScrollContainer _scrollPanel;
	private Tween _activeTween;
	
	private void _on_close_button_pressed() {
		EmitSignal(SignalName.Closed);
	}

	public override void _Ready() {
		_scrollPanel = GetNode<ScrollContainer>("%ScrollContainer");


		_scrollPanel.GetVScrollBar().Modulate = new Color(0, 0, 0, 0);
		StartAutoScroll();
	}

	private void StartAutoScroll()
	{

		if (_activeTween != null) _activeTween.Kill();
        
		_activeTween = CreateTween();
        

		float maxScroll = (float)_scrollPanel.GetVScrollBar().MaxValue - _scrollPanel.Size.Y;



            

		_activeTween.TweenProperty(_scrollPanel, "scroll_vertical", 0, 3.0f)
		            .SetTrans(Tween.TransitionType.Quad);
		
		_activeTween.TweenProperty(_scrollPanel, "scroll_vertical", maxScroll, 5.0f)
		            .SetTrans(Tween.TransitionType.Quad);

		_activeTween.SetLoops();
	}
}
