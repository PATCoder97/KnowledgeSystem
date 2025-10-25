using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class f311_AutoERP : DevExpress.XtraEditors.XtraForm
    {
        public f311_AutoERP(List<string> datas)
        {
            InitializeComponent();
            keyData = datas;
        }

        // Import the mouse_event function from user32.dll
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // Constants for mouse event flags
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000;
                return param;
            }
        }

        List<string> keyData = new List<string>();

        public void BlockUserInput(Action action)
        {
            Form overlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.Black,
                Opacity = 0.05,
                WindowState = FormWindowState.Maximized,
                TopMost = true,
                ShowInTaskbar = false
            };

            overlay.Show();
            overlay.BringToFront();
            Application.DoEvents();

            try
            {
                action?.Invoke(); // Gửi phím, chạy thao tác
            }
            finally
            {
                overlay.Close();
                overlay.Dispose();
            }
        }

        private void f311_AutoERP_Load(object sender, EventArgs e)
        {
            Text = "自動輸入ERP";
            TopMost = true;

            StartPosition = FormStartPosition.Manual;
            int x = Screen.PrimaryScreen.WorkingArea.Right - Width;
            int y = Screen.PrimaryScreen.WorkingArea.Top;
            Location = new Point(x, y);
        }

        private void btnAutoKey_Click(object sender, EventArgs e)
        {
            if (keyData == null || keyData.Count == 0)
            {
                XtraMessageBox.Show("Dữ liệu trống!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                //// 🧩 Dùng overlay để chặn người dùng trong suốt quá trình gửi phím
                //BlockUserInput(() =>
                //{
                foreach (string key in keyData)
                {
                    SendKeys.SendWait(key);
                    Thread.Sleep(1000);
                }
                //});
            }

            Close();
        }
    }
}