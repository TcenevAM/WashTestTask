using System;

namespace Data.Contracts
{
    public interface SaleCreated
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int SalesPointId { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
}