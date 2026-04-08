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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using System.Threading;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_Transaction_Info : DevExpress.XtraEditors.XtraForm
    {
        private const int RecoveryOptionNoneValue = 0;
        private const int RecoveryOptionScrapValue = 1;
        private const int RecoveryOptionRestockValue = 2;

        public f309_Transaction_Info()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeRecoverySection();
        }

        public string eventInfo = "";
        public int idMaterial = -1;

        dt309_Transactions transaction = new dt309_Transactions();
        dt309_Materials material;
        List<dt309_Storages> storages;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;
        Dictionary<LayoutControlItem, string> lcBaseTexts = new Dictionary<LayoutControlItem, string>();

        ImageComboBoxEdit cbbRecoveryOption;
        SearchLookUpEdit sleAssignedUser;
        GridView gvAssignedUserLookup;
        DateEdit dePlannedDisposeDate;
        LookUpEdit cbbRestockStorage;
        MemoEdit memoRecoveryNote;
        SimpleButton btnViewGuideDocs;

        LayoutControlItem lcRecoveryOption;
        LayoutControlItem lcAssignedUser;
        LayoutControlItem lcPlannedDisposeDate;
        LayoutControlItem lcRestockStorage;
        LayoutControlItem lcRecoveryNote;
        LayoutControlItem lcGuideDocs;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void InitializeRecoverySection()
        {
            cbbRecoveryOption = new ImageComboBoxEdit();
            cbbRecoveryOption.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbRecoveryOption.Properties.Appearance.ForeColor = Color.Black;
            cbbRecoveryOption.Properties.Appearance.Options.UseFont = true;
            cbbRecoveryOption.Properties.Appearance.Options.UseForeColor = true;
            cbbRecoveryOption.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbRecoveryOption.Properties.AppearanceDropDown.Options.UseFont = true;
            cbbRecoveryOption.Properties.Buttons.Clear();
            cbbRecoveryOption.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("沒有舊品拆出", RecoveryOptionNoneValue, -1));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("拆出後送報廢", RecoveryOptionScrapValue, -1));
            cbbRecoveryOption.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("拆出後回收入庫", RecoveryOptionRestockValue, -1));
            cbbRecoveryOption.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cbbRecoveryOption.EditValue = RecoveryOptionNoneValue;
            cbbRecoveryOption.EditValueChanged += cbbRecoveryOption_EditValueChanged;

            sleAssignedUser = new SearchLookUpEdit();
            sleAssignedUser.Properties.Appearance.Font = TPConfigs.fontUI14;
            sleAssignedUser.Properties.Appearance.ForeColor = Color.Black;
            sleAssignedUser.Properties.Appearance.Options.UseFont = true;
            sleAssignedUser.Properties.Appearance.Options.UseForeColor = true;
            sleAssignedUser.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            sleAssignedUser.Properties.AppearanceDropDown.Options.UseFont = true;
            sleAssignedUser.Properties.NullText = string.Empty;
            sleAssignedUser.Properties.PopupView = CreateLookupView(out gvAssignedUserLookup,
                ("Id", "工號"),
                ("DisplayName", "姓名"),
                ("IdDepartment", "單位"));
            sleAssignedUser.Properties.DisplayMember = "DisplayName";
            sleAssignedUser.Properties.ValueMember = "Id";

            dePlannedDisposeDate = new DateEdit();
            dePlannedDisposeDate.Properties.Appearance.Font = TPConfigs.fontUI14;
            dePlannedDisposeDate.Properties.Appearance.ForeColor = Color.Black;
            dePlannedDisposeDate.Properties.Appearance.Options.UseFont = true;
            dePlannedDisposeDate.Properties.Appearance.Options.UseForeColor = true;
            dePlannedDisposeDate.Properties.Buttons.Clear();
            dePlannedDisposeDate.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            dePlannedDisposeDate.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.False;
            dePlannedDisposeDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
            dePlannedDisposeDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.False;
            dePlannedDisposeDate.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
            dePlannedDisposeDate.Properties.UseMaskAsDisplayFormat = true;
            dePlannedDisposeDate.Properties.CalendarTimeProperties.Buttons.Clear();
            dePlannedDisposeDate.Properties.CalendarTimeProperties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));

            cbbRestockStorage = new LookUpEdit();
            cbbRestockStorage.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbRestockStorage.Properties.Appearance.ForeColor = Color.Black;
            cbbRestockStorage.Properties.Appearance.Options.UseFont = true;
            cbbRestockStorage.Properties.Appearance.Options.UseForeColor = true;
            cbbRestockStorage.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbRestockStorage.Properties.AppearanceDropDown.Options.UseFont = true;
            cbbRestockStorage.Properties.Buttons.Clear();
            cbbRestockStorage.Properties.Buttons.Add(new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo));
            cbbRestockStorage.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[]
            {
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "倉庫")
            });
            cbbRestockStorage.Properties.DisplayMember = "DisplayName";
            cbbRestockStorage.Properties.ValueMember = "Id";
            cbbRestockStorage.Properties.NullText = string.Empty;
            cbbRestockStorage.Properties.ShowHeader = false;
            cbbRestockStorage.Properties.DropDownRows = 5;

            memoRecoveryNote = new MemoEdit();
            memoRecoveryNote.Properties.Appearance.Font = TPConfigs.fontUI14;
            memoRecoveryNote.Properties.Appearance.ForeColor = Color.Black;
            memoRecoveryNote.Properties.Appearance.Options.UseFont = true;
            memoRecoveryNote.Properties.Appearance.Options.UseForeColor = true;

            btnViewGuideDocs = new SimpleButton();
            btnViewGuideDocs.Appearance.Font = TPConfigs.fontUI14;
            btnViewGuideDocs.Appearance.Options.UseFont = true;
            btnViewGuideDocs.Text = "查看報廢指引";
            btnViewGuideDocs.Click += btnViewGuideDocs_Click;

            layoutControl1.Controls.Add(cbbRecoveryOption);
            layoutControl1.Controls.Add(sleAssignedUser);
            layoutControl1.Controls.Add(dePlannedDisposeDate);
            layoutControl1.Controls.Add(cbbRestockStorage);
            layoutControl1.Controls.Add(memoRecoveryNote);
            layoutControl1.Controls.Add(btnViewGuideDocs);

            lcRecoveryOption = CreateRecoveryLayoutItem(cbbRecoveryOption, "舊品處理", 36);
            lcAssignedUser = CreateRecoveryLayoutItem(sleAssignedUser, "報廢經辦", 36);
            lcPlannedDisposeDate = CreateRecoveryLayoutItem(dePlannedDisposeDate, "預計日期", 36);
            lcRestockStorage = CreateRecoveryLayoutItem(cbbRestockStorage, "回收倉庫", 36);
            lcRecoveryNote = CreateRecoveryLayoutItem(memoRecoveryNote, "回收備註", 72);
            lcRecoveryNote.SizeConstraintsType = SizeConstraintsType.Custom;
            lcRecoveryNote.MinSize = new Size(200, 84);
            lcRecoveryNote.MaxSize = new Size(0, 84);
            lcGuideDocs = CreateRecoveryLayoutItem(btnViewGuideDocs, string.Empty, 36, false);
        }

        private GridView CreateLookupView(out GridView view, params (string fieldName, string caption)[] columns)
        {
            view = new GridView();
            view.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 12F);
            view.Appearance.HeaderPanel.ForeColor = Color.Black;
            view.Appearance.HeaderPanel.Options.UseFont = true;
            view.Appearance.HeaderPanel.Options.UseForeColor = true;
            view.Appearance.HeaderPanel.Options.UseTextOptions = true;
            view.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            view.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F);
            view.Appearance.Row.ForeColor = Color.Black;
            view.Appearance.Row.Options.UseFont = true;
            view.Appearance.Row.Options.UseForeColor = true;
            view.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.OptionsView.EnableAppearanceOddRow = true;
            view.OptionsView.ShowAutoFilterRow = true;
            view.OptionsView.ShowGroupPanel = false;

            foreach (var column in columns)
            {
                view.Columns.AddVisible(column.fieldName, column.caption);
            }

            return view;
        }

        private LayoutControlItem CreateRecoveryLayoutItem(Control control, string text, int minHeight, bool showCaption = true)
        {
            var item = new LayoutControlItem();
            item.Control = control;
            item.ControlAlignment = ContentAlignment.TopLeft;
            item.Text = text;
            item.TextVisible = showCaption;
            item.AllowHtmlStringInCaption = true;
            item.AppearanceItemCaption.Font = TPConfigs.fontUI14;
            item.AppearanceItemCaption.ForeColor = Color.Black;
            item.AppearanceItemCaption.Options.UseFont = true;
            item.AppearanceItemCaption.Options.UseForeColor = true;
            item.AppearanceItemCaptionDisabled.ForeColor = Color.Black;
            item.AppearanceItemCaptionDisabled.Options.UseForeColor = true;
            item.TextSize = new Size(76, 24);
            item.SizeConstraintsType = SizeConstraintsType.Custom;
            item.MinSize = new Size(200, minHeight);
            item.MaxSize = new Size(0, minHeight);
            item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Root.Add(item);
            return item;
        }

        private void AdjustIssueFormSize()
        {
            if (eventInfo != "領用" || lcControls == null || lcControls.Count == 0)
            {
                return;
            }

            layoutControl1.Root.BestFit();
            layoutControl1.BestFit();
            layoutControl1.PerformLayout();
            layoutControl1.Update();

            var preferredSize = layoutControl1.GetPreferredSize(new Size(layoutControl1.Width, 0));
            int targetClientHeight = barDockControlTop.Height + preferredSize.Height;

            Size = new Size(430, Height);
            ClientSize = new Size(ClientSize.Width, targetClientHeight);
        }

        private int GetSelectedRecoveryOptionValue()
        {
            return int.TryParse(cbbRecoveryOption.EditValue?.ToString(), out int optionValue)
                ? optionValue
                : RecoveryOptionNoneValue;
        }

        private string GetSelectedRecoveryOption()
        {
            switch (GetSelectedRecoveryOptionValue())
            {
                case RecoveryOptionScrapValue:
                    return dt309_RecoveryConst.RecoveryOptionScrap;
                case RecoveryOptionRestockValue:
                    return dt309_RecoveryConst.RecoveryOptionRestock;
                default:
                    return dt309_RecoveryConst.RecoveryOptionNone;
            }
        }

        private void cbbRecoveryOption_EditValueChanged(object sender, EventArgs e)
        {
            UpdateRecoverySectionVisibility();
        }

        private void btnViewGuideDocs_Click(object sender, EventArgs e)
        {
            var guides = dt309_RecoveryBUS.Instance.GetGuideList();
            if (guides.Count == 0)
            {
                XtraMessageBox.Show("目前尚未上傳報廢指引。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Material309RecoveryHelper.OpenGuideFiles(guides);
        }

        private void LoadRecoveryLookups()
        {
            if (material == null)
            {
                return;
            }

            var activeUsers = dm_UserBUS.Instance.GetList()
                .Where(r => (r.Status ?? 0) == 0
                    && !string.IsNullOrWhiteSpace(r.IdDepartment)
                    && r.IdDepartment.StartsWith(material.IdDept, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Id)
                .ThenBy(r => r.DisplayName)
                .ToList();

            sleAssignedUser.Properties.DataSource = activeUsers;
            sleAssignedUser.EditValue = TPConfigs.LoginUser.Id;

            cbbRestockStorage.Properties.DataSource = storages;
            cbbRestockStorage.EditValue = storages.FirstOrDefault(r => r.Id == 2)?.Id ?? storages.FirstOrDefault()?.Id;
        }

        private void UpdateRecoverySectionVisibility()
        {
            bool isIssue = eventInfo == "領用";
            int option = GetSelectedRecoveryOptionValue();
            bool showRecoveryFields = isIssue && option != RecoveryOptionNoneValue;
            bool isScrap = isIssue && option == RecoveryOptionScrapValue;
            bool isRestock = isIssue && option == RecoveryOptionRestockValue;

            lcRecoveryOption.Visibility = isIssue
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRecoveryNote.Visibility = showRecoveryFields
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcAssignedUser.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcPlannedDisposeDate.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcGuideDocs.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRestockStorage.Visibility = isRestock
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            RebuildRequiredItems();
            RefreshLayoutCaptions();
            AdjustIssueFormSize();
        }

        private void RebuildRequiredItems()
        {
            lcImpControls = new List<LayoutControlItem> { lcStorageFrom, lcStorageTo, lcQuantity };

            switch (eventInfo)
            {
                case "領用":
                    lcImpControls.Add(lcDesc);
                    if (GetSelectedRecoveryOptionValue() == RecoveryOptionScrapValue)
                    {
                        lcImpControls.Add(lcAssignedUser);
                        lcImpControls.Add(lcPlannedDisposeDate);
                    }

                    if (GetSelectedRecoveryOptionValue() == RecoveryOptionRestockValue)
                    {
                        lcImpControls.Add(lcRestockStorage);
                    }
                    break;
                case "收貨":
                    lcImpControls.Add(lcPrice);
                    break;
                case "盤點":
                    lcImpControls.Add(lcDesc);
                    break;
            }
        }

        private void RefreshLayoutCaptions()
        {
            foreach (var item in lcControls)
            {
                if (!lcBaseTexts.ContainsKey(item))
                {
                    lcBaseTexts[item] = item.Text.Replace("<color=#000000>", string.Empty)
                        .Replace("</color>", string.Empty)
                        .Replace("<color=red>*</color>", string.Empty);
                }

                string baseText = lcBaseTexts[item];
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{baseText}</color>";

                bool isRequired = lcImpControls.Contains(item)
                    && item.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && (!item.TextVisible || item.Control == null || item.Control.Enabled);

                if (isRequired)
                {
                    item.Text += "<color=red>*</color>";
                }
            }
        }

        private bool EnsureMaterialActive(bool closeWhenBlocked = false)
        {
            if (material == null)
            {
                MsgTP.MsgError("找不到對應物料");
                if (closeWhenBlocked) Close();
                return false;
            }

            if (material.IsDisable == true)
            {
                XtraMessageBox.Show("停用中的物料不可進行庫存作業。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (closeWhenBlocked) Close();
                return false;
            }

            return true;
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
                    Size = new Size(430, Size.Height);
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

            UpdateRecoverySectionVisibility();
        }

        private void f309_Transaction_Info_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>()
            {
                lcStorageFrom, lcStorageTo, lcStorageFromQuantity, lcStorageToQuantity,
                lcQuantity, lcDesc, lcPrice, lcExpDate,
                lcRecoveryOption, lcAssignedUser,
                lcPlannedDisposeDate, lcRestockStorage, lcRecoveryNote, lcGuideDocs
            };

            foreach (var item in lcControls.Where(r => r != null))
            {
                if (!lcBaseTexts.ContainsKey(item))
                {
                    lcBaseTexts[item] = item.Text;
                }
            }

            storages = dt309_StoragesBUS.Instance.GetList();
            cbbStorageFrom.Properties.DataSource = storages;
            cbbStorageFrom.Properties.DisplayMember = "DisplayName";
            cbbStorageFrom.Properties.ValueMember = "Id";

            cbbStorageTo.Properties.DataSource = storages;
            cbbStorageTo.Properties.DisplayMember = "DisplayName";
            cbbStorageTo.Properties.ValueMember = "Id";

            material = dt309_MaterialsBUS.Instance.GetItemById(idMaterial);
            if (!EnsureMaterialActive(true))
            {
                return;
            }

            lcBaseTexts[lcQuantity] = lcBaseTexts[lcQuantity].Replace("事件", eventInfo);
            LoadRecoveryLookups();

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
            material = dt309_MaterialsBUS.Instance.GetItemById(idMaterial);
            if (!EnsureMaterialActive())
            {
                return;
            }

            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (item.Enabled == false || item.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always) continue;
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

                    result = dt309_RecoveryBUS.Instance.CreateIssueTransactionWithRecovery(
                        idMaterial,
                        (int)cbbStorageFrom.EditValue,
                        quantity,
                        txbDesc.EditValue?.ToString(),
                        TPConfigs.LoginUser.Id,
                        GetSelectedRecoveryOption(),
                        sleAssignedUser.EditValue?.ToString(),
                        dePlannedDisposeDate.EditValue == null
                            ? (DateTime?)null
                            : dePlannedDisposeDate.DateTime.Date,
                        cbbRestockStorage.EditValue == null
                            ? (int?)null
                            : Convert.ToInt32(cbbRestockStorage.EditValue),
                        memoRecoveryNote.EditValue?.ToString(),
                        out string recoveryMessage);

                    if (!result)
                    {
                        MsgTP.MsgError(string.IsNullOrWhiteSpace(recoveryMessage)
                            ? "建立領用與回收單失敗。"
                            : recoveryMessage);
                        return;
                    }

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
