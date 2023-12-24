using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MSGMicroservice.IDP.Infrastructure.Common
{
    public class StringHeplers
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder hashSb = new StringBuilder();
            foreach (byte b in hash)
            {
                hashSb.Append(b.ToString("X2"));//x chữ thường
            }
            return hashSb.ToString();
        }

        public static string SHA1Hash(string text)
        {
            SHA1Managed sha1 = new SHA1Managed();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
            StringBuilder hashSb = new StringBuilder();
            foreach (byte b in hash)
            {
                hashSb.Append(b.ToString("X2"));
            }
            return hashSb.ToString();
        }

        public static string Encrypt(string text, string key)
        {
            var data = Encoding.UTF8.GetBytes(text);

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var keys = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                using (var tripDes = new TripleDESCryptoServiceProvider { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    var transform = tripDes.CreateEncryptor();
                    var results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        public static string Decrypt(string cipher, string key)
        {
            var data = Convert.FromBase64String(cipher);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var keys = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

                using (var tripDes = new TripleDESCryptoServiceProvider()
                {
                    Key = keys,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
                {
                    var transform = tripDes.CreateDecryptor();
                    var results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Encoding.UTF8.GetString(results);
                }
            }
        }
    }
}
