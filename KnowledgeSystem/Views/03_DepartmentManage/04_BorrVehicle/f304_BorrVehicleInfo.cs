﻿using DevExpress.XtraEditors;
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
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public int indexTypeVehicle = 0;
        public VehicleStatus vehicleStatus;
        int startKm = 0;

        private void InitializeIcon()
        {
            btnBorrVehicle.ImageOptions.SvgImage = TPSvgimages.Add;
            btnBackVehicle.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private async void f304_BorrVehicleInfo_Load(object sender, EventArgs e)
        {
            btnBorrVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnBackVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            VehicleBorrInfo vehicleInfo = new VehicleBorrInfo();
            var purposes = await BorrVehicleHelper.Instance.GetListPurposess();

            cbbTypeVehicle.Properties.Items.AddRange(TPConfigs.typeVehicles);
            cbbTypeVehicle.SelectedIndex = indexTypeVehicle;

            cbbPurpose.Properties.Items.AddRange(purposes);
            cbbPurpose.SelectedIndex = indexTypeVehicle;

            switch (indexTypeVehicle)
            {
                case 0:
                    vehicleInfo = (await BorrVehicleHelper.Instance.GetBorrMotorUser(vehicleStatus)).FirstOrDefault();

                    string lastKm = await BorrVehicleHelper.Instance.GetLastKmMotor(vehicleStatus.Name);
                    int.TryParse(lastKm.Split('|')[0].Trim(), out startKm);

                    txbStartKm.EditValue = startKm;
                    break;
            }

            txbName.EditValue = vehicleStatus.Name;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    timeBorrTime.EditValue = DateTime.Now;

                    timeBackTime.Enabled = false;
                    txbEndKm.Enabled = false;

                    btnBorrVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnBackVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:
                    timeBorrTime.Enabled = false;
                    txbFromPlace.Enabled = false;
                    txbToPlace.Enabled = false;
                    cbbPurpose.Enabled = false;
                    txbDescript.Enabled = false;
                    txbNumUser.Enabled = false;

                    var purpose = vehicleInfo.Uses;
                    int firstSpaceIndex = purpose.IndexOf(' ');
                    cbbPurpose.EditValue = purpose.Substring(0, firstSpaceIndex);
                    txbDescript.EditValue = purpose.Substring(firstSpaceIndex + 1);
                    timeBorrTime.EditValue = vehicleInfo.BorrTime;
                    txbStartKm.EditValue = vehicleInfo.StartKm;

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

                    timeBackTime.EditValue = DateTime.Now;

                    btnBorrVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnBackVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
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
                XtraMessageBox.Show("Mượn xe thành công!", TPConfigs.SoftNameTW);

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

            if (totalKm < 0) return;

            if (XtraMessageBox.Show($"Bạn chắc chắn muốn trả xe: {nameVehicle}, với {totalKm} Km ?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            bool result = await BorrVehicleHelper.Instance.BackMotor(nameVehicle, endKm, backTime, totalKm);
            if (result)
            {
                Close();
            }
        }
    }
}