using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Models;
using OrderManagementApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        // Constructor to inject OrderService
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/order - Create a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid order data.", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            // Ensure the order has items
            if (order.OrderItems == null || !order.OrderItems.Any())
            {
                return BadRequest(new { Message = "Order must contain at least one item." });
            }

            try
            {
                // Call service to add the order
                var createdOrder = await _orderService.AddOrderAsync(order);

                if (createdOrder == null)
                {
                    return BadRequest(new { Message = "Unable to create order. One or more items have insufficient stock." });
                }

                // Return the created order with its ID
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the order.", Details = ex.Message });
            }
        }


        // PUT: api/order/{orderId}
        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrderAsync(int orderId, [FromBody] Order updatedOrder)
        {
            try
            {
                if (updatedOrder == null || !updatedOrder.OrderItems.Any())
                {
                    return BadRequest(new { Message = "Invalid order data." });
                }

                var updated = await _orderService.UpdateOrderAsync(orderId, updatedOrder);
                if (updated == null)
                {
                    return NotFound(new { Message = "Order not found." });
                }

                return Ok(updated);  // Return the updated order
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        // DELETE: api/order/{orderId} - Delete an existing order
        [HttpDelete("{orderId}")]
public async Task<IActionResult> DeleteOrder(int orderId)
{
    try
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound(new { Message = "Order not found." });
        }

        var isDeleted = await _orderService.DeleteOrderAsync(orderId);

        if (!isDeleted)
        {
            return NotFound(new { Message = "Order not found or already deleted." });
        }

        // Return a message confirming the order deletion
        return Ok(new { Message = "Order deleted successfully.", OrderId = orderId });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = ex.Message });
    }
}

        // GET: api/order/{id} - Get an order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    return NotFound(new { Message = "Order not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/order - Get all orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                var response = orders.Select(order => new
                {
                    order.OrderId,
                    order.OrderDate,
                    order.TotalAmount,
                    OrderItems = order.OrderItems.Select(item => new
                    {
                        item.OrderItemId,
                        item.ProductId,
                        item.Quantity,
                        item.Price
                    })
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
