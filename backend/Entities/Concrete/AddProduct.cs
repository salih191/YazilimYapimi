using Core.Entities;

namespace Entities.Concrete
{
    public class AddProduct : IEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int SupplierId { get; set; }
        public bool Status { get; set; }
        public int CurrencyId { get; set; }
    }
}