namespace JR.AussieQualy.Infrastructure.LapTimes;

using JR.AussieQualy.Domain.Models;

public interface ILapTimesRepository
{
    IReadOnlyCollection<Driver> GetAllDrivers();
    IReadOnlyCollection<Lap> GetLapsForDriver(string driverCode);
}
