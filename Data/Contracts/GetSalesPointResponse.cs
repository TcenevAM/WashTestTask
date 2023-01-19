using System.Collections.Generic;

namespace Data.Contracts
{
    public interface GetSalesPointResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ProvidedProductResponse> ProvidedProducts { get; set; }
    }
    
    public interface ProvidedProductResponse
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
    }
}