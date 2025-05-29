using apbd_cw12.DTOs;
using apbd_cw12.Models;
using apbd_cw12.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_cw12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ITripService _tripService;
    private const int DefaultPageSize = 10;

    public TripsController(IClientService clientService, ITripService tripService)
    {
        _clientService = clientService;
        _tripService = tripService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips(int page = 1, int pageSize = DefaultPageSize)
    {
        var (trips, totalTrips) = await _tripService.GetTripsPagedAsync(page, pageSize);
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);

        var result = new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips.Select(t => new
            {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.IdCountries.Select(c => new { c.Name }),
                Clients = t.ClientTrips.Select(ct => new
                {
                    ct.IdClientNavigation.FirstName,
                    ct.IdClientNavigation.LastName
                })
            })
        };

        return Ok(result);
    }
    
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientCreateDto dto)
    {
        var existingClient = await _clientService.GetClientByPeselAsync(dto.Pesel);
        if (existingClient != null)
            return BadRequest("Client with this PESEL already exists.");

        var trip = await _tripService.GetTripByIdAsync(idTrip);
        if (trip == null)
            return NotFound("Trip not found.");
        if (trip.DateFrom <= DateTime.UtcNow)
            return BadRequest("Cannot register for a trip that has already started.");

        var alreadyRegistered = await _clientService.IsClientRegisteredForTripAsync(dto.Pesel, idTrip);
        if (alreadyRegistered)
            return BadRequest("Client is already registered for this trip.");

        var client = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        await _clientService.AddClientWithTripAsync(client, trip, DateTime.UtcNow, dto.PaymentDate);

        return Ok(new { Message = "Client successfully assigned to the trip." });
    }
}