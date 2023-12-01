using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class uc302_NewPersonnelMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc302_NewPersonnelMain()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> lsUser = new List<dm_User>();
        List<dm_JobTitle> lsJobs;

        List<dt302_ReportInfo> reportsInfo;
        List<dt302_ReportAttach> reportAttaches;
        List<dm_Attachment> attachments;

        DXMenuItem[] menuItemsBase;
        DXMenuItem[] menuItemsReport;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnFilter.ImageOptions.SvgImage = TPSvgimages.Filter;

            btnValidCert.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnBackCert.ImageOptions.SvgImage = TPSvgimages.Num2;
            btnInvalidCert.ImageOptions.SvgImage = TPSvgimages.Num3;
            btnWaitCert.ImageOptions.SvgImage = TPSvgimages.Num4;
            btnExpCert.ImageOptions.SvgImage = TPSvgimages.Num5;
            btnClearFilter.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        void InitializeMenuItems()
        {
            DXMenuItem itemViewInfo = new DXMenuItem("顯示信息", ItemViewInfo_Click, TPSvgimages.Search, DXMenuItemPriority.Normal);
            DXMenuItem itemViewFile = new DXMenuItem("讀取檔案", ItemViewFile_Click, TPSvgimages.Progress, DXMenuItemPriority.Normal);
            DXMenuItem itemAddReport = new DXMenuItem("上傳報告", ItemAddReport_Click, TPSvgimages.UploadFile, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewFile.AppearanceHovered.ForeColor = Color.Blue;

            itemAddReport.ImageOptions.SvgImageSize = new Size(24, 24);
            itemAddReport.AppearanceHovered.ForeColor = Color.Blue;

            menuItemsBase = new DXMenuItem[] { itemViewInfo, itemViewFile };

            menuItemsReport = new DXMenuItem[] { itemAddReport };
        }

        private void ItemAddReport_Click(object sender, EventArgs e)
        {
            GridView detailGridView = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            if (detailGridView != null)
            {
                int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
                if (detailFocusedRowHandle < 0) return;

                int idReport = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdReport));

                dm_Attachment att = new dm_Attachment()
                {
                    Thread = "302",
                    EncryptionName = "VNW0014732asdad",
                    ActualName = "du leij.pdf"
                };
                var idAtt = dm_AttachmentBUS.Instance.Add(att);

                dt302_ReportAttach reportAttach = new dt302_ReportAttach()
                {
                    IdReport = idReport,
                    IdAttach = idAtt
                };

                dt302_ReportAttachBUS.Instance.Add(reportAttach);
            }
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            //GridView view = sender as GridView;
            int idBase = Convert.ToInt16(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            //if (_base == null) return;

            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "證照";
            fInfo.idBase302 = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            helper.SaveViewInfo();

            var lsBases = dt302_NewPersonBaseBUS.Instance.GetList();
            lsUser = dm_UserBUS.Instance.GetList();
            lsJobs = dm_JobTitleBUS.Instance.GetList();
            reportsInfo = dt302_ReportInfoBUS.Instance.GetList();
            attachments = dm_AttachmentBUS.Instance.GetListByThread("302");

            var lsBasesDisplay = (from data in lsBases
                                  join urs in lsUser on data.IdUser equals urs.Id
                                  join supvr in lsUser on data.Supervisor equals supvr.Id
                                  join job in lsJobs on urs.JobCode equals job.Id
                                  where urs.IdDepartment.StartsWith(idDept2word)
                                  select new
                                  {
                                      Id = data.Id,
                                      IdDept = urs.IdDepartment,
                                      IdUser = data.IdUser,
                                      IdJobTitle = urs.JobCode,
                                      EnterDate = urs.DateCreate,
                                      Describe = data.Describe,
                                      UserName = $"{urs.DisplayName} {urs.DisplayNameVN}",
                                      JobName = $"{job.Id} {job.DisplayName}",
                                      Supervisor = $"{supvr.DisplayName} {supvr.DisplayNameVN}",
                                  }).ToList();

            sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc302_NewPersonnelMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvReport.ReadOnlyGridView();
            gvReport.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "大學新進人員";
            fInfo.ShowDialog();

            LoadData();
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));
            var reports = reportsInfo.Where(r => r.IdBase == idBase).Select(r => r.Id).ToList();
            reportAttaches = dt302_ReportAttachBUS.Instance.GetListByListReport(reports);

            e.IsEmpty = reports.Count == 0;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));

            int index = 1;
            e.ChildList = reportsInfo.Where(r => r.IdBase == idBase).Select(r => new
            {
                Index = $"第{index++}次",
                r.Id,
                r.Content,
                r.ExpectedDate,
                r.UploadDate,
                r.Attachment
            }).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "報告進度";
        }

        private void gridView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        // Master-Detail : gvReport
        private void gvReport_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idReport = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColIdReport));
            e.IsEmpty = !reportAttaches.Any(r => r.IdReport == idReport);
        }

        private void gvReport_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idReport = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColIdReport));

            int index = 1;
            var reportsAtt = dt302_ReportAttachBUS.Instance.GetListByReport(idReport);

            e.ChildList = (from data in reportsAtt
                           join att in attachments on data.IdAttach equals att.Id
                           select new
                           {
                               ActualName = att.ActualName
                           }).ToList();

        }

        private void gvReport_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvReport_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "附件";
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                foreach (DXMenuItem item in menuItemsBase)
                    e.Menu.Items.Add(item);
            }
        }

        private void gvReport_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                foreach (DXMenuItem item in menuItemsReport)
                    e.Menu.Items.Add(item);
            }
        }

        private void gridView_ExpandMasterRow(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            view.ExpandMasterRow(view.FocusedRowHandle);
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToPdf(filePath);
            Process.Start(filePath);
        }
    }
}
