using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().UseMauiCommunityToolkit();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "todo.db3");

        builder.Services.AddSingleton(new TodoDatabase(dbPath));
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
