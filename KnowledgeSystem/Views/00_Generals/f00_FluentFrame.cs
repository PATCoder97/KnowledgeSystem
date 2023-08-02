using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System.Reflection;
using DevExpress.XtraBars.Navigation;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;

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

        Font fontTW = new Font("DFKai-SB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

        int groupId = 0;
        List<Function> lsFunctions = new List<Function>();

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
                    XtraUserControl userControl = (XtraUserControl)Activator.CreateInstance(typeform);
                    if (!div_container.Controls.Contains(userControl))
                    {
                        div_container.Controls.Add(userControl);
                        userControl.Dock = DockStyle.Fill;
                        userControl.BringToFront();
                    }
                    else
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
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsFunctions = db.Functions.Where(r => r.IdParent == groupId).OrderBy(r => r.Prioritize).ToList();
            }

            foreach (var item in lsFunctions)
            {
                AccordionControlElement accordion = new AccordionControlElement();

                accordion.ImageOptions.SvgImage = item.Images != null ? DevExpress.Utils.Svg.SvgImage.FromFile($@"Images\{item.Images}") : null;
                accordion.Name = $"name_{item.ControlName}";
                accordion.Style = ElementStyle.Item;
                accordion.Text = item.DisplayName;
                accordion.Appearance.Normal.Font = fontTW;
                accordion.Appearance.Hovered.Font = fontTW;
                accordion.Appearance.Pressed.Font = fontTW;
                accordion.Appearance.Pressed.ForeColor = Color.Red;
                accordion.Hint = item.DisplayName;

                accordion.Click += new EventHandler(accordionElement_Click);
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
            // Nếu GroupId là 1, chọn AppForm đầu tiên trong TreeView và mở form tương ứng
            if (groupId == 1)
            {
                // Lấy AppForm đầu tiên trong TreeView và mở form tương ứng
                var formShow = lsFunctions[0];
                OpenForm(formShow.ControlName);
            }
        }
    }
}