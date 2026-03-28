using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using KnowledgeSystem.Helpers;
using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_BatchCreate : XtraForm
    {
        private readonly bool isMonthly;
        private readonly FixedAsset313Context module;

        public BatchCreateDialogResult ResultData { get; private set; }

        private f313_BatchCreate(FixedAsset313Context module, bool isMonthly)
        {
            InitializeComponent();
            this.module = module;
            this.isMonthly = isMonthly;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            FixedAsset313UIHelper.ApplyFormStyle(this, barManagerTP, bar2);
        }

        public static f313_BatchCreate CreateMonthly(FixedAsset313Context module)
        {
            return new f313_BatchCreate(module, true);
        }

        public static f313_BatchCreate CreateQuarterly(FixedAsset313Context module)
        {
            return new f313_BatchCreate(module, false);
        }

        private void f313_BatchCreate_Load(object sender, EventArgs e)
        {
            FixedAsset313UIHelper.ApplyLayoutItemCaptions(lcTarget, lcPeriod, lcSampleRate);

            Text = isMonthly ? "建立月檢批次" : "建立季檢批次";
            lcSampleRate.Visibility = isMonthly
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

            cbbTarget.Properties.Items.AddRange((isMonthly ? module.GetUserLookupItems(true) : module.GetDepartmentLookupItems(true)).ToArray());
            datePeriod.EditValue = DateTime.Today;
            spinRate.EditValue = 10;

            cbbTarget.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            if (!isMonthly)
            {
                cbbTarget.SelectedIndexChanged += cbbTarget_SelectedIndexChanged;
            }
        }

        private void cbbTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            var target = cbbTarget.SelectedItem as LookupItem;
            var setting = module.DepartmentSettings.Find(r => r.IdDept == target?.Value);
            spinRate.EditValue = setting?.QuarterlySampleRate ?? 10;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var target = cbbTarget.SelectedItem as LookupItem;
            if (target == null || string.IsNullOrWhiteSpace(target.Value))
            {
                MsgTP.MsgError("請選擇目標");
                return;
            }

            DateTime selectedDate = datePeriod.EditValue == null ? DateTime.Today : Convert.ToDateTime(datePeriod.EditValue);
            string periodKey = isMonthly
                ? selectedDate.ToString("yyyyMM")
                : $"{selectedDate.Year}Q{((selectedDate.Month - 1) / 3) + 1}";

            ResultData = new BatchCreateDialogResult
            {
                TargetId = target.Value,
                TargetDisplay = target.Display,
                SelectedDate = selectedDate,
                PeriodKey = periodKey,
                SampleRate = isMonthly ? 100 : Convert.ToInt32(spinRate.EditValue)
            };

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
