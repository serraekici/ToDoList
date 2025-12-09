using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TodoDatabase _db;

    [ObservableProperty]
    private string newTask = string.Empty;

    public ObservableCollection<TodoItem> Tasks { get; } = new();

    public MainViewModel(TodoDatabase db)
    {
        _db = db;
        LoadTasksCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTasks()
    {
        Tasks.Clear();
        var items = await _db.GetItemsAsync();
        foreach (var item in items)
            Tasks.Add(item);
    }

    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTask)) return;
        var todo = new TodoItem { Title = NewTask };
        await _db.SaveItemAsync(todo);
        Tasks.Insert(0, todo);
        NewTask = string.Empty;
    }

    [RelayCommand]
    private async Task ToggleComplete(TodoItem item)
    {
        item.IsCompleted = !item.IsCompleted;
        await _db.SaveItemAsync(item);
        await LoadTasks();
    }

    [RelayCommand]
    private async Task DeleteTask(TodoItem item)
    {
        await _db.DeleteItemAsync(item);
        Tasks.Remove(item);
    }
}
