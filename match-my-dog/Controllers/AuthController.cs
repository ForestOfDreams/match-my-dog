using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using match_my_dog.Data.Request.Auth;
using match_my_dog.Data.Response;
using match_my_dog.Models;
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

        private static readonly Regex UsernameRegex = new Regex(@"[A-Za-z0-9]{4,}");

        [HttpPut]
        public async Task<ActionResult<Token>> PutAuth(Put data)
        {
            if (data.Password != data.ConfirmPassword)
                return BadRequest(Error.PasswordNotMatch);

            if (!UsernameRegex.IsMatch(data.Username))
                return BadRequest(Error.BadUsername);

            if (context.Users.Any(user => user.Username == data.Username))
                return BadRequest(Error.UserExists);

            context.Users.Add(new Models.User() { 
                Phone = data.Phone, 
                Name = data.Name, 
                Password = GetHashedPassword(data.Password), 
                Username = data.Username, 
                Role = Roles.User 
            });
            
            await context.SaveChangesAsync();

            return GetTokenResponse(data.Username, data.Password);
        }

        private ActionResult<Token> GetTokenResponse(string username, string password)
        {
            var hashedPassword = GetHashedPassword(password);
            var identity = GetIdentity(username, hashedPassword);
            if (identity == null) return BadRequest(Error.BadUsernameOrPassword);
            return new Token() { AccessToken = GetToken(identity) };
        }

        [HttpPost]
        public async Task<ActionResult<Token>> PostAuth(Post data) => GetTokenResponse(data.Username, data.Password);

        private static string GetToken(ClaimsIdentity identity) => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: Config.AuthIssuer,
            audience: Config.AuthAudience,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddYears(100),
            claims: identity.Claims,
            signingCredentials: new SigningCredentials(Config.SymmetricAuthKey, SecurityAlgorithms.HmacSha256)
        ));

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var user = context.Users.FirstOrDefault(user => user.Username == username && user.Password == password);

            return user == null ? null : new ClaimsIdentity(
                new List<Claim> {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString(), ClaimValueTypes.Integer64),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                },
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );
        }

        private static readonly MD5 hash = MD5.Create();
        private static string GetHashedPassword(string password) => string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("x2")));
    }
}