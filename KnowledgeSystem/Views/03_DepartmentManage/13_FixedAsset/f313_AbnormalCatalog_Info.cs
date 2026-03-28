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
    internal partial class f313_AbnormalCatalog_Info : XtraForm
    {
        private readonly dt313_AbnormalCatalog catalog;
        private BarButtonItem btnEdit;
        private BarButtonItem btnDelete;
        private List<LayoutControlItem> lcControls;
        private List<LayoutControlItem> lcImpControls;

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = string.Empty;

        public f313_AbnormalCatalog_Info(FixedAsset313Context module, dt313_AbnormalCatalog catalog)
        {
            InitializeComponent();
            this.catalog = catalog ?? new dt313_AbnormalCatalog
            {
                CreatedBy = TPConfigs.LoginUser.Id,
                CreatedDate = DateTime.Now
            };
            InitializeIcon();
            InitializeDynamicBarItems();
        }

        private void f313_AbnormalCatalog_Info_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(formName))
            {
                formName = "異常項目";
            }

            lcControls = new List<LayoutControlItem> { lcCode, lcName, lcSort, lcActive, lcRemarks };
            lcImpControls = new List<LayoutControlItem> { lcCode, lcName };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            FixedAsset313UIHelper.ApplyFormStyle(this, barManagerTP, bar2);
            FixedAsset313UIHelper.ApplyLayoutItemCaptions(lcControls.ToArray());

            txbCode.EditValue = catalog.Code;
            txbName.EditValue = catalog.DisplayName;
            spinSort.EditValue = catalog.SortOrder;
            chkActive.Checked = catalog.Id == 0 || catalog.IsActive;
            memoRemarks.EditValue = catalog.Remarks;

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
                    result = dt313_AbnormalCatalogBUS.Instance.Add(catalog) > 0;
                    break;
                case EventFormInfo.Update:
                    FillEntity();
                    result = dt313_AbnormalCatalogBUS.Instance.AddOrUpdate(catalog);
                    break;
                case EventFormInfo.Delete:
                    var dialogResult = XtraMessageBox.Show(
                        $"您確認要刪除{formName}:\r\n{catalog.Code} {catalog.DisplayName}",
                        TPConfigs.SoftNameTW,
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Question);
                    if (dialogResult != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }

                    result = dt313_AbnormalCatalogBUS.Instance.DeactivateById(catalog.Id);
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
            txbCode.Enabled = enable && catalog.Id <= 0;
            txbName.Enabled = enable;
            spinSort.Enabled = enable;
            chkActive.Enabled = enable;
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
                    btnDelete.Visibility = catalog.Id > 0
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
            return !string.IsNullOrWhiteSpace(txbCode.Text) && !string.IsNullOrWhiteSpace(txbName.Text);
        }

        private void FillEntity()
        {
            catalog.Code = txbCode.Text.Trim();
            catalog.DisplayName = txbName.Text.Trim();
            catalog.SortOrder = Convert.ToInt32(spinSort.EditValue);
            catalog.IsActive = chkActive.Checked;
            catalog.Remarks = memoRemarks.Text.Trim();
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
