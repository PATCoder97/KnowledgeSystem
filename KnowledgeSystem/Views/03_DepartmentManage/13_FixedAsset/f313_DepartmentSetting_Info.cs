using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_DepartmentSetting_Info : XtraForm
    {
        private readonly FixedAsset313Context module;
        private readonly dt313_DepartmentSetting setting;

        public f313_DepartmentSetting_Info(FixedAsset313Context module, dt313_DepartmentSetting setting)
        {
            InitializeComponent();
            this.module = module;
            this.setting = setting ?? new dt313_DepartmentSetting();
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f313_DepartmentSetting_Info_Load(object sender, EventArgs e)
        {
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
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dept = cbbDept.SelectedItem as LookupItem;
            if (dept == null)
            {
                MsgTP.MsgError("請選擇部門");
                return;
            }

            setting.IdDept = dept.Value;
            setting.QuarterlySampleRate = Convert.ToInt32(spinRate.EditValue);
            setting.IsActive = chkActive.Checked;
            setting.UpdatedBy = TPConfigs.LoginUser.Id;
            setting.UpdatedDate = DateTime.Now;

            if (!dt313_DepartmentSettingBUS.Instance.AddOrUpdate(setting))
            {
                MsgTP.MsgErrorDB();
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
