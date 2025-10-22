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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._06_InternationalStd
{
    public partial class uc206_InternationalStdMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc206_InternationalStdMain()
        {
            InitializeComponent();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f206_Doc_Info doc_Info = new f206_Doc_Info();
            doc_Info.ShowDialog();
        }
    }
}
