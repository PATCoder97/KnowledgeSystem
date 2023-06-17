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

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class fMain : DevExpress.XtraEditors.XtraForm
    {
        public fMain()
        {
            InitializeComponent();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            Text = TempDatas.SoftNameTW;
            lbSoftName.Text = TempDatas.SoftNameTW;

            Size = new Size(100, 100);
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                          (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            StartPosition = FormStartPosition.CenterScreen;

            System.Threading.Thread.Sleep(5000);
        }

        private void btnKnowHow_ItemClick(object sender, TileItemEventArgs e)
        {
            fFrame formShow = new fFrame(207);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void btnISODocuments_ItemClick(object sender, TileItemEventArgs e)
        {
            fFrame formShow = new fFrame(201);
            formShow.Text = e.Item.Text + AppCopyRight.CopyRightString();
            Hide();
            formShow.ShowDialog();
            Show();
        }

        private void fMain_Shown(object sender, EventArgs e)
        {
            Size = new Size(100, 100);
            WindowState = FormWindowState.Normal;
            fLogin frm = new fLogin();
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

        }
    }
}