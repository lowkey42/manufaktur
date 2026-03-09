using Godot;
using System;

public partial class GhostEffect : Node
{

	[Export]
	public ShaderMaterial ghostMaterial;

	public override void _Ready()
	{

		GetViewport().SizeChanged += OnSizeChanged;
		OnSizeChanged();
	}

	private void OnSizeChanged() {
		var screenSize = GetViewport().GetVisibleRect().Size;
		var aspect = screenSize.X / screenSize.Y;
		ghostMaterial.SetShaderParameter("AspectRatio", aspect);
	}
}
