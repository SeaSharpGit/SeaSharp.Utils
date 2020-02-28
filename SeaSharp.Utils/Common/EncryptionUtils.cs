using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SeaSharp.Utils.Common
{
    public static class EncryptionUtils
    {
        #region 生成字母/数字随机码
        /// <summary>
        /// 生成字母/数字随机码
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string CreateRandomCode(int length)
        {
            if (length <= 0)
            {
                throw new Exception("随机码长度必须大于0");
            }
            var code = "";
            var num = new Random();
            for (int i = 0; i < length; i++)
            {
                var rand = num.Next();
                if (rand % 3 == 0)
                {
                    code += (char)('A' + (char)(rand % 26));
                }
                else
                {
                    code += (char)('0' + (char)(rand % 10));
                }
            }
            return code;
        }
        #endregion

        #region MD5
        /// <summary>
        /// MD5（16位）
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <returns>16位</returns>
        public static string GetMd5_16(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            var md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(encryptString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary>
        /// MD5（32位）
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <returns>32位</returns>
        public static string GetMd5_32(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            var sb = new StringBuilder();
            var md5 = MD5.Create();
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(encryptString));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式，格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                sb.Append(s[i].ToString("X"));
            }
            return sb.ToString();
        }
        #endregion

        #region DES（对称）
        /// <summary>
        /// DES加密（对称）
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>失败返回原字符串</returns>
        public static string DesEncrypt(string encryptString, string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(encryptKey) || encryptKey.Length != 8)
            {
                throw new Exception("加密密钥错误，必须位8位");
            }
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                string str = Convert.ToBase64String(mStream.ToArray());
                return str;
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密（对称）
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>失败返回原字符串</returns>
        public static string DesDecrypt(string decryptString, string decryptKey)
        {
            if (string.IsNullOrEmpty(decryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(decryptKey) || decryptKey.Length != 8)
            {
                throw new Exception("解密密钥错误，必须位8位");
            }
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion

        #region AES（对称）
        /// <summary>
        /// AES加密（对称）
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,长度为16/24/32,和解密密钥相同</param>
        /// <param name="iv">向量（如果是空，默认为加密密钥的前16位）</param>
        /// <returns>失败返回原字符串</returns>
        public static string AesEncrypt(string encryptString, string encryptKey, string iv = "")
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(encryptKey) || (encryptKey.Length != 16 && encryptKey.Length != 24 && encryptKey.Length != 32))
            {
                throw new Exception("加密密钥的长度必须是16/24/32");
            }
            if (!string.IsNullOrEmpty(iv) && iv.Length != 16)
            {
                throw new Exception("向量长度必须为16位");
            }

            try
            {
                using (var aes = new RijndaelManaged())
                {
                    var byteValue = Encoding.UTF8.GetBytes(encryptString);
                    aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(encryptKey.Substring(0, 16));
                    aes.Key = Encoding.UTF8.GetBytes(encryptKey);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    var cryptoTransform = aes.CreateEncryptor();
                    var resultArray = cryptoTransform.TransformFinalBlock(byteValue, 0, byteValue.Length);
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// AES解密（对称）
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,长度为16/24/32,和加密密钥相同</param>
        /// <param name="iv">向量（如果是空，默认为解密密钥的前16位）</param>
        /// <returns>失败返回原字符串</returns>
        public static string AesDecrypt(string decryptString, string decryptKey, string iv = "")
        {
            var x = new AesManaged();
            if (string.IsNullOrEmpty(decryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(decryptKey) || (decryptKey.Length != 16 && decryptKey.Length != 24 && decryptKey.Length != 32))
            {
                throw new Exception("加密密钥的长度必须是16/24/32");
            }
            if (!string.IsNullOrEmpty(iv) && iv.Length != 16)
            {
                throw new Exception("向量长度必须为16位");
            }

            try
            {
                using (var aes = new RijndaelManaged())
                {
                    var byteValue = Convert.FromBase64String(decryptString);
                    aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(decryptKey.Substring(0, 16));
                    aes.Key = Encoding.UTF8.GetBytes(decryptKey);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    var cryptoTransform = aes.CreateDecryptor();
                    var resultArray = cryptoTransform.TransformFinalBlock(byteValue, 0, byteValue.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch
            {
                return decryptString;
            }

        }
        #endregion

        #region RSA（非对称）
        /// <summary>
        /// RSA生成公钥和私钥（非对称）
        /// </summary>
        public static void RsaCreateKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            using (StreamWriter writer = new StreamWriter("PrivateKey.xml"))
            {
                writer.WriteLine(rsa.ToXmlString(true));
            }
            using (StreamWriter writer = new StreamWriter("PublicKey.xml"))
            {
                writer.WriteLine(rsa.ToXmlString(false));
            }
        }

        /// <summary>
        /// RSA加密（非对称）
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <returns>失败返回原字符串</returns>
        public static string RsaEncrypt(string encryptString, string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(encryptKey))
            {
                throw new Exception("加密密钥错误");
            }
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(encryptKey);
                var cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(encryptString), true);
                return Convert.ToBase64String(cipherbytes);
            }
            catch (Exception)
            {
                return encryptString;
            }
        }

        /// <summary>
        /// RSA解密（非对称）
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <returns>失败返回原字符串</returns>
        public static string RsaDecrypt(string decryptString, string decryptKey)
        {
            if (string.IsNullOrEmpty(decryptString))
            {
                return "";
            }
            if (string.IsNullOrEmpty(decryptKey))
            {
                throw new Exception("解密密钥错误");
            }
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(decryptKey);
                var cipherbytes = rsa.Decrypt(Convert.FromBase64String(decryptString), true);
                return Encoding.UTF8.GetString(cipherbytes);
            }
            catch (Exception)
            {
                return decryptString;
            }
        }
        #endregion



    }
}
