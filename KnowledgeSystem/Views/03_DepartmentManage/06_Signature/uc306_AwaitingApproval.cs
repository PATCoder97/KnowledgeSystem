using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_AwaitingApproval : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_AwaitingApproval()
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
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;
        DXMenuItem itemAddPlanFile;
        DXMenuItem itemCloseReport;
        DXMenuItem itemAddAttach;
        DXMenuItem itemDelAttach;

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("看信息", ItemViewInfo_Click, TPSvgimages.View);
            //itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            //itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            //itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            //itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            //itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;

            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f306_SignDocInfo fInfo = new f306_SignDocInfo();
            fInfo.idBase = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                bases = dt306_BaseBUS.Instance.GetListByNextStep(TPConfigs.LoginUser.Id);
                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();
                var dmTypes = dt306_TypeBUS.Instance.GetList();

                var basesDisplay = (from data in bases
                                    join types in dmTypes on data.IdType equals types.Id
                                    join urs in users on data.UploadUsr equals urs.Id
                                    //join job in jobs on urs.JobCode equals job.Id
                                    where data.IsProcess == true
                                    select new
                                    {
                                        types,
                                        data,
                                        urs,
                                        DisplayName = $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}"
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc306_AwaitingApproval_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f306_SignDocInfo fInfo = new f306_SignDocInfo();
            fInfo.idBase = idBase;
            fInfo.ShowDialog();

            LoadData();
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

            string filePath = Path.Combine(documentsPath, $"電子核簽進度 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
