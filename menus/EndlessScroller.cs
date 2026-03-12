using Godot;

public partial class EndlessScroller : TextureRect 
{

	[Export] 
	public Vector2 ScrollSpeed = new Vector2(0.2f, 0.2f); 

	private Vector2 _currentOffset = Vector2.Zero;
	private ShaderMaterial _material;

	public override void _Ready()
	{

		StretchMode = StretchModeEnum.Tile; 

		_material = Material as ShaderMaterial;

	}

	public override void _Process(double delta)
	{

		_currentOffset += ScrollSpeed * (float)delta;


		_material.SetShaderParameter("offset", _currentOffset);
	}
}
