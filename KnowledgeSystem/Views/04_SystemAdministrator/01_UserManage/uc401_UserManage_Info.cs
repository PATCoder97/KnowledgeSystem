using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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
    public partial class uc401_UserManage_Info : EditFormUserControl
    {
        public uc401_UserManage_Info()
        {
            InitializeComponent();
            FormLoad();
        }

        private void FormLoad()
        {
            // txbRole.Properties.Items.AddRange(new string[] { fSearchThread.QUY_PHAM, fSearchThread.NSX });
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

        public int IdDept
        {
            get { return Convert.ToInt16(txbDept.Text.Trim()); }
            set { IdDept = value; }
        }

        //public int IdRole
        //{
        //    get { return Convert.ToInt16(txbRole.Text.Trim()); }
        //    set { IdRole = value; }
        //}
    }
}
