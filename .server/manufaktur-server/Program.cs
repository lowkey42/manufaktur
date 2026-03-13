var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

HashSet<string> levelNames = [
    "test"
];

Dictionary<string, Scores> allScores = [];

app.MapGet("/scores/{level}", (string level) =>
{
    if (!VerifyLevel(level)) return null;
    return allScores.TryGetValue(level, out Scores scores) ? scores.scores : new();
});

app.MapPost("/score", (ScoreEntry score) => {
    if (!Verify(score)) throw new Exception();
    allScores.TryGetValue(score.level, out Scores scores);
    scores ??= (allScores[score.level] = new());
    scores.AddEntry(score);
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
    if (!VerifyLevel(score.level)) return false;
    if (score.time < 5) return false;

    return true;
}


record class ScoreEntry
{
    public required string level { get; set; }
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
