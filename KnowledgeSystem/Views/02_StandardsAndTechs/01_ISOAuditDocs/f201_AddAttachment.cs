using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;
using DataAccessLayer;
using KnowledgeSystem.Helpers;
using BusinessLayer;
using DevExpress.XtraGrid.Columns;
using DocumentFormat.OpenXml.Presentation;
using System.Web.Security;
using System.Runtime.InteropServices;
using DevExpress.XtraSplashScreen;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraGrid;
using KnowledgeSystem.Views._00_Generals;
using Org.BouncyCastle.Asn1.Crmf;
using DevExpress.XtraLayout;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AddAttachment : DevExpress.XtraEditors.XtraForm
    {
        public f201_AddAttachment()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public dt201_Forms baseForm = null;
        public List<dt201_Progress> baseProgresses;
        public dt201_Base currentData = new dt201_Base();
        public dt201_Base parentData = new dt201_Base();
        public int idBase = -1;

        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
        string filterAtt = TPConfigs.FilterFile;
        bool? IsPaper = false;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;
        List<ProgressDetail> progresses = new List<ProgressDetail>();
        List<dt201_Role> roles;
        Attachment attachment = new Attachment();

        BindingSource sourceProgresses = new BindingSource();

        class ProgressDetail : dt201_Progress
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
        }

        private class Attachment : dm_Attachment
        {
            public string FullPath { get; set; }
        }

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e, System.Drawing.Color color)
        {
            e.Info.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            e.Info.Appearance.ForeColor = color;
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDocCode.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            txbDisplayNameVN.Enabled = _enable;
            txbAtt.Enabled = _enable;
        }

        private void IniControl()
        {
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

                    lcImpControls.Remove(lcAtt);

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

        private void f201_AddAttachment_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDocCode, lcDisplayName, lcDisplayNameVN, lcAtt };
            lcImpControls = new List<LayoutControlItem>() { lcDisplayName, lcDisplayNameVN, lcAtt };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            // Ẩn lưu trình trình ký
            lcDefaultProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new System.Drawing.Size(Size.Width, Size.Height - 171);

            IsPaper = parentData.IsPaperType;
            idBase = currentData.Id;
            lcSignOrPaper.Enabled = IsPaper != true;

            IniControl();

            users = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roles = dt201_RoleBUS.Instance.GetList().Where(r => r.Id != 0).ToList();

            // Gắn các thông số cho các combobox
            lookupUser.ValueMember = "Id";
            lookupUser.DisplayMember = "Id";
            lookupUser.PopupView.Columns.AddRange(new[]
            {
                new GridColumn { FieldName = "IdDepartment", VisibleIndex = 0, Caption = "單位", MinWidth=50, Width=50 },
                new GridColumn { FieldName = "Id", VisibleIndex = 0, Caption = "工號", MinWidth=120, Width=120 },
                new GridColumn { FieldName = "DisplayName", VisibleIndex = 2, Caption = "名稱", MinWidth=100, Width=150 },
            });
            lookupUser.DataSource = users;

            lookupRole.DataSource = roles;
            lookupRole.DisplayMember = "DisplayName";
            lookupRole.ValueMember = "Id";

            sourceProgresses.DataSource = progresses;
            gcProgress.DataSource = sourceProgresses;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    baseForm = new dt201_Forms();

                    ckSignOrPaper.SelectedIndex = 0;

                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:
                    txbDocCode.EditValue = baseForm.Code;
                    txbDisplayName.EditValue = baseForm.DisplayName;
                    txbDisplayNameVN.EditValue = baseForm.DisplayNameVN;
                    txbDesc.EditValue = baseForm.Descript;

                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }
        }

        private void gvProgress_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.Column.FieldName != "IdUsr") return;

            var usrInfo = users.FirstOrDefault(r => r.Id == e.Value?.ToString());
            string nameUser = usrInfo?.DisplayName ?? "";
            string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
            view.SetRowCellValue(e.RowHandle, view.Columns["UserName"], nameUser);
            view.SetRowCellValue(e.RowHandle, view.Columns["JobName"], jobName);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Move focus away from the grid to update source
            gvProgress.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (string.IsNullOrEmpty(item.Control.Text)) IsValidate = false;
            }

            if (!IsValidate)
            {
                XtraMessageBox.Show("請填寫所有重要資訊！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get form inputs
            string code = txbDocCode.Text.Trim();
            string displayName = txbDisplayName.Text.Trim();
            string displayNameVN = txbDisplayNameVN.Text.Trim();
            string desc = txbDesc.Text.Trim();
            bool isDigitalSign = ckSignOrPaper.SelectedIndex == 1;

            if (StringHelper.CheckUpcase(displayNameVN, 33) && displayNameVN.Length > 20)
            {
                XtraMessageBox.Show("Tắt CAPSLOCK đi！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate progress
            bool HasProgress = progresses.Count <= 0;
            bool isProgressError = progresses.GroupBy(x => x.IdUsr).Any(g => g.Count() > 1);

            if (isDigitalSign && (HasProgress || isProgressError))
            {
                XtraMessageBox.Show("沒有核簽流程或重複！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (progresses.Any(r => r.IdRole == 0 || string.IsNullOrEmpty(r.IdUsr)))
            {
                XtraMessageBox.Show("請完成簽核流程！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool result = false;
            string message = $"{code} {displayName}";

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                baseForm.Code = code;
                baseForm.DisplayName = displayName;
                baseForm.DisplayNameVN = displayNameVN;
                baseForm.Descript = desc;
                baseForm.UploadUser = TPConfigs.LoginUser.Id;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        baseForm.UploadTime = DateTime.Now;
                        baseForm.IdBase = idBase;
                        baseForm.IsProcessing = isDigitalSign;
                        baseForm.DigitalSign = isDigitalSign;
                        HandleCreateEvent(ref result);

                        break;

                    case EventFormInfo.Update:

                        HandleUpdateEvent(ref result);

                        break;

                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void HandleCreateEvent(ref bool result)
        {
            // Create attachment
            var att = new dm_Attachment
            {
                Thread = attachment.Thread,
                ActualName = attachment.ActualName,
                EncryptionName = attachment.EncryptionName
            };

            int idAtt = dm_AttachmentBUS.Instance.Add(att);
            baseForm.AttId = idAtt;
            baseForm.NextStepProg = progresses.FirstOrDefault()?.IdUsr ?? "";

            int idForm = dt201_FormsBUS.Instance.Add(baseForm);

            if (baseForm.DigitalSign == true)
            {
                HandleDigitalSign(idAtt, idForm);
                result = idForm > 0;
            }
            else
            {
                File.Copy(attachment.FullPath, Path.Combine(TPConfigs.Folder201, attachment.EncryptionName), true);
                result = true;
            }
        }

        private void HandleUpdateEvent(ref bool result)
        {
            if (string.IsNullOrEmpty(txbAtt.EditValue?.ToString()))
            {
                result = dt201_FormsBUS.Instance.AddOrUpdate(baseForm);
            }
            else
            {
                bool isDigitalSign = ckSignOrPaper.SelectedIndex == 1;
                baseForm.IsProcessing = isDigitalSign;
                baseForm.DigitalSign = isDigitalSign;
                baseForm.IsCancel = false;

                // Create attachment
                var att = new dm_Attachment
                {
                    Thread = attachment.Thread,
                    ActualName = attachment.ActualName,
                    EncryptionName = attachment.EncryptionName
                };

                int idAtt = dm_AttachmentBUS.Instance.Add(att);
                baseForm.AttId = idAtt;
                baseForm.NextStepProg = progresses.FirstOrDefault()?.IdUsr ?? "";

                result = dt201_FormsBUS.Instance.AddOrUpdate(baseForm);
                int idForm = baseForm.Id;

                if (baseForm.DigitalSign == true)
                {
                    dt201_ProgressBUS.Instance.RemoveByIdForm(idForm);
                    dt201_ProgInfoBUS.Instance.RemoveByIdForm(idForm);

                    HandleDigitalSign(idAtt, idForm);
                    result = idForm > 0;
                }
                else
                {
                    File.Copy(attachment.FullPath, Path.Combine(TPConfigs.Folder201, attachment.EncryptionName), true);
                    result = true;
                }
            }
        }

        private void HandleDigitalSign(int idAtt, int idForm)
        {
            string folderDest = Path.Combine(TPConfigs.Folder201, idAtt.ToString());
            if (!Directory.Exists(folderDest)) Directory.CreateDirectory(folderDest);
            File.Copy(attachment.FullPath, Path.Combine(folderDest, attachment.EncryptionName), true);

            var baseProgresses = progresses.Select(data => new dt201_Progress
            {
                IdForm = idForm,
                IdUsr = data.IdUsr,
                IdRole = data.IdRole
            }).ToList();

            dt201_ProgressBUS.Instance.AddRange(baseProgresses);

            // Tạo 1 progStep là văn kiện đã đưa lên
            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdForm = idForm,
                IdUsr = "VNW0000000",
                RespTime = DateTime.Now,
                Desc = "呈核"
            };

            dt201_ProgInfoBUS.Instance.Add(info);
        }

        private void txbAtt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            string fileName = "";
            string encryptionName = "";
            string actualName = "";

            switch (e.Button.Caption)
            {
                case "Paste":

                    if (Clipboard.ContainsFileDropList())
                    {
                        var files = Clipboard.GetFileDropList();
                        var pdfFiles = new List<string>();

                        foreach (var file in files)
                        {
                            if (file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(file))
                            {
                                pdfFiles.Add(file);
                            }
                        }

                        if (pdfFiles.Count != 1)
                        {
                            XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        fileName = pdfFiles.First();
                        encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                        actualName = Path.GetFileName(fileName);

                        txbAtt.Text = actualName;
                        attachment = new Attachment()
                        {
                            Thread = "201",
                            EncryptionName = encryptionName,
                            ActualName = actualName,
                            FullPath = fileName
                        };
                    }
                    else
                    {
                        XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    break;
                default:
                    OpenFileDialog openFileDialog = new OpenFileDialog { Filter = filterAtt };

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    fileName = openFileDialog.FileName;
                    encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                    actualName = Path.GetFileName(fileName);

                    txbAtt.Text = actualName;
                    attachment = new Attachment()
                    {
                        Thread = "201",
                        EncryptionName = encryptionName,
                        ActualName = actualName,
                        FullPath = fileName
                    };
                    break;
            }
        }

        private void ckSignOrPaper_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSignSelected = ckSignOrPaper.SelectedIndex == 0;
            lcProgress.Visibility = isSignSelected ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            lcDefaultProgress.Visibility = isSignSelected ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            Size = new Size(Size.Width, Size.Height + (isSignSelected ? -171 : 171));

            filterAtt = ckSignOrPaper.SelectedIndex == 0 ? TPConfigs.FilterFile : "PDF | *.pdf";
            attachment = null;
            txbAtt.EditValue = "";
        }

        private void btnDefaultProgress_Click(object sender, EventArgs e)
        {
            f00_SelectFixedProg fProg = new f00_SelectFixedProg();
            fProg.ShowDialog();

            int idFixedProg = fProg.Id;

            progresses.Clear();
            string defaulProg = dm_FixedProgressBUS.Instance.GetItemById(idFixedProg)?.Progress;
            if (string.IsNullOrEmpty(defaulProg)) return;

            string[] defaulProgs = defaulProg.Split(';');

            foreach (var item in defaulProgs)
            {
                var usrInfo = users.FirstOrDefault(r => r.Id == item?.ToString());
                string nameUser = usrInfo?.DisplayName ?? "";
                string jobName = jobTitles.FirstOrDefault(r => r.Id == usrInfo.JobCode)?.DisplayName ?? "";
                progresses.Add(new ProgressDetail() { IdUsr = item, JobName = jobName, UserName = nameUser });
            }

            gcProgress.RefreshDataSource();
        }

        private void btnDelProgress_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvProgress;
            ProgressDetail prog = view.GetRow(view.FocusedRowHandle) as ProgressDetail;
            progresses.Remove(prog);

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
        }

        private void gvProgress_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            System.Drawing.Color color = view.Appearance.HeaderPanel.ForeColor;

            if (!view.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }
                    IndicatorDraw(e, color);
                    SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0));
                    Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                    BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = string.Format("[{0}]", (e.RowHandle * -1));
                IndicatorDraw(e, color);
                SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0));
                Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
            }
        }
    }
}