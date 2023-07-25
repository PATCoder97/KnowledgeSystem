using DevExpress.XtraBars;
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
using static DevExpress.Utils.Svg.CommonSvgImages;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_RibbonFrame : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public f00_RibbonFrame()
        {
            InitializeComponent();
        }

        public f00_RibbonFrame(int groupId_)
        {
            InitializeComponent();
            groupId = groupId_;
        }

        #region parameters

        int groupId = 0;
        List<AppForm> lsAppForms = new List<AppForm>();
        List<Function> lsFunctions = new List<Function>();

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

        private void f00_RibbonFrame_Load(object sender, EventArgs e)
        {
            // Lấy danh sách các AppForm từ cơ sở dữ liệu và điền vào TreeView control
            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Lấy danh sách các AppForm có GroupId bằng gruopId, sắp xếp theo IndexRow
                lsAppForms = db.AppForms.Select(r => r).Where(r => r.GroupId == groupId).OrderBy(r => r.IndexRow).ToList();

                lsFunctions = db.Functions.Where(r => r.IdParent == groupId).OrderBy(r => r.Prioritize).ToList();
            }

            foreach (var item in lsFunctions)
            {
                BarButtonItem barButtonItem2 = new BarButtonItem();

                barButtonItem2.Caption = item.DisplayName;
                barButtonItem2.Id = Convert.ToInt16(item.Prioritize);
                barButtonItem2.ImageOptions.SvgImage = DevExpress.Utils.Svg.SvgImage.FromFile(@"Images\Actions_AddCircled.svg");
                barButtonItem2.Name = $"name_{item.ControlName}";
                barButtonItem2.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
                barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ButtonItemAuto_ItemClick);

                barButtonItem2.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                barButtonItem2.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
                barButtonItem2.ItemAppearance.Normal.Options.UseFont = true;
                barButtonItem2.ItemAppearance.Normal.Options.UseForeColor = true;

                ribbonPageGroup1.ItemLinks.Add(barButtonItem2);
            }
        }

        private void ButtonItemAuto_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenForm(e.Item.Name.Replace("name_",""), e.Item.Caption);
        }
    }
}