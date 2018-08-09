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
    }
}
