namespace JR.AussieQualy.Application.Qualifying;

using JR.AussieQualy.Application.Dtos;

public interface IQualifyingService
{
    IReadOnlyCollection<string> GetAllDriverCodes();

    IReadOnlyCollection<DriverDto> GetAllDrivers();

    DriverQualifyingResultDto? GetDriverQualifyingResult(string driverCode);
}
