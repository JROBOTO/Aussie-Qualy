namespace JR.AussieQualy.Infrastructure.LapTimes;

using JR.AussieQualy.Domain.Models;

public sealed class LapTimesRepository : ILapTimesRepository
{
    private readonly IReadOnlyCollection<Driver> _drivers;
    private readonly IReadOnlyCollection<Lap> _laps;

    public LapTimesRepository(string jsonPath)
    {
        LapTimesJsonModel model = LapTimesJsonLoader.Load(jsonPath);

        _drivers = BuildDrivers(model);
        _laps = BuildLaps(model, _drivers);
    }

    public IReadOnlyCollection<Driver> GetAllDrivers()
    {
        return _drivers;
    }

    public IReadOnlyCollection<Lap> GetLapsForDriver(string driverCode)
    {
        string normalized = driverCode.ToUpperInvariant();

        return _laps
            .Where(l => string.Equals(l.Driver.Code, normalized, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static IReadOnlyCollection<Driver> BuildDrivers(LapTimesJsonModel model)
    {
        var drivers = new List<Driver>();

        foreach (string code in model.DriverCode.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                continue;
            }

            int index = model.DriverCode.FindIndex(d => string.Equals(d, code, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                continue;
            }

            var driver = new Driver
            {
                Code = code,
                Name = code,
                Number = model.DriverNumber[index] ?? 0,
                Team = model.Team[index]
            };

            drivers.Add(driver);
        }

        return drivers;
    }

    private static IReadOnlyCollection<Lap> BuildLaps(LapTimesJsonModel model, IReadOnlyCollection<Driver> drivers)
    {
        var laps = new List<Lap>(model.Time.Count);

        for (int i = 0; i < model.Time.Count; i++)
        {
            string driverCode = model.DriverCode[i];

            if (string.IsNullOrWhiteSpace(driverCode))
            {
                continue;
            }

            var driver = drivers.First(d =>
                string.Equals(d.Code, driverCode, StringComparison.OrdinalIgnoreCase));

            QualifyingSegment segment = ParseSegment(model.QSegment[i]);

            var tire = new TireInfo
            {
                Compound = model.Compound[i],
                Life = model.Life[i]
            };

            var conditions = new Conditions
            {
                AirTemperature = model.AirTemp[i],
                Humidity = model.Humidity[i],
                Pressure = model.Pressure[i],
                Rainfall = model.Rainfall[i],
                TrackTemperature = model.TrackTemp[i],
                WindDirectionBearing = model.WindDir[i],
                WindSpeedInMs = model.WindSpeed[i]
            };

            var sectorTimes = new SectorTimes
            {
                S1 = model.S1[i],
                S2 = model.S2[i],
                S3 = model.S3[i]
            };

            var lap = new Lap
            {
                Driver = driver,
                Segment = segment,
                LapNumber = model.Lap[i],
                LapTime = model.Time[i],
                LapStartTime = TimeSpan.FromSeconds(model.LapStartTime[i]),
                Tire = tire,
                Conditions = conditions,
                SegmentPositionAtLapEnd = model.Pos[i],
                SectorTimes = sectorTimes,
                Sector1Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.Ms1[i]) },
                Sector2Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.Ms2[i]) },
                Sector3Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.Ms3[i]) }
            };

            laps.Add(lap);
        }

        return laps;
    }

    private static QualifyingSegment ParseSegment(string segment)
    {
        if (segment == "Q1")
        {
            return QualifyingSegment.Q1;
        }

        if (segment == "Q2")
        {
            return QualifyingSegment.Q2;
        }

        if (segment == "Q3")
        {
            return QualifyingSegment.Q3;
        }

        throw new InvalidOperationException($"Unknown qualifying segment '{segment}'.");
    }

    private static IReadOnlyList<string> DecodeMiniSectors(string encoded)
    {
        var result = new List<string>(encoded.Length);

        foreach (char c in encoded)
        {
            string value = DecodeMiniSectorChar(c);
            result.Add(value);
        }

        return result;
    }

    private static string DecodeMiniSectorChar(char c)
    {
        if (c == '0')
        {
            return "yellow";
        }

        if (c == '1')
        {
            return "green";
        }

        if (c == '2')
        {
            return "purple";
        }

        return string.Empty;
    }
}
