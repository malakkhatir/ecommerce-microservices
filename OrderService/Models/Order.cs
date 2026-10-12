namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }

        // ✅ required supprime le warning CS8618
        public required string ProductId { get; set; }

        public int ClientId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}