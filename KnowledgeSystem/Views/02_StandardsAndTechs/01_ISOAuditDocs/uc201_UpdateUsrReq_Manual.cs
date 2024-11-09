using BusinessLayer;
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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_UpdateUsrReq_Manual : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_UpdateUsrReq_Manual()
        {
            InitializeComponent();

            var users = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0 && r.IdDepartment == TPConfigs.LoginUser.IdDepartment).ToList();
            txbUser.Properties.DataSource = users;
            txbUser.Properties.DisplayMember = "DisplayName";
            txbUser.Properties.ValueMember = "Id";

            txbEventChange.Properties.Items.AddRange(new object[] { "新增", "取消" });

            txbUser.DataBindings.Add("EditValue", this, "UserId", false, DataSourceUpdateMode.OnPropertyChanged);
            txbEventChange.DataBindings.Add("Text", this, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public string UserId { get; set; }
        public string Description { get; set; }
    }
}
