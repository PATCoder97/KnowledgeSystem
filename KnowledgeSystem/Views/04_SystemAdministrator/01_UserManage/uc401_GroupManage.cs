using BusinessLayer;
using DataAccessLayer;
using KnowledgeSystem.Configs;
using System;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage
{
    public partial class uc401_GroupManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;

        public uc401_GroupManage()
        {
            InitializeComponent();
            InitializeIcon();
        }

        #region parameters

        BindingSource sourceGroup = new BindingSource();

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadGroup()
        {
            helper.SaveViewInfo();
            var lsGroup = dm_GroupBUS.Instance.GetList();
            sourceGroup.DataSource = lsGroup;

            gvData.BestFitColumns();
            helper.LoadViewInfo();
        }

        #endregion

        private void f401_GroupManage_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();

            gcData.DataSource = sourceGroup;
            helper = new RefreshHelper(gvData, "Id");
            LoadGroup();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_GroupInfo frmInfo = new f401_GroupInfo();
            frmInfo.ShowDialog();

            LoadGroup();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvData.FocusedRowHandle;
            if (forcusRow < 0) return;

            dm_Group dataRow = gvData.GetRow(forcusRow) as dm_Group;
            int IdGroup = dataRow.Id;

            f401_GroupInfo formInfo = new f401_GroupInfo(IdGroup);
            formInfo.ShowDialog();

            LoadGroup();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadGroup();
        }
    }
}