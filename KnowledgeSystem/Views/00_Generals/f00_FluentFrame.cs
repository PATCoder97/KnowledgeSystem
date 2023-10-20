using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
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

        Font fontTW14 = new Font("DFKai-SB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        int groupId = 0;
        List<dm_FunctionM> lsFunctions = new List<dm_FunctionM>();

        #endregion

        #region methods

        public void OpenForm(string nameForm)
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
                    div_container.Controls.Add(userControl);
                    userControl.Dock = DockStyle.Fill;
                    userControl.BringToFront();
                }
            }
            else if (typeform.BaseType == typeof(XtraForm))
            {
                var f = (Form)Activator.CreateInstance(typeform);
                f.ShowDialog();
            }
        }

        #endregion

        private void f00_FluentFrame_Load(object sender, EventArgs e)
        {
            // Lấy danh sách các AppForm từ cơ sở dữ liệu và điền vào TreeView control
            var lsAllFunctions = dm_FunctionBUS.Instance.GetListByIdParent(groupId);
            var lsPermissions = AppPermission.lsPermissions;

            lsFunctions = (from data in lsAllFunctions
                           join granted in lsPermissions on data.Id equals granted
                           select data).ToList();

            foreach (var item in lsFunctions)
            {
                AccordionControlElement accordion = new AccordionControlElement();

                string pathImage = Path.Combine(Application.StartupPath, "Images", item.Images ?? "");
                accordion.ImageOptions.SvgImage = item.Images != null ? DevExpress.Utils.Svg.SvgImage.FromFile(pathImage) : null;
                accordion.Name = $"name_{item.ControlName}";
                accordion.Text = item.DisplayName;
                accordion.Appearance.Default.Font = fontTW14;

                accordion.Appearance.Normal.ForeColor = Color.Black;
                accordion.Appearance.Hovered.ForeColor = Color.OrangeRed;
                accordion.Appearance.Pressed.ForeColor = Color.OrangeRed;

                accordion.Hint = item.DisplayName;

                var lsFuncChild = dm_FunctionBUS.Instance.GetListByIdParent(item.Id);
                var lsChildren = (from data in lsFuncChild
                                  join granted in lsPermissions on data.Id equals granted
                                  select data).ToList();

                if (lsChildren.Count != 0)
                {
                    foreach (var child in lsChildren)
                    {
                        AccordionControlElement accordionChild = new AccordionControlElement();

                        pathImage = Path.Combine(Application.StartupPath, "Images", child.Images ?? "");
                        accordionChild.ImageOptions.SvgImage = child.Images != null ? DevExpress.Utils.Svg.SvgImage.FromFile($@"Images\{child.Images}") : null;
                        accordionChild.Name = $"name_{child.ControlName}";
                        accordionChild.Text = child.DisplayName;
                        accordionChild.Style = ElementStyle.Item;
                        accordionChild.Appearance.Default.Font = fontTW14;

                        accordionChild.Appearance.Normal.ForeColor = Color.Black;
                        accordionChild.Appearance.Hovered.ForeColor = Color.OrangeRed;
                        accordionChild.Appearance.Pressed.ForeColor = Color.Red;

                        accordionChild.Hint = child.DisplayName;
                        accordionChild.Click += new EventHandler(accordionElement_Click);

                        accordion.Elements.Add(accordionChild);
                    }
                }
                else
                {
                    accordion.Style = ElementStyle.Item;
                    accordion.Click += new EventHandler(accordionElement_Click);
                }

                fluentControl.Elements.Add(accordion);
            }
        }

        private void accordionElement_Click(object sender, EventArgs e)
        {
            AccordionControlElement buttonAuto = sender as AccordionControlElement;
            OpenForm(buttonAuto.Name.Replace("name_", ""));
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
    }
}