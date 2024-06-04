using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Wizards;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_SignatureMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_SignatureMain()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> users = new List<dm_User>();
        List<dm_JobTitle> jobs;

        List<dt306_Base> bases;
        //List<dt302_ReportAttach> reportAttaches;
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;
        DXMenuItem itemAddPlanFile;
        DXMenuItem itemCloseReport;
        DXMenuItem itemAddAttach;
        DXMenuItem itemDelAttach;

        private void InitializeMenuItems()
        {
            //itemViewInfo = new DXMenuItem("顯示信息", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            //itemViewFile = new DXMenuItem("讀取檔案", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);
            //itemAddPlanFile = new DXMenuItem("上傳計劃表", ItemAddPlanFile_Click, TPSvgimages.UpLevel, DXMenuItemPriority.Normal);
            //itemAddAttach = new DXMenuItem("上傳報告", ItemAddAttach_Click, TPSvgimages.UploadFile, DXMenuItemPriority.Normal);
            //itemCloseReport = new DXMenuItem("結案", ItemCloseReport_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);
            //itemDelAttach = new DXMenuItem("刪除附件", ItemDelAttach_Click, TPSvgimages.Remove, DXMenuItemPriority.Normal);

            //itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            //itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemViewFile.AppearanceHovered.ForeColor = Color.Blue;

            //itemAddPlanFile.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemAddPlanFile.AppearanceHovered.ForeColor = Color.Blue;

            //itemCloseReport.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemCloseReport.AppearanceHovered.ForeColor = Color.Blue;

            //itemAddAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemAddAttach.AppearanceHovered.ForeColor = Color.Blue;

            //itemDelAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            //itemDelAttach.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                bases = dt306_BaseBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();

                var basesDisplay = (from data in bases
                                    join urs in users on data.UploadUsr equals urs.Id
                                    //join job in jobs on urs.JobCode equals job.Id
                                    //where urs.IdDepartment.StartsWith(idDept2word)
                                    select new
                                    {
                                        data,
                                        urs,
                                        DisplayName = $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}"
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
                gvData.ExpandMasterRow(gvData.FocusedRowHandle);
            }
        }

        private void uc306_SignatureMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvDocs.ReadOnlyGridView();

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f306_NewSignDoc fAdd = new f306_NewSignDoc();
            fAdd.ShowDialog();

            LoadData();
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);
            bool isProgess = (bool)view.GetRowCellValue(e.RowHandle, gColIsProgess);
            bool isCancel = (bool)view.GetRowCellValue(e.RowHandle, gColIsCancel);

            var baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            var childList = (from data in baseAtts
                             join urs in users on data.UsrCancel equals urs.Id into userGroup
                             from urs in userGroup.DefaultIfEmpty()
                             select new
                             {
                                 data,
                                 DisplayName = urs != null ? $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}" : null,
                                 Status = data.IsCancel || isCancel ? "被退回" : isProgess ? "核簽中" : "核簽完畢"
                             }).ToList();

            e.ChildList = childList;
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "核簽文件";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
