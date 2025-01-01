using OrderManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagementApi.Services
{
    public class OrderService : IOrderService
    {
        private static List<Order> _orders = new List<Order>(); // In-memory storage for orders
        private readonly ProductService _productService;
        private static int _nextOrderId = 1;  // Static counter for auto-incrementing OrderId
        private static int _nextItemOrderId = 1;  // Static counter for OrderItemId

        // Constructor to inject ProductService
        public OrderService(ProductService productService)
        {
            _productService = productService;
        }

        // Method to add a new order
        public async Task<Order> AddOrderAsync(Order order)
        {
            // Validate that each order item has a valid quantity and product
            foreach (var item in order.OrderItems)
            {
                // Validate quantity (must be greater than 0)
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                item.OrderItemId = _nextItemOrderId++;  // Auto-assign Item ID
                var product = _productService.GetProducts().FirstOrDefault(p => p.ProductId == item.ProductId);

                // Validate if product exists and if stock is sufficient
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {item.ProductId} does not exist.");
                }

                if (product.Quantity < item.Quantity)
                {
                    return null;  // Insufficient stock
                }

                // Decrease product quantity in stock
                product.Quantity -= item.Quantity;

                // Calculate price for the order item
                item.Price = product.Price;
            }

            // Set OrderId automatically
            order.OrderId = _nextOrderId++;

            // Calculate total amount for the order
            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.Price);

            // Add the order to in-memory storage
            _orders.Add(order);

            return await Task.FromResult(order);
        }

        // Method to get all orders
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Task.FromResult(_orders.AsEnumerable());
        }

        // Method to get an order by ID
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == id);
            return await Task.FromResult(order);
        }


        public async Task<Order> UpdateOrderAsync(int orderId, Order updatedOrder)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (existingOrder == null)
            {
                return null;  // Order not found
            }

            // Validate and update order items
            foreach (var updatedItem in updatedOrder.OrderItems)
            {
                if (updatedItem.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                var product = _productService.GetProducts().FirstOrDefault(p => p.ProductId == updatedItem.ProductId);

                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {updatedItem.ProductId} not found.");
                }

                var existingItem = existingOrder.OrderItems.FirstOrDefault(i => i.OrderItemId == updatedItem.OrderItemId);

                if (existingItem != null)
                {
                    existingOrder.OrderItems = updatedOrder.OrderItems;
                    // Revert stock for the previous quantity
                    product.Quantity += existingItem.Quantity;

                    // Check if stock is sufficient for the new quantity
                    if (product.Quantity < updatedItem.Quantity)
                    {
                        throw new ArgumentException($"Insufficient stock for Product ID {updatedItem.ProductId}.");
                    }

                    // Update item and adjust stock
                    product.Quantity -= updatedItem.Quantity;
                    existingItem.Quantity = updatedItem.Quantity;
                    existingItem.Price = updatedItem.Price;
                }
                else
                {
                    // If item does not exist, validate and add it
                    if (product.Quantity < updatedItem.Quantity)
                    {
                        throw new ArgumentException($"Insufficient stock for Product ID {updatedItem.ProductId}.");
                    }

                    product.Quantity -= updatedItem.Quantity;
                    existingOrder.OrderItems.Add(updatedItem);
                }
            }

            // Recalculate total amount
            existingOrder.TotalAmount = existingOrder.OrderItems.Sum(item => item.Quantity * item.Price);

            return await Task.FromResult(existingOrder);
        }


        // Method to delete an order
        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var existingOrder = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (existingOrder == null)
            {
                return false; // Order not found
            }

            // Revert stock for all order items
            foreach (var item in existingOrder.OrderItems)
            {
                var product = _productService.GetProducts().FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Quantity += item.Quantity; // Revert the stock
                }
            }

            // Remove the order from in-memory storage
            _orders.Remove(existingOrder);

            return await Task.FromResult(true);
        }
    }
}
