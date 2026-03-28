using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    internal partial class f313_AbnormalCatalog_Info : XtraForm
    {
        private readonly dt313_AbnormalCatalog catalog;

        public f313_AbnormalCatalog_Info(FixedAsset313Context module, dt313_AbnormalCatalog catalog)
        {
            InitializeComponent();
            this.catalog = catalog ?? new dt313_AbnormalCatalog
            {
                CreatedBy = TPConfigs.LoginUser.Id,
                CreatedDate = DateTime.Now
            };
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f313_AbnormalCatalog_Info_Load(object sender, EventArgs e)
        {
            txbCode.EditValue = catalog.Code;
            txbName.EditValue = catalog.DisplayName;
            spinSort.EditValue = catalog.SortOrder;
            chkActive.Checked = catalog.Id == 0 || catalog.IsActive;
            memoRemarks.EditValue = catalog.Remarks;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbCode.Text) || string.IsNullOrWhiteSpace(txbName.Text))
            {
                MsgTP.MsgError("請輸入代碼及名稱");
                return;
            }

            catalog.Code = txbCode.Text.Trim();
            catalog.DisplayName = txbName.Text.Trim();
            catalog.SortOrder = Convert.ToInt32(spinSort.EditValue);
            catalog.IsActive = chkActive.Checked;
            catalog.Remarks = memoRemarks.Text.Trim();

            if (!dt313_AbnormalCatalogBUS.Instance.AddOrUpdate(catalog))
            {
                MsgTP.MsgErrorDB();
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
