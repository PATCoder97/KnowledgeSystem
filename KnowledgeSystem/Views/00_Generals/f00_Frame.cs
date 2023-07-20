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
    public partial class f00_Frame : DevExpress.XtraEditors.XtraForm
    {
        public f00_Frame()
        {
            InitializeComponent();
        }

        public f00_Frame(int groupId_)
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
            // Lấy kiểu của form cần mở từ assembly đang thực thi
            var typeform = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => a.BaseType == typeof(XtraForm) && a.Name == nameForm);

            // Nếu không tìm thấy kiểu của form, thoát khỏi phương thức
            if (typeform == null) return;

            // Kiểm tra xem form đã được mở chưa
            foreach (Form frm in MdiChildren)
            {
                if (frm.GetType() == typeform && frm.Text == textForm)
                {
                    // Nếu form đã được mở, kích hoạt và thoát khỏi phương thức
                    frm.Activate();
                    return;
                }
            }

            // Hiển thị màn hình chờ sử dụng SplashScreenManager
            using (var handle = SplashScreenManager.ShowOverlayForm(this, customPainter: new CustomOverlayPainter()))
            {
                // Tạo một đối tượng form mới, thiết lập các thuộc tính của nó và hiển thị form
                var f = (Form)Activator.CreateInstance(typeform);
                f.MdiParent = this;
                f.Text = textForm;
                f.Show();
            }
        }

        #endregion

        private void fFrame_Load(object sender, EventArgs e)
        {
            // Lấy danh sách các AppForm từ cơ sở dữ liệu và điền vào TreeView control
            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Lấy danh sách các AppForm có GroupId bằng gruopId, sắp xếp theo IndexRow
                lsAppForms = db.AppForms.Select(r => r).Where(r => r.GroupId == gruopId).OrderBy(r => r.IndexRow).ToList();
            }

            // Thiết lập các thuộc tính cho TreeView control
            treeAppForm.DataSource = lsAppForms;
            treeAppForm.ParentFieldName = "ParentId";
            treeAppForm.KeyFieldName = "Id";

            // Nếu GroupId là 207, chọn AppForm đầu tiên trong TreeView và mở form tương ứng
            if (gruopId == 207)
            {
                // Lấy AppForm đầu tiên trong TreeView và mở form tương ứng
                var formShow = treeAppForm.GetRow(0) as AppForm;
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