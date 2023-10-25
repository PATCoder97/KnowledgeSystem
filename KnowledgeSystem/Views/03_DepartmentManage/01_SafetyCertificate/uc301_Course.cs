using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class uc301_Course : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_Course()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        List<dt301_Course> lsCourses = new List<dt301_Course>();
        BindingSource sourceCourses = new BindingSource();

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            lsCourses = dt301_CourseBUS.Instance.GetList();
            sourceCourses.DataSource = lsCourses;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        private void uc301_Course_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();

            gcData.DataSource = sourceCourses;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f301_CourseInfo fInfo = new f301_CourseInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "課程";
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dt301_Course _course = view.GetRow(view.FocusedRowHandle) as dt301_Course;

            f301_CourseInfo fInfo = new f301_CourseInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "課程";
            fInfo._course = _course;
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
