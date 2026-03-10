using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;
using Manufaktur.gameplay.player;

namespace Manufaktur.gameplay;

public class HighscoreList {
	public class Entry {
		private readonly HighscoreList _list;

		public string Name { get; set; }
		public float Time { get; private set; }
		public string GhostId { get; private set; }

		internal Entry(HighscoreList list, string name, float time, string ghostId) {
			_list   = list;
			Name    = name;
			Time    = time;
			GhostId = ghostId;
		}

		internal Dictionary ToDictionary() => new() {{"name", Name}, {"time", Time}, {"ghost", GhostId}};

		internal static Entry FromDictionary(HighscoreList list, Dictionary dict) =>
			new(list, dict["name"].AsString(), (float) dict["time"].AsDouble(), dict["ghost"].AsString());

		public string GetUserGhostPath() {
			if (GhostId == null)
				return null;
			if (GhostId == "DEFAULT")
				return null;

			return $"user://{_list._levelName}_ghost_{GhostId}.json";
		}

		public PlayerPath LoadGhost() {
			if (GhostId == null)
				return null;
			if (GhostId == "DEFAULT")
				return new PlayerPath(_list._defaultGhost);
			if (FileAccess.FileExists(GetUserGhostPath()))
				return new PlayerPath(GetUserGhostPath());
			return null;
		}
	}

	private const int _maxEntries = 10;

	private readonly string _levelName;

	private readonly Json _defaultGhost;
	private readonly List<Entry> _entries = [];

	public Entry[] Entries => _entries.ToArray();

	public float TimeMinimum { get; private set; }
	public float TimeBronze { get; private set; }
	public float TimeSilver { get; private set; }
	public float TimeGold { get; private set; }
	public float TimePlatinum { get; private set; }

	public HighscoreList(Node levelNode) :
		this(levelNode != null ? levelNode.GetName() : "unknown", levelNode is Level l ? l : null) { }

	public HighscoreList(string levelName, Level level = null) {
		_levelName    = levelName.ToLower();
		TimeMinimum   = level?.TimeMinimum ?? 42.99999f;
		TimeBronze    = level?.TimeBronze ?? 42.99999f;
		TimeSilver    = level?.TimeSilver ?? 42.99999f;
		TimeGold      = level?.TimeGold ?? 42.99999f;
		TimePlatinum  = level?.TimePlatinum ?? 42.99999f;
		_defaultGhost = level?.MinimumTimeGhost;

		var path = $"user://{_levelName}_highscore.json";
		if (FileAccess.FileExists(path)) {
			var json        = new Json();
			var parseResult = json.Parse(FileAccess.Open(path, FileAccess.ModeFlags.Read).GetAsText());
			if (parseResult != Error.Ok) {
				GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {path} at line {json.GetErrorLine()}");
				return;
			}

			var jsonData = json.Data.AsGodotDictionary();
			foreach (var entry in jsonData["entries"].AsGodotArray<Dictionary>()) {
				_entries.Add(Entry.FromDictionary(this, entry));
			}
		}

		if (_entries.Count == 0) {
			// static fallback if no highscore list exists
			_entries.Add(new Entry(this, "Schmanu", TimeMinimum, "DEFAULT"));
		}
	}

	public static Entry LoadTopEntry(string levelName) {
		var list = new HighscoreList(levelName);
		return list.Entries.Length > 0 ? list.Entries[0] : null;
	}

	public void ResetProgress() {
		_entries.Clear();
		ResetProgress(_levelName);
	}

	public static void ResetProgress(string levelName) {
		using var dir = DirAccess.Open("user://");
		if (dir == null) {
			GD.PrintErr($"An error occurred when trying to access save data at path: user://");
			return;
		}

		foreach (var fileName in dir.GetFiles()) {
			if (fileName.StartsWith(levelName)) {
				var err = dir.Remove(fileName);
				if (err != Error.Ok)
					GD.PrintErr($"Failed to delete {fileName}: {err}");
			}
		}
	}

	public Entry AddHighscoreEntry(string name, float time) {
		var i = 0;
		for (; i < _entries.Count; i++) {
			if (_entries[i].Time > time)
				break;
		}

		if (i >= _maxEntries)
			return null;

		var newEntry = new Entry(this, name, time, Guid.NewGuid().ToString());
		_entries.Insert(i, newEntry);
		while (_entries.Count > _maxEntries) {
			var ghostPath = _entries[^1].GetUserGhostPath();
			if (ghostPath != null && FileAccess.FileExists(ghostPath)) {
				DirAccess.RemoveAbsolute(ghostPath);
			}

			_entries.RemoveAt(_entries.Count - 1);
		}

		return newEntry;
	}

	public void Save() {
		var path = $"user://{_levelName}_highscore.json";

		var entries = new Array<Dictionary>();
		foreach (var entry in _entries) {
			entries.Add(entry.ToDictionary());
		}

		var data = new Dictionary {{"entries", entries}};

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(data, "\t"));
	}

	public bool IsMinimumTimeReached() {
		return _entries.Any(e => e.GhostId != "DEFAULT" && e.Time < TimeMinimum);
	}
}
