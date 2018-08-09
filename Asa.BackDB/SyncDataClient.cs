using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using ICSharpCode.SharpZipLib.Zip;
using Asa.Common;

namespace Asa.BackDB
{
    public class SyncDataClient
    {
        private Timer _timer = null;
        private DateTime _syncTime;
        private SettingConfig settingConfig = null;
        private readonly ILog log = LogManager.GetLogger(
                                         typeof(SyncDataClient));
        public SyncDataClient()
        {
            this.log.Info("初始化定时器备份服务");
            this.settingConfig = XmlHelper.XmlDeserialize<SettingConfig>(SystemParam.ConfigFilePath);
            //计算首次执行时间 毫秒数 
            this._syncTime = this.settingConfig.SyncDate.ObjToDateTime();
            this._timer = new Timer(1000) { AutoReset = true };
            this._timer.Elapsed += this.Timer_SyncData;
            log.Info($"起始执行时间：{this._syncTime:s}");
            this.log.Info("初始化定时服务程序完成");

        }
        //同步次数
        private int _mSyncCount = 0;
        //同步日期
        private string _mSyncDate = "";

        #region 启动和停止服务

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            this._timer.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            this._timer.Stop();
        }
        #endregion

        #region # 定时器      private void Timer_SyncData(object sender, ElapsedEventArgs e)
        /// <summary> 定时器</summary>
        private void Timer_SyncData(object sender, ElapsedEventArgs e)
        {
            DateTime nowTime = DateTime.Now;
            //检查是否进入更新
            if (nowTime.ToString("yyyy-MM-dd HH:mm:ss") == this._syncTime.ToString("yyyy-MM-dd HH:mm:ss"))
            {
                //设置下次更新时间
                this._syncTime = this._syncTime.AddDays(this.settingConfig.Day).AddHours(this.settingConfig.Hour)
                    .AddMinutes(this.settingConfig.Minute);

                //更新配置文件
                this.settingConfig.SetSyncTime(this._syncTime);
                this.settingConfig.Update();
                try
                {
                    this._mSyncCount++;
                    string msg = "{0}第{1}次数据库完成".FormatEx(this._mSyncDate, this._mSyncCount);
                    this.log.Info(msg);
                    if (this._mSyncDate != DateTime.Now.ToString("yyyy-MM-dd"))
                    {
                        this._mSyncDate = DateTime.Now.ToString("yyyy-MM-dd");
                        this._mSyncCount = 1;
                    }
                    //同步数据
                    this.SyncOperator();
                    msg = "{0}第{1}次同步数据完成".FormatEx(this._mSyncDate, this._mSyncCount);

                    this.log.Info(msg);
                }
                catch (Exception ex)
                {
                    this.log.Error("同步时出现错误", ex);
                }
                this.log.Info($"下次更新时间：{this._syncTime:s}");

            }
            //如果时间不对，重新设置时间
            if (nowTime > this._syncTime)
            {
                this._syncTime = DateTime.Now.AddSeconds(5);

                //设置下次更新时间
                //this._syncTime = this._syncTime.AddDays(this.settingConfig.Day).AddHours(this.settingConfig.Hour)
                //    .AddMinutes(this.settingConfig.Minute);

                //更新配置文件
                this.settingConfig.SetSyncTime(this._syncTime);
                this.settingConfig.Update();
                log.Info($"发现起始时间已经过期，自动设定其实执行日期：{this._syncTime:s}");
                return;
            }

        }

        #endregion


        #region # 同步操作
        /// <summary> 同步操作</summary>
        private void SyncOperator()
        {
            this.log.Info("开始准备备份数据库。");
            this.CheckDirectory(this.settingConfig.BackDirectoryPath);
            ClearFiles();
            DBHelper.SetConnectionString(this.settingConfig.ServiceAddress, this.settingConfig.DataBaseLogin, this.settingConfig.DataBasePassword);
            foreach (string dbname in this.settingConfig.DataBaseNames)
            {
                if (DBHelper.ExecuteScalar($"select count(*) from sys.databases where name = '{dbname}'").ObjToInt() ==
                    0)
                {
                    this.log.Info($"没有发现数据库：{dbname},该库名将被忽略。");
                    continue;
                }
                string saveFileName = DateTime.Now.ToString(this.settingConfig.NameRegulation).Replace("{DB}", dbname);
                string saveFolder = this.settingConfig.BackDirectoryPath;

                if (this.settingConfig.IsNewFolder)
                {
                    saveFolder = Path.Combine(saveFolder, dbname);
                    this.CheckDirectory(saveFolder);
                }
                saveFileName = Path.Combine(saveFolder, saveFileName);
                this.log.Info($"开始准备备份数据库:{dbname},备份后的文件目录：{saveFileName}.bak");

                //还原的数据库MyDataBase
                string sql = $"BACKUP DATABASE {dbname} TO DISK = '{saveFileName}.bak' ";
                DBHelper.ExecuteNonQuery(sql);
                this.log.Info($"数据库{dbname}备份完成,开始准备压缩数据库文件。");
                string[] fileNames = ZipDataBase(saveFileName, saveFolder);

                this.log.Info($"删除压缩前文件完成，删除的文件：{string.Join(",", fileNames)}。");
            }
            //关闭数据库连接
            DBHelper.Connection.Close();
        }
        /// <summary>
        /// 清理过期文件
        /// </summary>
        /// <param name="saveFolder"></param>
        private void ClearFiles()
        {
            log.Info("检查是否有过期文件，准备清除。");
            string folder = settingConfig.BackDirectoryPath;
            string[] filenames = Directory.GetFiles(folder, "*.zip", SearchOption.AllDirectories);
            List<FileInfo> fileinfos = (from x in filenames select new FileInfo(x)).ToList();
            log.Info($"检查已经过期的内容，过期的直接删除，备份保留期限：{settingConfig.BackupDbYear}");
            DateTime delDate = DateTime.Now.AddYears(-settingConfig.BackupDbYear);
            foreach (FileInfo fileinfo in fileinfos.FindAll(p => p.CreationTime < delDate))
            {
                log.Info($"开始删除过期备份文件：{fileinfo.Name}");
                fileinfo.Delete();
            }

            log.Info($"开始检查每月保存一个备份的文件，当前备份前{settingConfig.ClearMonth}天，数据保留原有设置，之后每月保留一份备份文件。");
            DateTime clearDate = DateTime.Now.AddDays(-settingConfig.ClearMonth);
            List<DateTime> NoDel = new List<DateTime>();
            foreach (FileInfo fileinfo in fileinfos.FindAll(p => p.CreationTime < clearDate).OrderBy(p => p.CreationTime))
            {
                if (NoDel.Exists(p => p.Year == fileinfo.CreationTime.Year && fileinfo.CreationTime.Month == p.Month))
                {
                    log.Info($"开始删除过期备份文件：{fileinfo.Name}");
                    fileinfo.Delete();
                }
                else
                {
                    log.Info($"保留每月一份的文件：{fileinfo.Name}");
                    NoDel.Add(fileinfo.CreationTime);
                }
            }



            log.Info("清理过期文件操作完成");

        }

        /// <summary>
        /// 压缩备份数据库文件，压缩完成后删除bak文件，只保留包
        /// </summary>
        /// <param name="saveFileName"></param>
        /// <param name="saveFolder"></param>
        /// <returns></returns>
        private string[] ZipDataBase(string saveFileName, string saveFolder)
        {
            string[] fileNames = Directory.GetFiles(saveFolder, "*.bak");
            using (ZipFile zip = ZipFile.Create($"{saveFileName}.zip"))
            {
                zip.BeginUpdate();
                //添加文件 
                foreach (string fileName in fileNames)
                {
                    zip.Add(fileName);
                }
                zip.CommitUpdate();

            }
            this.log.Info($"压缩文件完成，压缩包地址：{saveFileName}.zip,开始准备删除备份文件。");
            foreach (string fileName in fileNames)
            {
                File.Delete(fileName);
            }

            return fileNames;
        }


        /// <summary>
        /// 检查备份目录是否存在
        /// </summary>
        private void CheckDirectory(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                this.log.Info($"发现备份目录不存在，开始创建目录,目录:{dirName}");
                Directory.CreateDirectory(dirName);

            }
        }
        #endregion
    }
}
