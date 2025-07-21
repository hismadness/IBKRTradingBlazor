using IBKRTradingBlazor.Desktop.Services;

namespace IBKRTradingBlazor.Desktop;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		
		try
		{
			var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
			Directory.CreateDirectory(logDir);
			var logFilePath = Path.Combine(logDir, $"log_{DateTime.Now:yyyyMMdd_HHmmss}_mainpage.txt");
			File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainPage constructor called.{Environment.NewLine}");
		}
		catch { /* ignore logging errors */ }
	}
}
