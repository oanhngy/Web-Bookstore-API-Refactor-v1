using BookstoreWeb.Application.DTOs.Cart;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreWeb.API.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    public CartController(ICartService cartService)
    {
        _cartService=cartService;
    } 

    //1-lấy cart hiện tại của user
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart([FromQuery] string userId)
    {
        //userId tạm nhận từ query param
        //phase 5 thay=jwt claim
        var cart=await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    //2-add sp
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToCart (
        [FromQuery] string userId,
        [FromBody] AddToCartRequest request)
    {
        //Service tự tạo cart mới nếu user chưa có
        await _cartService.AddToCartAsync(userId, request);
        return NoContent();
    }

    //3-delete 1 item khỏi cart
    [HttpDelete("items/{orderDetailId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromCart (
        [FromQuery] string userId,
        int orderDetailId)
    {
        //service throw
        await _cartService.RemoveFromCartAsync(userId, orderDetailId);
        return NoContent();
    }

    //4-update quantity
    [HttpPut("items/{orderDetailId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemQuantity(
        [FromQuery] string userId,
        int orderDetailId,
        [FromQuery] int quantity)
    {
        await _cartService.UpdateItemQuantityAsync(userId, orderDetailId, quantity);
        return NoContent();
    }

    //5-checkout
    [HttpPost("checkout")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromQuery] string userId)
    {
        //Service throw nếu rỗng/status k hợp lệ
        var result = await _cartService.CheckoutCart(userId);
        return Ok(result);
    }
}