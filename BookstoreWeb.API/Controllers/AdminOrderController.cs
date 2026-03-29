using BookstoreWeb.Application.DTOs.Orders;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace BookstoreWeb.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
public class AdminOrderController : ControllerBase
{
    private readonly IAdminOrderService _adminOrderService;

    public AdminOrderController(IAdminOrderService adminOrderService)
    {
        _adminOrderService = adminOrderService;
    }

    //1-lấy all orders, newest 1st
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _adminOrderService.GetAllAsync();
        return Ok(orders);
    }

    //2-get order by id
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _adminOrderService.GetByIdAsync(id);
        return Ok(order);
    }

    //3-update status of 1 order
    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        await _adminOrderService.UpdateStatusAsync(id, request);
        return NoContent();
    }

    //4- get revenue=timeFrame
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(IEnumerable<RevenueDataResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRevenue([FromQuery] string timeFrame)
    {
        var data = await _adminOrderService.GetRevenueDatasAsync(timeFrame);
        return Ok(data);
    }
}
