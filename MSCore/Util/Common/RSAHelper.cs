using K4os.Compression.LZ4.Internal;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

namespace MSCore.Util.Common
{
    public class RSAHelper
    {
        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <param name="withPrivate"></param>
        /// <param name="success">是否成功</param>
        /// <returns></returns>
        public static SecurityKey TryGetKeyParameters(string filePath, bool withPrivate, out bool success)
        {
            success = false;
            var rsa = RSA.Create();
            string filename = withPrivate ? "key.json" : "key.public.json";
            string fileTotalPath = Path.Combine(filePath, filename);
            SecurityKey securityKey = new RsaSecurityKey(rsa);
            if (!File.Exists(fileTotalPath))
            {
                return securityKey;
            }
            else
            {
                if (withPrivate)
                {
                    var privateRSAKey = File.ReadAllText(fileTotalPath).Trim();
                    rsa.ImportParameters(GetPrivateRSAParametersFromPEM(privateRSAKey));
                }
                else
                {
                    var publicRSAKey = File.ReadAllText(fileTotalPath).Trim();
                    rsa.ImportParameters(GetPublicRSAParametersFromPEM(publicRSAKey));
                }


                securityKey = new RsaSecurityKey(rsa);
                success = true;
                return securityKey;
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
                rsa.ImportParameters(GetPublicRSAParametersFromPEM(key));
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
            rsa.ImportParameters(GetPrivateRSAParametersFromPEM(privateTextWriter.ToString()));

            SecurityKey privateKey = new RsaSecurityKey(rsa);

            rsa = RSA.Create();
            rsa.ImportParameters(GetPublicRSAParametersFromPEM(publicTextWriter.ToString()));
            SecurityKey publicKey = new RsaSecurityKey(rsa);

            return withPrivate ? privateKey : publicKey;

        }

        /// <summary>
        /// 解析私钥
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static RSAParameters GetPrivateRSAParametersFromPEM(string pem)
        {
            System.Text.RegularExpressions.Regex privateRSAKeyRegex = new System.Text.RegularExpressions.Regex(@"-----(BEGIN|END) RSA PRIVATE KEY-----[\W]*");
            var privateRSAKey = privateRSAKeyRegex.Replace(pem.ToString(), "");

            var privateKeyBytes = Convert.FromBase64String(privateRSAKey);

            using (var ms = new System.IO.MemoryStream(privateKeyBytes))
            using (var br = new System.IO.BinaryReader(ms))
            {
                var twoBytes = br.ReadUInt16();
                if (twoBytes == 0x8130) br.ReadByte();
                else if (twoBytes == 0x8230) br.ReadInt16();
                else throw new Exception("Unexpected value read binr.ReadUInt16()");

                twoBytes = br.ReadUInt16();
                if (twoBytes != 0x0102) throw new Exception("Unexpected version");

                if (br.ReadByte() != 0x00) throw new Exception("Unexpected value read binr.ReadByte()");

                var rsaParams = new RSAParameters
                {
                    Modulus = br.ReadBytes(GetIntegerSize(br)),
                    Exponent = br.ReadBytes(GetIntegerSize(br)),
                    D = br.ReadBytes(GetIntegerSize(br)),
                    P = br.ReadBytes(GetIntegerSize(br)),
                    Q = br.ReadBytes(GetIntegerSize(br)),
                    DP = br.ReadBytes(GetIntegerSize(br)),
                    DQ = br.ReadBytes(GetIntegerSize(br)),
                    InverseQ = br.ReadBytes(GetIntegerSize(br))
                };

                return rsaParams;
            }
        }

        /// <summary>
        /// 解析公钥
        /// </summary>
        /// <param name="pem"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static RSAParameters GetPublicRSAParametersFromPEM(string pem)
        {
            System.Text.RegularExpressions.Regex publicRSAKeyRegex = new System.Text.RegularExpressions.Regex(@"-----(BEGIN|END) PUBLIC KEY-----[\W]*");
            var publicRSAKey = publicRSAKeyRegex.Replace(pem.ToString(), "");


            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicRSAKey);
            x509size = x509key.Length;

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        throw new Exception("Unexpected value read binr.ReadUInt16()");

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareByteArrays(seq, SeqOID))    //make sure Sequence for OID is correct
                        throw new Exception("Unexpected OID");

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        throw new Exception("Unexpected value read binr.ReadUInt16()");

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        throw new Exception("Unexpected value read binr.ReadByte()");

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        throw new Exception("Unexpected value read binr.ReadUInt16()");

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        throw new Exception("Unexpected value read binr.ReadByte()");
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        throw new Exception("Unexpected value read binr.ReadByte()");
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    RSAParameters RSAKeyInfo = new RSAParameters();
                    RSAKeyInfo.Modulus = modulus;
                    RSAKeyInfo.Exponent = exponent;

                    return RSAKeyInfo;
                }

            }
        }

        private static int GetIntegerSize(System.IO.BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }
    }
}
