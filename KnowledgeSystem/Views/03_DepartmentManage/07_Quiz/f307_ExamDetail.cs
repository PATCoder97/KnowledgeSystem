using BusinessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit.Model;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_ExamDetail : DevExpress.XtraEditors.XtraForm
    {
        public f307_ExamDetail()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
        }

        public string examCode = "";


        DXMenuItem itemViewInfo;
        DXMenuItem itemResetExam;
        DXMenuItem itemExportExam;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看詳情", ItemViewInfo_Click, TPSvgimages.View);
            itemResetExam = CreateMenuItem("刪除結果", ItemResetExam_Click, TPSvgimages.Remove);
            itemExportExam = CreateMenuItem("導出結果", ItemExportExam_Click, TPSvgimages.Print);
        }

        private void ItemExportExam_Click(object sender, EventArgs e)
        {

        }

        private void ItemResetExam_Click(object sender, EventArgs e)
        {
            GridView view = gvData;

            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var itemUpdate = dt307_ExamUserBUS.Instance.GetItemById(idBase);
            itemUpdate.SubmitTime = null;
            itemUpdate.Score = null;
            itemUpdate.IsPass = false;
            itemUpdate.ExamData = null;

            dt307_ExamUserBUS.Instance.AddOrUpdate(itemUpdate);

            LoadData();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {

        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void LoadData()
        {
            var bases = dt307_ExamUserBUS.Instance.GetListByExamCode(examCode);
            var usrs = dm_UserBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var depts = dm_DeptBUS.Instance.GetList();

            var datas = (from data in bases
                         join usr in usrs on data.IdUser equals usr.Id
                         join job in jobs on data.IdJob equals job.Id
                         join dept in depts on usr.IdDepartment equals dept.Id
                         let DeptName = $"{dept.Id}\r\n{dept.DisplayName}"
                         let DisplayName = $"{usr.DisplayName}\r\n{usr.DisplayNameVN}"
                         select new
                         {
                             data,
                             usr,
                             job,
                             DeptName,
                             DisplayName
                         }).ToList();

            gcData.DataSource = datas;
            gvData.BestFitColumns();
        }

        private void f307_ExamDetail_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isComplete = !string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "data.SubmitTime")?.ToString());

                e.Menu.Items.Add(itemViewInfo);
                if (isComplete)
                {
                    e.Menu.Items.Add(itemExportExam);
                    e.Menu.Items.Add(itemResetExam);
                }
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}