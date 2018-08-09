using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asa.Common
{
    /// <summary>
    /// 强制转换辅助类 
    /// </summary>
   public static class ConvertHelper
    {
        /// <summary>
        /// 取第一个指定字符串之后的字符串
        /// </summary>
        /// <param name=""></param>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ObjToCharAfterString(this object obj,char args='_')
        {
            string result = string.Empty;
            if (obj!=null)
            {
                string str = obj.ToString();
                if (str.IndexOf(args)!=-1)
                {
                    result = str.Substring(str.LastIndexOf(args)).Trim(args);
                }
            }
            return result;
        }
        /// <summary>
        ///  取最后一个指定字符之后的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToCharLastAfterString(this object obj, char args = '_')
        {
            string result = string.Empty;
            if (obj != null)
            {
                string str = obj.ToString();
                if (str.LastIndexOf(args) != -1)
                {
                    result = str.Substring(str.LastIndexOf(args)).Trim(args);
                }
            }
            return result;
        }

        /// <summary>
        ///  取第一个指定字符之前的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToCharBeforeString(this object obj, char args = '_')
        {
            string result = string.Empty;
            if (obj != null)
            {
                string str = obj.ToString();
                if (str.IndexOf(args) != -1)
                {
                    result = str.Substring(0, str.IndexOf(args)).Trim(args);
                }
            }
            return result;
        }

        /// <summary>
        ///  取第一个指定字符之前的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToCharLastString(this object obj, char args = '_')
        {
            string result = string.Empty;
            if (obj != null)
            {
                string str = obj.ToString();
                if (str.LastIndexOf(args) != -1)
                {
                    result = str.Substring(str.LastIndexOf(args)).Trim(args);
                }
            }
            return result;
        }

        /// <summary>
        ///  取最后一个指定字符之前的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToCharLastBeforeString(this object obj, char args = '_')
        {
            string result = string.Empty;
            if (obj != null)
            {
                string str = obj.ToString();
                if (str.LastIndexOf(args) != -1)
                {
                    result = str.Substring(0, str.LastIndexOf(args)).Trim(args);
                }
            }
            return result;
        }
        /// <summary>
        /// 将object类型对象转换到指定格式的字符串，如果对象null，则返回empty，格式无法转换datetime，则返回对象原型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ObjToDateFormat(this object obj, string format)
        {

            if (obj == null)
            {
                return string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(format))
                {
                    format = "yyyy-MM-dd HH:mm:ss";
                }
                try
                {
                    return Convert.ToDateTime(obj).ToString(format);
                }
                catch
                {
                    return obj.ToString();
                }
            }
        }

        #region 强制转化
        /// <summary>
        /// object转化为Bool类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ObjToBool(this object obj)
        {
            bool flag;
            if (obj == null)
            {
                return false;
            }
            if (obj.Equals(DBNull.Value))
            {
                return false;
            }
            return (bool.TryParse(obj.ToString(), out flag) && flag);
        }
        /// <summary>
        /// object强制转化为DateTime类型(吃掉异常)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ObjToDateNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            try
            {
                return new DateTime?(Convert.ToDateTime(obj));
            }
            catch (ArgumentNullException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// object强制转化为DateTime类型(吃掉异常)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime ObjToDateTime(this object obj)
        {
            if (string.IsNullOrEmpty(obj.ToString()))
            {
                return new DateTime(1970, 1, 1, 0, 0, 0);
            }
            try
            {
                return Convert.ToDateTime(obj.ToString().Trim());
            }
            catch (ArgumentNullException ex)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0);
            }
        }
        /// <summary>
        /// int强制转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ObjToInt(this object obj)
        {
            if (obj != null)
            {
                int num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (int.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }
        /// <summary>
        /// 强制转化为long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long ObjToLong(this object obj)
        {
            if (obj != null)
            {
                long num;
                if (obj.Equals(DBNull.Value))
                {
                    return 0;
                }
                if (long.TryParse(obj.ToString(), out num))
                {
                    return num;
                }
            }
            return 0;
        }
        /// <summary>
        /// 强制转化可空int类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? ObjToIntNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new int?(ObjToInt(obj));
        }
        /// <summary>
        /// 强制转化为string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjToStr(this object obj)
        {
            if (obj == null)
            {
                return "";
            }
            if (obj.Equals(DBNull.Value))
            {
                return "";
            }
            return Convert.ToString(obj);
        }
        /// <summary>
        /// Decimal转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal ObjToDecimal(this object obj)
        {
            if (obj == null)
            {
                return 0M;
            }
            if (obj.Equals(DBNull.Value))
            {
                return 0M;
            }
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch
            {
                return 0M;
            }
        }
        /// <summary>
        /// Decimal可空类型转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? ObjToDecimalNull(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj.Equals(DBNull.Value))
            {
                return null;
            }
            return new decimal?(ObjToDecimal(obj));
        }
        #endregion
    }
}
