using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.DataAdapter
{
    public class SecurityHelper
    {
        public static byte[] HexStringToBytes(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
            {
                return null;
            }
            if (hexString.Length % 2 == 1)
            {
                throw new ArgumentException("参数不是有效的16进制字符串", "hexString");
            }
            int length = hexString.Length / 2;
            byte[] buffer = new byte[length];

            for (int i = 0; i < length; i++)
            {
                buffer[i] = Byte.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);// Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return buffer;
        }

        public static string BytesToHexString(byte[] buffer)
        {
            if (buffer == null) return null;
            StringBuilder hexBuilder = new StringBuilder();
            foreach (byte b in buffer)
            {
                hexBuilder.Append(b.ToString("x2"));
            }
            return hexBuilder.ToString();
        }

        /// <summary>
        /// 生成64字节的随机数
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandom(uint length)
        {
            byte[] buffer = new byte[length];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            return buffer;
        }
        public static string GenerateRandomString(uint length)
        {
            byte[] buffer = GenerateRandom(length);
            return BytesToHexString(buffer);
        }

        /// <summary>
        /// 生成16字节的随机数
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandom16()
        {
            return GenerateRandom(16u);
        }
        /// <summary>
        /// 生成32字节的随机数
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandom32()
        {
            return GenerateRandom(32u);
        }
        /// <summary>
        /// 生成64字节的随机数
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateRandom64()
        {
            return GenerateRandom(64u);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">返回为16进制表示的字符串</param>
        /// <param name="iv">返回为16进制表示的字符串</param>
        public static void GenerateAESKey(out string key, out string iv)
        {
            System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create();
            key = BytesToHexString(aes.Key);
            iv = BytesToHexString(aes.IV);
        }
        /// <summary>
        /// 返回16进制MD5值
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GetMD5Value(string inputString)
        {
            byte[] inputBuffer = Encoding.UTF8.GetBytes(inputString);
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hashBuffer = md5.ComputeHash(inputBuffer);
            return BytesToHexString(hashBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static byte[] GetSHAValue(byte[] inputBuffer, string shaName)
        {
            HashAlgorithm sha = HashAlgorithm.Create(shaName.ToUpper());
            byte[] hashBuffer = sha.ComputeHash(inputBuffer);
            return hashBuffer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetSHAValue(string inputString, string shaName)
        {
            byte[] inputBuffer = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBuffer = GetSHAValue(inputBuffer, shaName);
            return BytesToHexString(hashBuffer);
        }
        public static string GetSHA1Value(string inputString)
        {
            return GetSHAValue(inputString, "SHA1");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static byte[] GetHMACValue(byte[] inputBuffer, string shaName, byte[] key)
        {
            System.Security.Cryptography.HMAC sha = System.Security.Cryptography.HMAC.Create(shaName);
            if (key != null && key.Length > 0)
            {
                sha.Key = key;
            }
            byte[] hashBuffer = sha.ComputeHash(inputBuffer);
            return hashBuffer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACValue(string inputString, string shaName, byte[] key)
        {
            byte[] inputBuffer = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBuffer = GetHMACValue(inputBuffer, shaName, key);
            return BytesToHexString(hashBuffer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACMD5Value(string inputString, byte[] key)
        {
            return GetHMACValue(inputString, "HMACMD5", key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACSHA1Value(string inputString, byte[] key)
        {
            return GetHMACValue(inputString, "HMACSHA1", key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACSHA256Value(string inputString, byte[] key)
        {
            return GetHMACValue(inputString, "HMACSHA256", key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACSHA384Value(string inputString, byte[] key)
        {
            return GetHMACValue(inputString, "HMACSHA384", key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">最长为64位的16进制字符串</param>
        /// <returns></returns>
        public static string GetHMACSHA512Value(string inputString, byte[] key)
        {
            return GetHMACValue(inputString, "HMACSHA512", key);
        }

        /// <summary>
        /// 可在配置文件中的AppSettings 中配置 AES_Key 和 AES_IV 来修改默认密钥
        /// </summary> 
        public static string AES_Encrypt(string plainText)
        {
            return AES.Default.Encrypt(plainText);
        }

        /// <summary>
        /// 可在配置文件中的AppSettings 中配置 AES_Key 和 AES_IV 来修改默认密钥
        /// </summary> 
        public static string AES_Decrypt(string cipherText)
        {
            return AES.Default.Decrypt(cipherText);
        }
    }

    public class AES
    {
        string _key;
        string _iv;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">base64加密</param>
        /// <param name="iv">base64加密</param>
        public AES(string key, string iv)
        {
            _key = key;
            _iv = iv;
        }
        public AES()
        {
            _key = "45a09183de3765e28317e059efd59bf2bd447ceb55245d791059898e4a9a143e";
            _iv = "661d8dbe97c754a3b5db7e00918b27a9";
        }

        private byte[] BKey
        {
            get
            {
                return SecurityHelper.HexStringToBytes(_key);
            }
        }

        private byte[] BIV
        {
            get
            {
                return SecurityHelper.HexStringToBytes(_iv);
            }
        }

        public string Encrypt(string plainText)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            string encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = BKey;
                aesAlg.IV = BIV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = SecurityHelper.BytesToHexString(msEncrypt.ToArray());
                    }
                }
            }
            return encrypted;
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            byte[] bCipherText = SecurityHelper.HexStringToBytes(cipherText);

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = BKey;
                aesAlg.IV = BIV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(bCipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        private static AES _default;

        /// <summary>
        /// 可在配置文件中的AppSettings 中配置 AES_Key 和 AES_IV 来修改默认密钥
        /// </summary> 
        public static AES Default
        {
            get
            {
                if (_default == null)
                {
                    string key = ConfigurationManager.AppSettings["AES_Key"];
                    string iv = ConfigurationManager.AppSettings["AES_IV"];
                    AES aes = new AES();
                    if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(iv))
                    {
                        aes._key = key;
                        aes._iv = iv;
                    }
                    _default = aes;
                }
                return _default;
            }
        }
    }
}
