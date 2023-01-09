using System.Collections.Generic;

namespace WashTestTask.Models
{
    public class SalesPoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProvidedProduct> ProvidedProducts { get; set; } = new List<ProvidedProduct>();
    }

    public class ProvidedProduct
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SalesPointId { get; set; }
        public SalesPoint SalesPoint { get; set; }
        public int Quantity { get; set; }
    }
}