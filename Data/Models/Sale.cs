using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public int SalesPointId { get; set; }
        public SalesPoint SalesPoint { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public List<SaleData> SalesData { get; set; } = new List<SaleData>();
        public decimal TotalAmount => SalesData.Sum(s => s.ProductAmount);
    }

    public class SaleData
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductAmount => Product.Price * ProductQuantity;
    }
}