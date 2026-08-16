using System.Text.Json;
using SQLite;
using ToDoApp.Models;

namespace ToDoApp.Services;

public class TodoDatabase
{
    private static readonly string[] SeedOrder = ["papatya", "lale", "lavanta", "aycicegi", "cilek"];
    private static readonly int[] SeedMins = [1, 4, 8, 13, 19];
    private const int PlotCount = 6;

    private readonly SQLiteAsyncConnection _db;

    public TodoDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<TodoItem>().Wait();
        _db.CreateTableAsync<GardenState>().Wait();
        try
        {
            _db.ExecuteAsync("ALTER TABLE TodoItem ADD COLUMN DueDate DATETIME").Wait();
        }
        catch
        {
            // column already exists
        }
    }

    public Task<List<TodoItem>> GetItemsAsync() =>
        _db.Table<TodoItem>().OrderByDescending(t => t.Id).ToListAsync();

    public Task<int> SaveItemAsync(TodoItem item) =>
        item.Id != 0 ? _db.UpdateAsync(item) : _db.InsertAsync(item);

    public Task<int> DeleteItemAsync(TodoItem item) =>
        _db.DeleteAsync(item);

    public async Task<GardenState> GetGardenAsync()
    {
        try
        {
            var row = await _db.FindAsync<GardenState>(1);
            return Sanitize(row);
        }
        catch
        {
            return EmptyGarden();
        }
    }

    public async Task SaveGardenAsync(GardenState garden)
    {
        try
        {
            garden.Id = 1;
            var clean = Sanitize(garden);
            await _db.InsertOrReplaceAsync(clean);
        }
        catch
        {
            // keep the app running even if disk write fails
        }
    }

    public async Task<(GardenState Garden, string? UnlockedLabel)> AwardCompletionAsync(int taskId)
    {
        var garden = await GetGardenAsync();
        var awarded = ReadIdList(garden.AwardedTaskIdsJson);
        if (awarded.Contains(taskId))
            return (garden, null);

        awarded.Add(taskId);
        garden.AwardedTaskIdsJson = JsonSerializer.Serialize(awarded);
        garden.SeedInventory += 1;
        garden.LifetimeCompletedTasks += 1;

        var unlocked = ReadStringList(garden.UnlockedSeedTypesJson);
        string? newly = null;
        for (var i = 0; i < SeedOrder.Length; i++)
        {
            var id = SeedOrder[i];
            if (garden.LifetimeCompletedTasks >= SeedMins[i] && !unlocked.Contains(id))
            {
                unlocked.Add(id);
                newly = LabelFor(id);
            }
        }
        garden.UnlockedSeedTypesJson = JsonSerializer.Serialize(unlocked);
        garden.PlotsJson = GrowPlots(garden.PlotsJson);
        await SaveGardenAsync(garden);
        return (garden, newly);
    }

    private static GardenState EmptyGarden()
    {
        var plots = Enumerable.Range(0, PlotCount)
            .Select(i => new GardenPlotDto { PlotId = i, Stage = "empty" })
            .ToList();
        return new GardenState
        {
            Id = 1,
            SeedInventory = 0,
            LifetimeCompletedTasks = 0,
            UnlockedSeedTypesJson = "[]",
            PlotsJson = JsonSerializer.Serialize(plots),
            DecorationsJson = "[]",
            AwardedTaskIdsJson = "[]"
        };
    }

    private static GardenState Sanitize(GardenState? raw)
    {
        var fresh = EmptyGarden();
        if (raw is null) return fresh;

        fresh.SeedInventory = Math.Max(0, raw.SeedInventory);
        fresh.LifetimeCompletedTasks = Math.Max(0, raw.LifetimeCompletedTasks);
        fresh.AwardedTaskIdsJson = JsonSerializer.Serialize(ReadIdList(raw.AwardedTaskIdsJson));

        var unlocked = ReadStringList(raw.UnlockedSeedTypesJson)
            .Where(SeedOrder.Contains)
            .Distinct()
            .ToList();
        for (var i = 0; i < SeedOrder.Length; i++)
        {
            if (fresh.LifetimeCompletedTasks >= SeedMins[i] && !unlocked.Contains(SeedOrder[i]))
                unlocked.Add(SeedOrder[i]);
        }
        fresh.UnlockedSeedTypesJson = JsonSerializer.Serialize(unlocked);

        var plots = ReadPlots(raw.PlotsJson);
        fresh.PlotsJson = JsonSerializer.Serialize(plots);

        var decor = ReadDecor(raw.DecorationsJson);
        fresh.DecorationsJson = JsonSerializer.Serialize(decor);
        return fresh;
    }

    private static List<int> ReadIdList(string? json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<int>>(json ?? "[]") ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static List<string> ReadStringList(string? json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json ?? "[]") ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static List<GardenPlotDto> ReadPlots(string? json)
    {
        List<GardenPlotDto> incoming;
        try
        {
            incoming = JsonSerializer.Deserialize<List<GardenPlotDto>>(json ?? "[]") ?? [];
        }
        catch
        {
            incoming = [];
        }

        var byId = incoming
            .Where(p => p is not null && IsStage(p.Stage) && p.PlotId >= 0 && p.PlotId < PlotCount)
            .GroupBy(p => p.PlotId)
            .ToDictionary(g => g.Key, g => g.First());

        return Enumerable.Range(0, PlotCount).Select(i =>
        {
            if (byId.TryGetValue(i, out var p))
            {
                p.PlotId = i;
                return p;
            }
            return new GardenPlotDto { PlotId = i, Stage = "empty" };
        }).ToList();
    }

    private static List<GardenDecorDto> ReadDecor(string? json)
    {
        try
        {
            return (JsonSerializer.Deserialize<List<GardenDecorDto>>(json ?? "[]") ?? [])
                .Where(d => d is not null && !string.IsNullOrWhiteSpace(d.Kind))
                .Select(d =>
                {
                    d.X = Math.Clamp(d.X, 8, 92);
                    d.Y = Math.Clamp(d.Y, 14, 90);
                    if (string.IsNullOrWhiteSpace(d.Id))
                        d.Id = Guid.NewGuid().ToString("N");
                    return d;
                })
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    private static string GrowPlots(string? json)
    {
        var plots = ReadPlots(json);
        foreach (var plot in plots)
        {
            plot.Stage = plot.Stage switch
            {
                "seed" => "filiz",
                "filiz" => "tomurcuk",
                "tomurcuk" => "cicek",
                _ => plot.Stage
            };
        }
        return JsonSerializer.Serialize(plots);
    }

    private static bool IsStage(string? stage) =>
        stage is "empty" or "seed" or "filiz" or "tomurcuk" or "cicek";

    private static string LabelFor(string id) => id switch
    {
        "lale" => "lale",
        "lavanta" => "lavanta",
        "aycicegi" => "ayçiçeği",
        "cilek" => "çilek çiçeği",
        _ => "papatya"
    };
}

public class GardenPlotDto
{
    public int PlotId { get; set; }
    public string Stage { get; set; } = "empty";
    public long? PlantedAt { get; set; }
    public string? FlowerKind { get; set; }
}

public class GardenDecorDto
{
    public string Id { get; set; } = "";
    public string Kind { get; set; } = "papatya";
    public double X { get; set; }
    public double Y { get; set; }
}
