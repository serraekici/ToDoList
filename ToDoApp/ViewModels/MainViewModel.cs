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
            if (hour < 18) return "iyi günler, bahçe seni bekliyor";
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
            if (Tasks.Count == 0) return "bahçe henüz uyanıyor";
            if (CompletedCount == Tasks.Count) return "bahçe tam çiçekte";
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

        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTask)) return;

        var todo = new TodoItem { Title = NewTask.Trim() };
        await _db.SaveItemAsync(todo);
        Tasks.Insert(0, todo);
        NewTask = string.Empty;
        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task ToggleComplete(TodoItem item)
    {
        item.IsCompleted = !item.IsCompleted;
        await _db.SaveItemAsync(item);
        NotifyGardenStats();
    }

    [RelayCommand]
    private async Task DeleteTask(TodoItem item)
    {
        await _db.DeleteItemAsync(item);
        Tasks.Remove(item);
        NotifyGardenStats();
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
