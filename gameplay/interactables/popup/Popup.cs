using Godot;
using System;

public partial class Popup : Node3D {
	[Export]
	public float TriggerRadius = 24.0f;
	
	[Export]
	public float PopupRotationDegrees = 80.0f;

	[Export]
	public float PushbackVelocity = 120f;

	[Export] private Node3D _rotationOrigin;
	[Export] private AudioStreamPlayer3D _popupSoundPlayer;
	[Export] private AudioStreamPlayer3D _pushbackSoundPlayer;
	[Export] private Area3D _pushbackTriggerArea;

	private Tween _tween;

	private bool _up = false;

	public override void _Ready() {
		_pushbackTriggerArea.BodyEntered += OnBodyEnteredPushBackArea;
	}

	private void OnBodyEnteredPushBackArea(Node3D body) {
		if (body is Player player) {
			GD.Print("Pushback entered");
			_pushbackSoundPlayer.Play();
			var direction = (player.GlobalPosition - this.GlobalPosition).Normalized();
			player.Velocity =  direction * PushbackVelocity;
		}
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

			if (!_up) {
				GD.Print("Player entered");
				this._up = true;
				this.PopUp();
			}
		}

		if (noPlayer && _up) {
			GD.Print("Player left");
			this._up = false;
			this.PopDown();
		}
	}

	public void PopDown() {
		GD.Print("Pop Down");
		
		var tween = this.CreateTween();
		tween.TweenProperty(_rotationOrigin, "rotation_degrees", new Vector3(PopupRotationDegrees, 0, 0), 0.5f)
		     .SetTrans(Tween.TransitionType.Back)
		     .SetEase(Tween.EaseType.Out);
	}

	public void PopUp() {
		GD.Print("Pop up");
		
		var tween = this.CreateTween();
		_popupSoundPlayer.Play();
		tween.TweenProperty(_rotationOrigin, "rotation_degrees", new Vector3(0, 0, 0), 0.5f)
		     .SetTrans(Tween.TransitionType.Back)
		     .SetEase(Tween.EaseType.Out);
	}
}
