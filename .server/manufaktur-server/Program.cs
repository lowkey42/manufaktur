using System.Diagnostics;
using System.Text.Json;

string filename = Path.Combine(".manufaktur", "data_$level.json");

HashSet<string> levelNames = [
	"baselevel",
	"level01",
	"test",
	"beachlevel01",
	"beachlevel02",
	"beachlevel03",
	"factorylevel",
	"factorylevel01",
	"factorylevel02",
	"factorylevel03",
];

Dictionary<string, Scores> allScores = [];

Load();

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}


app.MapGet("/scores/{level}", (string level) =>
{
	Debug.WriteLine(level);
    if (!VerifyLevel(level)) return null;
    return allScores.TryGetValue(level, out Scores scores) ? scores.scores : new();
});

app.MapPost("/score/{level}", (string level, ScoreEntry score) => {
	if (!VerifyLevel(level)) throw new Exception("Illegal level");
    if (!Verify(score)) throw new Exception("Invalid score");
	score.name = FilterName(score.name);
    allScores.TryGetValue(level, out Scores scores);
    scores ??= (allScores[level] = new());
    scores.AddEntry(score);
	Save(level);
});

app.Run();

bool VerifyLevel(string level)
{
    return levelNames.Contains(level);
}

bool Verify(ScoreEntry score)
{
    if (score.name.Length > 20) return false;
    if (score.name.Length < 1) return false;
    if (score.time < 1) return false;

    return true;
}

void Save(string level) {
	var data = JsonSerializer.Serialize(allScores[level].scores);
	Directory.CreateDirectory(Path.GetDirectoryName(filename));
	lock (allScores[level]) {
		File.WriteAllText(filename.Replace("$level", level), data);
	}
}

void Load() {
	foreach (var level in levelNames) {
		try {
			var json = File.ReadAllText(filename.Replace("$level", level));
			var data = JsonSerializer.Deserialize<List<ScoreEntry>>(json) ?? throw new Exception("Level data is null.");
			allScores[level] = new Scores() { scores = data };
		} catch (Exception e) {
			Console.WriteLine(e.Message);
		}
	}
}

string FilterName(string name) {
	return name.Replace("AfD", "gesichert Rechtsextrem");
}

record class ScoreEntry
{
    public required string name { get; set; }
    public required float time { get; set; }

    public class Comparer: IComparer<ScoreEntry>
    {
        int IComparer<ScoreEntry>.Compare(ScoreEntry? x, ScoreEntry? y)
        {
            return x.time <= y.time ? -1 : 1;
        }
    }
}

class Scores
{
    private const int maxScoreEntriesPerLevel = 999;

    public List<ScoreEntry> scores = new(maxScoreEntriesPerLevel);

    public void AddEntry(ScoreEntry score)
    {
        var x = scores.BinarySearch(score, new ScoreEntry.Comparer());
        scores.Insert(~x, score);
        Trim();
    }


    private void Trim()
    {
        if (scores.Count < maxScoreEntriesPerLevel) return;
        scores.RemoveRange(maxScoreEntriesPerLevel, scores.Count - maxScoreEntriesPerLevel);
    }
}
