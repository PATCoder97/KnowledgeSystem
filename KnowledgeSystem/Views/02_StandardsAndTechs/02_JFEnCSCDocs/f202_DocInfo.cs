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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Utils.Filtering.ExcelFilterOptions;

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
        List<Attachment> attachments;
        List<dm_User> users;

        private class Attachment : dm_Attachment
        {
            public string PathFile { get; set; }
            public dm_Attachment BaseAttachment { get; set; } = new dm_Attachment();
        }


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
            cbbHalfYear.Enabled = _enable;
            txbKeyword.Enabled = _enable;
            txbRequestUsr.Enabled = _enable;
            txbFilePath.Enabled = _enable;
            tabAttachments.Visibility = _enable ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }

        private void LockControl()
        {
            //txbTWName.Enabled = false;
            //txbENVNName.Enabled = false;
            //cbbTypeOf.Enabled = false;
            //txbKeyword.Enabled = false;
            //cbbRequestUsr.Enabled = false;
            //txbFilePath.Enabled = false;

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

            lcControls = new List<LayoutControlItem>() { lcTWName, lcENVNName, lcTypeOf, lcKeyword, lcRequestUsr, lcFilePath, lcHalfYear };
            lcImpControls = new List<LayoutControlItem>() { lcTWName, lcTypeOf, lcKeyword, lcRequestUsr, lcFilePath, lcHalfYear };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            LockControl();

            // Set datasource cho gridcontrol
            gcFiles.DataSource = sourceAtts;

            attachments = new List<Attachment>();

            var typeOfs = dt202_TypeBUS.Instance.GetList();
            cbbTypeOf.Properties.Items.AddRange(typeOfs.Select(r => r.DisplayName).ToList());

            // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;
            int startYear = 2019;

            // Sử dụng LINQ để tạo danh sách các năm với "上半年" và "下半年"
            var yearsList = Enumerable.Range(startYear, currentYear - startYear + 1)
                .SelectMany(year => new[] { year + "上半年", year + "下半年" }).OrderByDescending(year => year).ToList();
            cbbHalfYear.Properties.Items.AddRange(yearsList);
            //cbbHalfYear.SelectedIndex = 0;

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
                    cbbTypeOf.EditValue = typeOfs.FirstOrDefault(r => r.Id == docBase.TypeOf).DisplayName;
                    txbKeyword.EditValue = docBase.Keyword;
                    txbRequestUsr.EditValue = docBase.RequestUsr;
                    cbbHalfYear.EditValue = docBase.HalfYear;

                    var att = dt202_AttachBUS.Instance.GetListByBase(idBase202);
                    attachments = dm_AttachmentBUS.Instance.GetListById(att.Select(r => r.IdAttach).ToList())
                        .Select(r => new Attachment()
                        {
                            ActualName = r.ActualName,
                            EncryptionName = r.EncryptionName,
                            Thread = r.Thread
                        }).ToList();
                    sourceAtts.DataSource = attachments;
                    lbCountFile.Text = $"共{attachments.Count}個附件";

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
                Attachment attachment = new Attachment
                {
                    Thread = "202",
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}",
                    PathFile = fileName
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
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            if (string.IsNullOrEmpty(txbTWName.EditValue?.ToString())) IsValidate = false;
            if (string.IsNullOrEmpty(cbbTypeOf.EditValue?.ToString())) IsValidate = false;
            if (string.IsNullOrEmpty(txbKeyword.EditValue?.ToString())) IsValidate = false;
            if (string.IsNullOrEmpty(txbRequestUsr.EditValue?.ToString())) IsValidate = false;
            if (string.IsNullOrEmpty(cbbHalfYear.EditValue?.ToString())) IsValidate = false;
            if (string.IsNullOrEmpty(txbFilePath.EditValue?.ToString()) && eventInfo == EventFormInfo.Create) IsValidate = false;

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string nameTW = Regex.Replace(txbTWName.EditValue?.ToString().Trim(), @"\s+", " ");
                string nameEVN = Regex.Replace(txbENVNName.EditValue?.ToString().Trim(), @"\s+", " ");
                docBase.DisplayName = $"{nameTW}\n{nameEVN}";

                docBase.HalfYear = cbbHalfYear.EditValue?.ToString();
                docBase.TypeOf = (int)cbbTypeOf.SelectedIndex;
                docBase.RequestUsr = txbRequestUsr.EditValue?.ToString().Replace(" ", "");
                docBase.UploadTime = DateTime.Now;
                docBase.UsrUpload = TPConfigs.LoginUser.Id;
                string fileName = txbFilePath.EditValue?.ToString();

                string _keyword = Regex.Replace(txbKeyword.Text, @"[\t\n\r\s]+", match =>
                {
                    if (match.Value.Contains("\n"))
                    {
                        return "\r\n";
                    }
                    else
                    {
                        return " ";
                    }
                }).Trim();
                docBase.Keyword = _keyword;

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
                        File.Copy(fileName, Path.Combine(TPConfigs.Folder202, attachment.EncryptionName), true);

                        int idAttach = dm_AttachmentBUS.Instance.Add(attachment);
                        docBase.IdFile = idAttach;
                        result = dt202_BaseBUS.Instance.Add(docBase);

                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                            attachment = new dm_Attachment
                            {
                                Thread = "202",
                                ActualName = Path.GetFileName(fileName),
                                EncryptionName = $"{encryptionName}"
                            };
                            File.Copy(fileName, Path.Combine(TPConfigs.Folder202, attachment.EncryptionName), true);

                            idAttach = dm_AttachmentBUS.Instance.Add(attachment);
                            docBase.IdFile = idAttach;
                        }

                        result = dt202_BaseBUS.Instance.AddOrUpdate(docBase);

                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認刪除文件: {docBase.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        // result = dt202_BaseBUS.Instance.(userInfo.Id);
                        break;
                    default:
                        break;
                }

                if (result)
                {
                    dt202_AttachBUS.Instance.RemoveRangeByIdBase(docBase.Id);

                    foreach (var item in attachments)
                    {
                        dm_Attachment att = new dm_Attachment()
                        {
                            ActualName = item.ActualName,
                            EncryptionName = item.EncryptionName,
                            Thread = item.Thread,
                        };

                        int idAtt = dm_AttachmentBUS.Instance.Add(att);
                        dt202_AttachBUS.Instance.Add(new dt202_Attach() { IdBase = docBase.Id, IdAttach = idAtt });

                        if (string.IsNullOrEmpty(item.PathFile)) continue;

                        File.Copy(item.PathFile, Path.Combine(TPConfigs.Folder202, item.EncryptionName), true);
                    }
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
            var dialogResult = XtraMessageBox.Show($"您确定要删除该文件：\n{docBase.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            dt202_BaseBUS.Instance.Remove(docBase.Id);

            Close();
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

        private void btnDelFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Attachment attachment = gvFiles.GetRow(gvFiles.FocusedRowHandle) as Attachment;

            string msg = $"您想要刪除附件：\r\n{attachment.ActualName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            attachments.Remove(attachment);
            lbCountFile.Text = $"共{attachments.Count}個附件";

            int rowIndex = gvFiles.FocusedRowHandle;
            gvFiles.RefreshData();
            gvFiles.FocusedRowHandle = rowIndex;
        }
    }
}