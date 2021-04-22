using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Core.Entities.Concrete;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddRoles(this ICollection<Claim> claims, string[] roles)
        {
            roles.ToList().ForEach(role => claims.Add(new Claim("roles", role)));
        }
        public static void AddUser(this ICollection<Claim> claims, User user)
        {
            foreach (var property in user.GetType().GetProperties())
            {
                claims.Add(new Claim(property.Name.FirstCharToLowerCase(), property.GetValue(user).ToString()));
            }
        }
    }
}