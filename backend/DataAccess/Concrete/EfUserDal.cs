using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;

namespace DataAccess.Concrete
{
    public class EfUserDal : EfEntityRepositoryBase<User, YazilimYapimiContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new YazilimYapimiContext())
            {
                var result = from operationClaim in context.OperationClaims
                    join userOperationClaim in context.UserOperationClaims
                        on operationClaim.Id equals userOperationClaim.OperationClaimId
                    where userOperationClaim.UserId == user.Id
                    select new OperationClaim { Id = operationClaim.Id, Name = operationClaim.Name };
                return result.ToList();
            }
        }

        public void AddUserOperationClaim(UserOperationClaim userOperationClaim)
        {
            using (var context = new YazilimYapimiContext())
            {
                context.UserOperationClaims.Add(userOperationClaim);
                context.SaveChanges();
            }
        }

        public User GetMuhasabeci()
        {
            using (var context = new YazilimYapimiContext())
            {
                var result = from uso in context.UserOperationClaims
                    join User in context.Users on uso.UserId equals User.Id
                    where uso.OperationClaimId == 4
                             select User;
                return result.FirstOrDefault();
            }
        }
    }
}