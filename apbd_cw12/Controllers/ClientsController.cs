using apbd_cw12.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_cw12.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var hasTrips = await _clientService.HasClientTripsAsync(idClient);
        if (hasTrips)
            return BadRequest("Client cannot be deleted because they are assigned to at least one trip.");

        var client = await _clientService.GetClientByIdAsync(idClient);
        if (client == null)
            return NotFound("Client not found.");

        await _clientService.DeleteClientAsync(client);
        return NoContent();
    }
}