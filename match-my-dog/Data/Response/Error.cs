using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Response
{
    public class Error
    {
        public string Message { get; set; }

        public string Code { get; set; }

        public static Error BadUsername() => new Error() { Code = "BadUsername", Message = "Bad username (latin-only, min 4 symbols)" };

        public static Error UserExists() => new Error() { Code = "UserExists", Message = "User with such username already exists" };

        public static Error PasswordNotMatch() => new Error() { Code = "PasswordNotMatch", Message = "Confirm password does not match" };

        public static Error BadUsernameOrPassword() => new Error() { Code = "BadUsernameOrPassword", Message = "There is no user with such username and password" };

        public static Error BadDogId() => new Error() { Code = "BadDogId", Message = "There is no dog with such id" };
        public static Error BadUserId() => new Error() { Code = "BadUserId", Message = "There is no user with such id" };

        public static Error BadPhone() => new Error() { Code = "BadPhone",  Message = "Bad phone number" };

        public static Error BadWeight() => new Error() { Code = "BadWeight", Message = "Bad weight" };

        public static Error FileUploadError(string message) => new Error() { Code = "FileUploadError", Message = message };
        }
        
}
