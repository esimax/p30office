using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace POL.Lib.Utils
{
    public class HelperSecurity
    {
        public static string ComputeSaltHashSHA512(string plainText, byte[] saltBytes)
        {
            if (saltBytes == null)
            {
                const int minSaltSize = 123;
                const int maxSaltSize = 345;

                var random = new Random();
                var saltSize = random.Next(minSaltSize, maxSaltSize);

                saltBytes = new byte[saltSize];

                var rng = new RNGCryptoServiceProvider();

                rng.GetNonZeroBytes(saltBytes);
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            var plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            for (var i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            for (var i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            HashAlgorithm hash = new SHA512Managed();

            var hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            var hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            for (var i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            for (var i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            var hashValue = Convert.ToBase64String(hashWithSaltBytes);

            return hashValue;
        }

        public static bool VerifySaltHashSHA512(string plainText, string hashValue)
        {
            var hashWithSaltBytes = Convert.FromBase64String(hashValue);


            const int hashSizeInBits = 512;


            const int hashSizeInBytes = hashSizeInBits/8;

            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            var saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

            for (var i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            var expectedHashString = ComputeSaltHashSHA512(plainText, saltBytes);

            return hashValue == expectedHashString;
        }


        public static byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            var ms = new MemoryStream();

            var alg = Rijndael.Create();


            alg.Key = key;
            alg.IV = iv;

            var cs = new CryptoStream(ms,
                alg.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(clearData, 0, clearData.Length);

            cs.Close();

            var encryptedData = ms.ToArray();

            return encryptedData;
        }

        public static string Encrypt(string clearText, string password)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);

            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            var encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return Convert.ToBase64String(encryptedData);
        }


        public static byte[] Encrypt(byte[] clearData, string password)
        {
            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            return Encrypt(clearData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        public static void Encrypt(string fileIn, string fileOut, string password)
        {
            var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
            var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);

            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            var alg = Rijndael.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            var cs = new CryptoStream(fsOut, alg.CreateEncryptor(), CryptoStreamMode.Write);

            const int bufferLen = 4096;
            var buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                bytesRead = fsIn.Read(buffer, 0, bufferLen);
                cs.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);


            cs.Close();
            fsIn.Close();
        }

        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            var ms = new MemoryStream();

            var alg = Rijndael.Create();

            alg.Key = key;
            alg.IV = iv;

            var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(cipherData, 0, cipherData.Length);

            cs.Close();

            var decryptedData = ms.ToArray();

            return decryptedData;
        }


        public static string Decrypt(string cipherText, string password)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});

            var decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return Encoding.Unicode.GetString(decryptedData);
        }


        public static byte[] Decrypt(byte[] cipherData, string password)
        {
            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});


            return Decrypt(cipherData, pdb.GetBytes(32), pdb.GetBytes(16));
        }

        public static void Decrypt(string fileIn, string fileOut, string password)
        {
            var fsIn = new FileStream(fileIn, FileMode.Open, FileAccess.Read);
            var fsOut = new FileStream(fileOut, FileMode.OpenOrCreate, FileAccess.Write);

            var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
            var alg = Rijndael.Create();

            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            var cs = new CryptoStream(fsOut, alg.CreateDecryptor(), CryptoStreamMode.Write);

            const int bufferLen = 4096;
            var buffer = new byte[bufferLen];
            int bytesRead;

            do
            {
                bytesRead = fsIn.Read(buffer, 0, bufferLen);

                cs.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);

            cs.Close(); 
            fsIn.Close();
        }
    }
}
