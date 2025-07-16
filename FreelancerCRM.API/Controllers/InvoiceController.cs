using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetAll()
        {
            var result = await _invoiceService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceResponseDto>> GetById(int id)
        {
            var result = await _invoiceService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceResponseDto>> Create(InvoiceCreateDto createDto)
        {
            var result = await _invoiceService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceResponseDto>> Update(int id, InvoiceUpdateDto updateDto)
        {
            var result = await _invoiceService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _invoiceService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByUserId(int userId)
        {
            var result = await _invoiceService.GetInvoicesByUserIdAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByClientId(int clientId)
        {
            var result = await _invoiceService.GetInvoicesByClientIdAsync(clientId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByStatus(string status)
        {
            var result = await _invoiceService.GetInvoicesByStatusAsync(status);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetOverdueInvoices()
        {
            var result = await _invoiceService.GetOverdueInvoicesAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<InvoiceResponseDto>> GetWithItems(int id)
        {
            var result = await _invoiceService.GetInvoiceWithItemsAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost("from-timeentries")]
        public async Task<ActionResult<InvoiceResponseDto>> CreateFromTimeEntries([FromBody] CreateInvoiceFromTimeEntriesDto dto)
        {
            var result = await _invoiceService.CreateInvoiceFromTimeEntriesAsync(dto.UserId, dto.ClientId, dto.ProjectId, dto.TimeEntryIds);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("from-project/{projectId}")]
        public async Task<ActionResult<InvoiceResponseDto>> CreateFromProject(int projectId)
        {
            var result = await _invoiceService.CreateInvoiceFromProjectAsync(projectId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("{id}/send")]
        public async Task<ActionResult> SendInvoice(int id)
        {
            var result = await _invoiceService.SendInvoiceAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/mark-paid")]
        public async Task<ActionResult> MarkAsPaid(int id, [FromBody] DateTime paidDate)
        {
            var result = await _invoiceService.MarkAsPaidAsync(id, paidDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> CancelInvoice(int id)
        {
            var result = await _invoiceService.CancelInvoiceAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("generate-number")]
        public async Task<ActionResult<string>> GenerateInvoiceNumber()
        {
            var result = await _invoiceService.GenerateInvoiceNumberAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}/total")]
        public async Task<ActionResult<decimal>> CalculateTotal(int id)
        {
            var result = await _invoiceService.CalculateInvoiceTotalAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("{id}/discount")]
        public async Task<ActionResult> ApplyDiscount(int id, [FromBody] decimal discountAmount)
        {
            var result = await _invoiceService.ApplyDiscountAsync(id, discountAmount);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/payment-terms")]
        public async Task<ActionResult> UpdatePaymentTerms(int id, [FromBody] string paymentTerms)
        {
            var result = await _invoiceService.UpdatePaymentTermsAsync(id, paymentTerms);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("outstanding/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalOutstandingAmount(int userId)
        {
            var result = await _invoiceService.GetTotalOutstandingAmountAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("paid/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalPaidAmount(int userId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _invoiceService.GetTotalPaidAmountAsync(userId, startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("daterange")]
        public async Task<ActionResult<IEnumerable<InvoiceResponseDto>>> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }

    public class CreateInvoiceFromTimeEntriesDto
    {
        public int UserId { get; set; }
        public int ClientId { get; set; }
        public int? ProjectId { get; set; }
        public List<int> TimeEntryIds { get; set; } = new();
    }
} 