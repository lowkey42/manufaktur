using Godot;
using System;

public partial class HexAnim : TextureRect
{
	[Export]
	public Texture2D TargetTexture { get; set; }

	[Export]
	public float Duration = 2.0f; 

	private ShaderMaterial mat;
	private float time = 0f;
    

	private int lastPhase = 0;
	private int currentDir = 0;

	public override void _Ready()
	{
		if (Material is ShaderMaterial shaderMat)
		{
			mat      = shaderMat.Duplicate() as ShaderMaterial;
			Material = mat; 
          
			if (TargetTexture != null)
			{
				mat.SetShaderParameter("to_tex", TargetTexture);
				mat.SetShaderParameter("dir", currentDir);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (mat != null)
		{
			time += (float)delta;
          
			
			int currentPhase = (int)(time / Duration);
          
			
			if (currentPhase != lastPhase)
			{
				lastPhase = currentPhase;
				
				currentDir = (currentDir + 1) % 4;
				mat.SetShaderParameter("dir", currentDir);
			}

			float pingPongTime = Mathf.PingPong(time, Duration);
			float progress     = pingPongTime / Duration;
            
			mat.SetShaderParameter("progress", progress);
		}
	}
}
