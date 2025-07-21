using Microsoft.Extensions.Logging;
using IBKRTradingBlazor.Desktop.Components.Layout;
using IBKRTradingBlazor.Desktop.Models;
using IBKRTradingBlazor.Desktop.Services;

namespace IBKRTradingBlazor.Desktop;

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
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<FileLogger>();
		builder.Services.AddSingleton<TradingModeService>();
		builder.Services.AddScoped<IbkrService, IbkrService>(); // Default to paper
		builder.Services.AddScoped<RealIbkrService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001") });

		return builder.Build();
	}
}
