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
            gvData.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;

            gcData.DataSource = sourceGroup;
            helper = new RefreshHelper(gvData, "Id");
            LoadGroup();
        }
    }
}