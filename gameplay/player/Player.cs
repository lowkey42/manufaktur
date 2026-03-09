using Godot;

public partial class Player : CharacterBody3D {
	/// <summary>max jump height in meter (shorter for shorter press duration</summary>
	[Export]
	private float _jumpHeight = 5;

	/// <summary>max number of jumps while airborne</summary>
	[Export]
	private float _jumpReleaseFactor = 0.1f;

	/// <summary>max horizontal speed in km/h</summary>
	[Export]
	private float _maxSpeed = 80;

	/// <summary>max vertical speed in km/h</summary>
	[Export]
	private float _maxFallFSpeed = 160;

	/// <summary>acceleration (in m/s) based on current velocity (in km/h)</summary>
	[Export]
	private Curve _acceleration;

	/// <summary>acceleration (in m/s) to stop</summary>
	[Export]
	private float _deceleration = 30;

	/// <summary>max number of jumps while airborne</summary>
	[Export]
	private int _maxAirJumps = 1;

	[Export] private float _mouseSensitivity = 0.002f;
	[Export] private float _stickSensitivity = 3.0f;
	
	[Export]
	private Camera3D _camera;
	
	private float _gravity;
	private float _jumpVelocity;
	private int _jumpsLeft;
	private bool _mouseCaptured = false;

	public override void _Ready() {
		// TODO: tweak jump
		_gravity      = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * 2;
		_jumpVelocity = Mathf.Sqrt(2 * _gravity * _jumpHeight);
	}

	public override void _Input(InputEvent e) {
		if ((e is InputEventMouseButton mb) && mb.ButtonIndex == MouseButton.Right) {
			_mouseCaptured = e.IsPressed();
			Input.SetMouseMode(_mouseCaptured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
		}

		if ((e is InputEventMouseMotion mm) && _mouseCaptured) {
			RotateY(-mm.Relative.X * _mouseSensitivity); // Horizontal look
			_camera.Rotation =  new Vector3(Mathf.Clamp(_camera.Rotation.X - mm.Relative.Y * _mouseSensitivity, Mathf.DegToRad(-80), Mathf.DegToRad(80)), _camera.Rotation.Y, _camera.Rotation.Z);
		}
	}

	public override void _ExitTree() {
		Input.SetMouseMode(Input.MouseModeEnum.Visible);
	}

	public override void _PhysicsProcess(double delta) {
		UpdateLookDirection(delta);
		UpdateMovement(delta);
	}

	private void UpdateMovement(double delta) {
		var velocity = Velocity;

		if (!IsOnFloor()) {
			velocity.Y -= _gravity * (float) delta;
			if(velocity.Y < -_maxFallFSpeed)
				velocity.Y = -_maxFallFSpeed;

		}  else {
			_jumpsLeft = _maxAirJumps;
			velocity.Y = Mathf.Max(velocity.Y, 0);
		}

		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || _jumpsLeft > 0)) {
			velocity.Y = _jumpVelocity;
			if (!IsOnFloor()) _jumpsLeft--;
		}

		if (Input.IsActionJustReleased("jump") && velocity.Y > 0) velocity.Y *= _jumpReleaseFactor;

		// TODO: use un-normalized input to scale acceleration to allow more precise movement
		var inputDir  = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();;

		var targetVelocity = direction * (_maxSpeed / 3.6f);
		targetVelocity.Y = velocity.Y;
		
		var acceleration = _acceleration.Sample(Mathf.Clamp(new Vector2(velocity.X, velocity.Z).Length() * 3.6f / _maxSpeed, 0, 1));

		// TODO: improve acceleration/deceleration logic
		if (direction != Vector3.Zero) {
			velocity.X = Mathf.MoveToward(velocity.X, targetVelocity.X, acceleration * (float) delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, targetVelocity.Z, acceleration * (float) delta);
		} else {
			velocity.X = Mathf.MoveToward(velocity.X, 0, _deceleration * (float) delta);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, _deceleration * (float) delta);
		}

		Velocity = velocity;
		
		MoveAndSlide();
	}

	private void UpdateLookDirection(double delta) {
		var lookDir = Input.GetVector("look_left", "look_right", "look_up", "look_down");
		if (lookDir.Length() > 0.1f) {
			RotateY(-lookDir.X * _stickSensitivity * (float)delta);
			_camera.Rotation =  new Vector3(Mathf.Clamp(_camera.Rotation.X - lookDir.Y * _stickSensitivity * (float)delta, Mathf.DegToRad(-80), Mathf.DegToRad(80)), _camera.Rotation.Y, _camera.Rotation.Z);
		}
	}
}
