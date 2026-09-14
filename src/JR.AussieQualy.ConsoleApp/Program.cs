using System.Net.Http.Json;
using JR.AussieQualy.ConsoleApp;

const string ApiBaseUrl = "http://localhost:5144";

using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

List<DriverDto> drivers;

try
{
    drivers = await http.GetFromJsonAsync<List<DriverDto>>("/2026/AUS/drivers") ?? new List<DriverDto>();
}
catch (HttpRequestException)
{
    Console.WriteLine($"Unable to connect to the API at {ApiBaseUrl}. Make sure JR.AussieQualy.Api is running.");
    return;
}

if (drivers.Count == 0)
{
    Console.WriteLine("No driver data was returned by the API.");
    return;
}

foreach (var teamGroup in drivers.GroupBy(d => d.Team).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine($"=== {teamGroup.Key} ===");
    foreach (var driver in teamGroup.OrderBy(d => d.DriverNumber))
    {
        Console.WriteLine($"  #{driver.DriverNumber} - {driver.Name} ({driver.Code})");
    }

    Console.WriteLine();
}

while (true)
{
    Console.WriteLine("Enter a driver code to view their qualifying performance (or 'exit' to quit):");
    string? input = ReadInput();

    if (input is null || string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    string code = input.Trim();

    if (!drivers.Any(d => string.Equals(d.Code, code, StringComparison.OrdinalIgnoreCase)))
    {
        Console.WriteLine($"'{code}' is not a valid driver code. Please try again.");
        Console.WriteLine();
        continue;
    }

    DriverQualifyingResultDto? result;

    try
    {
        result = await http.GetFromJsonAsync<DriverQualifyingResultDto>(
            $"/2026/AUS/Q/{Uri.EscapeDataString(code.ToUpperInvariant())}");
    }
    catch (HttpRequestException)
    {
        result = null;
    }

    if (result is null)
    {
        Console.WriteLine($"No qualifying data found for '{code}'. Please try again.");
        Console.WriteLine();
        continue;
    }

    ShowQualifyingSummary(result);

    if (!ShowSegmentMenu(result))
    {
        return;
    }

    Console.WriteLine();
}

static string? ReadInput()
{
    if (Console.IsInputRedirected)
    {
        return Console.ReadLine();
    }

    var buffer = new System.Text.StringBuilder();

    while (true)
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Escape)
        {
            Console.WriteLine();
            return null;
        }

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return buffer.ToString();
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (buffer.Length > 0)
            {
                buffer.Length--;
                Console.Write("\b \b");
            }

            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            buffer.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }
    }
}

static void ShowQualifyingSummary(DriverQualifyingResultDto result)
{
    Console.WriteLine();
    Console.WriteLine($"=== {result.Driver.Code} - {result.Driver.Name} ({result.Driver.Team}) ===");
    PrintSegmentRow("Q1", result.Q1);
    PrintSegmentRow("Q2", result.Q2);
    PrintSegmentRow("Q3", result.Q3);
    Console.WriteLine($"Final classification: P{result.FinalQualifyingPosition}");
    Console.WriteLine();
}

static void PrintSegmentRow(string label, LapDataDto? lap)
{
    if (lap is null)
    {
        Console.WriteLine($"{label}: Did not participate");
        return;
    }

    string position = lap.SegmentPosition.HasValue ? $"P{lap.SegmentPosition}" : "N/A";
    Console.WriteLine($"{label}: {position} - {FormatLapTime(lap.LapTime)}");
}

static bool ShowSegmentMenu(DriverQualifyingResultDto result)
{
    while (true)
    {
        Console.WriteLine("Select an option:");
        Console.WriteLine("  1 - Q1 details");
        Console.WriteLine("  2 - Q2 details");
        Console.WriteLine("  3 - Q3 details");
        Console.WriteLine("  4 - Tyre info");
        Console.WriteLine("  5 - Conditions");
        Console.WriteLine("  6 - Back to driver list");
        string? input = ReadInput();

        if (input is null)
        {
            return false;
        }

        switch (input.Trim())
        {
            case "1":
                ShowSectorMenu("Q1", result.Q1);
                break;
            case "2":
                ShowSectorMenu("Q2", result.Q2);
                break;
            case "3":
                ShowSectorMenu("Q3", result.Q3);
                break;
            case "4":
                ShowTyreInfo(result);
                break;
            case "5":
                ShowConditions(result);
                break;
            case "6":
                return true;
            default:
                Console.WriteLine("Invalid option. Please enter 1-6.");
                break;
        }
    }
}

static void ShowSectorMenu(string segmentName, LapDataDto? lap)
{
    if (lap is null)
    {
        Console.WriteLine($"{segmentName}: Did not participate in this segment.");
        Console.WriteLine();
        return;
    }

    string position = lap.SegmentPosition.HasValue ? $"P{lap.SegmentPosition}" : "N/A";

    while (true)
    {
        Console.WriteLine($"{segmentName} ({position}) - Select a sector:");
        PrintSectorListOption("1 - Sector 1", lap.Sector1);
        PrintSectorListOption("2 - Sector 2", lap.Sector2);
        PrintSectorListOption("3 - Sector 3", lap.Sector3);
        Console.WriteLine("  4 - Back");
        string? input = ReadInput();

        if (input is null)
        {
            return;
        }

        switch (input.Trim())
        {
            case "1":
                PrintSector("Sector 1", lap.Sector1);
                break;
            case "2":
                PrintSector("Sector 2", lap.Sector2);
                break;
            case "3":
                PrintSector("Sector 3", lap.Sector3);
                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid option. Please enter 1-4.");
                break;
        }
    }
}

static void PrintSectorListOption(string label, SectorDto sector)
{
    string time = sector.SectorTime.HasValue ? $"{sector.SectorTime.Value:F3}s" : "N/A";
    string status = string.IsNullOrWhiteSpace(sector.SectorStatus) ? "" : $" [{sector.SectorStatus}]";
    Console.WriteLine($"  {label} - {time}{status}");
}

static void PrintSector(string label, SectorDto sector)
{
    string time = sector.SectorTime.HasValue ? $"{sector.SectorTime.Value:F3}s" : "N/A";
    string status = string.IsNullOrWhiteSpace(sector.SectorStatus) ? "none" : sector.SectorStatus!;
    Console.WriteLine($"{label}: {time} ({status})");

    if (sector.MiniSectors.Count > 0)
    {
        Console.WriteLine($"  Mini-sectors: {string.Join(", ", sector.MiniSectors)}");
    }

    Console.WriteLine();
}

static void ShowTyreInfo(DriverQualifyingResultDto result)
{
    Console.WriteLine("Tyre info:");

    foreach ((string label, LapDataDto? lap) in Segments(result))
    {
        if (lap is null)
        {
            Console.WriteLine($"  {label}: Did not participate");
            continue;
        }

        Console.WriteLine($"  {label}: {lap.Tire.Compound} (life: {lap.Tire.Life} laps)");
    }

    Console.WriteLine();
}

static void ShowConditions(DriverQualifyingResultDto result)
{
    Console.WriteLine("Conditions:");

    foreach ((string label, LapDataDto? lap) in Segments(result))
    {
        if (lap is null)
        {
            Console.WriteLine($"  {label}: Did not participate");
            continue;
        }

        ConditionsDto c = lap.Conditions;
        Console.WriteLine($"  {label}:");
        Console.WriteLine($"    Air temp: {FormatValue(c.AirTemperature, "°C")}, Track temp: {FormatValue(c.TrackTemperature, "°C")}");
        Console.WriteLine($"    Humidity: {FormatValue(c.Humidity, "%")}, Pressure: {FormatValue(c.Pressure, " hPa")}");
        Console.WriteLine($"    Rainfall: {FormatValue(c.Rainfall, " mm")}");
        Console.WriteLine($"    Wind: {FormatValue(c.WindSpeedInMs, " m/s")} at {FormatValue(c.WindDirectionBearing, "°")}");
    }

    Console.WriteLine();
}

static IEnumerable<(string Label, LapDataDto? Lap)> Segments(DriverQualifyingResultDto result)
{
    yield return ("Q1", result.Q1);
    yield return ("Q2", result.Q2);
    yield return ("Q3", result.Q3);
}

static string FormatValue(double? value, string suffix) =>
    value.HasValue ? $"{value.Value}{suffix}" : "N/A";

static string FormatLapTime(double? lapTime)
{
    if (!lapTime.HasValue)
    {
        return "No time set";
    }

    var time = TimeSpan.FromSeconds(lapTime.Value);
    return $"{(int)time.TotalMinutes}:{time.Seconds:D2}.{time.Milliseconds:D3}";
}
