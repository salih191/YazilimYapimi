using Core.Entities;

namespace Entities.Concrete
{
    public class Currency:IEntity
    {
        public int Id { get; set; }
        public string CurrencyType { get; set; }
    }
}