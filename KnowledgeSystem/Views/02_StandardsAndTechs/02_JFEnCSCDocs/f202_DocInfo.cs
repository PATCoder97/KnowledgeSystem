using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Drawing.Charts;
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
        public string idBase202 = "";
        string idDept2word;

        dt202_Base docBase;

        BindingSource sourceAtts = new BindingSource();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;
        List<dm_Attachment> attachments;
        List<dm_User> users;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbTWName.Enabled = _enable;
            txbENVNName.Enabled = _enable;
            cbbTypeOf.Enabled = _enable;
            txbKeyword.Enabled = _enable;
            cbbRequestUsr.Enabled = _enable;
        }

        private void LockControl()
        {
            txbTWName.Enabled = false;
            txbENVNName.Enabled = false;
            cbbTypeOf.Enabled = false;
            txbKeyword.Enabled = false;
            cbbRequestUsr.Enabled = false;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

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

            attachments = new List<dm_Attachment>();

            var users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            cbbRequestUsr.Properties.DataSource = users;
            cbbRequestUsr.Properties.DisplayMember = "DisplayName";
            cbbRequestUsr.Properties.ValueMember = "Id";

            var typeOfs = dt202_TypeBUS.Instance.GetList();
            cbbTypeOf.Properties.Items.AddRange(typeOfs.Select(r => r.DisplayName).ToList());

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    docBase = new dt202_Base();
                    break;
                case EventFormInfo.View:
                    docBase = dt202_BaseBUS.Instance.GetItemById(idBase202);

                    string[] displayName = docBase.DisplayName.Split(new[] { "\n" }, StringSplitOptions.None);
                    txbTWName.EditValue = displayName[0];
                    txbENVNName.EditValue = displayName.Count() > 1 ? displayName[1] : "";
                    cbbTypeOf.EditValue = docBase.TypeOf;
                    txbKeyword.EditValue = docBase.Keyword;
                    cbbRequestUsr.EditValue = docBase.RequestUsr;

                    break;
            }
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

            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                dm_Attachment attachment = new dm_Attachment
                {
                    Thread = "202",
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}"
                };
                attachments.Add(attachment);
                Thread.Sleep(5);
            }

            sourceAtts.DataSource = attachments;
            lbCountFile.Text = $"共{attachments.Count}個附件";
            gvFiles.RefreshData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Lấy các thông tin người dùng từ giao diện
                dt202_Base docBase = new dt202_Base();

                docBase.DisplayName = $"{txbTWName.EditValue?.ToString().Trim()}\n{txbENVNName.EditValue?.ToString().Trim()}";
                docBase.TypeOf = (int)cbbTypeOf.SelectedIndex;
                docBase.Keyword = txbKeyword.EditValue?.ToString();
                docBase.RequestUsr = cbbRequestUsr.EditValue?.ToString();
                docBase.UploadTime = DateTime.Now;
                docBase.UsrUpload = TPConfigs.LoginUser.Id;
                string fileName = txbFilePath.EditValue?.ToString();

                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        docBase.Id = dt202_BaseBUS.Instance.GetNewBaseId(TPConfigs.LoginUser.IdDepartment);

                        string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                        dm_Attachment attachment = new dm_Attachment
                        {
                            Thread = "202",
                            ActualName = Path.GetFileName(fileName),
                            EncryptionName = $"{encryptionName}"
                        };
                        int idAttach = dm_AttachmentBUS.Instance.Add(attachment);
                        docBase.IdFile = idAttach;
                        result = dt202_BaseBUS.Instance.Add(docBase);

                        if (result)
                        {
                            foreach (var item in attachments)
                            {
                                idAttach = dm_AttachmentBUS.Instance.Add(item);
                                dt202_AttachBUS.Instance.Add(new dt202_Attach() { IdBase = docBase.Id, IdAttach = idAttach });
                            }
                        }

                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:

                        //result = dt302_BaseBUS.Instance.AddOrUpdate(personBase);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認刪除人員: {docBase.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        //result = dm_UserBUS.Instance.Remove(userInfo.Id);
                        //dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);
                        break;
                    default:
                        break;
                }
            }

            msg = $"{docBase.Id} {docBase.DisplayName} {docBase.RequestUsr}";
            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void txbFilePath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = TPConfigs.FilterFile,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            txbFilePath.EditValue = openFileDialog.FileName;
        }
    }
}