using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AddRangeAtts : DevExpress.XtraEditors.XtraForm
    {
        public f201_AddRangeAtts()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idBase = -1;

        List<FormDetail> forms = new List<FormDetail>();
        BindingSource sourceForms = new BindingSource();
        BindingSource sourceProgresses = new BindingSource();

        List<dm_User> users;
        List<dm_JobTitle> jobTitles;
        List<ProgressDetail> progresses = new List<ProgressDetail>();
        List<dt201_Role> roles;

        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        class ProgressDetail : dt201_Progress
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
        }

        class FormDetail : dt201_Forms
        {
            public string FileName { get; set; }
            public string FileNameEncypt { get; set; }
            public string FilePath { get; set; }
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
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f201_AddRangeAtts_Load(object sender, EventArgs e)
        {
            sourceForms.DataSource = forms;
            gcInfo.DataSource = sourceForms;

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
        }

        private void btnPasteMultiFile_Click(object sender, EventArgs e)
        {
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

                foreach (var item in pdfFiles)
                {
                    var data = new FormDetail()
                    {
                        Code = "",
                        DisplayName = "",
                        DisplayNameVN = "",
                        FileName = Path.GetFileName(item),
                        FilePath = item,
                        FileNameEncypt = EncryptionHelper.EncryptionFileName(item)
                    };

                    forms.Add(data);
                }

                gvInfo.RefreshData();
            }
            else
            {
                XtraMessageBox.Show("請選擇PDF檔案", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAddMultiFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Files|*.pdf" };
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var item in openFileDialog.FileNames)
            {
                var data = new FormDetail()
                {
                    Code = "",
                    DisplayName = "",
                    DisplayNameVN = "",
                    FileName = Path.GetFileName(item),
                    FilePath = item,
                    FileNameEncypt = EncryptionHelper.EncryptionFileName(item)
                };

                forms.Add(data);
            }

            gvInfo.RefreshData();
        }

        private void btnDel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            GridView view = gvInfo;
            FormDetail detail = view.GetRow(view.FocusedRowHandle) as FormDetail;
            forms.Remove(detail);

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
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

        private void HandleDigitalSign(int idAtt, int idForm, FormDetail detail)
        {
            string folderDest = Path.Combine(TPConfigs.Folder201, idAtt.ToString());
            if (!Directory.Exists(folderDest)) Directory.CreateDirectory(folderDest);
            File.Copy(detail.FilePath, Path.Combine(folderDest, detail.FileNameEncypt), true);

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

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Move focus away from the grid to update source
            gvProgress.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            gvInfo.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            // Kiểm tra thông tin hợp lệ
            if (forms.Count() <= 0)
            {
                XtraMessageBox.Show("Kiểm tra lại thông tin văn kiện văn kiện！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool nullData = forms.Any(r => string.IsNullOrEmpty(r.Code) || string.IsNullOrEmpty(r.DisplayName) || string.IsNullOrEmpty(r.DisplayNameVN));
            if (nullData)
            {
                XtraMessageBox.Show("Kiểm tra lại thông tin văn kiện văn kiện！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool caplockValue = forms.Any(r => StringHelper.CheckUpcase(r.DisplayNameVN, 33) && r.DisplayNameVN.Length > 20);
            if (caplockValue)
            {
                XtraMessageBox.Show("Tắt CAPSLOCK đi！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate progress
            bool HasProgress = progresses.Count <= 0;
            bool isProgressError = progresses.GroupBy(x => x.IdUsr).Any(g => g.Count() > 1);

            if (HasProgress || isProgressError)
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

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                foreach (var item in forms)
                {
                    dt201_Forms baseForm = new dt201_Forms();

                    baseForm.Code = item.Code;
                    baseForm.DisplayName = item.DisplayName;
                    baseForm.DisplayNameVN = item.DisplayNameVN;
                    baseForm.UploadUser = TPConfigs.LoginUser.Id;
                    baseForm.UploadTime = DateTime.Now;
                    baseForm.IdBase = idBase;
                    baseForm.IsProcessing = true;
                    baseForm.DigitalSign = true;

                    // Create attachment
                    var att = new dm_Attachment
                    {
                        Thread = "201",
                        ActualName = item.FileName,
                        EncryptionName = item.FileNameEncypt,
                    };

                    int idAtt = dm_AttachmentBUS.Instance.Add(att);
                    baseForm.AttId = idAtt;
                    baseForm.NextStepProg = progresses.FirstOrDefault()?.IdUsr ?? "";

                    int idForm = dt201_FormsBUS.Instance.Add(baseForm);

                    HandleDigitalSign(idAtt, idForm, item);
                    result = idForm > 0;
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
    }
}