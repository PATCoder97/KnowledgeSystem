using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_Func_Info : EditFormUserControl
    {
        public uc402_Func_Info()
        {
            InitializeComponent();
        }

        private void uc402_Func_Info_Load(object sender, EventArgs e)
        {
            var lsControl = Assembly.GetExecutingAssembly().GetTypes().Where(r => r.BaseType.Name == "XtraUserControl" || r.BaseType.Name == "XtraForm").Select(r => r.Name).ToList();
            cbbControl.Properties.Items.AddRange(lsControl);
        }
    }
}
