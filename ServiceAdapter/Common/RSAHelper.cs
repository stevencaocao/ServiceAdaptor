using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ServiceAdapter.Common
{
    public class RSAHelper
    {
        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <param name="withPrivate"></param>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public static bool TryGetKeyParameters(string filePath, bool withPrivate, out SecurityKey securityKey)
        {
            var rsa = RSA.Create();
            string filename = withPrivate ? "key.json" : "key.public.json";
            string fileTotalPath = Path.Combine(filePath, filename);
            securityKey = new RsaSecurityKey(rsa);
            if (!File.Exists(fileTotalPath))
            {
                return false;
            }
            else
            {
                //keyParameters = JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(fileTotalPath));
                if (withPrivate)
                {
                    var privateRSAKey = File.ReadAllText(fileTotalPath).Trim();
                    Regex privateRSAKeyRegex = new Regex(@"-----(BEGIN|END) RSA PRIVATE KEY-----[\W]*");
                    privateRSAKey = privateRSAKeyRegex.Replace(privateRSAKey, "");

                    var keyByte = Convert.FromBase64String(privateRSAKey);
                    rsa.ImportRSAPrivateKey(keyByte, out _);
                }
                else
                {
                    var publicRSAKey = File.ReadAllText(fileTotalPath).Trim();
                    Regex publicRSAKeyRegex = new Regex(@"-----(BEGIN|END) PUBLIC KEY-----[\W]*");
                    publicRSAKey = publicRSAKeyRegex.Replace(publicRSAKey, "");

                    var keyByte = Convert.FromBase64String(publicRSAKey);
                    rsa.ImportSubjectPublicKeyInfo(keyByte, out _);
                }


                securityKey = new RsaSecurityKey(rsa);

                return true;
            }
        }

        /// <summary>
        /// 转换密钥
        /// </summary>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static SecurityKey GetPublicKey(string key)
        {
            try
            {
                var rsa = RSA.Create();
                Regex publicRSAKeyRegex = new Regex(@"-----(BEGIN|END) PUBLIC KEY-----[\W]*");
                var publicRSAKey = publicRSAKeyRegex.Replace(key, "");

                var keyByte = Convert.FromBase64String(publicRSAKey);
                rsa.ImportSubjectPublicKeyInfo(keyByte, out _);
                return new RsaSecurityKey(rsa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <returns></returns>
        public static SecurityKey GenerateAndSaveKey(string filePath, bool withPrivate = true)
        {
            RsaKeyPairGenerator r = new RsaKeyPairGenerator();

            r.Init(new KeyGenerationParameters(new SecureRandom(), 2048));

            AsymmetricCipherKeyPair keys = r.GenerateKeyPair();

            TextWriter privateTextWriter = new StringWriter();

            PemWriter privatePemWriter = new PemWriter(privateTextWriter);

            privatePemWriter.WriteObject(keys.Private);

            privatePemWriter.Writer.Flush();

            TextWriter publicTextWriter = new StringWriter();

            PemWriter publicPemWriter = new PemWriter(publicTextWriter);

            publicPemWriter.WriteObject(keys.Public);

            publicPemWriter.Writer.Flush();

            File.WriteAllText(Path.Combine(filePath, "key.json"), privateTextWriter.ToString());
            File.WriteAllText(Path.Combine(filePath, "key.public.json"), publicTextWriter.ToString());

            var rsa = RSA.Create();
            Regex privateRSAKeyRegex = new Regex(@"-----(BEGIN|END) RSA PRIVATE KEY-----[\W]*");
            var privateRSAKey = privateRSAKeyRegex.Replace(privateTextWriter.ToString(), "");
            var privateByte = Convert.FromBase64String(privateRSAKey);
            rsa.ImportRSAPrivateKey(privateByte, out _);
            SecurityKey privateKey = new RsaSecurityKey(rsa);

            rsa = RSA.Create();
            Regex publicRSAKeyRegex = new Regex(@"-----(BEGIN|END) PUBLIC KEY-----[\W]*");
            var publicRSAKey = publicRSAKeyRegex.Replace(publicTextWriter.ToString(), "");
            var publicByte = Convert.FromBase64String(publicRSAKey);
            rsa.ImportSubjectPublicKeyInfo(publicByte, out _);
            SecurityKey publicKey = new RsaSecurityKey(rsa);

            return withPrivate ? privateKey : publicKey;

        }
    }
}
