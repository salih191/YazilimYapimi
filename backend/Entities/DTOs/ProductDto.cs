using Core.Entities;

namespace Entities.DTOs
{
    public class ProductDto : IDto
    {
        public int ProductId { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitsInPrice { get; set; }
        public decimal Quantity { get; set; }
        public string SupplierUserName { get; set; }
    }
}