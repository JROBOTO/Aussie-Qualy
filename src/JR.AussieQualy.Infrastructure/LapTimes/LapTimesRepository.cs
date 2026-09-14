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

        foreach (string code in model.driverCode.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            int index = model.driverCode.FindIndex(d => string.Equals(d, code, StringComparison.OrdinalIgnoreCase));

            if (index < 0)
            {
                continue;
            }

            var driver = new Driver
            {
                Code = code,
                Name = model.driverName[index],
                Number = model.driverNumber[index],
                Team = model.team[index]
            };

            drivers.Add(driver);
        }

        return drivers;
    }

    private static IReadOnlyCollection<Lap> BuildLaps(LapTimesJsonModel model, IReadOnlyCollection<Driver> drivers)
    {
        var laps = new List<Lap>(model.time.Count);

        for (int i = 0; i < model.time.Count; i++)
        {
            var driverCode = model.driverCode[i];

            var driver = drivers.First(d =>
                string.Equals(d.Code, driverCode, StringComparison.OrdinalIgnoreCase));

            QualifyingSegment segment = ParseSegment(model.qSegment[i]);

            var tire = new TireInfo
            {
                Compound = model.compound[i],
                Life = model.stint[i]
            };

            var conditions = new Conditions
            {
                AirTemperature = model.airTemp[i],
                Humidity = model.humidity[i],
                Pressure = model.pressure[i],
                Rainfall = model.rainfall[i],
                TrackTemperature = model.trackTemp[i],
                WindDirectionBearing = model.windDir[i],
                WindSpeedInMs = model.windSpeed[i]
            };

            var sectorTimes = new SectorTimes
            {
                S1 = model.s1[i],
                S2 = model.s2[i],
                S3 = model.s3[i]
            };

            var lap = new Lap
            {
                Driver = driver,
                Segment = segment,
                LapNumber = model.lap[i],
                LapTime = model.time[i],
                LapStartTime = TimeSpan.FromSeconds(model.lapStartTime[i]),
                Tire = tire,
                Conditions = conditions,
                SegmentPositionAtLapEnd = model.pos[i],
                SectorTimes = sectorTimes,
                Sector1Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.ms1[i]) },
                Sector2Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.ms2[i]) },
                Sector3Mini = new MiniSectorStatus { MiniSectors = DecodeMiniSectors(model.ms3[i]) }
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
