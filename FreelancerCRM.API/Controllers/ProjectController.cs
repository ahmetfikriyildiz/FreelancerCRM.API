using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
        {
            var result = await _projectService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> GetById(int id)
        {
            var result = await _projectService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ProjectResponseDto>> Create(ProjectCreateDto createDto)
        {
            var result = await _projectService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return StatusCode(500, "Created project data is null");

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectResponseDto>> Update(int id, ProjectUpdateDto updateDto)
        {
            var result = await _projectService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _projectService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetByClientId(int clientId)
        {
            var result = await _projectService.GetProjectsByClientIdAsync(clientId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetActiveProjects()
        {
            var result = await _projectService.GetActiveProjectsAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetCompletedProjects()
        {
            var result = await _projectService.GetCompletedProjectsAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetOverdueProjects()
        {
            var result = await _projectService.GetOverdueProjectsAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("{id}/relations")]
        public async Task<ActionResult<ProjectResponseDto>> GetWithAllRelations(int id)
        {
            var result = await _projectService.GetProjectWithAllRelationsAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> StartProject(int id)
        {
            var result = await _projectService.StartProjectAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteProject(int id)
        {
            var result = await _projectService.CompleteProjectAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/pause")]
        public async Task<ActionResult> PauseProject(int id)
        {
            var result = await _projectService.PauseProjectAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelProject(int id)
        {
            var result = await _projectService.CancelProjectAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("{id}/profitability")]
        public async Task<ActionResult<decimal>> GetProjectProfitability(int id)
        {
            var result = await _projectService.CalculateProjectProfitabilityAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}/progress")]
        public async Task<ActionResult<decimal>> GetProjectProgress(int id)
        {
            var result = await _projectService.GetProjectProgressAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut("{id}/budget")]
        public async Task<ActionResult> UpdateBudget(int id, [FromBody] decimal newBudget)
        {
            var result = await _projectService.UpdateBudgetAsync(id, newBudget);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/deadline")]
        public async Task<ActionResult> ExtendDeadline(int id, [FromBody] DateTime newEndDate)
        {
            var result = await _projectService.ExtendDeadlineAsync(id, newEndDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetByStatus(string status)
        {
            var result = await _projectService.GetProjectsByStatusAsync(status);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _projectService.GetProjectsByDateRangeAsync(startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }
    }
} 