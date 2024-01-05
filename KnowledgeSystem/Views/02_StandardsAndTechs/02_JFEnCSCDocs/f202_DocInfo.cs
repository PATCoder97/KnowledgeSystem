using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs
{
    public partial class f202_DocInfo : DevExpress.XtraEditors.XtraForm
    {
        public f202_DocInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public int idBase302 = -1;
        string idDept2word;

        dt302_Base personBase;

        BindingSource sourceAtts = new BindingSource();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;
        List<dm_Attachment> lsAttachments;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            //cbbSupervisor.Enabled = _enable;
            //txbSchool.Enabled = _enable;
            //txbMajor.Enabled = _enable;
            // txbDateStart.Enabled = _enable;
        }

        private void LockControl()
        {
            //txbUserId.Enabled = false;
            //txbUserNameVN.Enabled = false;
            //cbbDept.Enabled = false;
            //cbbJobTitle.Enabled = false;
            //txbDateStart.Enabled = false;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                   // txbUserId.Enabled = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    //txbUserId.Enabled = true;
                    //txbUserId.ReadOnly = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f202_DocInfo_Load(object sender, EventArgs e)
        {
            Text = "新增文件";
            idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            lcControls = new List<LayoutControlItem>() { lcTWName, lcENVNName, lcTypeOf, lcKeyword, lcRequestUsr };
            lcImpControls = new List<LayoutControlItem>() { lcTWName, lcTypeOf, lcKeyword, lcRequestUsr };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            LockControl();

            // Set datasource cho gridcontrol
            gcFiles.DataSource = sourceAtts;

            lsAttachments = new List<dm_Attachment>();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = TPConfigs.FilterFile,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            int index = 0;
            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                dm_Attachment attachment = new dm_Attachment
                {
                    Thread = "202",
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}{index++}"
                };
                lsAttachments.Add(attachment);
                Thread.Sleep(5);
            }

            sourceAtts.DataSource = lsAttachments;
            lbCountFile.Text = $"共{lsAttachments.Count}個附件";
            gvFiles.RefreshData();
        }
    }
}