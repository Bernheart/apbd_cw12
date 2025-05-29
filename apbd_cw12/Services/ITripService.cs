using apbd_cw12.Models;

namespace apbd_cw12.Services;

public interface ITripService
{
    Task<(IEnumerable<Trip> trips, int totalCount)> GetTripsPagedAsync(int page, int pageSize);
    Task<Trip?> GetTripByIdAsync(int idTrip);
}