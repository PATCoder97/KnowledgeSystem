using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs;
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
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using DevExpress.XtraGrid.Views.Card;
using System.Drawing.Drawing2D;
using OfficeOpenXml.FormulaParsing;
using System.Diagnostics;
using DocumentFormat.OpenXml.Spreadsheet;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DocumentFormat.OpenXml.Bibliography;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class uc307_QuizMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_QuizMain()
        {
            InitializeComponent();
            InitializeIcon();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        BindingSource sourceBases = new BindingSource();


        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            grPractise.ImageOptions.SvgImage = TPSvgimages.Learn;
            btnPractiseMyJob.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnPractiseOtherJob.ImageOptions.SvgImage = TPSvgimages.Num2;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var myExams = dt307_ExamUserBUS.Instance.GetListNeedDoByUserId(TPConfigs.LoginUser.Id);
                var bases = dt307_ExamMgmtBUS.Instance.GetListProcessing();
                var users = dm_UserBUS.Instance.GetList();
                var settting = dt307_JobQuesManageBUS.Instance.GetList();

                var dataDisplays = (from exam in myExams
                                    join data in bases on exam.ExamCode equals data.Code
                                    join set in settting on exam.IdJob equals set.JobId
                                    select new
                                    {
                                        exam,
                                        data,
                                        TestDuration = $"{set.TestDuration}分鐘",
                                        QuesCount = $"{set.QuesCount}題目",
                                        PassingScore = $"{set.PassingScore}/100"
                                    }).ToList();

                sourceBases.DataSource = dataDisplays;
            }
        }

        private void uc307_QuizMain_Load(object sender, EventArgs e)
        {
            LoadData();
            gcData.DataSource = sourceBases;
        }

        private void lvData_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            LayoutView view = sender as LayoutView;
            LayoutViewHitInfo hi = view.CalcHitInfo(args.Location);
            if (hi.InCard)
            {
                string idJob = view.GetRowCellValue(hi.RowHandle, "exam.IdJob").ToString();

                f307_DoExam fDoExam = new f307_DoExam();
                fDoExam.idJob = idJob;
                fDoExam.ShowDialog();
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"考試系統 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void btnPractiseMyJob_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_DoExam fDoExam = new f307_DoExam();
            fDoExam.idJob = TPConfigs.LoginUser.ActualJobCode;
            fDoExam.ShowDialog();
        }

        private void btnPractiseOtherJob_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Khởi tạo SearchLookUpEdit và GridView
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();

            // Cấu hình các cột của GridView
            GridColumn gcol1 = new GridColumn() { Caption = "職務代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "職務名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };
            editView.Columns.AddRange(new GridColumn[] { gcol1, gcol2 });

            // Thiết lập giao diện cho GridView
            editView.Appearance.HeaderPanel.Font = fontUI14;
            editView.Appearance.HeaderPanel.ForeColor = Color.Black;
            editView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            editView.Appearance.Row.Font = fontUI12;
            editView.Appearance.Row.ForeColor = Color.Black;

            // Lấy danh sách dữ liệu và thiết lập DataSource
            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Where(r => r.Id.EndsWith("J"));
            editor.Properties.DataSource = lsJobTitles;
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";

            // Gán GridView cho PopupView của SearchLookUpEdit
            editor.Properties.PopupView = editView;

            // Thiết lập giao diện cho SearchLookUpEdit
            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = Color.Black;
            editor.Properties.NullText = "";

            // Hiển thị hộp thoại với SearchLookUpEdit
            XtraInputBoxArgs args = new XtraInputBoxArgs();
            args.Editor = editor;
            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "請選職務";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選擇要練習的職務</font>";
            args.DefaultButtonIndex = 0;

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string idJobSelect = result?.ToString() ?? "";

            f307_DoExam fDoExam = new f307_DoExam();
            fDoExam.idJob = idJobSelect;
            fDoExam.ShowDialog();
        }
    }
}
