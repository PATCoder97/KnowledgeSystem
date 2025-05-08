using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Wordprocessing;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_Material_Info : DevExpress.XtraEditors.XtraForm
    {
        public f309_Material_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public string idDeptGetData = TPConfigs.LoginUser.IdDepartment;

        dt309_Materials material;
        dt309_Materials oldMaterial;

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
            txbCode.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            cbbUnit.Enabled = _enable;
            txbLocation.Enabled = _enable;
            cbbUsr.Enabled = _enable;
            cbbTypeUse.Enabled = _enable;
            tokenMachine.Enabled = _enable;
            txbExpDate.Enabled = _enable;
            txbMinQuantity.Enabled = _enable;
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

        private void f309_Material_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcCode, lcDisplayName, lcUnit, lcLocation, lcUser, lcTypeUse, lcMachine, lcExpDate, lcMinQuantity };
            lcImpControls = new List<LayoutControlItem>() { lcCode, lcDisplayName, lcUnit, lcLocation, lcUser, lcMachine, lcMinQuantity };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var units = dt309_UnitsBUS.Instance.GetList();
            cbbUnit.Properties.DataSource = units;
            cbbUnit.Properties.DisplayMember = "DisplayName";
            cbbUnit.Properties.ValueMember = "Id";

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0 && r.IdDepartment == idDeptGetData).ToList();
            cbbUsr.Properties.DataSource = usrs;
            cbbUsr.Properties.DisplayMember = "DisplayName";
            cbbUsr.Properties.ValueMember = "Id";

            var machines = dt309_MachinesBUS.Instance.GetListByIdDept(idDeptGetData);
            tokenMachine.Properties.DataSource = machines;
            tokenMachine.Properties.DisplayMember = "DisplayName";
            tokenMachine.Properties.ValueMember = "Id";

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    material = new dt309_Materials();

                    break;
                case EventFormInfo.View:

                    material = dt309_MaterialsBUS.Instance.GetItemById(idBase);
                    oldMaterial = dt309_MaterialsBUS.Instance.GetItemById(idBase);

                    txbCode.EditValue = material.Code;
                    txbDisplayName.EditValue = material.DisplayName;
                    cbbUnit.EditValue = material.IdUnit;
                    txbLocation.EditValue = material.Location;
                    cbbUsr.EditValue = material.IdManager;
                    cbbTypeUse.EditValue = material.TypeUse;
                    tokenMachine.EditValue = string.Join(",", dt309_MachineMaterialsBUS.Instance.GetListByIdMaterial(material.Id).Select(r => r.MachineId));
                    txbExpDate.EditValue = material.ExpDate;
                    txbMinQuantity.EditValue = material.MinQuantity;

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

            var code = txbCode.EditValue?.ToString();
            var displayName = txbDisplayName.EditValue?.ToString();
            var idUnit = Convert.ToInt16(cbbUnit.EditValue);
            var location = txbLocation.EditValue?.ToString();
            var idUser = cbbUsr.EditValue?.ToString();
            var minQuantity = Convert.ToDouble(txbMinQuantity.EditValue);
            var typeUse = cbbTypeUse.EditValue?.ToString();
            var idMachines = tokenMachine.EditValue?.ToString();
            var expDate = txbExpDate.DateTime;

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                material.IdDept = idDeptGetData;
                material.Code = code;
                material.DisplayName = displayName;
                material.IdUnit = idUnit;
                material.Location = location;
                material.IdManager = idUser;
                material.TypeUse = typeUse;
                material.ExpDate = expDate;
                material.MinQuantity = minQuantity;

                List<int> machineIds = idMachines.Split(',').Select(int.Parse).ToList();

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        material.QuantityInMachine = 0;
                        material.QuantityInStorage = 0;

                        int idMaterial = dt309_MaterialsBUS.Instance.Add(material);
                        result = idMaterial != -1;

                        if (!result) goto RESULT;

                        foreach (var item in machineIds)
                        {
                            dt309_MachineMaterialsBUS.Instance.Add(new dt309_MachineMaterials()
                            {
                                MachineId = item,
                                MaterialId = idMaterial
                            });
                        }

                        break;
                    case EventFormInfo.Update:

                        bool isDifferent = oldMaterial.Code != material.Code ||
                            oldMaterial.DisplayName != material.DisplayName ||
                            oldMaterial.IdUnit != material.IdUnit ||
                            oldMaterial.Location != material.Location ||
                            oldMaterial.IdManager != material.IdManager ||
                            oldMaterial.Price != material.Price ||
                            oldMaterial.TypeUse != material.TypeUse ||
                            oldMaterial.ExpDate != material.ExpDate ||
                            oldMaterial.MinQuantity != material.MinQuantity;

                        if (isDifferent)
                        {
                            result = dt309_MaterialsBUS.Instance.AddOrUpdate(material);
                        }
                        else
                        {
                            result = true;
                        }

                        dt309_MachineMaterialsBUS.Instance.RemoveByIdMaterial(material.Id);
                        foreach (var item in machineIds)
                        {
                            dt309_MachineMaterialsBUS.Instance.Add(new dt309_MachineMaterials()
                            {
                                MachineId = item,
                                MaterialId = material.Id
                            });
                        }

                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}:\r\n{material.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt309_MaterialsBUS.Instance.RemoveById(material.Id, TPConfigs.LoginUser.Id);

                        break;
                    default:
                        break;
                }
            }

        RESULT:
            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
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
    }
}