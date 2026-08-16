using SQLite;

namespace ToDoApp.Models;

public class GardenState
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public int SeedInventory { get; set; }

    public int LifetimeCompletedTasks { get; set; }

    public string UnlockedSeedTypesJson { get; set; } = "[]";

    public string PlotsJson { get; set; } = "[]";

    public string DecorationsJson { get; set; } = "[]";

    public string AwardedTaskIdsJson { get; set; } = "[]";
}
