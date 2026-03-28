using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_Setting : XtraUserControl
    {
        private readonly FixedAsset313Context module = new FixedAsset313Context();
        private readonly BindingSource deptSource = new BindingSource();
        private readonly BindingSource catalogSource = new BindingSource();

        public uc313_Setting()
        {
            InitializeComponent();
            InitializeIcon();
            Load += uc313_Setting_Load;
        }

        private void InitializeIcon()
        {
            btnDeptAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnDeptEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnCatalogAdd.ImageOptions.SvgImage = TPSvgimages.Add2;
            btnCatalogEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void uc313_Setting_Load(object sender, EventArgs e)
        {
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvDept);
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvCatalog);
            gvDept.DoubleClick += gvDept_DoubleClick;
            gvCatalog.DoubleClick += gvCatalog_DoubleClick;
            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(layoutControl1))
            {
                module.Initialize();
                deptSource.DataSource = module.BuildDepartmentSettingRows();
                catalogSource.DataSource = module.BuildAbnormalCatalogRows();

                gcDept.DataSource = deptSource;
                gcCatalog.DataSource = catalogSource;

                gvDept.PopulateColumns();
                gvCatalog.PopulateColumns();
                ConfigureDeptColumns();
                ConfigureCatalogColumns();
                ApplyPermissions();
            }
        }

        private void ApplyPermissions()
        {
            bool enable = module.IsManager313;
            btnDeptAdd.Enabled = enable;
            btnDeptEdit.Enabled = enable;
            btnCatalogAdd.Enabled = enable;
            btnCatalogEdit.Enabled = enable;
        }

        private void ConfigureDeptColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvDept, nameof(DepartmentSettingGridRow.Entity));
            FixedAsset313GridHelper.SetColumn(gvDept, nameof(DepartmentSettingGridRow.IdDept), "部門編號", 100);
            FixedAsset313GridHelper.SetColumn(gvDept, nameof(DepartmentSettingGridRow.DeptName), "部門名稱", 180);
            FixedAsset313GridHelper.SetColumn(gvDept, nameof(DepartmentSettingGridRow.QuarterlySampleRate), "抽樣率%", 90);
            FixedAsset313GridHelper.SetColumn(gvDept, nameof(DepartmentSettingGridRow.IsActive), "啟用", 70);
            FixedAsset313GridHelper.SetColumn(gvDept, nameof(DepartmentSettingGridRow.UpdatedBy), "更新者", 100);
            FixedAsset313GridHelper.SetDateColumn(gvDept, nameof(DepartmentSettingGridRow.UpdatedDate), "更新時間", 130, "yyyy-MM-dd HH:mm");
            gvDept.BestFitColumns();
        }

        private void ConfigureCatalogColumns()
        {
            FixedAsset313GridHelper.HideColumn(gvCatalog, nameof(AbnormalCatalogGridRow.Entity));
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.Code), "代碼", 100);
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.DisplayName), "異常名稱", 180);
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.SortOrder), "排序", 70);
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.IsActive), "啟用", 70);
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.Remarks), "備註", 220);
            FixedAsset313GridHelper.SetColumn(gvCatalog, nameof(AbnormalCatalogGridRow.CreatedBy), "建立者", 90);
            FixedAsset313GridHelper.SetDateColumn(gvCatalog, nameof(AbnormalCatalogGridRow.CreatedDate), "建立時間", 130, "yyyy-MM-dd HH:mm");
            gvCatalog.BestFitColumns();
        }

        private DepartmentSettingGridRow GetFocusedDept()
        {
            return gvDept.GetFocusedRow() as DepartmentSettingGridRow;
        }

        private AbnormalCatalogGridRow GetFocusedCatalog()
        {
            return gvCatalog.GetFocusedRow() as AbnormalCatalogGridRow;
        }

        private void EditDeptSetting(dt313_DepartmentSetting setting)
        {
            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_DepartmentSetting_Info(module, setting == null ? null : module.CloneDepartmentSetting(setting)))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void EditCatalog(dt313_AbnormalCatalog catalog)
        {
            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_AbnormalCatalog_Info(module, catalog == null ? null : module.CloneAbnormalCatalog(catalog)))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnDeptAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditDeptSetting(null);
        }

        private void btnDeptEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditDeptSetting(GetFocusedDept()?.Entity);
        }

        private void btnCatalogAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditCatalog(null);
        }

        private void btnCatalogEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditCatalog(GetFocusedCatalog()?.Entity);
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvDept_DoubleClick(object sender, EventArgs e)
        {
            EditDeptSetting(GetFocusedDept()?.Entity);
        }

        private void gvCatalog_DoubleClick(object sender, EventArgs e)
        {
            EditCatalog(GetFocusedCatalog()?.Entity);
        }
    }
}
