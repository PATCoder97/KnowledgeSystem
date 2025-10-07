using BusinessLayer;
using DataAccessLayer;
using DevExpress.Export.Xl;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Drawing.Charts;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_Area5SResponsible_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_Area5SResponsible_Info()
        {
            InitializeComponent();
            InitializeIcon();

            pic5SArea.AllowDrop = true;
            pic5SArea.SizeMode = PictureBoxSizeMode.Zoom;

            // Gán sự kiện
            pic5SArea.DragEnter += pic5SArea_DragEnter;
            pic5SArea.DragDrop += pic5SArea_DragDrop;
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;
        string imgAreaPath = "";
        HashSet<string> validImageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };

        dt310_Area5S area5S;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbArea.Enabled = _enable;
            txbDesc.Enabled = _enable;
            //cbbFunc.Enabled = _enable;
            //txbStartDate.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();

                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f310_Area5SResponsible_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcArea, lcDesc };
            lcImpControls = new List<LayoutControlItem>() { lcArea };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            //var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).ToList();
            //cbbUsr.Properties.DataSource = usrs;
            //cbbUsr.Properties.DisplayMember = "DisplayName";
            //cbbUsr.Properties.ValueMember = "Id";

            //var depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true).ToList();
            //cbbDept.Properties.DataSource = depts;
            //cbbDept.Properties.DisplayMember = "DisplayName";
            //cbbDept.Properties.ValueMember = "Id";

            //var funcs = dt310_FunctionBUS.Instance.GetList();
            //cbbFunc.Properties.DataSource = funcs;
            //cbbFunc.Properties.DisplayMember = "DisplayName";
            //cbbFunc.Properties.ValueMember = "Id";

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    area5S = new dt310_Area5S();

                    break;
                case EventFormInfo.View:

                    area5S = dt310_Area5SBUS.Instance.GetItemById(idBase);

                    txbArea.EditValue = area5S.DisplayName;

                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }

            LockControl();
        }

        private void pic5SArea_DragDrop(object sender, DragEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(pic5SArea))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    try
                    {
                        pic5SArea.Image = Image.FromFile(files[0]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể mở ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void pic5SArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string ext = Path.GetExtension(files[0]).ToLower();

                e.Effect = validImageExtensions.Contains(ext) ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pic5SArea_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var handle = SplashScreenManager.ShowOverlayForm(pic5SArea))
                    {
                        imgAreaPath = ofd.FileName;
                        pic5SArea.Image = Image.FromFile(imgAreaPath);
                    }
                }
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var area = txbArea.EditValue?.ToString();
            var desc = txbDesc.EditValue?.ToString();

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                area5S.DisplayName = area;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        area5S.CreatedAt = DateTime.Now;
                        area5S.CreatedBy = TPConfigs.LoginUser.Id;

                        string encryptionName = EncryptionHelper.EncryptionFileName(imgAreaPath);
                        area5S.FileName = encryptionName;

                        using (Image img = Image.FromFile(imgAreaPath))
                        {
                            Directory.CreateDirectory(TPConfigs.Folder310);
                            img.Save(Path.Combine(TPConfigs.Folder310, encryptionName), ImageFormat.Png);
                        }

                        result = dt310_Area5SBUS.Instance.Add(area5S);

                        break;
                    case EventFormInfo.Update:

                        result = dt310_Area5SBUS.Instance.AddOrUpdate(area5S);

                        break;
                    case EventFormInfo.Delete:

                        //var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}\r\n{cbbFunc.Text}：{cbbUsr.Text}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (dialogResult != DialogResult.Yes) return;
                        //result = dt310_EHSFunctionBUS.Instance.RemoveById(EHSFunc.Id, TPConfigs.LoginUser.Id);

                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }
    }
}