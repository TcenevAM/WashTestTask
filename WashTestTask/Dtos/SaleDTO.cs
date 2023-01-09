using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WashTestTask.Dtos
{
    public class SaleDTO
    {
        public int Id { get; set; }
        
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
        
        [Required(ErrorMessage = "SalesPointId is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "SalesPointId must be between 0 and int.MaxValue.")]
        public int SalesPointId { get; set; }
        
        public int? CustomerId { get; set; }
        
        
        [Required(ErrorMessage = "SalesData is required.")]
        [MinLength(1, ErrorMessage = "SalesData must contain at least 1 element")]
        public List<SaleDataDTO> SalesData { get; set; } = new List<SaleDataDTO>();
        
        public decimal TotalAmount { get; set; }
    }

    public class SaleDataDTO
    {
        [Required(ErrorMessage = "ProductId is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "ProductId must be between 0 and int.MaxValue.")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "ProductQuantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "ProductQuantity must be between 0 and int.MaxValue.")]
        public int ProductQuantity { get; set; }
        
        public decimal ProductAmount { get; set; }
    }
}