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
        public string formName = "";
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
            Text = $"新增{formName}事件";

            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            txbStorageFromQuantity.Enabled = false;
            txbStorageToQuantity.Enabled = false;

            switch (eventInfo)
            {
                case "收貨":
                    cbbStorageTo.EditValue = 2;
                    cbbStorageTo.Enabled = false;
                    cbbStorageFrom.Enabled = false;
                    break;

                case "領用":
                    cbbStorageFrom.Enabled = true;
                    cbbStorageTo.Enabled = false;
                    break;

                case "轉庫":

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
            lcControls = new List<LayoutControlItem>() { lcStorageFrom, lcStorageTo, lcStorageFromQuantity, lcStorageToQuantity, lcQuantity, lcDesc };
            lcImpControls = new List<LayoutControlItem>() { lcStorageFrom, lcStorageTo, lcQuantity };
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

            switch (eventInfo)
            {
                case "收貨":

                    transaction.TransactionType = "IN";
                    transaction.Quantity = Convert.ToInt32(txbQuantity.EditValue);
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "領用":

                    transaction.TransactionType = "OUT";
                    transaction.Quantity = Convert.ToInt32(txbQuantity.EditValue);
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "轉庫":

                    transaction.TransactionType = "OUT";
                    transaction.Quantity = Convert.ToInt32(txbQuantity.EditValue);
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    Thread.Sleep(200);

                    transaction.TransactionType = "IN";
                    transaction.Quantity = Convert.ToInt32(txbQuantity.EditValue);
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = dt309_TransactionsBUS.Instance.Add(transaction);

                    break;

                case "盤點":

                    transaction.TransactionType = "CHECK";
                    transaction.Quantity = Convert.ToInt32(txbQuantity.EditValue);
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