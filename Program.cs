using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManagementApi.Middleware;
using OrderManagementApi.Services;  

namespace OrderManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<ProductService>();

            // Register OrderService to handle order operations
            builder.Services.AddSingleton<IOrderService, OrderService>();

            // Add controllers to handle API endpoints
            builder.Services.AddControllers();

            // Add Swagger/OpenAPI for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS Policy to allow requests from any origin
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>(); // Global exception handler

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // Enable CORS with the policy "AllowAll"
            app.UseCors("AllowAll");

            // Enable exception handling middleware (optional)
            app.UseExceptionHandler("/error");

            // Enable authorization middleware (if needed)
            app.UseAuthorization();

            // Map API controllers
            app.MapControllers();

            // Run the app
            app.Run();
        }
    }
}
