using ChatApp.Context.EntityClasses;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ChatApp.Business.Helpers
{
    public static class CustomAuthorization
    {
        
        public static string GetUsernameFromToken(string token)
        {
            string newToken = token.Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(newToken);
            return jsonToken.Claims.First(claim => claim.Type == "sub").Value;
        }

        
    }
}
