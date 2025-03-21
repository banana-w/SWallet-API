using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SWallet.Domain.Models;
using SWallet.Repository.Interfaces;
using SWallet.Repository.Payload.Response.Account;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class JwtService : BaseService<JwtService>, IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IUnitOfWork<SwalletDbContext> unitOfWork, ILogger<JwtService> logger, IConfiguration configuration) : base(unitOfWork, logger)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(AccountResponse account, Tuple<string, string> guidClaim)
        {
            var secrectKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secrectKey, SecurityAlgorithms.HmacSha256Signature);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            List<Claim> claims = new List<Claim>()
                {
                    new Claim("Role", account.RoleName.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString())
                };

            if (guidClaim != null) claims.Add(new Claim(guidClaim.Item1, guidClaim.Item2.ToString()));

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
