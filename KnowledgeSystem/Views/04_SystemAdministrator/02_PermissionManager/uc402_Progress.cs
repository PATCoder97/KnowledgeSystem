using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_Progress : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_Progress()
        {
            InitializeComponent();
            InitializeIcon();
        }

        #region parameters

        RefreshHelper helper;
        BindingSource sourceProgress = new BindingSource();

        List<dm_Progress> lsProgresses;
        List<dm_Group> lsGroups;
        List<dm_Departments> lsDepts;

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnNew.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            lsProgresses = dm_ProgressBUS.Instance.GetList();
            lsGroups = dm_GroupBUS.Instance.GetList();
            lsDepts = dm_DeptBUS.Instance.GetList();

            sourceProgress.DataSource = (from data in lsProgresses
                                         join dept in lsDepts on data.IdDept equals dept.Id
                                         select new dm_Progress
                                         {
                                             Id = data.Id,
                                             Prioritize = data.Prioritize,
                                             DisplayName = data.DisplayName,
                                             IdDept = $"{dept.Id} {dept.DisplayName}",
                                         }).ToList();

            gcData.RefreshDataSource();
            helper.LoadViewInfo();
            gvData.BestFitColumns();
        }

        #endregion

        private void uc207_Progress_Load(object sender, EventArgs e)
        {
            helper = new RefreshHelper(gvData, "Id");
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceProgress;
            LoadData();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_ProgressInfo fInfo = new f402_ProgressInfo();
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            int _focusHandle = gvData.FocusedRowHandle;
            if (_focusHandle < 0) return;
            var data = gvData.GetRow(gvData.FocusedRowHandle) as dm_Progress;

            f402_ProgressInfo fInfo = new f402_ProgressInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._idProgress = data.Id;
            fInfo.ShowDialog();
            LoadData();
        }
    }
}
