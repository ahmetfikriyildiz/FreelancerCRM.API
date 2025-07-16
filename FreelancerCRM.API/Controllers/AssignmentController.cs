using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetAll()
        {
            var result = await _assignmentService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentResponseDto>> GetById(int id)
        {
            var result = await _assignmentService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentResponseDto>> Create(AssignmentCreateDto createDto)
        {
            var result = await _assignmentService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AssignmentResponseDto>> Update(int id, AssignmentUpdateDto updateDto)
        {
            var result = await _assignmentService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _assignmentService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetByProjectId(int projectId)
        {
            var result = await _assignmentService.GetAssignmentsByProjectIdAsync(projectId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetByStatus(string status)
        {
            var result = await _assignmentService.GetAssignmentsByStatusAsync(status);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetOverdueAssignments()
        {
            var result = await _assignmentService.GetOverdueAssignmentsAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("assignedto/{assignedTo}")]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetByAssignedTo(string assignedTo)
        {
            var result = await _assignmentService.GetAssignmentsByAssignedToAsync(assignedTo);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}/timeentries")]
        public async Task<ActionResult<AssignmentResponseDto>> GetWithTimeEntries(int id)
        {
            var result = await _assignmentService.GetAssignmentWithTimeEntriesAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> StartAssignment(int id)
        {
            var result = await _assignmentService.StartAssignmentAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> CompleteAssignment(int id)
        {
            var result = await _assignmentService.CompleteAssignmentAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/assign/{assignedTo}")]
        public async Task<ActionResult> AssignToUser(int id, string assignedTo)
        {
            var result = await _assignmentService.AssignToUserAsync(id, assignedTo);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/progress")]
        public async Task<ActionResult> UpdateProgress(int id, [FromBody] decimal actualHours)
        {
            var result = await _assignmentService.UpdateProgressAsync(id, actualHours);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/deadline")]
        public async Task<ActionResult> ExtendDeadline(int id, [FromBody] DateTime newDueDate)
        {
            var result = await _assignmentService.ExtendDeadlineAsync(id, newDueDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("{id}/progress")]
        public async Task<ActionResult<decimal>> GetProgress(int id)
        {
            var result = await _assignmentService.GetAssignmentProgressAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<IEnumerable<AssignmentResponseDto>>> GetByPriority(string priority)
        {
            var result = await _assignmentService.GetAssignmentsByPriorityAsync(priority);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPut("{id}/priority")]
        public async Task<ActionResult> SetPriority(int id, [FromBody] string priority)
        {
            var result = await _assignmentService.SetPriorityAsync(id, priority);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }
} 