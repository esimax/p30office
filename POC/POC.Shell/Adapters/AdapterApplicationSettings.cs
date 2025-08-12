using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using POL.Lib.Interfaces;

namespace POC.Shell.Adapters
{
    internal class AdapterApplicationSettings : IApplicationSettings
    {
        static Dictionary<object, object> Cach { get; set; }
        static AdapterApplicationSettings()
        {
            Cach = new Dictionary<object, object>();
        }
        #region IApplicationSettings
        public TResult GetValue<TResult, TEnum>(TEnum enumName)
        {
            return GetValue<TResult, TEnum>(enumName, false);
        }

        public TResult GetValue<TResult, TEnum>(TEnum enumName, bool fromCach)
        {
            return GetValue<TResult, TEnum>(enumName, fromCach, false);
        }

        public TResult GetValue<TResult, TEnum>(TEnum enumName, bool fromCach, bool isEncripted)
        {
            if (!(typeof(TEnum).IsEnum)) return default(TResult);
            if (isEncripted)
            {
                if (typeof(TResult) != typeof(string))
                    throw new ArgumentOutOfRangeException("TResult", "sould be string when using encription");
            }
            if (fromCach)
            {
                if (Cach.ContainsKey(enumName))
                    return (TResult)Cach[enumName];
                throw new IndexOutOfRangeException("There is no value in the cach");
            }
            TResult result;
            if (typeof(TResult).IsEnum)
            {
                var data = RootKey.GetValue(enumName.ToString());
                int i = Convert.ToInt32(data);
                result = (TResult)Convert.ChangeType(i, typeof(int));
            }
            else
            {
                var data = RootKey.GetValue(enumName.ToString());
                if (data == null)
                    return default(TResult);
                result =
                    (TResult)
                    Convert.ChangeType(
                        isEncripted
                            ? Decrypt(RootKey.GetValue(enumName.ToString()).ToString(), false)
                            : RootKey.GetValue(enumName.ToString()), typeof(TResult));
            }
            if (Cach.ContainsKey(enumName))
                Cach[enumName] = result;
            else
                Cach.Add(enumName, result);
            return result;
        }

        public void SetValue<TResult, TEnum>(TEnum enumName, TResult value)
        {
            SetValue(enumName, value, false);
        }

        public void SetValue<TResult, TEnum>(TEnum enumName, TResult value, bool isEncripted)
        {
            if (!(typeof(TEnum).IsEnum)) throw new ArgumentOutOfRangeException("Please use Enum as enumName.");
            if (isEncripted)
            {
                if (typeof(TResult) != typeof(string))
                    throw new ArgumentOutOfRangeException("TResult sould be string when using encription");
            }
            if (typeof(TResult) == typeof(string))
            {
                var v = (value == null ? string.Empty : value.ToString());
                var sval = isEncripted ? Encrypt(v, false) : v;
                RootKey.SetValue(enumName.ToString(), sval, RegistryValueKind.String);
            }
            else if (typeof(TResult) == typeof(int))
            {
                RootKey.SetValue(enumName.ToString(), value, RegistryValueKind.DWord);
            }
            else if (typeof(TResult) == typeof(Int64))
            {
                RootKey.SetValue(enumName.ToString(), value, RegistryValueKind.QWord);
            }
            else if (typeof(TResult) == typeof(bool))
            {
                RootKey.SetValue(enumName.ToString(), value, RegistryValueKind.DWord);
            }
            else if (typeof(TResult) == typeof(DateTime))
            {
                RootKey.SetValue(enumName.ToString(), Convert.ToDateTime(value).Ticks, RegistryValueKind.String);
            }
            else if (typeof(TResult).IsEnum)
            {
                RootKey.SetValue(enumName.ToString(), Convert.ToInt32(value), RegistryValueKind.DWord);
            }


            if (Cach.ContainsKey(enumName))
            {
                Cach[enumName] = value;
            }
            else
            {
                Cach.Add(enumName, value);
            }
        }
        #endregion

        #region RootKey
        private static RegistryKey _rootKey;
        private static RegistryKey RootKey
        {
            get
            {
                if (_rootKey == null)
                    try
                    {
                        var registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE");
                        if (registryKey != null)
                        {
                            var subKey = registryKey.CreateSubKey("P30Office3");
                            if (subKey != null)
                                _rootKey = subKey.CreateSubKey("Client");
                        }
                    }
                    catch
                    {
                    }
                return _rootKey;
            }
        } 
        #endregion

        #region [METHODS]
        private string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = System.Text.UnicodeEncoding.UTF8.GetString(new byte[] { 23, 45, 68, 97, 35, 76, 87, 1, 3, 09, 35, 77, 89, 00, 00, 00 });

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        private string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);


            string key = System.Text.UnicodeEncoding.UTF8.GetString(new byte[] { 23, 45, 68, 97, 35, 76, 87, 1, 3, 09, 35, 77, 89, 00, 00, 00 });

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;

            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        } 
        #endregion
    }
}
