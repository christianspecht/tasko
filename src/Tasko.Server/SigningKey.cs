using System;
using System.Configuration;
using Thinktecture.IdentityModel;

namespace Tasko.Server
{
    public class SigningKey
    {
        public byte[] Get()
        {
            string key = ConfigurationManager.AppSettings["SigningKey"];

            if (!string.IsNullOrWhiteSpace(key))
            {
                return Convert.FromBase64String(key);
            }

            return CryptoRandom.CreateRandomKey(32);
        }
    }
}