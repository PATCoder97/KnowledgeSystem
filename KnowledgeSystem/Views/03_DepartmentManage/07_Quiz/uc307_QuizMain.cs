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

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class uc307_QuizMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_QuizMain()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;

        List<dm_User> lsUser = new List<dm_User>();
        List<dt202_Type> types;

        List<dt202_Attach> attachments;
        List<dm_Attachment> attachmentsInfo;

        private bool IsCanEdit = false;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("編輯", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemViewFile = new DXMenuItem("讀取", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewFile.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            //int idFile = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdFile));

            //var fileInfo = attachmentsInfo.FirstOrDefault(r => r.Id == idFile);

            //if (fileInfo == null) return;

            //string source = Path.Combine(TPConfigs.Folder202, fileInfo.EncryptionName);
            //string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {fileInfo.ActualName}");
            //if (!Directory.Exists(TPConfigs.TempFolderData))
            //    Directory.CreateDirectory(TPConfigs.TempFolderData);

            //File.Copy(source, dest, true);

            //f00_VIewFile viewFile = new f00_VIewFile(dest);
            //viewFile.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            //string idBase = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId).ToString();

            //f202_DocInfo fInfo = new f202_DocInfo();
            //fInfo.eventInfo = EventFormInfo.View;
            //fInfo.formName = "文件";
            //fInfo.idBase202 = idBase;
            //fInfo.ShowDialog();

            //LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                // Check quyền hạn có thể sửa văn kiện
                IsCanEdit = AppPermission.Instance.CheckAppPermission(AppPermission.EditDoc202);

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
                object id = view.GetRowCellValue(hi.RowHandle, "exam.Id");
                MessageBox.Show(id.ToString());
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

        private void btnPractise_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_DoExam fDoExam = new f307_DoExam();
            fDoExam.ShowDialog();
        }
    }
}
