using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WashTestTask.Dtos
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        
        public List<int> SalesIds { get; set; } = new List<int>();
    }
}