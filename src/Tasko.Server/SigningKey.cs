using System;
using System.Configuration;
using Thinktecture.IdentityModel;

namespace Tasko.Server
{
    public class SigningKey
    {
        public string Get()
        {
            string key = ConfigurationManager.AppSettings["SigningKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                key = Convert.ToBase64String(CryptoRandom.CreateRandomKey(32));
            }

            return key;
        }
    }
}