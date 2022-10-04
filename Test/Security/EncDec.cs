using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Security
{
    public class EncDec
    {
        public static string key = "fghjeir3";

        public static string Encrypt(string password)
        {
            password += key;
            var passwordbytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passwordbytes);
        }
    }
}
