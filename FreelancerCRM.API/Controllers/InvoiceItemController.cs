using FreelancerCRM.API.DTOs;
using FreelancerCRM.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreelancerCRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IInvoiceItemService _invoiceItemService;

        public InvoiceItemController(IInvoiceItemService invoiceItemService)
        {
            _invoiceItemService = invoiceItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceItemResponseDto>>> GetAll()
        {
            var result = await _invoiceItemService.GetAllAsync();
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceItemResponseDto>> GetById(int id)
        {
            var result = await _invoiceItemService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceItemResponseDto>> Create(InvoiceItemCreateDto createDto)
        {
            var result = await _invoiceItemService.CreateAsync(createDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceItemResponseDto>> Update(int id, InvoiceItemUpdateDto updateDto)
        {
            var result = await _invoiceItemService.UpdateAsync(id, updateDto);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (result.Data == null)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _invoiceItemService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            if (!result.Data)
                return NotFound();

            return NoContent();
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<InvoiceItemResponseDto>>> GetByInvoiceId(int invoiceId)
        {
            var result = await _invoiceItemService.GetItemsByInvoiceIdAsync(invoiceId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("invoice/{invoiceId}")]
        public async Task<ActionResult<InvoiceItemResponseDto>> AddItemToInvoice(int invoiceId, [FromBody] AddInvoiceItemDto dto)
        {
            var result = await _invoiceItemService.AddItemToInvoiceAsync(invoiceId, dto.Description, dto.Quantity, dto.UnitPrice);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpDelete("invoice/item/{invoiceItemId}")]
        public async Task<ActionResult> RemoveItemFromInvoice(int invoiceItemId)
        {
            var result = await _invoiceItemService.RemoveItemFromInvoiceAsync(invoiceItemId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/quantity")]
        public async Task<ActionResult> UpdateItemQuantity(int id, [FromBody] decimal quantity)
        {
            var result = await _invoiceItemService.UpdateItemQuantityAsync(id, quantity);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpPut("{id}/price")]
        public async Task<ActionResult> UpdateItemPrice(int id, [FromBody] decimal unitPrice)
        {
            var result = await _invoiceItemService.UpdateItemPriceAsync(id, unitPrice);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }

        [HttpGet("{id}/total")]
        public async Task<ActionResult<decimal>> CalculateItemTotal(int id)
        {
            var result = await _invoiceItemService.CalculateItemTotalAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("invoice/{invoiceId}/total")]
        public async Task<ActionResult<decimal>> GetInvoiceTotal(int invoiceId)
        {
            var result = await _invoiceItemService.GetInvoiceTotalAsync(invoiceId);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpPost("invoice/{invoiceId}/bulk")]
        public async Task<ActionResult> BulkAddItems(int invoiceId, [FromBody] List<AddInvoiceItemDto> items)
        {
            var itemsList = items.Select(item => new { 
                Description = item.Description, 
                Quantity = item.Quantity, 
                UnitPrice = item.UnitPrice 
            }).ToList();

            foreach (var item in itemsList)
            {
                var result = await _invoiceItemService.AddItemToInvoiceAsync(invoiceId, item.Description, item.Quantity, item.UnitPrice);
                if (!result.IsSuccess)
                    return BadRequest(result.ErrorMessage);
            }

            return Ok();
        }

        [HttpDelete("bulk")]
        public async Task<ActionResult> BulkRemoveItems([FromBody] List<int> invoiceItemIds)
        {
            var result = await _invoiceItemService.BulkRemoveItemsAsync(invoiceItemIds);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok();
        }
    }

    public class AddInvoiceItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
} 