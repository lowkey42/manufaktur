using Godot;
using System;

public partial class Popup : Node3D {
	[Export]
	public float TriggerRadius = 6.0f;

	private Node3D _origin;
	private Tween _tween;

	private bool _up = false;

	public override void _Ready() {
		_tween = this.CreateTween();

		_origin                 = GetNode<Node3D>("Origin");
		_origin.RotationDegrees = new Vector3(-90, 0, 0);
	}

	public override void _Process(double delta) {
		var sphere = new SphereShape3D {Radius = TriggerRadius};
		var query = new PhysicsShapeQueryParameters3D {
			Shape             = sphere,
			Transform         = new Transform3D(Basis.Identity, this.GlobalPosition),
			CollideWithBodies = true,
			CollideWithAreas  = false,
			CollisionMask     = 2
		};

		var spaceState = GetWorld3D().DirectSpaceState;
		var hits       = spaceState.IntersectShape(query, 32);

		bool noPlayer = true;

		foreach (Godot.Collections.Dictionary hit in hits) {
			var collider = hit["collider"].AsGodotObject();
			if (collider is not Player player) { continue; }

			noPlayer = false;
			this._up = true;
			this.PopUp();
		}

		if (noPlayer && _up) {
			this._up = false;
			this.PopDown();
		}
	}

	public void PopDown() {
		var tween = CreateTween();
		tween.TweenProperty(_origin, "rotation_degrees", new Vector3(-90, 0, 0), 0.5f)
		     .SetTrans(Tween.TransitionType.Back)
		     .SetEase(Tween.EaseType.Out);
	}

	public void PopUp() {
		var tween = CreateTween();
		tween.TweenProperty(_origin, "rotation_degrees", new Vector3(0, 0, 0), 0.5f)
		     .SetTrans(Tween.TransitionType.Back)
		     .SetEase(Tween.EaseType.Out);
	}
}
