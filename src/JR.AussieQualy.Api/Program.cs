namespace JR.AussieQualy.Api;

using JR.AussieQualy.Application.Dtos;
using JR.AussieQualy.Application.Qualifying;
using JR.AussieQualy.Infrastructure.LapTimes;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Register services
        builder.Services.AddSingleton<ILapTimesRepository>(sp =>
        {
            string jsonPath = Path.Combine(AppContext.BaseDirectory, "data", "session_laptimes.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException($"Lap times JSON file not found at '{jsonPath}'.");
            }

            return new LapTimesRepository(jsonPath);
        });

        builder.Services.AddScoped<IQualifyingService, QualifyingService>();

        WebApplication app = builder.Build();

        app.MapGet("/2026/AUS/Q/{driverCode}", (string driverCode, IQualifyingService service) =>
        {
            if (string.IsNullOrWhiteSpace(driverCode))
            {
                return Results.BadRequest("Driver code must be provided.");
            }

            try
            {
                var result = service.GetDriverQualifyingResult(driverCode);

                if (result is null)
                {
                    return Results.NotFound($"Driver with code '{driverCode}' not found or has no qualifying data.");
                }

                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        // GET all driver details for the 2026 AUS qualifying session
        app.MapGet("/2026/AUS/drivers", (IQualifyingService service) =>
        {
            try
            {
                return Results.Ok(service.GetAllDrivers());
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        app.Run();
    }
}
