using System;
using System.Security.Cryptography;
using System.Text;

namespace ASyncFramework.Application.Common.Hashing
{
    public static class HashObjectExtension
    {
        public static string HashObject(this object obj)
        {
            var hash = new SHA1Managed();
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            var hashByte = hash.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hashByte);
        }
    }
}