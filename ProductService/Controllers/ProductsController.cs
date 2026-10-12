using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductMongoService _service;

        public ProductsController(ProductMongoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            return await _service.GetAsync(); // ✅ doit exister dans le service
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var created = await _service.CreateAsync(product); // ✅ doit exister dans le service
            return Created("", created);
        }
    }
}