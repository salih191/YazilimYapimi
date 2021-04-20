using Core.Entities;

namespace Entities.Concrete
{
    public class AddMoney : IEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }//admin onayı admin onaylarsa true, true olursa veritabanındaki trigger otomatik olarak wallet tablosuna ekleme yapıyor 
    }
}