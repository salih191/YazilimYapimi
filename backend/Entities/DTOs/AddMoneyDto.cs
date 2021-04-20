using Core.Entities;

namespace Entities.DTOs
{
    public class AddMoneyDto : IDto
    {
        public int AddMoneyId { get; set; }
        public decimal Amount { get; set; }
        public string UserName { get; set; }
    }
}