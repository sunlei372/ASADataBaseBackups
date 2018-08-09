using Asa.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asa.BackDB
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Add a DataBase Name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddDbName_Click(object sender, EventArgs e)
        {
            if (this.txtDbName.Text.IsNullOrEmptyEx())
            {
                MessageBox.Show("请输入数据库名称");
                return;
            }

            DBHelper.SetConnectionString(this.txtServiceIp.Text, this.txtDbLoginName.Text, this.txtDbPassword.Text);
            if (DBHelper.ExecuteScalar($"select count(*) from sys.databases where name = '{this.txtDbName.Text}'").ObjToInt() ==
                0)
            {
                MessageBox.Show($"数据库名称：{this.txtDbName.Text}不存在，请检查数据库在试~！");
                return;
            }



            foreach (object item in this.listDb.Items)
            {
                if (item.ObjToStr() == this.txtDbName.Text)
                {
                    MessageBox.Show("数据库名称已经存在");
                    return;
                }
            }

            this.listDb.Items.Add(this.txtDbName.Text);
            this.txtDbName.Text = string.Empty;
        }

        /// <summary>
        /// Delete DataBase Name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DelDbName_Click(object sender, EventArgs e)
        {
            if (this.listDb.SelectedItems.Count == 0)
            {
                MessageBox.Show("没有选择要删除的数据名称");
                return;
            }
            List<string> strs = new List<string>();
            foreach (object item in this.listDb.SelectedItems)
            {
                strs.Add(item.ObjToStr());
            }

            foreach (string str in strs)
            {
                this.listDb.Items.Remove(str);
            }
        }

        /// <summary>
        /// Setting Directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetDirectory_Click(object sender, EventArgs e)
        {
            if (this.GetDirPathDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtSaveDirectory.Text = this.GetDirPathDialog.SelectedPath;
            }
        }

        /// <summary>
        /// save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            SettingConfig configinfo = new SettingConfig()
            {
                BackDirectoryPath = this.txtSaveDirectory.Text,
                ServiceAddress = this.txtServiceIp.Text,
                TimeInterval = this.txtTimeInterval.Text,
                SyncDate = this.txtStartDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                NameRegulation = this.txtCreateNameRegulation.Text,
                DataBaseLogin = this.txtDbLoginName.Text,
                DataBasePassword = this.txtDbPassword.Text,
                IsNewFolder = this.ckIsNewFolder.Checked,
                BackupDbYear = this.cbBakYear.SelectedIndex,
                ClearMonth = (int)this.cbClearMonth.SelectedValue

            };

            foreach (object item in this.listDb.Items)
            {
                configinfo.DataBaseNames.Add(item.ObjToStr());
            }

            if (configinfo.DataBaseNames.Count == 0)
            {
                MessageBox.Show("没有设置要备份的数据库，无法保存。", "系统提示");
                return;
            }
            XmlHelper.XmlSerialize<SettingConfig>(SystemParam.ConfigFilePath, configinfo);
            if (MessageBox.Show("保存成功,点击是后，窗口关闭。", "系统提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }

        }

        /// <summary>
        /// Cancel Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("取消操作后，窗口将关闭，是否取消操作？", "系统提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.cbClearMonth.DataSource = Asa.BackDB.BindComboxEnumType<ClearMonth>.BindTyps;
            this.cbClearMonth.DisplayMember = "Name";
            this.cbClearMonth.ValueMember = "Type";

            this.cbBakYear.SelectedIndex = 1;
            this.listDb.Items.Clear();
            this.AssertExistsXmlConfig();
            SettingConfig config = XmlHelper.XmlDeserialize<SettingConfig>(SystemParam.ConfigFilePath);
            this.txtServiceIp.Text = config.ServiceAddress;
            this.txtCreateNameRegulation.Text = config.NameRegulation;
            this.txtDbLoginName.Text = config.DataBaseLogin;
            this.txtDbPassword.Text = config.DataBasePassword;

            this.txtSaveDirectory.Text = config.BackDirectoryPath;
            this.txtTimeInterval.Text = config.TimeInterval;
            this.ckIsNewFolder.Checked = config.IsNewFolder;

            this.cbClearMonth.SelectedValue = (ClearMonth)config.ClearMonth;
            this.cbBakYear.SelectedIndex = config.BackupDbYear;

            foreach (string name in config.DataBaseNames)
            {
                this.listDb.Items.Add(name);
            }

            this.txtStartDate.Value = config.SyncDate.ObjToDateTime();
        }

        private void AssertExistsXmlConfig()
        {
            if (!File.Exists(SystemParam.ConfigFilePath))
            {
                SettingConfig configinfo = new SettingConfig()
                {
                    BackDirectoryPath = "d:\\DataBase\\BackupDB",
                    ServiceAddress = ".",
                    TimeInterval = "00天00小时01分钟",
                    SyncDate = DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    NameRegulation = "{DB}-yyyyMMddHHmmssfff",
                    DataBaseLogin = "sa",
                    DataBasePassword = "1234",
                    IsNewFolder = true,
                    BackupDbYear = 1,
                    ClearMonth = 0


                };
                XmlHelper.XmlSerialize<SettingConfig>(SystemParam.ConfigFilePath, configinfo);
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                DBHelper.SetConnectionString(this.txtServiceIp.Text, this.txtDbLoginName.Text, this.txtDbPassword.Text);
                if (DBHelper.Connection.State == ConnectionState.Open)
                {
                    MessageBox.Show("连接成功。", "系统提示");
                }
                else
                {
                    MessageBox.Show("连接失败，请检查配置。", "系统提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
