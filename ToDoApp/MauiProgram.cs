using Microsoft.Extensions.Logging;
using ToDoApp.Services;
using ToDoApp.ViewModels;

namespace ToDoApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("PlayfairDisplay.ttf", "Playfair");
				fonts.AddFont("Nunito.ttf", "Nunito");
				fonts.AddFont("Caveat.ttf", "Caveat");
			});

		builder.Services.AddSingleton(sp =>
		{
			var dbPath = Path.Combine(FileSystem.AppDataDirectory, "bloom.db3");
			return new TodoDatabase(dbPath);
		});
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
