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
        _repo = Setup.SetupRepository();
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

        var result = _service.GetDriverQualifyingResult("ZZZ");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDriverQualifyingResult_ReturnsResult_WhenDriverExists()
    {
        DriverQualifyingResultDto result = _service.GetDriverQualifyingResult("ABC");

        Assert.That(result.Driver.Name, Is.EqualTo("Abracadabra"));
        Assert.That(result.Q1, Is.Not.Null);
        Assert.That(result.Q2, Is.Null);
        Assert.That(result.Q3, Is.Null);
    }
}
