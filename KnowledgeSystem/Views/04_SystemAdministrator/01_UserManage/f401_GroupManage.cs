using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage
{
    public partial class f401_GroupManage : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;

        public f401_GroupManage()
        {
            InitializeComponent();
        }

        #region parameters

        BindingSource sourceGroup = new BindingSource();

        #endregion

        #region methods

        private void LoadGroup()
        {
            helper.SaveViewInfo();
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsGroup = db.Groups.ToList();
                sourceGroup.DataSource = lsGroup;
            }

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
            f401_GroupManage_Info frmInfo = new f401_GroupManage_Info();
            frmInfo.ShowDialog();

            LoadGroup();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvData.FocusedRowHandle;
            if (forcusRow < 0) return;

            Group dataRow = gvData.GetRow(forcusRow) as Group;
            int IdGroup = dataRow.Id;

            f401_GroupManage_Info formInfo = new f401_GroupManage_Info(IdGroup);
            formInfo.ShowDialog();

            LoadGroup();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadGroup();
        }
    }
}