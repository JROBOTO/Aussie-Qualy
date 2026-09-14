using JR.AussieQualy.Application.Dtos;
using JR.AussieQualy.Domain.Models;
using JR.AussieQualy.Infrastructure.LapTimes;

namespace JR.AussieQualy.Application.Qualifying;

public sealed class QualifyingService : IQualifyingService
{
    private readonly ILapTimesRepository _repository;
    private readonly IReadOnlyCollection<Lap> _allLaps;
    private readonly IReadOnlyCollection<Driver> _allDrivers;

    public QualifyingService(ILapTimesRepository repository)
    {
        _repository = repository;
        _allDrivers = repository.GetAllDrivers();
        _allLaps = _allDrivers
            .SelectMany(d => repository.GetLapsForDriver(d.Code))
            .ToArray();
    }

    public IReadOnlyCollection<string> GetAllDriverCodes() =>
        _allDrivers.Select(d => d.Code).OrderBy(c => c).ToArray();

    public DriverQualifyingResultDto? GetDriverQualifyingResult(string driverCode)
    {
        var driver = _allDrivers.FirstOrDefault(d =>
            string.Equals(d.Code, driverCode, StringComparison.OrdinalIgnoreCase));

        if (driver is null)
        {
            return null;
        }

        var driverLaps = _allLaps.Where(l => l.Driver == driver).ToArray();

        var q1Best = GetBestLapForSegment(driverLaps, QualifyingSegment.Q1);
        var q2Best = GetBestLapForSegment(driverLaps, QualifyingSegment.Q2);
        var q3Best = GetBestLapForSegment(driverLaps, QualifyingSegment.Q3);

        var finalPosition = ComputeFinalQualifyingPosition(driver);

        return new DriverQualifyingResultDto
        {
            Driver = new DriverDto
            {
                Name = driver.Name,
                DriverNumber = driver.Number,
                Team = driver.Team
            },
            FinalQualifyingPosition = finalPosition,
            Q1 = q1Best,
            Q2 = q2Best,
            Q3 = q3Best
        };
    }

    private LapDataDto? GetBestLapForSegment(IEnumerable<Lap> driverLaps, QualifyingSegment segment)
    {
        var lapsInSegment = driverLaps
            .Where(l => l.Segment == segment && l.LapTime.HasValue)
            .ToArray();

        if (!lapsInSegment.Any())
        {
            return null;
        }

        var bestLap = lapsInSegment.OrderBy(l => l.LapTime!.Value).First();

        // Sector status: compute session best & personal best for this segment
        var segmentLaps = _allLaps.Where(l => l.Segment == segment && l.LapTime.HasValue).ToArray();

        var bestS1 = segmentLaps.Where(l => l.SectorTimes.S1.HasValue)
                                .Min(l => l.SectorTimes.S1!.Value);
        var bestS2 = segmentLaps.Where(l => l.SectorTimes.S2.HasValue)
                                .Min(l => l.SectorTimes.S2!.Value);
        var bestS3 = segmentLaps.Where(l => l.SectorTimes.S3.HasValue)
                                .Min(l => l.SectorTimes.S3!.Value);

        var personalBestS1 = lapsInSegment.Where(l => l.SectorTimes.S1.HasValue)
                                          .Min(l => l.SectorTimes.S1!.Value);
        var personalBestS2 = lapsInSegment.Where(l => l.SectorTimes.S2.HasValue)
                                          .Min(l => l.SectorTimes.S2!.Value);
        var personalBestS3 = lapsInSegment.Where(l => l.SectorTimes.S3.HasValue)
                                          .Min(l => l.SectorTimes.S3!.Value);

        // Sector status is computed by GetSectorStatus

        return new LapDataDto
        {
            LapTime = bestLap.LapTime,
            LapStartTime = bestLap.LapStartTime,
            Tire = new TireDto
            {
                Compound = bestLap.Tire.Compound,
                Life = bestLap.Tire.Life
            },
            Conditions = new ConditionsDto
            {
                AirTemperature = bestLap.Conditions.AirTemperature,
                Humidity = bestLap.Conditions.Humidity,
                Pressure = bestLap.Conditions.Pressure,
                Rainfall = bestLap.Conditions.Rainfall,
                TrackTemperature = bestLap.Conditions.TrackTemperature,
                WindDirectionBearing = bestLap.Conditions.WindDirectionBearing,
                WindSpeedInMs = bestLap.Conditions.WindSpeedInMs
            },
            SegmentPosition = bestLap.SegmentPositionAtLapEnd,
                Sector1 = new SectorDto
            {
                SectorTime = bestLap.SectorTimes.S1,
                MiniSectors = bestLap.Sector1Mini.MiniSectors,
                    SectorStatus = GetSectorStatus(bestLap.SectorTimes.S1, bestS1, personalBestS1)
            },
            Sector2 = new SectorDto
            {
                SectorTime = bestLap.SectorTimes.S2,
                MiniSectors = bestLap.Sector2Mini.MiniSectors,
                    SectorStatus = GetSectorStatus(bestLap.SectorTimes.S2, bestS2, personalBestS2)
            },
            Sector3 = new SectorDto
            {
                SectorTime = bestLap.SectorTimes.S3,
                MiniSectors = bestLap.Sector3Mini.MiniSectors,
                    SectorStatus = GetSectorStatus(bestLap.SectorTimes.S3, bestS3, personalBestS3)
            }
        };
    }

    private string? GetSectorStatus(double? sectorTime, double sessionBest, double personalBest)
    {
        if (!sectorTime.HasValue)
        {
            return null;
        }

        if (Math.Abs(sectorTime.Value - sessionBest) < 1e-3)
        {
            return "purple";
        }

        if (Math.Abs(sectorTime.Value - personalBest) < 1e-3)
        {
            return "green";
        }

        return null;
    }

    private int ComputeFinalQualifyingPosition(Driver driver)
    {
        // Q1 classification
        var q1BestByDriver = _allDrivers
            .Select(d => new
            {
                Driver = d,
                BestLap = GetBestLapForSegment(
                    _allLaps.Where(l => l.Driver == d),
                    QualifyingSegment.Q1)
            })
            .Where(x => x.BestLap?.LapTime is not null)
            .OrderBy(x => x.BestLap!.LapTime!.Value)
            .ToList();

        // Top 16 go to Q2, bottom 6 finish here
        var q1Top16 = q1BestByDriver.Take(16).ToList();
        var q1Bottom = q1BestByDriver.Skip(16).ToList();

        // Q2 classification (only top 16 from Q1)
        var q2BestByDriver = q1Top16
            .Select(x => new
            {
                x.Driver,
                BestLap = GetBestLapForSegment(
                    _allLaps.Where(l => l.Driver == x.Driver),
                    QualifyingSegment.Q2)
            })
            .Where(x => x.BestLap?.LapTime is not null)
            .OrderBy(x => x.BestLap!.LapTime!.Value)
            .ToList();

        var q2Top10 = q2BestByDriver.Take(10).ToList();
        var q2Bottom = q2BestByDriver.Skip(10).ToList();

        // Q3 classification (top 10 from Q2)
        var q3BestByDriver = q2Top10
            .Select(x => new
            {
                x.Driver,
                BestLap = GetBestLapForSegment(
                    _allLaps.Where(l => l.Driver == x.Driver),
                    QualifyingSegment.Q3)
            })
            .Where(x => x.BestLap?.LapTime is not null)
            .OrderBy(x => x.BestLap!.LapTime!.Value)
            .ToList();

        // Build final grid:
        // Positions 1‑10: Q3 order
        // Positions 11‑16: Q2 bottom order
        // Positions 17‑22: Q1 bottom order (assuming 22 drivers)
        var finalGrid = new List<Driver>();

        finalGrid.AddRange(q3BestByDriver.Select(x => x.Driver));
        finalGrid.AddRange(q2Bottom.Select(x => x.Driver));
        finalGrid.AddRange(q1Bottom.Select(x => x.Driver));

        var index = finalGrid.FindIndex(d => d == driver);
        return index >= 0 ? index + 1 : 0; // 1‑based position, 0 if not classified
    }
}
