using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace match_my_dog
{
    public class Config
    {
        public static string AuthAudience = Environment.GetEnvironmentVariable(Constants.EnvironmentAuthAudience);
        public static string AuthIssuer = Environment.GetEnvironmentVariable(Constants.EnvironmentAuthIssuer);
        public static string AuthKey = Environment.GetEnvironmentVariable(Constants.EnvironmentAuthKey);
        public static string ConnectionString = Environment.GetEnvironmentVariable(Constants.EnvironmentConnectionString);
        public static SymmetricSecurityKey SymmetricAuthKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthKey));

        public static string ImgurClientId = Environment.GetEnvironmentVariable(Constants.EnvironmentImgurClientId);
        public static string ImgurClientSecret = Environment.GetEnvironmentVariable(Constants.EnvironmentImgurClientSecret);
    }
}
