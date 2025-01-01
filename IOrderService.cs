using OrderManagementApi.Models;

namespace OrderManagementApi.Services
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> UpdateOrderAsync(int orderId, Order updatedOrder);
        Task<bool> DeleteOrderAsync(int id);
        Task<Order> AddOrderAsync(Order order);
    }

}
