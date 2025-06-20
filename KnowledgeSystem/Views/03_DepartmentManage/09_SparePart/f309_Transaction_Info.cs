using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using System.Windows.Media.Media3D;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System.Threading;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_Transaction_Info : DevExpress.XtraEditors.XtraForm
    {
        public f309_Transaction_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string eventInfo = "";
        public int idMaterial = -1;

        dt309_Transactions transaction = new dt309_Transactions();
        dt309_Materials material;
        List<dt309_Storages> storages;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void LockControl()
        {
            Text = $"新增{eventInfo}事件";

            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            txbStorageFromQuantity.Enabled = false;
            txbStorageToQuantity.Enabled = false;

            lcPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcExpDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new Size(400, 245);

            switch (eventInfo)
            {
                case "收貨":
                    cbbStorageTo.EditValue = 2;
                    cbbStorageTo.Enabled = false;
                    cbbStorageFrom.Enabled = false;

                    lcPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lcExpDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    Size = new Size(400, 315);
                    break;

                case "調撥":
                    cbbStorageTo.EditValue = 1;
                    cbbStorageTo.Enabled = true;
                    cbbStorageFrom.Enabled = false;
                    break;

                case "領用":
                    cbbStorageFrom.Enabled = true;
                    cbbStorageTo.Enabled = false;
                    break;

                case "轉庫":

                    cbbStorageFrom.EditValue = 2;
                    cbbStorageTo.Enabled = true;
                    cbbStorageFrom.Enabled = true;
                    break;

                case "盤點": goto case "領用";
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

        private void f309_Transaction_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcStorageFrom, lcStorageTo, lcStorageFromQuantity, lcStorageToQuantity, lcQuantity, lcDesc, lcPrice, lcExpDate };
            lcImpControls = new List<LayoutControlItem>() { lcStorageFrom, lcStorageTo, lcQuantity };

            if (eventInfo == "收貨")
            {
                lcImpControls.Add(lcPrice);
            }

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            storages = dt309_StoragesBUS.Instance.GetList();
            cbbStorageFrom.Properties.DataSource = storages;
            cbbStorageFrom.Properties.DisplayMember = "DisplayName";
            cbbStorageFrom.Properties.ValueMember = "Id";

            cbbStorageTo.Properties.DataSource = storages;
            cbbStorageTo.Properties.DisplayMember = "DisplayName";
            cbbStorageTo.Properties.ValueMember = "Id";

            material = dt309_MaterialsBUS.Instance.GetItemById(idMaterial);
            lcQuantity.Text = lcQuantity.Text.Replace("事件", eventInfo);

            LockControl();
        }

        private void cbbStorageFrom_EditValueChanged(object sender, EventArgs e)
        {
            switch (cbbStorageFrom.EditValue)
            {
                case 1:
                    txbStorageFromQuantity.EditValue = material.QuantityInMachine;
                    break;
                case 2:
                    txbStorageFromQuantity.EditValue = material.QuantityInStorage;
                    break;
                default:
                    txbStorageFromQuantity.EditValue = "";
                    break;
            }

            cbbStorageTo.Properties.DataSource = storages.Where(r => r.Id != (int)cbbStorageFrom.EditValue).ToList();
            cbbStorageTo.EditValue = "";
            txbStorageToQuantity.EditValue = "";
        }

        private void cbbStorageTo_EditValueChanged(object sender, EventArgs e)
        {
            switch (cbbStorageTo.EditValue)
            {
                case 1:
                    txbStorageToQuantity.EditValue = material.QuantityInMachine;
                    break;
                case 2:
                    txbStorageToQuantity.EditValue = material.QuantityInStorage;
                    break;
                default:
                    txbStorageToQuantity.EditValue = "";
                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (item.Enabled == false) continue;
                    {

                    }
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


            bool result = false;
            transaction.MaterialId = idMaterial;
            transaction.CreatedDate = DateTime.Now;
            transaction.Desc = txbDesc.EditValue?.ToString();
            transaction.UserDo = TPConfigs.LoginUser.Id;

            double storageQuantityFrom = 0;
            double storageQuantityTo = 0;
            double quantity = 0;

            double.TryParse(txbStorageFromQuantity.EditValue?.ToString(), out storageQuantityFrom);
            double.TryParse(txbStorageToQuantity.EditValue?.ToString(), out storageQuantityTo);
            double.TryParse(txbQuantity.EditValue?.ToString(), out quantity);

            if (quantity <= 0)
            {
                XtraMessageBox.Show($"{eventInfo}數量不得小於零", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (eventInfo)
            {
                case "收貨":

                    var newPrice = Convert.ToInt32(txbPrice.EditValue);
                    if (newPrice <= 0)
                    {
                        XtraMessageBox.Show($"單價不得小於零", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    transaction.TransactionType = "in";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    var resultUpdate = dt309_PricesBUS.Instance.Add(new dt309_Prices()
                    {
                        MaterialId = idMaterial,
                        Price = newPrice,
                        ChangedAt = DateTime.Now,
                        ChangedBy = TPConfigs.LoginUser.Id
                    });

                    if (!string.IsNullOrEmpty(txbExpDate.Text))
                    {
                        material = dt309_MaterialsBUS.Instance.GetItemById(idMaterial);

                        material.ExpDate = txbExpDate.DateTime;
                        dt309_MaterialsBUS.Instance.AddOrUpdate(material);
                    }

                    break;

                case "調撥":

                    transaction.TransactionType = "in";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "領用":

                    if (quantity > storageQuantityFrom)
                    {
                        XtraMessageBox.Show($"{eventInfo}數量大於庫存數量", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    transaction.TransactionType = "out";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "轉庫":

                    if (quantity > storageQuantityFrom)
                    {
                        XtraMessageBox.Show($"{eventInfo}數量大於庫存數量", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    transaction.TransactionType = "transfer";
                    transaction.Quantity = -quantity;
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    Thread.Sleep(200);

                    transaction.TransactionType = "transfer";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "盤點":

                    transaction.TransactionType = "check";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;
                default:
                    break;
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