using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Wordprocessing;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949
{
    public partial class f205_Node_Info : DevExpress.XtraEditors.XtraForm
    {
        public f205_Node_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        public int idParent = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        dt205_Base baseDoc;
        public dt205_Base parentData;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

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
            txbNotifyCycle.Enabled = _enable;
            txbPreAlertMonths.Enabled = _enable;
            cbbBaseType.Enabled = _enable;
            txbCreateDate.Enabled = _enable;
            cbbClass.Enabled = _enable;
            txbDisplayName.Enabled = _enable;
            txbDisplayNameVN.Enabled = _enable;
            txbDisplayNameEN.Enabled = _enable;
            ckConfidential.Enabled = _enable ? parentData?.Confidential != true : _enable;
            txbKeyword.Enabled = _enable;

            if (formName == "節點")
            {
                txbCreateDate.Enabled = false;
                txbNotifyCycle.Enabled = false;
                txbPreAlertMonths.Enabled = false;
                txbKeyword.Enabled = false;
            }
        }

        private void LockControl()
        {
            if (parentData != null)
            {
                cbbBaseType.EditValue = parentData.BaseTypeId;
            }

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
                if (item.Control.Enabled || (item.Control as BaseEdit).Properties.ReadOnly)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }


        private void f205_Node_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcBaseType, lcClass, lcDisplayName, lcDisplayNameVN, lcDisplayNameEN, lcNotifyCycle, lcCreateDate, lcPreAlertMonths, lcKeyword };
            lcImpControls = new List<LayoutControlItem>() { lcBaseType, lcClass, lcDisplayName, lcDisplayNameVN, lcDisplayNameEN, lcCreateDate, lcNotifyCycle, lcPreAlertMonths, lcKeyword };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            cbbClass.Properties.Items.AddRange(new string[] { "一階", "二階", "三階" });

            // them du lieu basetype

            switch (eventInfo)
            {
                case EventFormInfo.Create:

                    baseDoc = new dt205_Base();

                    if (parentData != null)
                    {
                        ckConfidential.Checked = parentData.Confidential == true;
                        cbbBaseType.EditValue = parentData.BaseTypeId;
                        cbbBaseType.Enabled = false;
                    }

                    break;
                case EventFormInfo.View:

                    baseDoc = dt205_BaseBUS.Instance.GetItemById(idBase);

                    cbbClass.EditValue = baseDoc.DocType;
                    ckConfidential.CheckState = baseDoc.Confidential ? CheckState.Checked : CheckState.Unchecked;
                    txbDisplayName.EditValue = baseDoc.DisplayName;
                    txbDisplayNameVN.EditValue = baseDoc.DisplayNameVN;
                    txbDisplayNameEN.EditValue = baseDoc.DisplayNameEN;
                    txbCreateDate.EditValue = baseDoc.CreateDate;
                    txbNotifyCycle.EditValue = baseDoc.NotifyCycle;
                    txbPreAlertMonths.EditValue = baseDoc.PreAlertMonths;

                    break;
                case EventFormInfo.Update:
                    goto case EventFormInfo.View;

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
                        break;
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            string idDept = TPConfigs.LoginUser.IdDepartment;
            string displayName = txbDisplayName.Text.Trim();
            string displayNameVN = txbDisplayNameVN.Text.Trim();
            string displayNameEN = txbDisplayNameEN.Text.Trim();
            string docType = cbbClass.Text.Trim();
            DateTime? createDate = txbCreateDate.EditValue as DateTime?;
            int? periodNotify = txbNotifyCycle.EditValue as int?;
            int? preAlertMonths = txbPreAlertMonths.EditValue as int?;
            bool confidential = ckConfidential.CheckState == CheckState.Checked;

            if (StringHelper.CheckUpcase(displayNameVN, 33) && displayNameVN.Length > 20)
            {
                XtraMessageBox.Show("Tắt CAPSLOCK đi！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                baseDoc.IdDept = idDept;
                baseDoc.DisplayName = displayName;
                baseDoc.DisplayNameVN = displayNameVN;
                baseDoc.DisplayNameEN = displayNameEN;
                baseDoc.DocType = docType;
                baseDoc.CreateDate = createDate;
                baseDoc.Confidential = confidential;
                baseDoc.NotifyCycle = periodNotify;
                baseDoc.PreAlertMonths = preAlertMonths;

                switch (eventInfo)
                {
                    case EventFormInfo.Create:

                        baseDoc.IdParent = idParent;
                        baseDoc.IsFinalNode = false;
                        result = dt205_BaseBUS.Instance.Add(baseDoc);

                        break;
                    case EventFormInfo.Update:

                        result = dt205_BaseBUS.Instance.AddOrUpdate(baseDoc);

                        break;
                    case EventFormInfo.Delete:

                        //var dialogResult = XtraMessageBox.Show($"您確認要刪除{formName}: {dt204Base.Code} {dt204Base.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        //if (dialogResult != DialogResult.Yes) return;
                        //result = dt204_InternalDocMgmtBUS.Instance.RemoveById(dt204Base.Id, TPConfigs.LoginUser.Id);

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