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
    public partial class uc00_AdminLogin : XtraUserControl
    {
        public uc00_AdminLogin()
        {
            InitializeComponent();

            txbID.DataBindings.Add("Text", this, "ID");
            txbPass.DataBindings.Add("Text", this, "MasterKey");
        }

        public string ID { get; set; }
        public string MasterKey { get; set; }
    }
}
