namespace Shared.Messaging.Messages
{
    public class ProductStockUpdated
    {
        public int ProductId { get; set; }
        public int NewQuantity { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UpdateReason { get; set; }
    }

    public class StockReservationRequested
    {
        public Guid OrderId { get; set; }
        public List<OrderItemStockRequest> Items { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class OrderItemStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class StockReservationResult
    {
        public Guid OrderId { get; set; }
        public bool Success { get; set; }
        public List<StockReservationItemResult> Items { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class StockReservationItemResult
    {
        public int ProductId { get; set; }
        public bool Reserved { get; set; }
        public string? ReasonForFailure { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
    }
}