using Asa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asa.BackDB
{
    [Serializable]
    public class SettingConfig
    {

        public SettingConfig()
        {
            DataBaseNames = new List<string>();
        }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServiceAddress { get; set; }
        /// <summary>
        /// 数据库登陆名
        /// </summary>
        public string DataBaseLogin { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DataBasePassword { get; set; }
        /// <summary>
        /// 开始备份时间
        /// </summary>
        public string SyncDate { get; set; }
        /// <summary>
        /// 数据保留年份
        /// </summary>
        public int BackupDbYear { get; set; }
        public int ClearMonth { get; set; }
        /// <summary>
        /// 备份间隔
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public string TimeInterval
        {
            get { return $"{this.Day:00}天{this.Hour:00}小时{this.Minute:00}分钟"; }
            set
            {
                string str = value.Replace("天", "-").Replace("小时", "-").Replace("分钟", "-");
                List<string> list = str.Split('-').ToList();
                this.Day = list[0].ObjToInt();
                this.Hour = list[1].ObjToInt();
                this.Minute = list[2].ObjToInt();
            }
        }
        /// <summary>
        /// 备份保存目录
        /// </summary>
        public string BackDirectoryPath { get; set; }
        /// <summary>
        /// 备份命名规则
        /// </summary>
        public string NameRegulation { get; set; }
        /// <summary>
        /// 间隔天数
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// 间隔小时数
        /// </summary>
        public int Hour { get; set; }
        /// <summary>
        /// 间隔分钟
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// 为每个目录创建子目录
        /// </summary>
        public bool IsNewFolder { get; set; }
        /// <summary>
        /// 需要备份的数据库名称集合
        /// </summary>
        public List<string> DataBaseNames { get; set; }

        public void Update()
        {
            XmlHelper.XmlSerialize<SettingConfig>(SystemParam.ConfigFilePath, this);
        }

        public void SetSyncTime(DateTime syncTime)
        {
            this.SyncDate = syncTime.ToString("s");
        }
    }
}
