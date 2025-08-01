namespace MAS_Shared.Data
{
    public class Order
    {
        public int OrderID { get; set; }
        public required string UserID { get; set; }
        public required string OrderItems { get; set; }
        public DateTime DttmInitiated { get; set; }
        public decimal? OrderTotal { get; set; }
        public required string DriverID { get; set; }
        public bool IsBeingCollected { get; set; } = false;
        public bool IsEnrouteToCust { get; set; } = false;
        public bool IsArrivedAtCust { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public DateTime? DttmDelivered { get; set; }
        public string? DisputedReason { get; set; }
        public DateTime? DttmClosed { get; set; }
    }
}

