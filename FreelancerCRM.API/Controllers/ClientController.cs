using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientResponseDto>>> GetAll()
        {
            var result = await _clientService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetById(int id)
        {
            var result = await _clientService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ClientResponseDto>> Create(ClientCreateDto createDto)
        {
            var result = await _clientService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return StatusCode(500, "Created client data is null");

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClientResponseDto>> Update(int id, ClientUpdateDto updateDto)
        {
            var result = await _clientService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _clientService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }
    }
} 