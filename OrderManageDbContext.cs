using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Models;

namespace Order_Management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Order> Orders { get; set; } // Represents Orders table
        public DbSet<OrderItem> OrderItems { get; set; } // Represents OrderItems table 
        public DbSet<Product> Products { get; set; } // Represents Products table 
    }
}
