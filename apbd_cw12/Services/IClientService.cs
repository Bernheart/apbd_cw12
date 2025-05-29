using apbd_cw12.Models;

namespace apbd_cw12.Services;

public interface IClientService
{
    Task<Client?> GetClientByPeselAsync(string pesel);
    Task<bool> HasClientTripsAsync(int clientId);
    Task AddClientWithTripAsync(Client client, Trip trip, DateTime registeredAt, DateTime? paymentDate);
    Task<bool> IsClientRegisteredForTripAsync(string pesel, int idTrip);
    Task DeleteClientAsync(Client client);
    Task<Client?> GetClientByIdAsync(int idClient);
}