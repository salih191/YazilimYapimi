using Core.Entities;

namespace Entities.Concrete
{
    public class Wallet : IEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
    }
}