using DevExpress.XtraEditors;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._04_BorrVehicle
{
    public partial class f304_BorrVehicleInfo : DevExpress.XtraEditors.XtraForm
    {
        public f304_BorrVehicleInfo()
        {
            InitializeComponent();
        }

        public int indexTypeVehicle = 0;
        public VehicleStatus vehicleStatus;
        int startKm = 0;

        private async void f304_BorrVehicleInfo_Load(object sender, EventArgs e)
        {
            VehicleBorrInfo vehicleInfo = new VehicleBorrInfo();
            var purposes = await BorrVehicleHelper.Instance.GetListPurposess();

            cbbTypeVehicle.Properties.Items.AddRange(TPConfigs.typeVehicles);
            cbbTypeVehicle.SelectedIndex = indexTypeVehicle;

            cbbPurpose.Properties.Items.AddRange(purposes);
            cbbPurpose.SelectedIndex = indexTypeVehicle;

            timeBorrTime.EditValue = DateTime.Now;

            switch (indexTypeVehicle)
            {
                case 0:
                    vehicleInfo = (await BorrVehicleHelper.Instance.GetBorrMotorUser(vehicleStatus)).FirstOrDefault();

                    string lastKm = await BorrVehicleHelper.Instance.GetLastKmMotor(vehicleStatus.Name);
                    int.TryParse(lastKm.Split('|')[0].Trim(), out startKm);

                    txbStartKm.EditValue = vehicleInfo.StartKm;
                    break;
            }

            txbName.EditValue = vehicleStatus.Name;

            var purpose = vehicleInfo.Uses;
            int firstSpaceIndex = purpose.IndexOf(' ');
            cbbPurpose.EditValue = purpose.Substring(0, firstSpaceIndex);
            txbDescript.EditValue = purpose.Substring(firstSpaceIndex + 1);

            var place = vehicleInfo.Place;
            firstSpaceIndex = place.IndexOf('-');
            if (firstSpaceIndex < 0)
            {
                txbFromPlace.EditValue = place;
            }
            else
            {
                txbFromPlace.EditValue = place.Substring(0, firstSpaceIndex);
                txbToPlace.EditValue = place.Substring(firstSpaceIndex + 1);
            }
        }

        private void cbbTypeVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbbTypeVehicle.SelectedIndex)
            {
                case 0:

                    txbNumUser.Enabled = true;
                    break;
                case 1:

                    txbNumUser.Enabled = false;
                    break;
            }
        }

        private async void btnBorrVehicle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string nameVehicle = txbName.EditValue?.ToString();
            string borrTime = timeBorrTime.DateTimeOffset.ToString("yyyyMMddHHmm");
            string purposes = $"{cbbPurpose.EditValue} {txbDescript.EditValue}";
            string place = $"{txbFromPlace.EditValue}-{txbToPlace.EditValue}";
            string numUser = txbNumUser.EditValue?.ToString() ?? "1";

            bool result = await BorrVehicleHelper.Instance.BorrMotor(nameVehicle, startKm, borrTime, place, purposes, numUser);
            if (result)
            {
                Close();
            }
        }

        private async void btnBackVehicle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string nameVehicle = txbName.EditValue?.ToString();
            string backTime = timeBackTime.DateTimeOffset.ToString("yyyyMMddHHmm");
            int startKm = Convert.ToInt32(txbStartKm.EditValue);
            int endKm = Convert.ToInt32(txbEndKm.EditValue);
            int totalKm = endKm - startKm;

            bool result = await BorrVehicleHelper.Instance.BackMotor(nameVehicle, endKm, backTime, totalKm);
            if (result)
            {
                Close();
            }
        }
    }
}