using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class fFrame : DevExpress.XtraEditors.XtraForm
    {
        public fFrame()
        {
            InitializeComponent();
        }

        public fFrame(int groupId_)
        {
            InitializeComponent();
            gruopId = groupId_;
        }

        #region parameters

        int gruopId = 0;
        List<AppForm> lsAppForms = new List<AppForm>();

        #endregion

        #region methods

        public void OpenForm(string nameForm, string textForm)
        {
            var typeform = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(XtraForm) && a.Name == nameForm).FirstOrDefault();
            if (typeform == null)
            {
                return;
            }

            foreach (Form frm in MdiChildren)
            {
                if (frm.GetType() == typeform && frm.Text == textForm)
                {
                    frm.Activate();
                    return;
                }
            }

            IOverlaySplashScreenHandle handle = SplashScreenManager.ShowOverlayForm(this, customPainter: new CustomOverlayPainter());

            Form f = (Form)Activator.CreateInstance(typeform);
            f.MdiParent = this;
            f.Text = textForm;
            f.Show();

            SplashScreenManager.CloseOverlayForm(handle);
        }

        #endregion

        private void fFrame_Load(object sender, EventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsAppForms = db.AppForms.Select(r => r).Where(r => r.GroupId == gruopId).OrderBy(r => r.IndexRow).ToList();
            }

            treeAppForm.DataSource = lsAppForms;
            treeAppForm.ParentFieldName = "ParentId";
            treeAppForm.KeyFieldName = "Id";

            if (gruopId == 207)
            {
                AppForm formShow = treeAppForm.GetRow(0) as AppForm;
                OpenForm(formShow.NameForm, formShow.DisplayName);
            }
        }

        private void btnShowForm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            AppForm formShow = treeAppForm.GetRow(treeAppForm.FocusedNode.Id) as AppForm;
            OpenForm(formShow.NameForm, formShow.DisplayName);
        }
    }
}