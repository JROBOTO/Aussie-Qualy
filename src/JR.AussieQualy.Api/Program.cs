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
        app.MapGet("/2026/AUS/Q", (IQualifyingService service) =>
        {
            try
            {
                var codes = service.GetAllDriverCodes();

                var results = codes
                    .Select(code => service.GetDriverQualifyingResult(code))
                    .Where(r => r is not null)
                    .ToArray()!;

                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        app.Run();
    }
}
