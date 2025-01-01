// OrderManagementApi/Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Models;
using OrderManagementApi.Services;
using System.Collections.Generic;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        // Inject ProductService into the controller
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _productService.GetProducts(); // Get products from ProductService
            return Ok(products); // Return products as response
        }
    }
}
