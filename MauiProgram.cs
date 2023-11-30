using Plugin.Maui.Audio;

namespace OConnor_Sarah_VS22;

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
				fonts.AddFont("ArcadeClassic.ttf", "Arcade"); //Aracde classic font - source: https://www.1001fonts.com/arcadeclassic-font.html
            });

        /* Audio manager code - source: https://www.youtube.com/watch?v=oIYnEuZ9oew&ab_channel=GeraldVersluis */
        builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddTransient<MainPage>();

		
        return builder.Build();
	}
}
