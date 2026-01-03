using BusinessLayer;
using DataAccessLayer;
using DevExpress.Export.Xl;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_Area5SResponsible_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_Area5SResponsible_Info()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            pic5SArea.AllowDrop = true;
            pic5SArea.SizeMode = PictureBoxSizeMode.Zoom;

            // Gán sự kiện
            pic5SArea.DragEnter += pic5SArea_DragEnter;
            pic5SArea.DragDrop += pic5SArea_DragDrop;

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;
        string imgAreaPath = "";
        HashSet<string> validImageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".bmp" };
        public bool isEditorInfo = true;
        public bool isAddInfo = true;

        dt310_Area5S area5S;
        List<dt310_Area5SResponsible> area5SResponsibles = new List<dt310_Area5SResponsible>();
        BindingSource source5sResp = new BindingSource();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        DXMenuItem itemRemoveItem;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEditResponsibility.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnConfirmResponsibility.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void InitializeMenuItems()
        {
            itemRemoveItem = CreateMenuItem("刪除", ItemRemoveItem_Click, TPSvgimages.Remove);
        }

        private void ItemRemoveItem_Click(object sender, EventArgs e)
        {
            DXMenuItem item = sender as DXMenuItem;
            if (item == null) return;

            int rowHandle = (int)item.Tag;
            if (rowHandle < 0) return;

            dt310_Area5SResponsible areaItem = gvData.GetRow(rowHandle) as dt310_Area5SResponsible;
            if (areaItem == null) return;

            area5SResponsibles.Remove(areaItem);
            gvData.RefreshData();
        }

        private void EnabledController(bool _enable = true)
        {
            txbArea.Enabled = _enable;
            txbDesc.Enabled = _enable;
            pic5SArea.Enabled = _enable;

            gvData.ReadOnlyGridView(!_enable);
        }

        private void LockControl()
        {
            if (isAddInfo)
            {
                lcGridUser.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    EnabledController();

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEditResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnConfirmResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    if (isEditorInfo)
                    {
                        EnabledController();

                        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                        btnEditResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnConfirmResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                        gvData.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
                        gvData.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
                    }
                    else
                    {
                        EnabledController(false);

                        gvData.ReadOnlyGridView(false);

                        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                        btnEditResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                        btnConfirmResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                        gvData.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
                        gvData.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
                    }

                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    EnabledController(false);

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    btnEditResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnConfirmResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    EnabledController(false);

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    btnEditResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnConfirmResponsibility.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    gvData.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
                    gvData.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;

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

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).Select(r => new
            {
                DisplayName = $"LG{r.IdDepartment}/{r.DisplayName}",
                Id = r.Id,
                DeptId = r.IdDepartment
            }).ToList();

            itemcbbEmp.DataSource = usrs;
            itemcbbEmp.DisplayMember = "DisplayName";
            itemcbbEmp.ValueMember = "Id";

            var depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true).Select(r => r.Id).ToList();
            itemcbbDept.Items.AddRange(depts);

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
                    txbDesc.EditValue = area5S.DESC;

                    pic5SArea.Image = Image.FromFile(Path.Combine(TPConfigs.Folder310, area5S.FileName));

                    area5SResponsibles = dt310_Area5SResponsibleBUS.Instance.GetListByAreaId(idBase);
                    source5sResp.DataSource = area5SResponsibles;
                    gcData.DataSource = source5sResp;
                    gvData.BestFitColumns();

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

        private void pic5SArea_DoubleClick(object sender, EventArgs e)
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
            isEditorInfo = true;
            LockControl();
        }

        private void btnEditResponsibility_ItemClick(object sender, ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            isEditorInfo = false;
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
                        break;
                    }
                }
            }

            if (pic5SArea.Image == null)
            {
                IsValidate = false;
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
                area5S.DESC = desc;

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

                        if (!string.IsNullOrEmpty(imgAreaPath))
                        {
                            encryptionName = EncryptionHelper.EncryptionFileName(imgAreaPath);
                            area5S.FileName = encryptionName;

                            using (Image img = Image.FromFile(imgAreaPath))
                            {
                                Directory.CreateDirectory(TPConfigs.Folder310);
                                img.Save(Path.Combine(TPConfigs.Folder310, encryptionName), ImageFormat.Png);
                            }
                        }

                        result = dt310_Area5SBUS.Instance.AddOrUpdate(area5S);

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}：{area5S.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt310_Area5SBUS.Instance.RemoveById(area5S.Id, TPConfigs.LoginUser.Id);

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

        private void btnConfirmResponsibility_ItemClick(object sender, ItemClickEventArgs e)
        {
            var result = true;

            var newList = source5sResp.DataSource as List<dt310_Area5SResponsible>;

            var compareProps = new[]
            {
                "DeptId",
                "EmployeeId",
                "AgentId",
                "BossId",
                "AreaName",
                "AreaCode",
            };

            // Kiểm tra null/rỗng
            foreach (var item in newList)
            {
                foreach (var prop in compareProps)
                {
                    var value = item.GetType().GetProperty(prop)?.GetValue(item) as string;

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        MsgTP.MsgError("資料尚未填寫完成，請檢查後再試！");
                        return;
                    }
                }
            }

            // Lấy danh sách cũ trong DB
            var oldList = dt310_Area5SResponsibleBUS.Instance.GetListByAreaId(idBase);

            var inserts = newList.Where(x => x.Id == 0).ToList();
            var updates = newList.Where(x => x.Id > 0).ToList();
            var deletes = oldList.Where(o => !newList.Any(n => n.Id == o.Id)).ToList();

            // INSERT
            foreach (var item in inserts)
            {
                item.AreaId = idBase;
                item.CreatedAt = DateTime.Now;
                item.CreatedBy = TPConfigs.LoginUser.Id;

                result &= dt310_Area5SResponsibleBUS.Instance.Add(item);
            }

            // UPDATE
            foreach (var item in updates)
            {
                var old = oldList.FirstOrDefault(o => o.Id == item.Id);

                if (old != null && !AreEqual(old, item))
                {
                    result &= dt310_Area5SResponsibleBUS.Instance.AddOrUpdate(item);
                }
            }

            // DELETE
            foreach (var item in deletes)
            {
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = TPConfigs.LoginUser.Id;

                result &= dt310_Area5SResponsibleBUS.Instance.AddOrUpdate(item);
            }

            // KẾT QUẢ
            if (result)
                Close();
            else
                MsgTP.MsgErrorDB();
        }

        private bool AreEqual(dt310_Area5SResponsible oldItem, dt310_Area5SResponsible newItem)
        {
            // Danh sách property cho phép chỉnh sửa
            var compareProps = new[]
            {
                "DeptId",
                "EmployeeId",
                "AgentId",
                "BossId",
                "AreaName",
                "AreaCode",
            };

            foreach (var name in compareProps)
            {
                var prop = typeof(dt310_Area5SResponsible).GetProperty(name);
                var oldValue = prop.GetValue(oldItem);
                var newValue = prop.GetValue(newItem);

                if (oldValue == null && newValue == null)
                    continue;

                if (!(oldValue?.Equals(newValue) ?? false))
                    return false; // khác → cần update
            }

            return true; // không thay đổi
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {

        }

        private void gvData_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            view.BestFitColumns();


            //if (e.Column.FieldName == "EmployeeId")
            //{
            //    // Lấy giá trị mới
            //    var name = e.Value;

            //    //// Tìm tên tương ứng (anh đổi thành nguồn dữ liệu thật của anh)
            //    //string name = GetAreaNameFromFileId(fileId);

            //    //// Cập nhật cột B
            //    view.SetRowCellValue(e.RowHandle, "DeptId", name);
            //}
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                int rowHandle = e.HitInfo.RowHandle;
                itemRemoveItem.Tag = rowHandle;

                e.Menu.Items.Add(itemRemoveItem);

                //e.Menu.Items.Add(itemERP02);
                //e.Menu.Items.Add(itemERP03);

                //GridView view = gvData;
                //string typeOfSeller = view.GetRowCellValue(view.FocusedRowHandle, gColSellerType)?.ToString();
                //int attId = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColAttId) ?? -1);

                //itemAddFile.BeginGroup = true;
                //e.Menu.Items.Add(itemAddFile);
                //if (attId != -1)
                //{
                //    e.Menu.Items.Add(itemViewFile);
                //}

                //if (typeOfSeller == "xang_dau")
                //{
                //    itemUpdateAddFuel.BeginGroup = true;
                //    e.Menu.Items.Add(itemUpdateAddFuel);
                //}
            }
        }
    }
}