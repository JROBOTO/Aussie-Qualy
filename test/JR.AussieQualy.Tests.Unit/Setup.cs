using JR.AussieQualy.Domain.Models;
using JR.AussieQualy.Infrastructure.LapTimes;
using Moq;

namespace JR.AussieQualy.Tests.Unit;

public static class Setup
{
    public static Mock<ILapTimesRepository> SetupRepository()
    {
        var driver1 = new Driver()
        {
            Code = "ABC",
            Name = "Abracadabra",
            Number = 4,
            Team = "Power Rangers"
        };

        var driver2 = new Driver()
        {
            Code = "DEF",
            Name = "Definitely a real driver",
            Number = 99,
            Team = "Thunderbirds"
        };

        var repo = new Mock<ILapTimesRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetAllDrivers())
            .Returns(
            [
                driver1,
                driver2
            ]);

        repo.Setup(x => x.GetLapsForDriver(driver1.Code))
            .Returns([
                new()
                {
                    Driver = driver1,
                    Segment = QualifyingSegment.Q1,
                    LapNumber = 1,
                    LapTime = 90.123,
                    LapStartTime = TimeSpan.FromSeconds(10),
                    Tire = new TireInfo { Compound = "Soft", Life = 3 },
                    Conditions = new Conditions
                    {
                        AirTemperature = 25,
                        Humidity = 40,
                        Pressure = 1010,
                        Rainfall = 0,
                        TrackTemperature = 32,
                        WindDirectionBearing = 180,
                        WindSpeedInMs = 3
                    },
                    SegmentPositionAtLapEnd = 1,
                    SectorTimes = new SectorTimes { S1 = 30.0, S2 = 30.0, S3 = 30.123 },
                    Sector1Mini = new MiniSectorStatus { MiniSectors = new[] { "green" } },
                    Sector2Mini = new MiniSectorStatus { MiniSectors = new[] { "yellow" } },
                    Sector3Mini = new MiniSectorStatus { MiniSectors = new[] { "purple" } }
                }
            ]);

        repo.Setup(x => x.GetLapsForDriver(driver2.Code))
            .Returns([
                new()
                {
                    Driver = driver2,
                    Segment = QualifyingSegment.Q1,
                    LapNumber = 1,
                    LapTime = 90.423,
                    LapStartTime = TimeSpan.FromSeconds(10),
                    Tire = new TireInfo { Compound = "Soft", Life = 1 },
                    Conditions = new Conditions
                    {
                        AirTemperature = 25,
                        Humidity = 40,
                        Pressure = 1010,
                        Rainfall = 0,
                        TrackTemperature = 32,
                        WindDirectionBearing = 180,
                        WindSpeedInMs = 3
                    },
                    SegmentPositionAtLapEnd = 1,
                    SectorTimes = new SectorTimes { S1 = 30.0, S2 = 30.0, S3 = 30.123 },
                    Sector1Mini = new MiniSectorStatus { MiniSectors = new[] { "green" } },
                    Sector2Mini = new MiniSectorStatus { MiniSectors = new[] { "yellow" } },
                    Sector3Mini = new MiniSectorStatus { MiniSectors = new[] { "green" } }
                }
            ]);
        return repo;
    }

}
