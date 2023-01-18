using System.Collections.Generic;

namespace Data.Contracts
{
    public interface GetCustomerResponse
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public List<int> SalesIds { get; set; }
    }
}