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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._03_TechnicalPrj
{
    public partial class uc203_TechnicalPrjBase : DevExpress.XtraEditors.XtraUserControl
    {
        public uc203_TechnicalPrjBase()
        {
            InitializeComponent();
        }

        private void uc203_TechnicalPrjBase_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f203_PrjInfo frm = new f203_PrjInfo();
            frm.ShowDialog();
        }
    }
}
