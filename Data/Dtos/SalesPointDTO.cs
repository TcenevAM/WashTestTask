using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Dtos
{
    public class SalesPointDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        public List<ProvidedProductDTO> ProvidedProducts { get; set; } = new List<ProvidedProductDTO>();
    }

    public class ProvidedProductDTO
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be between 0 and int.MaxValue.")]
        public int Quantity { get; set; }
    }
}