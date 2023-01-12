using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Data.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<int> SalesIds => Sales.Select(s => s.Id).ToList();
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}