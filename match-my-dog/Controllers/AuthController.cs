using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using match_my_dog.Data;
using match_my_dog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace match_my_dog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DatabaseContext context;

        public AuthController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TokenData>> PostAuth(AuthData authData)
        {
            var hashedPassword = GetHashedPassword(authData.Password);
            var identity = GetIdentity(authData.Username, hashedPassword);
            if (identity == null) return BadRequest();
            return new TokenData() { AccessToken = GetToken(identity) };
        }

        private static string GetToken(ClaimsIdentity identity) => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: Config.AuthIssuer,
            audience: Config.AuthAudience,
            notBefore: DateTime.UtcNow,
            claims: identity.Claims,
            signingCredentials: new SigningCredentials(Config.SymmetricAuthKey, SecurityAlgorithms.HmacSha256)
        ));

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = context.Users.FirstOrDefault(user => user.Username == username && user.Password == password);

            if (user == null) return null;

            return new ClaimsIdentity(
                new List<Claim> {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                },
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
        }

        private static MD5 hash = MD5.Create();
        private static string GetHashedPassword(string password) => string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
    }
}