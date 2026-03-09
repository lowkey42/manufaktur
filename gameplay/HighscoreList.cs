using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Manufaktur.gameplay.player;

namespace Manufaktur.gameplay;

public class HighscoreList {
	public class Entry {
		private readonly string _levelName;
		
		public string Name { get; set; }
		public float Time { get; private set; }
		public string GhostId { get; private set; }

		internal Entry(string levelName, string name, float time, string ghostId) {
			_levelName = levelName;
			Name       = name;
			Time       = time;
			GhostId    = ghostId;
		}

		internal Dictionary ToDictionary() => new() {{"name", Name}, {"time", Time}, {"ghost", GhostId}};

		internal static Entry FromDictionary(string levelName, Dictionary dict) => new(levelName, dict["name"].AsString(), (float) dict["time"].AsDouble(), dict["ghost"].AsString());

		public string GetUserGhostPath() => GhostId!=null ? $"user://{_levelName}_ghost_{GhostId}.json" : null;
		
		public string GetSystemGhostPath() => GhostId!=null ? $"res://gameplay/levels/{_levelName}_ghost_{GhostId}.json" : null;
		
		public PlayerPath LoadGhost() {
			if (GhostId == null)
				return null;

			if(FileAccess.FileExists(GetUserGhostPath()))
				return new PlayerPath(GetUserGhostPath());
			if(FileAccess.FileExists(GetSystemGhostPath()))
				return new PlayerPath(GetSystemGhostPath());

			return null;
		}
	}

	private const int _maxEntries = 10;

	private readonly string _levelName;
	
	private readonly List<Entry> _entries = [];

	public Entry[] Entries => _entries.ToArray();
	
	public HighscoreList(string levelName) {
		_levelName = levelName.ToLower();
		
		var path = $"user://{_levelName}_highscore.json";
		if (!FileAccess.FileExists(path)) {
			path = $"res://gameplay/levels/{_levelName}_highscore.json";
		}

		if (!FileAccess.FileExists(path)) {
			// static fallback if no highscore list exists
			_entries.Add(new Entry(_levelName, "Schmanu", 4.2f, null));
			_entries.Add(new Entry(_levelName, "Kanu", 23f, null));
			_entries.Add(new Entry(_levelName, "Manu", 42.42f, null));
			return;
		}

		var json        = new Json();
		var parseResult = json.Parse(FileAccess.Open(path, FileAccess.ModeFlags.Read).GetAsText());
		if (parseResult != Error.Ok) {
			GD.PrintErr($"JSON Parse Error: {json.GetErrorMessage()} in {path} at line {json.GetErrorLine()}");
			return;
		}

		var jsonData = json.Data.AsGodotDictionary();
		foreach (var entry in jsonData["entries"].AsGodotArray<Dictionary>()) {
			_entries.Add(Entry.FromDictionary(_levelName, entry));
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

		var newEntry = new Entry(_levelName, name, time, Guid.NewGuid().ToString());
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
}
