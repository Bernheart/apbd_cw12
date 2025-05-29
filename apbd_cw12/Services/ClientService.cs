using apbd_cw12.Data;
using apbd_cw12.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd_cw12.Services;

public class ClientService : IClientService
{
    private readonly S31249Context _context;

    public ClientService(S31249Context context)
    {
        _context = context;
    }

    public async Task<Client?> GetClientByPeselAsync(string pesel)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == pesel);
    }

    public async Task<bool> HasClientTripsAsync(int clientId)
    {
        return await _context.ClientTrips.AnyAsync(ct => ct.IdClient == clientId);
    }

    public async Task<bool> IsClientRegisteredForTripAsync(string pesel, int idTrip)
    {
        return await _context.ClientTrips
            .Include(ct => ct.IdClientNavigation)
            .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClientNavigation.Pesel == pesel);
    }

    public async Task AddClientWithTripAsync(Client client, Trip trip, DateTime registeredAt, DateTime? paymentDate)
    {
        _context.Clients.Add(client);
        _context.ClientTrips.Add(new ClientTrip
        {
            IdClientNavigation = client,
            IdTripNavigation = trip,
            RegisteredAt = registeredAt,
            PaymentDate = paymentDate
        });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(Client client)
    {
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Client?> GetClientByIdAsync(int idClient)
    {
        return await _context.Clients.FindAsync(idClient);
    }
}