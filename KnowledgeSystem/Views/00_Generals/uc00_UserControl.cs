using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class uc00_UserControl : XtraUserControl
    {
        public uc00_UserControl()
        {
            InitializeComponent();
        }

        public UserComtrolE EventControl { get; set; }

        private void btnChangePass_Click(object sender, EventArgs e)
        {
            EventControl = UserComtrolE.ChangePass;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            EventControl = UserComtrolE.LogOut;
        }
    }

    public enum UserComtrolE
    {
        ChangePass,
        LogOut
    }
}
