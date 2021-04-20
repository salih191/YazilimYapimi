using Core.Entities;

namespace Entities.DTOs
{
    public class AddProductDto : IDto
    {
        public int AddProductId { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitsInPrice { get; set; }
        public decimal Quantity { get; set; }
        public string SupplierUserName { get; set; }
    }
}