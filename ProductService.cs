using OrderManagementApi.Models;

namespace OrderManagementApi.Services
{
    public class ProductService
    {
        // Hardcoded product catalog with at least 5 products
        private static List<Product> _products = new List<Product>
        {
            new Product { ProductId = 1, Name = "T-Shirt", Price = 25.00m, Quantity = 100 },
            new Product { ProductId = 2, Name = "Jeans", Price = 50.00m, Quantity = 50 },
            new Product { ProductId = 3, Name = "Jacket", Price = 100.00m, Quantity = 30 },
            new Product { ProductId = 4, Name = "Shoes", Price = 60.00m, Quantity = 80 },
            new Product { ProductId = 5, Name = "Hat", Price = 15.00m, Quantity = 200 }
        };

        public IEnumerable<Product> GetProducts()
        {
            return _products;
        }
    }
}
