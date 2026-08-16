using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TodoDatabase _db;
    private static readonly CultureInfo Turkish = new("tr-TR");

    [ObservableProperty]
    private string newTask = string.Empty;

    [ObservableProperty]
    private int seedInventory;

    [ObservableProperty]
    private string gardenStatus = "tohum: 0";

    [ObservableProperty]
    private bool includeDueDate;

    [ObservableProperty]
    private DateTime newDueDate = DateTime.Today;

    public ObservableCollection<TodoItem> Tasks { get; } = new();

    public MainViewModel(TodoDatabase db)
    {
        _db = db;
    }

    public string TodayLabel => DateTime.Now.ToString("d MMMM yyyy", Turkish);

    public string Greeting
    {
        get
        {
            var hour = DateTime.Now.Hour;
            if (hour < 12) return "günaydın, küçük bahçıvan";
            if (hour < 18) return "iyi günler, Profilim seni bekliyor";
            return "akşamın tatlı saatleri";
        }
    }

    public int CompletedCount => Tasks.Count(t => t.IsCompleted);

    public int RemainingCount => Tasks.Count(t => !t.IsCompleted);

    public double Progress => Tasks.Count == 0 ? 0 : (double)CompletedCount / Tasks.Count;

    public string ProgressLabel
    {
        get
        {
            if (Tasks.Count == 0) return "Profilim henüz uyanıyor";
            if (CompletedCount == Tasks.Count) return "Profilim tam çiçekte";
            if (CompletedCount == 0) return $"{RemainingCount} tomurcuk bekliyor";
            return $"{CompletedCount} çiçek açtı · {RemainingCount} tomurcuk";
        }
    }

    public bool IsEmpty => Tasks.Count == 0;

    [RelayCommand]
    private async Task LoadTasks()
    {
        Tasks.Clear();
        var items = await _db.GetItemsAsync();
        foreach (var item in items)
            Tasks.Add(item);

        await RefreshGarden();
        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTask)) return;

        var todo = new TodoItem
        {
            Title = NewTask.Trim(),
            DueDate = IncludeDueDate ? NewDueDate.Date : null
        };
        await _db.SaveItemAsync(todo);
        Tasks.Insert(0, todo);
        NewTask = string.Empty;
        IncludeDueDate = false;
        NewDueDate = DateTime.Today;
        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task ToggleComplete(TodoItem item)
    {
        var completing = !item.IsCompleted;
        item.IsCompleted = completing;
        await _db.SaveItemAsync(item);
        if (completing)
        {
            var (_, unlocked) = await _db.AwardCompletionAsync(item.Id);
            if (!string.IsNullOrEmpty(unlocked))
                GardenStatus = "yeni tohum açıldı: " + unlocked;
        }
        await RefreshGarden();
        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task DeleteTask(TodoItem item)
    {
        await _db.DeleteItemAsync(item);
        Tasks.Remove(item);
        NotifyGardenStats();
    }

    private async Task RefreshGarden()
    {
        var garden = await _db.GetGardenAsync();
        SeedInventory = garden.SeedInventory;
        if (string.IsNullOrEmpty(GardenStatus) || !GardenStatus.StartsWith("yeni tohum açıldı", StringComparison.Ordinal))
            GardenStatus = "tohum: " + garden.SeedInventory;
    }

    private void NotifyGardenStats()
    {
        OnPropertyChanged(nameof(CompletedCount));
        OnPropertyChanged(nameof(RemainingCount));
        OnPropertyChanged(nameof(Progress));
        OnPropertyChanged(nameof(ProgressLabel));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(Greeting));
        OnPropertyChanged(nameof(TodayLabel));
    }
}
