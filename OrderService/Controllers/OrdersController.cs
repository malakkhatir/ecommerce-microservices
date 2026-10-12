using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly KafkaProducerService _kafka;

        public OrdersController(
            AppDbContext context,
            KafkaProducerService kafka)
        {
            _context = context;
            _kafka = kafka;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            await _kafka.PublishOrderCreated(order);

            return Created("", order);
        }
    }
}