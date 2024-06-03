using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_SignatureMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_SignatureMain()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f306_NewSignDoc fAdd = new f306_NewSignDoc();
            fAdd.ShowDialog();
        }
    }
}
