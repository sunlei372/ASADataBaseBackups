using Asa.BackDB;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Topshelf;
using log4net.Config;
namespace Asa.BackDB
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            string p1 = string.Empty;
            if (args.Length == 1)
            {
                p1 = args.First();
            }
            //p1 = "setting";

            switch (p1)
            {
                case "setting":
                    Application.Run(new Settings());
                    break;
                default:
                    Program.RunService();
                    break;
            }
        }

        private static void RunService()
        {
            HostFactory.Run(x =>
            {
                x.Service<SyncDataClient>(s =>
                {
                    s.ConstructUsing(name => new SyncDataClient());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("本服务负责定时备份指定数据，并压缩为zip压缩包，如有必要，可以将压缩包上传到指定服务器。");
                x.SetDisplayName("Realgoal数据数据库备份服务");
                x.SetServiceName("RealgoalBackupDataBaseService");
            });
        }

    }
}
