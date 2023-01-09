using System.ComponentModel.DataAnnotations;

namespace WashTestTask.Dtos
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be between 0 and double.MaxValue.")]
        public decimal Price { get; set; }
    }
}