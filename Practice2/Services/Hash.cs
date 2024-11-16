using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Practice2.Services
{
    internal class Hash
    {
        public static class HashHelper
        {
            public static string HashPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] sourceBytePassword = Encoding.UTF8.GetBytes(password);
                    byte[] hash = sha256Hash.ComputeHash(sourceBytePassword);
                    return BitConverter.ToString(hash).Replace("-", String.Empty);
                }
            }

        }
    }
}
