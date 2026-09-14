namespace JR.AussieQualy.Tests.Unit;

using JR.AussieQualy.Application.Dtos;
using JR.AussieQualy.Application.Qualifying;
using JR.AussieQualy.Domain.Models;
using JR.AussieQualy.Infrastructure.LapTimes;
using Moq;
using NUnit.Framework;

public sealed class QualifyingServiceTests
{
    private Mock<ILapTimesRepository> _repo = null!;
    private QualifyingService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILapTimesRepository>(MockBehavior.Strict);
        _service = new QualifyingService(_repo.Object);
    }

    [Test]
    public void GetDriverQualifyingResult_Throws_WhenDriverCodeIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _service.GetDriverQualifyingResult("");
        });
    }

    [Test]
    public void GetDriverQualifyingResult_Throws_WhenDriverNotFound()
    {
        _repo.Setup(r => r.GetAllDrivers()).Returns(Array.Empty<Driver>());

        Assert.Throws<InvalidOperationException>(() =>
        {
            _service.GetDriverQualifyingResult("HAM");
        });
    }

    [Test]
    public void GetDriverQualifyingResult_ReturnsResult_WhenDriverExists()
    {
        var driver = new Driver
        {
            Code = "HAM",
            Name = "Lewis Hamilton",
            Number = 44,
            Team = "Mercedes"
        };

        var lap = new Lap
        {
            Driver = driver,
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
        };

        _repo.Setup(r => r.GetAllDrivers()).Returns(new[] { driver });
        _repo.Setup(r => r.GetLapsForDriver("HAM")).Returns(new[] { lap });

        DriverQualifyingResultDto result = _service.GetDriverQualifyingResult("HAM");

        Assert.That(result.Driver.Name, Is.EqualTo("Lewis Hamilton"));
        Assert.That(result.Q1, Is.Not.Null);
        Assert.That(result.Q2, Is.Null);
        Assert.That(result.Q3, Is.Null);
    }
}
