using System;
using Godot;

public partial class Player : CharacterBody3D {
	[Signal]
	public delegate void PlayerTeleportedEventHandler(Vector3 origin, Vector3 destination);

	/// <summary>max jump height in meter (shorter for shorter press duration</summary>
	[Export]
	private float _jumpHeight = 5;

	/// <summary>max number of jumps while airborne</summary>
	[Export]
	private float _jumpReleaseFactor = 0.1f;

	/// <summary>scales world gravity to make character fall faster</summary>
	[Export]
	private float _gravityFactor = 6.0f;

	/// <summary>max horizontal speed in km/h</summary>
	[Export]
	private float _maxSpeed = 120;

	/// <summary>max vertical speed in km/h</summary>
	[Export]
	private float _maxFallFSpeed = 160;

	/// <summary>acceleration (in m/s) based on current velocity (in km/h)</summary>
	[Export]
	private Curve _acceleration;

	/// <summary>modulates acceleration while airborne</summary>
	[Export]
	private float _airAccelerationFactor = 0.75f;

	/// <summary>acceleration (in m/s) to stop</summary>
	[Export]
	private float _deceleration = 40;

	/// <summary>controls how fast we can change direction</summary>
	[Export]
	private float _friction = 15;

	/// <summary>max number of jumps while airborne</summary>
	[Export]
	private int _maxAirJumps = 1;

	[Export] private Curve _targetFov;

	[Export] private float _fovSpringStiffness = 200.0f;
	[Export] private float _fovSpringDamping = 10.0f;

	[Export] private float _mouseSensitivity = 0.002f;
	[Export] private float _stickSensitivity = 3.0f;

	[Export] private Camera3D _camera;

	[Export] public Inventory Inventory { get; private set; }

	public float VelocityPercent => Mathf.Min(1.0f, (Velocity * new Vector3(1, 0, 1)).Length() / _maxSpeed);
	public Vector3 RelativeVelocityPercent => Vector3.One.Min((_camera.GlobalTransform.Basis.Inverse() * Velocity) / new Vector3(_maxSpeed, _maxFallFSpeed, _maxSpeed));
	
	private bool _firstFrame = true;
	private float _gravity;
	private float _jumpVelocity;
	private int _jumpsLeft;
	private bool _mouseCaptured = false;
	private float _fovVelocity = 0.0f;

	public void TeleportTo(Vector3 position) {
		var origin = position;
		GlobalPosition = position;
		ResetPhysicsInterpolation();
		EmitSignalPlayerTeleported(origin, position);
	}

	public override void _Ready() {
		_gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle() * _gravityFactor;
		_jumpVelocity = Mathf.Sqrt(2 * _gravity * _jumpHeight);
		_camera.Fov = _targetFov.Sample(0);
	}

	public override void _Input(InputEvent e) {
		if ((e is InputEventMouseButton mb) && mb.ButtonIndex == MouseButton.Right) {
			_mouseCaptured = e.IsPressed();
			Input.SetMouseMode(_mouseCaptured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
		}

		if ((e is InputEventMouseMotion mm) && _mouseCaptured) {
			RotateY(-mm.Relative.X * _mouseSensitivity); // Horizontal look
			_camera.Rotation = new Vector3(Mathf.Clamp(_camera.Rotation.X - mm.Relative.Y * _mouseSensitivity, Mathf.DegToRad(-80), Mathf.DegToRad(80)),
										   _camera.Rotation.Y, _camera.Rotation.Z);
		}

		if (e.IsActionPressed("use_item")) {
			Inventory.UseItem(this);
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
			if (velocity.Y < -_maxFallFSpeed)
				velocity.Y = -_maxFallFSpeed;
		} else {
			_jumpsLeft = _maxAirJumps;
			velocity.Y = Mathf.Max(velocity.Y, 0);
		}

		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || _jumpsLeft > 0)) {
			velocity.Y = _jumpVelocity;
			if (!IsOnFloor())
				_jumpsLeft--;
		}

		if (Input.IsActionJustReleased("jump") && velocity.Y > 0)
			velocity.Y *= _jumpReleaseFactor;

		var inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		var oldVelocity = ToHorizontal(velocity);

		if (inputDir.IsZeroApprox()) {
			if (Mathf.Abs(velocity.X) < 0.01f || Mathf.Abs(velocity.Z) < 0.01f) {
				velocity.X = 0;
				velocity.Z = 0;
			} else {
				var revAcceleration = -_deceleration * ((float) delta);

				velocity += (-_friction * oldVelocity + revAcceleration * oldVelocity.Normalized()) * ((float) delta);

				// Don't reverse direction
				if (velocity.Dot(oldVelocity) <= 0.0f) {
					velocity.X = 0;
					velocity.Z = 0;
				}
			}
		} else {
			var targetSpeed = _maxSpeed / 3.6f;
			var acceleration = _acceleration.Sample(Mathf.Clamp(oldVelocity.Length() * 3.6f / _maxSpeed, 0, 1));

			if (!IsOnFloor())
				acceleration *= _airAccelerationFactor;

			// modulate by analog input
			targetSpeed *= Math.Min(1.0f, inputDir.Length());
			acceleration *= Math.Min(1.0f, inputDir.Length());

			// apply friction
			velocity -= (oldVelocity - direction * oldVelocity.Length()) * Mathf.Min(_friction * ((float) delta), 1.0f);

			var maxSpeed = Mathf.Max(oldVelocity.Length(), targetSpeed);
			velocity += direction * acceleration * ((float) delta);

			var horizontalVelocity = ToHorizontal(velocity);
			if (horizontalVelocity.Length() > maxSpeed) {
				horizontalVelocity = horizontalVelocity.Normalized() * maxSpeed;
				velocity.X = horizontalVelocity.X;
				velocity.Z = horizontalVelocity.Z;
			}
		}

		Velocity = velocity;

		MoveAndSlide();
	}

	public override void _Process(double delta) {
		var targetFov = _targetFov.Sample(ToHorizontal(Velocity).Length());

		if (_firstFrame) {
			_camera.Fov = targetFov;
			_firstFrame = false;
			return;
		}

		var displacement = _camera.Fov - targetFov;
		var springForce = -_fovSpringStiffness * displacement;
		var dampingForce = -_fovSpringDamping * _fovVelocity;

		_fovVelocity += (springForce + dampingForce) * (float) delta;
		_camera.Fov = Math.Clamp(_camera.Fov + _fovVelocity * (float) delta, 10.0f, 130.0f);
	}

	private static Vector3 ToHorizontal(Vector3 v) {
		return new Vector3(v.X, 0, v.Z);
	}

	private void UpdateLookDirection(double delta) {
		var lookDir = Input.GetVector("look_left", "look_right", "look_up", "look_down");
		if (lookDir.Length() > 0.1f) {
			RotateY(-lookDir.X * _stickSensitivity * (float) delta);
			_camera.Rotation =
				new Vector3(Mathf.Clamp(_camera.Rotation.X - lookDir.Y * _stickSensitivity * (float) delta, Mathf.DegToRad(-80), Mathf.DegToRad(80)),
							_camera.Rotation.Y, _camera.Rotation.Z);
		}
	}

	public Vector3 GetFacingDirection() {
		return -Transform.Basis.Z;
	}

	public void SetJumpHeight(float height) {
		_jumpHeight = height;
		_jumpVelocity = Mathf.Sqrt(2 * _gravity * _jumpHeight);
	}

	public float GetJumpHeight() {
		return _jumpHeight;
	}
}
