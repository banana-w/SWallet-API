using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SWallet.Repository.Payload.Response.Account;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Utils
{
    public class JwtUtil
    {
        public static string GenerateJwtToken(AccountResponse account)
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();

            var secrectKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secrectKey, SecurityAlgorithms.HmacSha256Signature);
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim("Role", account.RoleName.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString())
                };

            var preparedToken = new JwtSecurityToken(
                       issuer: issuer,
                       audience: audience,
                       claims: claims,
                       expires: DateTime.Now.AddMinutes(30),
                       signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(preparedToken);
            return token;
        }
    }
}
