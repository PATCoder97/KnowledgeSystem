using BusinessLayer;
using DataAccessLayer;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_EditSignDoc : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_EditSignDoc()
        {
            InitializeComponent();
            txbTitle.DataBindings.Add("Text", this, "DisplayName"); 
            txbType.DataBindings.Add("EditValue", this, "IdType");
            txbCode.DataBindings.Add("Text", this, "Code");
        }

        public dt306_Base baseData { get; set; }

        public string DisplayName { get; set; }
        public int IdType { get; set; }
        public string Code { get; set; }

        private void uc306_EditSignDoc_Load(object sender, EventArgs e)
        {
            var dmType = dt306_TypeBUS.Instance.GetList();
            txbType.Properties.DataSource = dmType;
            txbType.Properties.DisplayMember = "DisplayName";
            txbType.Properties.ValueMember = "Id";

            DisplayName = baseData.DisplayName;
            IdType = baseData.IdType;
            Code = baseData.Code;
        }
    }
}
