using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Concrete
{
    public class EfAddMoneyDal : EfEntityRepositoryBase<AddMoney, YazilimYapimiContext>, IAddMoneyDal
    {
        public List<AddMoneyDto> GetAddMoneyDtos()
        {
            using (var context = new YazilimYapimiContext())
            {
                var result = from addMoney in context.AddMoney
                    join user in context.Users on addMoney.UserId equals user.Id
                    where addMoney.Status == false
                    select new AddMoneyDto
                    {
                        AddMoneyId = addMoney.Id,
                        Amount = addMoney.Amount,
                        UserName = user.UserName
                    };
                return result.ToList();
            }
        }
    }
}