using BookstoreWeb.Application.DTOs.Orders;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace BookstoreWeb.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    //1-lấy all orders của 1 user
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser([FromQuery] string userId)
    {
        var orders = await _orderService.GetByUserIdAsync(userId);
        return Ok(orders);
    }

    //2-lấy order theo id
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order);
    }

    //3-xác nhận order, điền thông tin giao
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Confirm(
        int id,
        [FromBody] ConfirmOrderRequest request,
        [FromQuery] string userId)
    {
        var order = await _orderService.ConfirmAsync(id, request, userId);
        return Ok(order);
    }

    //4-cancel order
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, [FromQuery] string userId)
    {
        await _orderService.CancelAsync(id, userId);
        return NoContent();
    }
}
