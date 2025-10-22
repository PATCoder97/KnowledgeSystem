using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_FluentFrame : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public f00_FluentFrame()
        {
            InitializeComponent();
        }

        public f00_FluentFrame(int groupId_)
        {
            InitializeComponent();
            groupId = groupId_;
        }

        #region parameters

        // Khai báo các BUS để dùng BD
        dm_FunctionRoleBUS _dm_FunctionRoleBUS = new dm_FunctionRoleBUS();

        Font fontTW14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        int groupId = 0;
        List<dm_FunctionM> lsFunctions = new List<dm_FunctionM>();

        #endregion

        #region methods

        public void OpenForm(string nameForm, string formName = "")
        {
            // Lấy kiểu của form cần mở từ assembly đang thực thi
            var typeform = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(a => a.Name == nameForm);
            if (typeform == null) return;

            // Hiển thị màn hình chờ sử dụng SplashScreenManager
            if (typeform.BaseType == typeof(XtraUserControl))
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(this))
                {
                    div_container.Controls.Clear();

                    XtraUserControl userControl = (XtraUserControl)Activator.CreateInstance(typeform);
                    userControl.Dock = DockStyle.Fill;
                    userControl.BringToFront();
                    userControl.Text = formName;
                    div_container.Controls.Add(userControl);
                }
            }
            else if (typeform.BaseType == typeof(XtraForm))
            {
                var f = (XtraForm)Activator.CreateInstance(typeform);
                f.Text = formName;
                f.ShowDialog();
            }
            else if (typeform.BaseType == typeof(RibbonForm))
            {
                var f = (RibbonForm)Activator.CreateInstance(typeform);
                f.Text = formName;
                f.ShowDialog();
            }
        }

        #endregion

        private void f00_FluentFrame_Load(object sender, EventArgs e)
        {
            // 1️ Lấy toàn bộ cây chức năng
            var lsAllFunctions = dm_FunctionBUS.Instance.GetAllDescendantsByParent(groupId);
            var lsPermissions = AppPermission.lsPermissions;

            // 2️ Lọc theo quyền được cấp
            lsFunctions = (from data in lsAllFunctions
                           join granted in lsPermissions on data.Id equals granted
                           select data)
                             .OrderBy(x => x.Prioritize)
                             .ToList();

            // 3️ Xây dựng menu Accordion tự động theo cây
            BuildAccordionElements(lsFunctions, fluentControl.Elements, lsPermissions, fontTW14);
        }

        /// <summary>
        /// Đệ quy thêm các phần tử vào AccordionControl.
        /// Hỗ trợ nhiều cấp con (cha, con, cháu, chắt...).
        /// </summary>
        private void BuildAccordionElements(List<dm_FunctionM> functions, AccordionControlElementCollection parentCollection, List<int> lsPermissions, Font fontTW14)
        {
            foreach (var item in functions)
            {
                AccordionControlElement accordion = new AccordionControlElement();

                // 🖼 Hình ảnh
                string pathImage = Path.Combine(TPConfigs.StartupPath, "Images", item.Images ?? "");
                if (!string.IsNullOrEmpty(item.Images) && File.Exists(pathImage))
                    accordion.ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromFile(pathImage);

                // 🏷️ Thông tin hiển thị
                accordion.Name = "name_" + item.ControlName;
                accordion.Text = item.DisplayName;
                accordion.Appearance.Default.Font = fontTW14;
                accordion.Appearance.Normal.ForeColor = Color.Black;
                accordion.Appearance.Hovered.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                accordion.Appearance.Pressed.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
                accordion.Hint = item.DisplayName;

                // 🔁 Lấy các con có quyền
                var children = item.Children
                    .Where(c => lsPermissions.Contains(c.Id))
                    .OrderBy(c => c.Prioritize)
                    .ToList();

                if (children.Count > 0)
                {
                    // Có con → là nhóm
                    accordion.Style = ElementStyle.Group;
                    BuildAccordionElements(children, accordion.Elements, lsPermissions, fontTW14);
                }
                else
                {
                    // Không có con → là item cuối cùng
                    accordion.Style = ElementStyle.Item;
                    accordion.Click += new EventHandler(accordionElement_Click);
                    accordion.Appearance.Normal.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Hyperlink;
                }

                // Thêm vào danh sách hiện tại
                parentCollection.Add(accordion);
            }
        }

        private void accordionElement_Click(object sender, EventArgs e)
        {
            AccordionControlElement buttonAuto = sender as AccordionControlElement;
            OpenForm(buttonAuto.Name.Replace("name_", ""), buttonAuto.Text);
        }

        private void f00_FluentFrame_Shown(object sender, EventArgs e)
        {
            List<int> lsAutoOpenForm = AppPermission.GetListAutoOpenForm();

            // Nếu GroupId có trong list, chọn AppForm đầu tiên trong TreeView và mở form tương ứng
            if (lsAutoOpenForm.Contains(groupId) && lsFunctions.Count > 0)
            {
                // Lấy AppForm đầu tiên trong TreeView và mở form tương ứng
                var formShow = lsFunctions[0];
                OpenForm(formShow.ControlName);
            }
        }

        private void f00_FluentFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Giải phóng tài nguyên của f00_ViewMultiFile
            var viewFilesForm = f00_ViewMultiFile.Instance;
            viewFilesForm.Dispose();
        }
    }
}