using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_SparePartMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_SparePartMain()
        {
            InitializeComponent();
        }

        private void uc309_SparePartMain_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Material_Info finfo = new f309_Material_Info();
            finfo.ShowDialog();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Material_Info finfo = new f309_Material_Info()
            {
                idBase = 1,
                eventInfo = Helpers.EventFormInfo.View
            };
            finfo.ShowDialog();
        }
    }
}
