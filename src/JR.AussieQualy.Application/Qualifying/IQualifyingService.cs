namespace JR.AussieQualy.Application.Qualifying;

using JR.AussieQualy.Application.Dtos;

public interface IQualifyingService
{
    IReadOnlyCollection<string> GetAllDriverCodes();

    DriverQualifyingResultDto? GetDriverQualifyingResult(string driverCode);
}
