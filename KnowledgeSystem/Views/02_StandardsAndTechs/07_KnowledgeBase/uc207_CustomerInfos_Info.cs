using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_CustomerInfos_Info : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_CustomerInfos_Info()
        {
            InitializeComponent();
        }

        private void uc207_CustomerInfos_Info_Load(object sender, EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsUsers = db.Users.ToList();
                cbbIdUser.Properties.DataSource = lsUsers;
                cbbIdUser.Properties.DisplayMember = "DisplayName";
                cbbIdUser.Properties.ValueMember = "Id";

                cbbIdUser.EditValue = TempDatas.LoginId;
            }
        }
    }
}
