using Godot;
using System;

public partial class ItemDurationIndicator : Control {
	public Texture2D Texture { get; set; }

	public float Duration { get; set; }
	
	private TextureProgressBar _progressBar;

	public override void _Ready() {
		base._Ready();
		
		_progressBar = GetNode<TextureProgressBar>("PanelContainer/TextureProgressBar");

		// Scale texture to the control size
		_progressBar.TextureProgress = Texture;

		_progressBar.Scale = new Vector2(0.5f, 0.5f);

		var tween = this.CreateTween();
		tween.TweenProperty(_progressBar, "value", 0, Duration);

		this.AddChild(_progressBar);

		var timer = GetTree().CreateTimer(Duration);
		timer.Timeout += this.QueueFree;
	}
}
