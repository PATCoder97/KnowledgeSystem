using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class f311_AddFuel_Info : DevExpress.XtraEditors.XtraForm
    {
        public f311_AddFuel_Info(string idBase)
        {
            InitializeComponent();
            InitializeIcon();
            this.idBase = idBase;
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
        string idBase;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f311_AddFuel_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcOdometerReading, lcLicensePlate, lcFuelFilledBy };
            lcImpControls = new List<LayoutControlItem>() { lcOdometerReading, lcLicensePlate, lcFuelFilledBy };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
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

            var users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            cbbFuelFilledBy.Properties.DataSource = users;
            cbbFuelFilledBy.Properties.DisplayMember = "DisplayName";
            cbbFuelFilledBy.Properties.ValueMember = "Id";

            var vehicles = dt311_VehicleManagementBUS.Instance.GetList().Where(r => r.IdDept.StartsWith(idDept2word)).Select(r => r.LicensePlate).ToList();
            txbLicensePlate.Properties.Items.AddRange(vehicles);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var invoice = dt311_InvoiceBUS.Instance.GetItemById(idBase);
            if (invoice == null) return;

            string lisencePlate = txbLicensePlate.Text.ToString();
            int? km = txbOdometerReading.EditValue as int?;
            string fillfuelby = cbbFuelFilledBy.EditValue?.ToString();

            invoice.LicensePlate = lisencePlate;
            invoice.OdometerReading = km;
            invoice.FuelFilledBy = fillfuelby;

            string result = dt311_InvoiceBUS.Instance.AddOrUpdate(invoice);
            if (string.IsNullOrEmpty(result))
            {
                MsgTP.MsgErrorDB();
                return;
            }

            Close();
        }
    }
}