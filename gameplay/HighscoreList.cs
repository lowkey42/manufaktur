using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Manufaktur.gameplay.player;

namespace Manufaktur.gameplay;

public class HighscoreList {
	private const string ScoreServerUrl = null; // "http://localhost:5189/";

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
			if (GhostId == null) {
				return null;
			}
			if (GhostId == "DEFAULT") {
				return null;
			}

			return $"user://{_list._levelName}_ghost_{GhostId}.json";
		}

		public PlayerPath LoadGhost() {
			if (GhostId == null) {
				return null;
			}
			if (GhostId == "DEFAULT" && _list._defaultGhost != null) {
				return new PlayerPath(_list._defaultGhost);
			}
			if (FileAccess.FileExists(GetUserGhostPath())) {
				return new PlayerPath(GetUserGhostPath());
			}
			return null;
		}

		public void ClearGhostId() {
			GhostId = null;
		}
	}

	private const int _maxEntries = 10;

	private readonly string _levelName;

	private readonly Json _defaultGhost;
	private readonly List<Entry> _entriesLocal = [];

	private readonly List<Entry> _unsavedGlobalEntries = [];

	public Entry[] EntriesLocal => _entriesLocal.ToArray();

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
			if(jsonData.TryGetValue("entries", out var entriesLocal)) {
				foreach (var entry in entriesLocal.AsGodotArray<Dictionary>()) {
					_entriesLocal.Add(Entry.FromDictionary(this, entry));
				}
			}
		}

		if (_entriesLocal.Count == 0) {
			// static fallback if no highscore list exists
			_entriesLocal.Add(new Entry(this, "Schmanu", TimeMinimum, "DEFAULT"));
		}
	}

	public static Entry LoadTopEntry(string levelName) {
		var list = new HighscoreList(levelName);
		return list.EntriesLocal.Length > 0 ? list.EntriesLocal[0] : null;
	}

	public void ResetProgress() {
		_entriesLocal.Clear();
		ResetProgress(_levelName);
	}

	public static void ResetProgress(string levelName) {
		using var dir = DirAccess.Open("user://");
		if (dir == null) {
			GD.PrintErr($"An error occurred when trying to access save data at path: user://");
			return;
		}

		// delete all ghosts
		foreach (var fileName in dir.GetFiles()) {
			if (fileName.StartsWith(levelName + "_ghost_")) {
				var err = dir.Remove(fileName);
				if (err != Error.Ok) {
					GD.PrintErr($"Failed to delete {fileName}: {err}");
				}
			}
		}
		
		// clear local score boards
		var scores = new HighscoreList(levelName);
		scores._entriesLocal.Clear();
		scores.Save();
	}

	public Entry AddHighscoreEntry(string name, float time) {
		var newEntry    = new Entry(this, name, time, Guid.NewGuid().ToString());
		var newInLocal  = InsertNewEntry(newEntry, _entriesLocal, true);
		_unsavedGlobalEntries.Add(newEntry);
		return newEntry;
	}

	private bool InsertNewEntry(Entry newEntry, List<Entry> entries, bool deleteGhosts) {
		var i = 0;
		for (; i < entries.Count; i++) {
			if (entries[i].Time > newEntry.Time) {
				break;
			}
		}

		if (i >= _maxEntries) {
			return false;
		}
		
		entries.Insert(i, newEntry);
		while (entries.Count > _maxEntries) {
			var ghostPath = entries[^1].GetUserGhostPath();
			if (deleteGhosts && ghostPath != null && FileAccess.FileExists(ghostPath)) {
				DirAccess.RemoveAbsolute(ghostPath);
			}

			entries.RemoveAt(entries.Count - 1);
		}

		return true;
	}

	public void Save() {
		var path = $"user://{_levelName}_highscore.json";

		var entriesLocal = new Array<Dictionary>();
		foreach (var entry in _entriesLocal) {
			entriesLocal.Add(entry.ToDictionary());
		}
		
		var data = new Dictionary {{"entries", entriesLocal}};

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(Json.Stringify(data, "\t"));

		SaveGlobalEntries();
	}

	public async Task<HighscoreList.Entry[]> GetGlobalScores(HighscoreList.Entry newEntry = null) {
		if (ScoreServerUrl == null) {
			return newEntry == null ? [] : [newEntry];
		}

		System.Net.Http.HttpClient client = new();
		var response = await client.GetAsync(ScoreServerUrl + "scores/" + _levelName);
		var resultJson = await response.Content.ReadAsStringAsync();
		var data = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(resultJson);
		var result = data.Select(entry => new HighscoreList.Entry(null, entry.GetProperty("name").GetString(), (float) entry.GetProperty("time").GetDouble(), null)).ToList();

		if (newEntry != null) {
			var index = ~(result.Select(x => x.Time).ToList().BinarySearch(newEntry.Time));
			result.Insert(index, newEntry);
		}

		return result.ToArray();
	}

	private void SaveGlobalEntries() {
		if (ScoreServerUrl == null) {
			_unsavedGlobalEntries.Clear();
			return; 
		}

		System.Net.Http.HttpClient client = new();
		foreach (var entry in _unsavedGlobalEntries) {
			object obj = new {
				name = entry.Name,
				time = entry.Time,
			};
			try {
				client.PostAsJsonAsync(ScoreServerUrl + "score/" + _levelName, obj);
			} catch (Exception e) {
				GD.PrintErr(e);
			}
		}
		_unsavedGlobalEntries.Clear();
	}

	public bool IsMinimumTimeReached() {
		return _entriesLocal.Any(e => e.GhostId != "DEFAULT" && e.Time < TimeMinimum);
	}
}
