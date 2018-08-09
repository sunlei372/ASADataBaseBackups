using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Asa.Common
{
    public static class StringEX
    {
        public static string FormatEx(this string strFormat, params object[] objs)
        {
            return string.Format(strFormat, objs);
        }
        public static bool IsNullOrEmptyEx(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNullOrWhiteSpaceEx(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        #region # 计算16位MD5值扩展方法 —— static string ToHash16(this string text)
        /// <summary>
        /// 计算16位MD5值扩展方法
        /// </summary>
        /// <param name="text">待转换的字符串</param>
        /// <returns>16位MD5值</returns>
        public static string ToHash16(this string text)
        {
            MD5CryptoServiceProvider md5Crypto = new MD5CryptoServiceProvider();
            byte[] buffer = md5Crypto.ComputeHash(Encoding.Default.GetBytes(text));
            string hash = BitConverter.ToString(buffer, 4, 8);
            hash = hash.Replace("-", "");

            return hash;
        }
        #endregion
    }
}
