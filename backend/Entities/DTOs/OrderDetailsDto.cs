using System;
using Core.Entities;

namespace Entities.DTOs
{
    public class OrderDetailsDto : IDto
    {
        public int OrderDetailId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerUserName { get; set; }
        public string SupplierUserName { get; set; }
        public string ProductCategory { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }

    }
}