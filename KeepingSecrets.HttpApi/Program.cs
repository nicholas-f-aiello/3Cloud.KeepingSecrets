var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var azureAppConfigurationEndpointUri = builder.Configuration["AzureAppConfiguration:Endpoint"];

builder.Host.ConfigureAppConfiguration(builder =>
{
    builder.AddEnvironmentVariables();

    if (!string.IsNullOrWhiteSpace(azureAppConfigurationEndpointUri))
    {
        var credentials = new DefaultAzureCredential();

        builder.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(azureAppConfigurationEndpointUri), credentials)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credentials);
                });
        });
    }
});

builder.Services.AddOptions<OpenWeatherConfiguration>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection("OpenWeather").Bind(settings);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var secret = app.Configuration["OpenWeather:ApiKey"];
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}