using apbd_cw12.Data;
using apbd_cw12.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd_cw12.Services;

public class TripService : ITripService
{
    private readonly S31249Context _context;

    public TripService(S31249Context context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Trip>, int)> GetTripsPagedAsync(int page, int pageSize)
    {
        var totalCount = await _context.Trips.CountAsync();
        var trips = await _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (trips, totalCount);
    }

    public async Task<Trip?> GetTripByIdAsync(int idTrip)
    {
        return await _context.Trips
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
            .FirstOrDefaultAsync(t => t.IdTrip == idTrip);
    }

    public async Task<bool> IsTripInFutureAsync(int idTrip)
    {
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null) return false;
        return trip.DateFrom > DateTime.UtcNow;
    }
}