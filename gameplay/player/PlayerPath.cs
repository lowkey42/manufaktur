using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Manufaktur.gameplay.player;

public class PlayerPath {
	private struct Sample {
		public float Time;
		public Vector3 Position;
		public Quaternion Rotation;

		public bool Teleported;
		// TODO: maybe animation, action or other parameters

		public Dictionary ToDictionary() => new() {{"time", Time}, {"pos", GD.VarToStr(Position)}, {"rot", GD.VarToStr(Rotation)}, {"tele", Teleported}};

		public static Sample FromDictionary(Dictionary dict) => new() {
			Time       = (float) dict["time"].AsDouble(),
			Position   = (Vector3) GD.StrToVar((string) dict["pos"]),
			Rotation   = (Quaternion) GD.StrToVar((string) dict["rot"]),
			Teleported = dict["tele"].AsBool()
		};

		public override string ToString() {
			return $"{nameof(Time)}: {Time}, {nameof(Position)}: {Position}, {nameof(Rotation)}: {Rotation}, {nameof(Teleported)}: {Teleported}";
		}
	}

	public float TotalTime => _samples.Count == 0 ? 0 : _samples[^1].Time;
	public string PlayerName { get; private set; }

	private readonly List<Sample> _samples = [];

	public PlayerPath() { }

	public PlayerPath(Json ghostFile) {
		Parse(ghostFile);
	}

	public PlayerPath(string path) {
		var json        = new Json();
		var parseResult = json.Parse(FileAccess.Open(path, FileAccess.ModeFlags.Read).GetAsText());
		if (parseResult != Error.Ok) {
			GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {path} at line {json.GetErrorLine()}");
			return;
		}

		Parse(json);
	}

	private void Parse(Json json) {
		var jsonData = json.Data.AsGodotDictionary();

		PlayerName = jsonData["player_name"].AsString();
		foreach (var sample in jsonData["samples"].AsGodotArray<Dictionary>()) {
			_samples.Add(Sample.FromDictionary(sample));
		}
	}

	public void Save(string path) {
		var sampleList = new Array<Dictionary>();
		foreach (var sample in _samples) {
			sampleList.Add(sample.ToDictionary());
		}

		var data = new Dictionary {{"player_name", PlayerName}, {"total_time", TotalTime}, {"samples", sampleList}};

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(data, "\t"));
	}

	public void AddSample(float time, Player player, float minSampleDistance, float minSampleTimespan) {
		if (_samples.Count > 0 && (_samples[^1].Position - player.GlobalPosition).LengthSquared() < minSampleDistance * minSampleDistance) {
			return;
		}
		if (_samples.Count > 0 && (time - _samples[^1].Time) < minSampleTimespan) {
			return;
		}

		var transform = player.GlobalTransform;
		_samples.Add(new Sample() {Time = time, Position = transform.Origin, Rotation = transform.Basis.GetRotationQuaternion(), Teleported = false});
	}

	public void AddTeleportSample(float time, Player player, Vector3 origin, Vector3 destination) {
		var transform = player.GlobalTransform;
		_samples.Add(new Sample() {Time = time, Position = origin, Rotation      = transform.Basis.GetRotationQuaternion(), Teleported = false});
		_samples.Add(new Sample() {Time = time, Position = destination, Rotation = transform.Basis.GetRotationQuaternion(), Teleported = true});
	}

	public void SampleGhost(float time, float previousTime, Ghost ghost) {
		var lastIndexBefore = 0;
		for (var i = 0; i < _samples.Count; i++) {
			if (_samples[i].Time > time) {
				break;
			}
			lastIndexBefore = i;
		}

		var state0 = _samples[lastIndexBefore];
		if (lastIndexBefore == _samples.Count - 1) {
			var transform = new Transform3D(new Basis(state0.Rotation).Scaled(ghost.GlobalTransform.Basis.Scale), state0.Position);
			ghost.GlobalTransform = transform;
		} else {
			var state1 = _samples[lastIndexBefore + 1];
			var weight = Mathf.Remap(time, state0.Time, state1.Time, 0.0f, 1.0f);

			var rotation  = state0.Rotation.Normalized().Slerp(state1.Rotation.Normalized(), weight).Normalized();
			var position  = state0.Position.Lerp(state1.Position, weight);
			var transform = new Transform3D(new Basis(rotation).Scaled(ghost.GlobalTransform.Basis.Scale), position);
			ghost.GlobalTransform = transform;

			if (state0.Teleported && previousTime < state0.Time) {
				ghost.ResetPhysicsInterpolation();
			}
		}
	}
}
