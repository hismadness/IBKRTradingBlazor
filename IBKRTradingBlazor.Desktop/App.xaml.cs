using Microsoft.Extensions.Logging;
using IBKRTradingBlazor.Desktop.Services;

namespace IBKRTradingBlazor.Desktop;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "IBKR Trading Desktop" };
	}
}
