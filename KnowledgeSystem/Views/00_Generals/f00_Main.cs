using DevExpress.XtraEditors;
using KnowledgeSystem.Configs;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_Main : DevExpress.XtraEditors.XtraForm
    {
        public f00_Main()
        {
            InitializeComponent();
        }

        TileItemElement elementName = new TileItemElement();
        TileItemElement elementIdDept = new TileItemElement();
        TileItemElement elementGrade = new TileItemElement();
        TileItemElement elementClass = new TileItemElement();

        private void IniUserInfo()
        {
            elementName.Appearance.Normal.Font = new Font("DFKai-SB", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementName.Appearance.Normal.Options.UseFont = true;
            elementName.TextAlignment = TileItemContentAlignment.TopLeft;

            elementIdDept.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementIdDept.Appearance.Normal.Options.UseFont = true;
            elementIdDept.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementIdDept.TextLocation = new Point(5, -40);

            elementGrade.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementGrade.Appearance.Normal.Options.UseFont = true;
            elementGrade.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementGrade.TextLocation = new Point(5, -20);

            elementClass.Appearance.Normal.Font = new Font("DFKai-SB", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            elementClass.Appearance.Normal.Options.UseFont = true;
            elementClass.TextAlignment = TileItemContentAlignment.BottomLeft;
            elementClass.TextLocation = new Point(5, 0);

            tileInfoUser.Elements.Add(elementName);
            tileInfoUser.Elements.Add(elementIdDept);
            tileInfoUser.Elements.Add(elementGrade);
            tileInfoUser.Elements.Add(elementClass);
        }

        private void GetUserLogin()
        {
            Size = new Size(100, 100);
            WindowState = FormWindowState.Normal;
            f00_Login frm = new f00_Login();
            frm.ShowDialog();

            if (!TempDatas.LoginSuccessful)
            {
                Close();
                return;
            }

            WindowState = FormWindowState.Maximized;
            Size = new Size(1000, 600);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                           (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var userLogin = db.Users.First(r => r.Id == TempDatas.LoginId);
                string userName = userLogin.DisplayName;
                string idDept = userLogin.IdDepartment;
                var gradeName = db.dm_Departments.First(r => r.Id == idDept.Substring(0, 2)).DisplayName;
                var gradeClass = db.dm_Departments.First(r => r.Id == idDept).DisplayName;

                elementName.Text = userName;
                elementIdDept.Text = idDept;
                elementGrade.Text = gradeName;
                elementClass.Text = gradeClass;
            }
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            Text = TempDatas.SoftNameTW;
            lbSoftName.Text = TempDatas.SoftNameTW;

            Size = new Size(100, 100);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            StartPosition = FormStartPosition.CenterScreen;

            IniUserInfo();
        }

        private void fMain_Shown(object sender, EventArgs e)
        {
            GetUserLogin();
        }

        private void btnKnowHow_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(1);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnISODocuments_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(201);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnUserManage_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(7);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnRoleManage_ItemClick(object sender, TileItemEventArgs e)
        {
            f00_FluentFrame formShow = new f00_FluentFrame(17);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void tileInfoUser_ItemClick(object sender, TileItemEventArgs e)
        {
            GetUserLogin();
        }
    }
}