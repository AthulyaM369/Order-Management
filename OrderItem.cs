using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OrderManagementApi.Models
{
    public class OrderItem
    {
        [Key]
        [JsonIgnore]
        public int OrderItemId { get; set; }

        [JsonIgnore]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Computed by the backend
        [JsonIgnore]
        public decimal Price { get; set; }

        // Reference to the parent order, not needed in requests
        [JsonIgnore]
        public Order? Order { get; set; }
    }
}
