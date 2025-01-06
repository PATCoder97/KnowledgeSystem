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
            txbDocType.DataBindings.Add("EditValue", this, "IdDocType");
            txbFieldType.DataBindings.Add("EditValue", this, "IdFieldType");
        }

        public dt306_Base baseData { get; set; }

        public string DisplayName { get; set; }
        public string IdFieldType { get; set; }
        public string IdDocType { get; set; }
        public bool Confidential { get; set; }

        private void uc306_EditSignDoc_Load(object sender, EventArgs e)
        {
            var dmFieldType = dt306_FieldTypeBUS.Instance.GetList();
            txbFieldType.Properties.DataSource = dmFieldType;
            txbFieldType.Properties.DisplayMember = "DisplayName";
            txbFieldType.Properties.ValueMember = "Id";

            DisplayName = baseData.DisplayName;
            IdFieldType = baseData.IdFieldType;
            IdDocType = baseData.IdDocType;
            Confidential = baseData.Confidential;
        }

        private void txbFieldType_EditValueChanged(object sender, EventArgs e)
        {
            txbDocType.EditValue = "";

            var fieldType = txbFieldType.EditValue.ToString();
            var idDocTypes = dt306_FieldTypeDocTypeBUS.Instance.GetListByIdField(fieldType);
            var dmDocTypes = dt306_DocTypeBUS.Instance.GetListByIds(idDocTypes.Select(r => r.IdDocType).ToList()).ToList();

            txbDocType.Properties.DataSource = dmDocTypes;
            txbDocType.Properties.DisplayMember = "DisplayName";
            txbDocType.Properties.ValueMember = "Id";
        }
    }
}
