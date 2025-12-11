using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._05_CalipStandardMgmt
{
    public partial class f403_05_AddStandard : DevExpress.XtraEditors.XtraForm
    {
        public f403_05_AddStandard()
        {
            InitializeComponent();
            InitializeIcon();
        }
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        
        dt403_05_Standard dt403_05_Standard;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        string baseFilePath = "";
        private class Attachment : dm_Attachment
        {
            public string FilePath { get; set; }
        }
        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }
        private void EnabledController(bool _enable = true)
        {
            txbSN.Enabled = _enable;
            txbDisplayNameTW.Enabled = _enable;
            txbDisplayNameVN.Enabled = _enable;
           
           
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
            //foreach (var item in lcControls)
            //{
            //    string colorHex = item.Control.Enabled ? "000000" : "000000";
            //    item.Text = item.Text.Replace("000000", colorHex);
            //}

            //// Các thông tin phải điền có thêm dấu * màu đỏ
            //foreach (var item in lcImpControls)
            //{
            //    if (item.Control.Enabled || (item.Control as BaseEdit).Properties.ReadOnly)
            //    {
            //        item.Text += "<color=red>*</color>";
            //    }
            //    else
            //    {
            //        item.Text = item.Text.Replace("<color=red>*</color>", "");
            //    }
            //}
        }

        private void f403_05_AddStandard_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcDisplayNameTW, lcDisplayNameVN, lcSN };
            lcImpControls = new List<LayoutControlItem>() {  lcDisplayNameTW, lcSN };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            switch (eventInfo)
            { 
                case EventFormInfo.Create:
                    dt403_05_Standard =  new dt403_05_Standard();
                    break;
                case EventFormInfo.View:
                    dt403_05_Standard = dt403_05_StandardBUS.Instance.GetItemById(idBase);
                    txbSN.EditValue = dt403_05_Standard.SN;
                    txbDisplayNameTW.EditValue= dt403_05_Standard.DisplayNameTW;
                    txbDisplayNameVN.EditValue= dt403_05_Standard.DisplayNameVN;
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

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()) && item.Control.Enabled)
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

            string SN = txbSN.Text;
            string displayNameTW = txbDisplayNameTW.Text.Trim();
            string displayNameVN = txbDisplayNameVN.Text.Trim();
            

            var result = false;
          //  var resultAtt = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            { 
                dt403_05_Standard.SN = SN;
                dt403_05_Standard.DisplayNameVN = displayNameVN;
                dt403_05_Standard.DisplayNameTW = displayNameTW;

                switch(eventInfo)
                {
                    case EventFormInfo.Create:
                        dt403_05_Standard.ManagerId = TPConfigs.LoginUser.Id;
                        result = dt403_05_StandardBUS.Instance.Add(dt403_05_Standard);
                        break;
                    case EventFormInfo.Update:
                        dt403_05_Standard.Id = idBase;
                        result = dt403_05_StandardBUS.Instance.AddOrUpdate(dt403_05_Standard);
                        break;
                    case EventFormInfo.Delete:

                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}: {dt403_05_Standard.SN}_{dt403_05_Standard.DisplayNameTW}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt403_05_StandardBUS.Instance.RemoveById(dt403_05_Standard.Id);//, TPConfigs.LoginUser.Id);
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
