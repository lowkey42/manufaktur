using Godot;
using Manufaktur;

public partial class SandwitchThrowable : Node3D
{
	[Export] public float Duration { get; set; } = 1.5f;
	[Export] public float ThrowSpeed { get; set; } = 16.0f;
	[Export] public float ThrowArcUpwardSpeed { get; set; } = 4.0f;

	[ExportGroup("Shockwave")]
	[Export] public float ShockwaveRadius { get; set; } = 5.0f;
	[Export] public float ShockwaveStrength { get; set; } = 150.0f;
	[Export] public uint ShockwaveCollisionMask { get; set; } = 2;
	
	[ExportGroup("Debug")]
	[Export] public bool ShowDebugShockwave { get; set; } = true;
	[Export(PropertyHint.Range, "0.0,1.0,0.01")] public float DebugAlpha { get; set; } = 0.2f;
	[Export] public Color DebugColor { get; set; } = new Color(0.2f, 0.8f, 1.0f, 1.0f);

	private MeshInstance3D _debugShockwaveMesh;

	private RigidBody3D _body;

	public override void _Ready()
	{
		_body = GetNode<RigidBody3D>("RigidBody3D");
		
		if (ShowDebugShockwave)
		{
			_debugShockwaveMesh      = new MeshInstance3D();
			_debugShockwaveMesh.Name = "DebugShockwaveSphere";

			// Unit sphere scaled to your radius
			_debugShockwaveMesh.Mesh  = new SphereMesh { Radius = 1.0f, Height = 2.0f };
			_debugShockwaveMesh.Scale = Vector3.One * ShockwaveRadius;

			var mat = new StandardMaterial3D
			{
				ShadingMode  = BaseMaterial3D.ShadingModeEnum.Unshaded,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				CullMode     = BaseMaterial3D.CullModeEnum.Disabled,
				AlbedoColor  = new Color(DebugColor.R, DebugColor.G, DebugColor.B, DebugAlpha)
			};

			_debugShockwaveMesh.MaterialOverride = mat;

			// Parent to body so it moves with physics
			_body.AddChild(_debugShockwaveMesh);
			_debugShockwaveMesh.Position = Vector3.Zero;
		}
	}

	public void Launch(Vector3 forwardDirection, Vector3 inheritedVelocity)
	{
		var dir = forwardDirection.Normalized();
		
		_body.LinearVelocity = dir * ThrowSpeed + Vector3.Up * ThrowArcUpwardSpeed + inheritedVelocity;
		_body.AngularVelocity = new Vector3(
			GdExtensions.RandfRange(-8f, 8f),
			GdExtensions.RandfRange(-8f, 8f),
			GdExtensions.RandfRange(-8f, 8f)
		);
		
		GetTree().CreateTimer(Duration).Timeout += Explode;
	}

	private void Explode()
	{
		GD.Print("BANG");
		ApplyShockwave();
		QueueFree();
	}

	private void ApplyShockwave()
	{	
		GD.Print("SHOCKWAVE");
		var sphere = new SphereShape3D { Radius = ShockwaveRadius };
		var query = new PhysicsShapeQueryParameters3D
		{
			Shape             = sphere,
			Transform         = new Transform3D(Basis.Identity, _body.GlobalPosition),
			CollideWithBodies = true,
			CollideWithAreas  = false,
			CollisionMask     = ShockwaveCollisionMask
		};

		var spaceState = GetWorld3D().DirectSpaceState;
		var hits = spaceState.IntersectShape(query, 32);

		foreach (Godot.Collections.Dictionary hit in hits)
		{
			var collider = hit["collider"].AsGodotObject();
			if (collider is not Player player)
			{ continue; }

			GD.Print("Player hit");
			player.Velocity += (player.GlobalPosition - _body.GlobalPosition).Normalized() * ShockwaveStrength;
		}
	}
}
