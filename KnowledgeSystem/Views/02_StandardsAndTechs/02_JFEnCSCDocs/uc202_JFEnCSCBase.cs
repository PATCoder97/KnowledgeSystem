using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs
{
    public partial class uc202_JFEnCSCBase : DevExpress.XtraEditors.XtraUserControl
    {
        public uc202_JFEnCSCBase()
        {
            InitializeComponent();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f202_DocInfo fInfo = new f202_DocInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "大學新進人員";
            fInfo.ShowDialog();

          //  LoadData();
        }
    }
}
