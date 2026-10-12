using Microsoft.AspNetCore.Mvc;
using ClientService.Models;
using ClientService.Services;

namespace ClientService.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientMongoService _service;

        public ClientsController(ClientMongoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Client>>> Get()
        {
            return await _service.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            await _service.CreateAsync(client);

            return CreatedAtAction(
                nameof(Get),
                new { id = client.Id },
                client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            Client client)
        {
            await _service.UpdateAsync(id, client);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);

            return NoContent();
        }
    }
}