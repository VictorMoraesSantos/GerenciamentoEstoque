namespace Shared.Messaging.Messages
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderStatusChanged
    {
        public Guid OrderId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class OrderCancelled
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}