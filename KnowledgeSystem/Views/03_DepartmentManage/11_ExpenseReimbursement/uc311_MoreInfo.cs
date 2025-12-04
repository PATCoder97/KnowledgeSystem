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

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class uc311_MoreInfo : DevExpress.XtraEditors.XtraUserControl
    {
        public uc311_MoreInfo()
        {
            InitializeComponent();

            var accountingSubjects = dt311_AccountingSubjectBUS.Instance.GetList()
                .Select(x => new dt311_AccountingSubject
                {
                    Code = x.Code,
                    DisplayName = $"{x.Code} - {x.DisplayName}"
                }).ToList();

            cbbSubjectCode.Properties.DataSource = accountingSubjects;
            cbbSubjectCode.Properties.ValueMember = "Code";
            cbbSubjectCode.Properties.DisplayMember = "DisplayName";

            // Thiết lập DataBindings
            txbDesc.DataBindings.Add("Text", this, nameof(Desc), false, DataSourceUpdateMode.OnPropertyChanged);
            cbbSubjectCode.DataBindings.Add("EditValue", this, nameof(Code), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public string Desc { get; set; }
        public string Code { get; set; }
    }
}
