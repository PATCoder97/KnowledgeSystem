using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_UserManage_Info : EditFormUserControl
    {
        public uc401_UserManage_Info()
        {
            InitializeComponent();
            FormLoad();
        }

        private void FormLoad()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsRoles = db.dm_Role.ToList();
                cbbRole.Properties.DataSource = lsRoles;
                cbbRole.Properties.DisplayMember = "DisplayName";
                cbbRole.Properties.ValueMember = "Id";

                var lsDepts = db.dm_Departments.ToList();
                cbbDept.Properties.DataSource = lsDepts;
                cbbDept.Properties.DisplayMember = "DisplayName";
                cbbDept.Properties.ValueMember = "Id";

                cbbDept.EditValue = "7820";
            }
        }

        public string Id
        {
            get { return txbId.Text.Trim(); }
            set { Id = value; }
        }

        public string DisplayName
        {
            get { return txbDisplayName.Text.Trim(); }
            set { DisplayName = value; }
        }

        public string IdDept
        {
            get { return cbbDept.EditValue.ToString(); }
            set { IdDept = value; }
        }

        public int IdRole
        {
            get { return string.IsNullOrEmpty(cbbRole.EditValue.ToString()) ? 0 : Convert.ToInt16(cbbRole.EditValue); }
            set { IdRole = value; }
        }

        private void txbId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (TPConfigs.DomainComputer == DomainVNFPG.domainVNFPG)
            {
                string userID = txbId.Text.Trim().ToUpper();
                string userNameByDomain = DomainVNFPG.Instance.GetAccountName(userID);

                string[] displayNameFHS = userNameByDomain.Split('/');
                string idDeptFHS = displayNameFHS[0].Replace("LG", string.Empty);
                string userNameFHS = displayNameFHS[1];

                txbDisplayName.Text = userNameFHS;
                cbbDept.EditValue = idDeptFHS;
            }
        }
    }
}
