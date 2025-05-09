using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_CostCalculation : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_CostCalculation()
        {
            InitializeComponent();
            InitializeIcon();
            //InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            System.Drawing.Font fontUI12 = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        DateTime dateFrom, dateTo;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            uc309_ChooseDate ucInfo = new uc309_ChooseDate();
            if (XtraDialog.Show(ucInfo, "選時間", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            if (ucInfo.DateTo >= ucInfo.DateForm)
            {
                dateFrom = ucInfo.DateForm;
                dateTo = ucInfo.DateTo;
            }
            else
            {
                XtraMessageBox.Show("結束時間必須大於或等於開始時間。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var depts = dm_DeptBUS.Instance.GetList();
                var users = dm_UserBUS.Instance.GetList();
                var materials = dt309_MaterialsBUS.Instance.GetList();
                var storages = dt309_StoragesBUS.Instance.GetList();

                var datas = dt309_TransactionsBUS.Instance.GetListByDate(dateFrom, dateTo).Where(r => r.TransactionType == "out")
                    .Select(r => new dt309_Transactions
                    {
                        Id = r.Id,
                        StorageId = r.StorageId,
                        MaterialId = r.MaterialId,
                        Quantity = -r.Quantity,
                        TransactionType = r.TransactionType,
                        CreatedDate = r.CreatedDate,
                        AftQuantity = r.AftQuantity,
                        TotalQuantity = r.TotalQuantity,
                        UserDo = r.UserDo,
                        Desc = r.Desc
                    }).ToList();

                var displayData = (from data in datas
                                   join material in materials on data.MaterialId equals material.Id
                                   group new { data, material } by material.IdDept into deptGroup
                                   select new
                                   {
                                       Dept = depts.Where(d => d.Id == deptGroup.Key).Select(d => $"{d.Id} {d.DisplayName}").FirstOrDefault() ?? "N/A",
                                       IdDept = deptGroup.Key,
                                       TotalAmount = deptGroup.Sum(d => d.data.Quantity * d.material.Price),  // Tổng tiền của Dept
                                       Materials = deptGroup.GroupBy(x => x.data.StorageId).Select(storageGroup => new
                                       {
                                           StorageName = storages.Where(s => s.Id == storageGroup.Key).Select(s => s.DisplayName).FirstOrDefault() ?? "N/A",
                                           StorageId = storageGroup.Key,  // Kho lưu trữ
                                           TotalAmount = storageGroup.Sum(d => d.data.Quantity * d.material.Price),  // Tổng tiền của Storage
                                           Items = storageGroup.Select(item => new
                                           {
                                               Material = item.material,         // Tên vật liệu
                                               Count = storageGroup.Count(),     // Số lần xuất hiện
                                               UserMngr = users.FirstOrDefault(u => u.Id == item.material.IdManager).DisplayName,
                                               TotalQuantity = storageGroup.Sum(d => d.data.Quantity),  // Tổng số lượng
                                               OutputTime = storageGroup.Count(),  // Đếm số lần
                                               TotalAmount = storageGroup.Sum(d => d.data.Quantity * item.material.Price) // Tổng tiền vật liệu
                                           }).ToList()
                                       }).ToList()
                                   }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "庫存";
        }

        private void gvStorage_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvStorage_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "物料";
        }

        private void gridView_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void uc309_CostCalculation_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvStorage.ReadOnlyGridView();
            gvStorage.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvStorage.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvMaterial.ReadOnlyGridView();
            gvMaterial.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            //// Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            //var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            //var departments = dm_DeptBUS.Instance.GetList();
            //var groups = dm_GroupBUS.Instance.GetListByName("機邊庫");

            //var accessibleGroups = groups
            //    .Where(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id))
            //    .ToList();

            //var departmentItems = departments
            //    .Where(dept => accessibleGroups.Any(group => group.IdDept == dept.Id))
            //    .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
            //    .ToArray();

            //cbbDept.Items.AddRange(departmentItems);
            //barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            //CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }
    }
}
