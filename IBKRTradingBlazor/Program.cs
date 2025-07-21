using IBKRTradingBlazor;
using IBKRTradingBlazor.Client.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IbkrService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Keep this commented out for now

app.UseStaticFiles();

app.UseRouting();

app.UseBlazorFrameworkFiles();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapPost("/api/trade", (TradeRequest req, IbkrService ibkr) =>
{
    try
    {
        ibkr.Connect(); // Optionally pass host/port/clientId from req
        ibkr.PlaceOrder(req.Symbol, req.Exchange, req.SecType, req.Currency, req.Quantity, req.Price);
        return Results.Ok("Order placed");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Order failed: {ex.Message}");
    }
});

app.MapPost("/api/connect", (IbkrService ibkr) =>
{
    try
    {
        ibkr.Connect();
        return Results.Ok("Connected to IBKR");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Connect failed: {ex.Message}");
    }
});

app.MapPost("/api/disconnect", (IbkrService ibkr) =>
{
    try
    {
        ibkr.Disconnect();
        return Results.Ok("Disconnected from IBKR");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Disconnect failed: {ex.Message}");
    }
});

app.MapGet("/api/positions", async (IbkrService ibkr) =>
{
    ibkr.RequestPositionsWithMarketData();
    int waited = 0;
    while (!ibkr.PositionsComplete && waited < 5000) // wait up to 5 seconds
    {
        await Task.Delay(100);
        waited += 100;
    }
    await Task.Delay(1000); // Wait 1s for market data to arrive
    return Results.Ok(ibkr.GetPositions());
});

app.MapGet("/api/account", async (IbkrService ibkr) =>
{
    ibkr.RequestAccountSummary();
    int waited = 0;
    while (!ibkr.AccountSummaryComplete && waited < 5000) // wait up to 5 seconds
    {
        await Task.Delay(100);
        waited += 100;
    }
    return Results.Ok(ibkr.GetAccountSummary());
});

app.MapGet("/api/history", async (IbkrService ibkr) =>
{
    ibkr.RequestOrderHistory();
    int waited = 0;
    while (!ibkr.OrderHistoryComplete && waited < 5000) // wait up to 5 seconds
    {
        await Task.Delay(100);
        waited += 100;
    }
    return Results.Ok(ibkr.GetOrderHistory());
});

app.MapPost("/api/closeposition", async (IbkrService ibkr, PositionInfo position) =>
{
    ibkr.CancelAllOrders();
    await Task.Delay(500); // Give some time for cancels to process
    ibkr.ClosePosition(position);
    return Results.Ok();
});

app.Run();

