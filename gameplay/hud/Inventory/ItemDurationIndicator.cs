using Godot;
using System;

public partial class ItemDurationIndicator : Control
{
	public Texture2D Texture { get; set; }

	public float Duration { get; set; }

	[Export]
	private NodePath _progressBarPath;

	public override void _Ready() {
		base._Ready();

		var progressBar = GetNode<TextureProgressBar>(_progressBarPath);

		if (progressBar != null) {
			progressBar.TextureProgress = Texture;
		}

		var tween = progressBar.CreateTween();
		tween.TweenProperty(progressBar, "value", 0, Duration);

		this.AddChild(progressBar);
		this.MoveChild(progressBar, 0);

		var timer = GetTree().CreateTimer(Duration);
		timer.Timeout += progressBar.QueueFree;
	}
}
