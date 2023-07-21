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

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage
{
    public partial class f401_GroupManage_Info : DevExpress.XtraEditors.XtraForm
    {
        public f401_GroupManage_Info()
        {
            InitializeComponent();
            LockControl(false);
        }

        private void LockControl(bool isFormView = true)
        {
            txbName.ReadOnly = isFormView;
            txbDescrip.ReadOnly = isFormView;

            btnEdit.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDel.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;

            //   Text = isFormView ? "Thông tin ngân hàng" : "Thêm/sửa ngân hàng";
        }

        private void f401_GroupManage_Info_Load(object sender, EventArgs e)
        {

        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }
    }
}