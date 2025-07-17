using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntryController : ControllerBase
    {
        private readonly ITimeEntryService _timeEntryService;

        public TimeEntryController(ITimeEntryService timeEntryService)
        {
            _timeEntryService = timeEntryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeEntryResponseDto>>> GetAll()
        {
            var result = await _timeEntryService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TimeEntryResponseDto>> GetById(int id)
        {
            var result = await _timeEntryService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<TimeEntryResponseDto>> Create(TimeEntryCreateDto createDto)
        {
            var result = await _timeEntryService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);
            
            if (result.Data == null)
                return StatusCode(500, "Created client data is null");

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TimeEntryResponseDto>> Update(int id, TimeEntryUpdateDto updateDto)
        {
            var result = await _timeEntryService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _timeEntryService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TimeEntryResponseDto>>> GetByUserId(int userId)
        {
            var result = await _timeEntryService.GetTimeEntriesByUserIdAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TimeEntryResponseDto>>> GetByProjectId(int projectId)
        {
            var result = await _timeEntryService.GetTimeEntriesByProjectIdAsync(projectId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<TimeEntryResponseDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _timeEntryService.GetTimeEntriesByDateRangeAsync(startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("start")]
        public async Task<ActionResult<TimeEntryResponseDto>> StartTimeTracking([FromBody] StartTimeTrackingDto dto)
        {
            var result = await _timeEntryService.StartTimeTrackingAsync(dto.UserId, dto.ProjectId, dto.AssignmentId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("stop/{userId}")]
        public async Task<ActionResult<TimeEntryResponseDto>> StopTimeTracking(int userId)
        {
            var result = await _timeEntryService.StopTimeTrackingAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("active/{userId}")]
        public async Task<ActionResult<TimeEntryResponseDto>> GetActiveTimeEntry(int userId)
        {
            var result = await _timeEntryService.GetActiveTimeEntryAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPut("{id}/details")]
        public async Task<ActionResult> UpdateTimeEntry(int id, [FromBody] UpdateTimeEntryDetailsDto dto)
        {
            var result = await _timeEntryService.UpdateTimeEntryAsync(id, dto.Description, dto.IsBillable);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("totalhours/user/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalHoursByUser(int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _timeEntryService.GetTotalHoursByUserAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("totalhours/project/{projectId}")]
        public async Task<ActionResult<decimal>> GetTotalHoursByProject(int projectId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _timeEntryService.GetTotalHoursByProjectAsync(projectId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("billablehours/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalBillableHours(int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _timeEntryService.GetTotalBillableHoursAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("earnings/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalEarnings(int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _timeEntryService.GetTotalEarningsAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("billable/{userId}")]
        public async Task<ActionResult<IEnumerable<TimeEntryResponseDto>>> GetBillableTimeEntries(int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _timeEntryService.GetBillableTimeEntriesAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("{id}/calculate")]
        public async Task<ActionResult> CalculateAmount(int id)
        {
            var result = await _timeEntryService.CalculateAmountAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("bulk/billable")]
        public async Task<ActionResult> BulkUpdateBillableStatus([FromBody] BulkUpdateBillableStatusDto dto)
        {
            var result = await _timeEntryService.BulkUpdateBillableStatusAsync(dto.TimeEntryIds, dto.IsBillable);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }

    public class StartTimeTrackingDto
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int? AssignmentId { get; set; }
    }

    public class UpdateTimeEntryDetailsDto
    {
        public string Description { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
    }

    public class BulkUpdateBillableStatusDto
    {
        public List<int> TimeEntryIds { get; set; } = new();
        public bool IsBillable { get; set; }
    }
} 