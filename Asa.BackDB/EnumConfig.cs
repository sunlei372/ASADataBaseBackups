using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asa.BackDB
{
    public enum ClearMonth
    {
        全部都清理 = 0,
        忽略前30天 = 30,
        忽略前60天 = 60,
        忽略前90天 = 90,
    }
    /// <summary>
    /// 绑定对象类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BindComboxEnumType<T>
    {
        /// <summary>
        /// 类型的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public T Type { get; set; }

        private static readonly List<BindComboxEnumType<T>> bindTyps;

        static BindComboxEnumType()
        {
            bindTyps = new List<BindComboxEnumType<T>>();

            foreach (var name in Enum.GetNames(typeof(T)))
            {
                bindTyps.Add(new BindComboxEnumType<T>()
                {
                    Name = name,
                    Type = (T)Enum.Parse(typeof(T), name)
                });
            }
        }

        /// <summary>
        /// 绑定的类型数据
        /// </summary>
        public static List<BindComboxEnumType<T>> BindTyps
        {
            get { return bindTyps; }
        }
    }
}
