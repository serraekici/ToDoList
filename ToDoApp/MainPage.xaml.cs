using ToDoApp.ViewModels;

namespace ToDoApp;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MainViewModel vm)
            await vm.LoadTasksCommand.ExecuteAsync(null);
    }

    private void OnNewTaskCompleted(object? sender, EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
            vm.AddTaskCommand.Execute(null);
    }
}
