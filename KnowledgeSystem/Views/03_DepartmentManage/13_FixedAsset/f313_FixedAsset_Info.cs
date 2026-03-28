using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_FixedAsset_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private dt313_FixedAsset asset;
        private List<LayoutControlItem> lcControls;
        private List<LayoutControlItem> lcImpControls;

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = string.Empty;

        public f313_FixedAsset_Info(FixedAsset313Context module, dt313_FixedAsset source)
        {
            InitializeComponent();
            InitializeIcon();
            this.module = module;
            asset = source;
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f313_FixedAsset_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>
            {
                lcAssetCode, lcAssetNameTW, lcAssetNameVN, lcDept, lcManager, lcCategory,
                lcTypeName, lcLocation, lcBrandSpec, lcOrigin, lcAcquireDate, lcStatus, lcRemarks
            };
            lcImpControls = new List<LayoutControlItem> { lcAssetCode, lcAssetNameTW, lcDept, lcCategory, lcStatus };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            cbbDept.Properties.Items.AddRange(module.GetDepartmentLookupItems(true).ToArray());
            cbbManager.Properties.Items.AddRange(module.GetUserLookupItems(true).ToArray());
            cbbCategory.Properties.Items.AddRange(new object[]
            {
                new LookupItem("General", "一般設備"),
                new LookupItem("DutyFreeImported", "免稅進口設備")
            });
            cbbStatus.Properties.Items.AddRange(new object[]
            {
                "Active",
                "Idle",
                "Repair",
                "Disposed"
            });

            if (eventInfo == EventFormInfo.Create || asset == null)
            {
                asset = new dt313_FixedAsset
                {
                    CreatedBy = TPConfigs.LoginUser.Id,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false,
                    Status = "Active",
                    AssetCategory = "General"
                };
            }
            else
            {
                BindData();
            }

            LockControl();
        }

        private void BindData()
        {
            txbAssetCode.EditValue = asset.AssetCode;
            txbAssetNameTW.EditValue = asset.AssetNameTW;
            txbAssetNameVN.EditValue = asset.AssetNameVN;
            txbTypeName.EditValue = asset.TypeName;
            txbLocation.EditValue = asset.Location;
            txbBrandSpec.EditValue = asset.BrandSpec;
            txbOrigin.EditValue = asset.Origin;
            dateAcquire.EditValue = asset.AcquireDate;
            memoRemarks.EditValue = asset.Remarks;
            cbbStatus.EditValue = asset.Status;

            SelectComboValue(cbbDept, asset.IdDept);
            SelectComboValue(cbbManager, asset.IdManager);
            SelectComboValue(cbbCategory, asset.AssetCategory);
        }

        private void SelectComboValue(ComboBoxEdit combo, string value)
        {
            foreach (var item in combo.Properties.Items)
            {
                if (item is LookupItem lookup && string.Equals(lookup.Value, value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = item;
                    return;
                }

                if (!(item is LookupItem) && string.Equals(item?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private void EnabledController(bool enable = true)
        {
            txbAssetCode.Enabled = enable;
            txbAssetNameTW.Enabled = enable;
            txbAssetNameVN.Enabled = enable;
            cbbDept.Enabled = enable;
            cbbManager.Enabled = enable;
            cbbCategory.Enabled = enable;
            txbTypeName.Enabled = enable;
            txbLocation.Enabled = enable;
            txbBrandSpec.Enabled = enable;
            txbOrigin.Enabled = enable;
            dateAcquire.Enabled = enable;
            cbbStatus.Enabled = enable;
            memoRemarks.Enabled = enable;
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
                default:
                    Text = $"{formName}資訊";
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    EnabledController(false);
                    break;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

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

        private bool ValidateRequired()
        {
            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit && string.IsNullOrWhiteSpace(baseEdit.EditValue?.ToString()))
                {
                    return false;
                }
            }

            return true;
        }

        private void FillEntity()
        {
            asset.AssetCode = txbAssetCode.Text.Trim();
            asset.AssetNameTW = txbAssetNameTW.Text.Trim();
            asset.AssetNameVN = txbAssetNameVN.Text.Trim();
            asset.TypeName = txbTypeName.Text.Trim();
            asset.Location = txbLocation.Text.Trim();
            asset.BrandSpec = txbBrandSpec.Text.Trim();
            asset.Origin = txbOrigin.Text.Trim();
            asset.AcquireDate = dateAcquire.EditValue == null ? (DateTime?)null : Convert.ToDateTime(dateAcquire.EditValue);
            asset.Remarks = memoRemarks.Text.Trim();
            asset.IdDept = (cbbDept.SelectedItem as LookupItem)?.Value ?? cbbDept.Text.Trim();
            asset.IdManager = (cbbManager.SelectedItem as LookupItem)?.Value ?? cbbManager.Text.Trim();
            asset.AssetCategory = (cbbCategory.SelectedItem as LookupItem)?.Value ?? module.NormalizeCategory(cbbCategory.Text);
            asset.Status = cbbStatus.Text.Trim();
            asset.UpdatedBy = TPConfigs.LoginUser.Id;
            asset.UpdatedDate = DateTime.Now;
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
            if (eventInfo != EventFormInfo.Delete && !ValidateRequired())
            {
                MsgTP.MsgError("請填寫所有必填信息<color=red>(*)</color>");
                return;
            }

            bool result = false;
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        FillEntity();
                        result = dt313_FixedAssetBUS.Instance.Add(asset) > 0;
                        break;
                    case EventFormInfo.Update:
                        FillEntity();
                        result = dt313_FixedAssetBUS.Instance.AddOrUpdate(asset);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show(
                            $"您確認要刪除{formName}:\r\n{asset.AssetCode} {asset.AssetNameTW}",
                            TPConfigs.SoftNameTW,
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Question);
                        if (dialogResult != System.Windows.Forms.DialogResult.Yes)
                        {
                            return;
                        }

                        result = dt313_FixedAssetBUS.Instance.RemoveById(asset.Id, TPConfigs.LoginUser.Id);
                        break;
                }
            }

            if (!result)
            {
                MsgTP.MsgErrorDB();
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
