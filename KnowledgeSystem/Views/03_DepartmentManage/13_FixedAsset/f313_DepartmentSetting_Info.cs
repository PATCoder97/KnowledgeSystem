using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_DepartmentSetting_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private readonly dt313_DepartmentSetting setting;
        private BarButtonItem btnEdit;
        private BarButtonItem btnDelete;
        private List<LayoutControlItem> lcControls;
        private List<LayoutControlItem> lcImpControls;

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = string.Empty;

        public f313_DepartmentSetting_Info(FixedAsset313Context module, dt313_DepartmentSetting setting)
        {
            InitializeComponent();
            this.module = module;
            this.setting = setting ?? new dt313_DepartmentSetting();
            InitializeIcon();
            InitializeDynamicBarItems();
        }

        private void f313_DepartmentSetting_Info_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(formName))
            {
                formName = "部門設定";
            }

            lcControls = new List<LayoutControlItem> { lcDept, lcRate, lcActive };
            lcImpControls = new List<LayoutControlItem> { lcDept, lcRate };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            FixedAsset313UIHelper.ApplyFormStyle(this, barManagerTP, bar2);
            FixedAsset313UIHelper.ApplyLayoutItemCaptions(lcControls.ToArray());

            cbbDept.Properties.Items.AddRange(module.GetDepartmentLookupItems(true).ToArray());
            cbbDept.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            if (setting.Id > 0)
            {
                foreach (var item in cbbDept.Properties.Items)
                {
                    if (item is LookupItem lookup && lookup.Value == setting.IdDept)
                    {
                        cbbDept.SelectedItem = item;
                        break;
                    }
                }

                spinRate.EditValue = setting.QuarterlySampleRate;
                chkActive.Checked = setting.IsActive;
                cbbDept.Enabled = false;
            }
            else
            {
                spinRate.EditValue = 10;
                chkActive.Checked = true;
            }

            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (eventInfo != EventFormInfo.Delete && !ValidateRequired())
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            bool result;
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    FillEntity();
                    result = dt313_DepartmentSettingBUS.Instance.Add(setting) > 0;
                    break;
                case EventFormInfo.Update:
                    FillEntity();
                    result = dt313_DepartmentSettingBUS.Instance.AddOrUpdate(setting);
                    break;
                case EventFormInfo.Delete:
                    var dialogResult = XtraMessageBox.Show(
                        $"您確認要刪除{formName}:\r\n{setting.IdDept}",
                        TPConfigs.SoftNameTW,
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogResult != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }

                    result = dt313_DepartmentSettingBUS.Instance.RemoveById(setting.Id);
                    break;
                default:
                    result = false;
                    break;
            }

            if (!result)
            {
                MsgTP.MsgErrorDB();
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void InitializeDynamicBarItems()
        {
            btnEdit = new BarButtonItem
            {
                Caption = "編輯",
                Id = barManagerTP.MaxItemId++,
                ImageOptions = { SvgImage = TPSvgimages.Edit, SvgImageSize = new System.Drawing.Size(32, 32) },
                Name = "btnEdit"
            };
            btnDelete = new BarButtonItem
            {
                Caption = "刪除",
                Id = barManagerTP.MaxItemId++,
                ImageOptions = { SvgImage = TPSvgimages.Remove, SvgImageSize = new System.Drawing.Size(32, 32) },
                Name = "btnDelete"
            };

            btnEdit.ItemClick += btnEdit_ItemClick;
            btnDelete.ItemClick += btnDelete_ItemClick;

            barManagerTP.Items.Add(btnEdit);
            barManagerTP.Items.Add(btnDelete);
            bar2.LinksPersistInfo.Clear();
            bar2.LinksPersistInfo.AddRange(new[]
            {
                new LinkPersistInfo(BarLinkUserDefines.PaintStyle, btnEdit, BarItemPaintStyle.CaptionGlyph),
                new LinkPersistInfo(BarLinkUserDefines.PaintStyle, btnDelete, string.Empty, true, true, true, 0, null, BarItemPaintStyle.CaptionGlyph),
                new LinkPersistInfo(BarLinkUserDefines.PaintStyle, btnConfirm, string.Empty, true, true, true, 0, null, BarItemPaintStyle.CaptionGlyph)
            });
        }

        private void EnabledController(bool enable = true)
        {
            cbbDept.Enabled = enable && setting.Id <= 0;
            spinRate.Enabled = enable;
            chkActive.Enabled = enable;
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
                    btnDelete.Visibility = setting.Id > 0
                        ? DevExpress.XtraBars.BarItemVisibility.Always
                        : DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
            }

            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    if (!item.Text.Contains("<color=red>*</color>"))
                    {
                        item.Text += "<color=red>*</color>";
                    }
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private bool ValidateRequired()
        {
            return cbbDept.SelectedItem is LookupItem && spinRate.EditValue != null;
        }

        private void FillEntity()
        {
            var dept = cbbDept.SelectedItem as LookupItem;
            if (dept == null)
            {
                throw new InvalidOperationException("Department is required.");
            }

            setting.IdDept = dept.Value;
            setting.QuarterlySampleRate = Convert.ToInt32(spinRate.EditValue);
            setting.IsActive = chkActive.Checked;
            setting.UpdatedBy = TPConfigs.LoginUser.Id;
            setting.UpdatedDate = DateTime.Now;
        }

        private void btnEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();
            eventInfo = EventFormInfo.Delete;
            LockControl();
        }
    }
}
