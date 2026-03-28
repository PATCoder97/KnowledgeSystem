using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal sealed class AssetEditForm : XtraForm
    {
        private readonly List<LookupItem> deptItems;
        private readonly List<LookupItem> userItems;
        private readonly dt313_FixedAsset source;

        private TextEdit txtAssetCode;
        private TextEdit txtAssetNameTw;
        private TextEdit txtAssetNameVn;
        private ComboBoxEdit cbbDept;
        private ComboBoxEdit cbbManager;
        private ComboBoxEdit cbbCategory;
        private TextEdit txtTypeName;
        private TextEdit txtLocation;
        private TextEdit txtBrandSpec;
        private TextEdit txtOrigin;
        private DateEdit dateAcquire;
        private ComboBoxEdit cbbStatus;
        private MemoEdit memoRemarks;

        public dt313_FixedAsset Asset { get; private set; }

        public AssetEditForm(List<LookupItem> deptItems, List<LookupItem> userItems, dt313_FixedAsset source)
        {
            this.deptItems = deptItems ?? new List<LookupItem>();
            this.userItems = userItems ?? new List<LookupItem>();
            this.source = source;
            InitializeUi();
            BindData();
        }

        private void InitializeUi()
        {
            Text = source == null ? "新增固定資產" : "編輯固定資產";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(720, 620);
            MinimizeBox = false;
            MaximizeBox = false;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 13,
                Padding = new Padding(12)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtAssetCode = CreateTextEdit();
            txtAssetNameTw = CreateTextEdit();
            txtAssetNameVn = CreateTextEdit();
            cbbDept = CreateComboBox(deptItems);
            cbbManager = CreateComboBox(userItems);
            cbbCategory = CreateComboBox(new List<LookupItem>
            {
                new LookupItem("General", "General / 一般設備"),
                new LookupItem("DutyFreeImported", "DutyFreeImported / 免稅進口設備")
            });
            txtTypeName = CreateTextEdit();
            txtLocation = CreateTextEdit();
            txtBrandSpec = CreateTextEdit();
            txtOrigin = CreateTextEdit();
            dateAcquire = new DateEdit { Dock = DockStyle.Fill };
            dateAcquire.Properties.Appearance.Font = TPConfigs.fontUI14;
            dateAcquire.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Combo));
            dateAcquire.Properties.CalendarTimeProperties.Buttons.Add(new EditorButton(ButtonPredefines.Combo));
            cbbStatus = CreateComboBox(new List<LookupItem>
            {
                new LookupItem("Active", "Active"),
                new LookupItem("Repair", "Repair"),
                new LookupItem("Idle", "Idle"),
                new LookupItem("Inactive", "Inactive")
            });
            cbbStatus.Properties.TextEditStyle = TextEditStyles.Standard;
            memoRemarks = new MemoEdit
            {
                Dock = DockStyle.Fill,
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };

            AddRow(root, 0, "資產編號", txtAssetCode);
            AddRow(root, 1, "資產中文名稱", txtAssetNameTw);
            AddRow(root, 2, "資產越文名稱", txtAssetNameVn);
            AddRow(root, 3, "部門", cbbDept);
            AddRow(root, 4, "經辦", cbbManager);
            AddRow(root, 5, "分類", cbbCategory);
            AddRow(root, 6, "類別", txtTypeName);
            AddRow(root, 7, "位置", txtLocation);
            AddRow(root, 8, "廠牌規格", txtBrandSpec);
            AddRow(root, 9, "產地", txtOrigin);
            AddRow(root, 10, "取得日期", dateAcquire);
            AddRow(root, 11, "狀態", cbbStatus);
            AddRow(root, 12, "備註", memoRemarks);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new SimpleButton { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 };
            var btnSave = new SimpleButton { Text = "儲存", Width = 90 };
            btnSave.Click += BtnSave_Click;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnSave);

            Controls.Add(root);
            Controls.Add(bottom);
        }

        private TextEdit CreateTextEdit()
        {
            return new TextEdit
            {
                Dock = DockStyle.Fill,
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };
        }

        private ComboBoxEdit CreateComboBox(List<LookupItem> items)
        {
            var combo = new ComboBoxEdit { Dock = DockStyle.Fill };
            combo.Properties.Appearance.Font = TPConfigs.fontUI14;
            combo.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            combo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            combo.Properties.Items.AddRange(items.ToArray());
            return combo;
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, Control control)
        {
            panel.RowStyles.Add(new RowStyle(rowIndex == 12 ? SizeType.Percent : SizeType.AutoSize, rowIndex == 12 ? 100F : 0F));
            var label = new LabelControl
            {
                Text = title,
                Dock = DockStyle.Fill,
                Appearance = { Font = TPConfigs.fontUI14 },
                Padding = new Padding(0, 8, 0, 0)
            };
            control.Margin = new Padding(0, 0, 0, 8);
            panel.Controls.Add(label, 0, rowIndex);
            panel.Controls.Add(control, 1, rowIndex);
        }

        private void BindData()
        {
            if (source == null)
            {
                cbbCategory.SelectedItem = cbbCategory.Properties.Items[0];
                cbbStatus.EditValue = "Active";
                return;
            }

            txtAssetCode.Text = source.AssetCode;
            txtAssetCode.Enabled = false;
            txtAssetNameTw.Text = source.AssetNameTW;
            txtAssetNameVn.Text = source.AssetNameVN;
            SelectComboValue(cbbDept, source.IdDept);
            SelectComboValue(cbbManager, source.IdManager);
            SelectComboValue(cbbCategory, source.AssetCategory);
            txtTypeName.Text = source.TypeName;
            txtLocation.Text = source.Location;
            txtBrandSpec.Text = source.BrandSpec;
            txtOrigin.Text = source.Origin;
            dateAcquire.EditValue = source.AcquireDate;
            cbbStatus.EditValue = source.Status;
            memoRemarks.Text = source.Remarks;
        }

        private void SelectComboValue(ComboBoxEdit combo, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                combo.SelectedIndex = 0;
                return;
            }

            foreach (var item in combo.Properties.Items)
            {
                if (item is LookupItem lookup && string.Equals(lookup.Value, value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedItem = item;
                    return;
                }
            }

            combo.EditValue = value;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAssetCode.Text))
            {
                XtraMessageBox.Show("請輸入資產編號。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAssetNameTw.Text))
            {
                XtraMessageBox.Show("請輸入資產中文名稱。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dept = cbbDept.SelectedItem as LookupItem;
            if (dept == null || string.IsNullOrWhiteSpace(dept.Value))
            {
                XtraMessageBox.Show("請選擇部門。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var category = cbbCategory.SelectedItem as LookupItem;
            var manager = cbbManager.SelectedItem as LookupItem;

            Asset = source ?? new dt313_FixedAsset
            {
                CreatedBy = TPConfigs.LoginUser.Id,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            Asset.AssetCode = txtAssetCode.Text.Trim();
            Asset.AssetNameTW = txtAssetNameTw.Text.Trim();
            Asset.AssetNameVN = txtAssetNameVn.Text.Trim();
            Asset.IdDept = dept.Value;
            Asset.IdManager = manager?.Value;
            Asset.AssetCategory = category?.Value ?? "General";
            Asset.TypeName = txtTypeName.Text.Trim();
            Asset.Location = txtLocation.Text.Trim();
            Asset.BrandSpec = txtBrandSpec.Text.Trim();
            Asset.Origin = txtOrigin.Text.Trim();
            Asset.AcquireDate = dateAcquire.EditValue == null ? (DateTime?)null : dateAcquire.DateTime.Date;
            Asset.Status = string.IsNullOrWhiteSpace(cbbStatus.Text) ? "Active" : cbbStatus.Text.Trim();
            Asset.Remarks = memoRemarks.Text.Trim();
            Asset.UpdatedBy = TPConfigs.LoginUser.Id;
            Asset.UpdatedDate = DateTime.Now;

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    internal sealed class BatchCreateForm : XtraForm
    {
        private readonly bool isMonthly;
        private readonly List<LookupItem> items;
        private readonly List<dt313_DepartmentSetting> settings;

        private ComboBoxEdit cbbTarget;
        private DateEdit datePeriod;
        private SpinEdit spinRate;

        public BatchCreateDialogResult Result { get; private set; }

        private BatchCreateForm(bool isMonthly, List<LookupItem> items, List<dt313_DepartmentSetting> settings)
        {
            this.isMonthly = isMonthly;
            this.items = items ?? new List<LookupItem>();
            this.settings = settings ?? new List<dt313_DepartmentSetting>();
            InitializeUi();
        }

        public static BatchCreateForm CreateMonthly(List<LookupItem> users)
        {
            return new BatchCreateForm(true, users, null);
        }

        public static BatchCreateForm CreateQuarterly(List<LookupItem> departments, List<dt313_DepartmentSetting> settings)
        {
            return new BatchCreateForm(false, departments, settings);
        }

        private void InitializeUi()
        {
            Text = isMonthly ? "建立月檢批次" : "建立季檢批次";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(520, 240);
            MinimizeBox = false;
            MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            cbbTarget = new ComboBoxEdit { Dock = DockStyle.Fill };
            cbbTarget.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbTarget.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbTarget.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cbbTarget.Properties.Items.AddRange(items.ToArray());

            datePeriod = new DateEdit { Dock = DockStyle.Fill };
            datePeriod.Properties.Appearance.Font = TPConfigs.fontUI14;
            datePeriod.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Combo));
            datePeriod.Properties.CalendarTimeProperties.Buttons.Add(new EditorButton(ButtonPredefines.Combo));
            datePeriod.EditValue = DateTime.Today;

            spinRate = new SpinEdit { Dock = DockStyle.Fill, Enabled = !isMonthly };
            spinRate.Properties.Appearance.Font = TPConfigs.fontUI14;
            spinRate.Properties.MinValue = 1;
            spinRate.Properties.MaxValue = 100;

            AddRow(panel, 0, isMonthly ? "經辦" : "部門", cbbTarget);
            AddRow(panel, 1, isMonthly ? "月份" : "季度日期", datePeriod);
            if (!isMonthly)
            {
                AddRow(panel, 2, "抽樣率%", spinRate);
            }

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new SimpleButton { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 };
            var btnSave = new SimpleButton { Text = "建立", Width = 90 };
            btnSave.Click += BtnSave_Click;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnSave);

            Controls.Add(panel);
            Controls.Add(bottom);

            cbbTarget.SelectedIndexChanged += (s, e) => UpdateRate();
            if (cbbTarget.Properties.Items.Count > 0)
            {
                cbbTarget.SelectedIndex = 0;
            }
            UpdateRate();
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, Control control)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = new LabelControl
            {
                Text = title,
                Appearance = { Font = TPConfigs.fontUI14 },
                Padding = new Padding(0, 8, 0, 0)
            };
            control.Margin = new Padding(0, 0, 0, 12);
            panel.Controls.Add(label, 0, rowIndex);
            panel.Controls.Add(control, 1, rowIndex);
        }

        private void UpdateRate()
        {
            if (isMonthly) return;
            var dept = cbbTarget.SelectedItem as LookupItem;
            int rate = settings.FirstOrDefault(r => r.IdDept == dept?.Value && r.IsActive)?.QuarterlySampleRate ?? 10;
            spinRate.EditValue = Math.Max(1, rate);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var target = cbbTarget.SelectedItem as LookupItem;
            if (target == null || string.IsNullOrWhiteSpace(target.Value))
            {
                XtraMessageBox.Show("請先選擇目標對象。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectedDate = datePeriod.DateTime.Date;
            Result = new BatchCreateDialogResult
            {
                TargetId = target.Value,
                TargetDisplay = target.Display,
                SelectedDate = selectedDate,
                PeriodKey = isMonthly ? selectedDate.ToString("yyyyMM") : $"{selectedDate.Year}Q{((selectedDate.Month - 1) / 3) + 1}",
                SampleRate = isMonthly ? 0 : Convert.ToInt32(spinRate.Value)
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }

    internal sealed class DepartmentSettingForm : XtraForm
    {
        private readonly dt313_DepartmentSetting source;
        private readonly List<LookupItem> deptItems;

        private ComboBoxEdit cbbDept;
        private SpinEdit spinRate;
        private CheckEdit chkActive;

        public dt313_DepartmentSetting Setting { get; private set; }

        public DepartmentSettingForm(List<LookupItem> deptItems, dt313_DepartmentSetting source)
        {
            this.deptItems = deptItems ?? new List<LookupItem>();
            this.source = source;
            InitializeUi();
            BindData();
        }

        private void InitializeUi()
        {
            Text = source == null ? "新增部門設定" : "編輯部門設定";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(460, 220);
            MinimizeBox = false;
            MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            cbbDept = new ComboBoxEdit { Dock = DockStyle.Fill };
            cbbDept.Properties.Appearance.Font = TPConfigs.fontUI14;
            cbbDept.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            cbbDept.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            cbbDept.Properties.Items.AddRange(deptItems.ToArray());

            spinRate = new SpinEdit { Dock = DockStyle.Fill };
            spinRate.Properties.Appearance.Font = TPConfigs.fontUI14;
            spinRate.Properties.MinValue = 1;
            spinRate.Properties.MaxValue = 100;

            chkActive = new CheckEdit
            {
                Text = "啟用此設定",
                Dock = DockStyle.Left,
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };

            AddRow(panel, 0, "部門", cbbDept);
            AddRow(panel, 1, "抽樣率%", spinRate);
            AddRow(panel, 2, "", chkActive);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new SimpleButton { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 };
            var btnSave = new SimpleButton { Text = "儲存", Width = 90 };
            btnSave.Click += BtnSave_Click;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnSave);

            Controls.Add(panel);
            Controls.Add(bottom);
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, Control control)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = new LabelControl
            {
                Text = title,
                Appearance = { Font = TPConfigs.fontUI14 },
                Padding = new Padding(0, 8, 0, 0)
            };
            control.Margin = new Padding(0, 0, 0, 10);
            panel.Controls.Add(label, 0, rowIndex);
            panel.Controls.Add(control, 1, rowIndex);
        }

        private void BindData()
        {
            if (source == null)
            {
                chkActive.Checked = true;
                spinRate.Value = 10;
                return;
            }

            foreach (var item in cbbDept.Properties.Items)
            {
                if (item is LookupItem lookup && lookup.Value == source.IdDept)
                {
                    cbbDept.SelectedItem = item;
                    break;
                }
            }
            cbbDept.Enabled = false;
            spinRate.Value = source.QuarterlySampleRate;
            chkActive.Checked = source.IsActive;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var dept = cbbDept.SelectedItem as LookupItem;
            if (dept == null)
            {
                XtraMessageBox.Show("請選擇部門。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Setting = source ?? new dt313_DepartmentSetting();
            Setting.IdDept = dept.Value;
            Setting.QuarterlySampleRate = Convert.ToInt32(spinRate.Value);
            Setting.IsActive = chkActive.Checked;
            Setting.UpdatedBy = TPConfigs.LoginUser.Id;
            Setting.UpdatedDate = DateTime.Now;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    internal sealed class AbnormalCatalogForm : XtraForm
    {
        private readonly dt313_AbnormalCatalog source;
        private TextEdit txtCode;
        private TextEdit txtName;
        private SpinEdit spinSort;
        private CheckEdit chkActive;
        private MemoEdit memoRemarks;

        public dt313_AbnormalCatalog Catalog { get; private set; }

        public AbnormalCatalogForm(dt313_AbnormalCatalog source)
        {
            this.source = source;
            InitializeUi();
            BindData();
        }

        private void InitializeUi()
        {
            Text = source == null ? "新增異常項目" : "編輯異常項目";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(520, 360);
            MinimizeBox = false;
            MaximizeBox = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtCode = new TextEdit { Dock = DockStyle.Fill, Properties = { Appearance = { Font = TPConfigs.fontUI14 } } };
            txtName = new TextEdit { Dock = DockStyle.Fill, Properties = { Appearance = { Font = TPConfigs.fontUI14 } } };
            spinSort = new SpinEdit { Dock = DockStyle.Fill };
            spinSort.Properties.Appearance.Font = TPConfigs.fontUI14;
            spinSort.Properties.MinValue = 0;
            spinSort.Properties.MaxValue = 9999;
            chkActive = new CheckEdit
            {
                Text = "啟用此項目",
                Dock = DockStyle.Left,
                Properties = { Appearance = { Font = TPConfigs.fontUI14 } }
            };
            memoRemarks = new MemoEdit { Dock = DockStyle.Fill, Properties = { Appearance = { Font = TPConfigs.fontUI14 } } };

            AddRow(panel, 0, "Code", txtCode);
            AddRow(panel, 1, "顯示名稱", txtName);
            AddRow(panel, 2, "排序", spinSort);
            AddRow(panel, 3, "", chkActive);
            AddRow(panel, 4, "備註", memoRemarks);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                FlowDirection = FlowDirection.RightToLeft
            };
            var btnCancel = new SimpleButton { Text = "取消", DialogResult = DialogResult.Cancel, Width = 90 };
            var btnSave = new SimpleButton { Text = "儲存", Width = 90 };
            btnSave.Click += BtnSave_Click;
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnSave);

            Controls.Add(panel);
            Controls.Add(bottom);
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, Control control)
        {
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var label = new LabelControl
            {
                Text = title,
                Appearance = { Font = TPConfigs.fontUI14 },
                Padding = new Padding(0, 8, 0, 0)
            };
            control.Margin = new Padding(0, 0, 0, 10);
            panel.Controls.Add(label, 0, rowIndex);
            panel.Controls.Add(control, 1, rowIndex);
        }

        private void BindData()
        {
            if (source == null)
            {
                chkActive.Checked = true;
                return;
            }

            txtCode.Text = source.Code;
            txtName.Text = source.DisplayName;
            spinSort.Value = source.SortOrder;
            chkActive.Checked = source.IsActive;
            memoRemarks.Text = source.Remarks;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                XtraMessageBox.Show("請輸入完整的異常代碼與名稱。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Catalog = source ?? new dt313_AbnormalCatalog
            {
                CreatedBy = TPConfigs.LoginUser.Id,
                CreatedDate = DateTime.Now
            };

            Catalog.Code = txtCode.Text.Trim();
            Catalog.DisplayName = txtName.Text.Trim();
            Catalog.SortOrder = Convert.ToInt32(spinSort.Value);
            Catalog.IsActive = chkActive.Checked;
            Catalog.Remarks = memoRemarks.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
