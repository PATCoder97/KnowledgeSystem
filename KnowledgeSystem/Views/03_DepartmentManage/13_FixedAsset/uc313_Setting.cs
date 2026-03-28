using DataAccessLayer;
using DevExpress.Utils.Menu;
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
        private DXMenuItem itemViewDeptInfo;
        private DXMenuItem itemViewCatalogInfo;

        public uc313_Setting()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
            FixedAsset313UIHelper.ApplyUserControlStyle(this, barManagerTP, bar2);
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

        private void InitializeMenuItems()
        {
            itemViewDeptInfo = FixedAsset313UIHelper.CreateMenuItem("查看資訊", ItemViewDeptInfo_Click, TPSvgimages.View);
            itemViewCatalogInfo = FixedAsset313UIHelper.CreateMenuItem("查看資訊", ItemViewCatalogInfo_Click, TPSvgimages.View);
        }

        private void uc313_Setting_Load(object sender, EventArgs e)
        {
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvDept);
            FixedAsset313GridHelper.ConfigureReadOnlyView(gvCatalog);
            gvDept.DoubleClick += gvDept_DoubleClick;
            gvCatalog.DoubleClick += gvCatalog_DoubleClick;
            gvDept.PopupMenuShowing += gvDept_PopupMenuShowing;
            gvCatalog.PopupMenuShowing += gvCatalog_PopupMenuShowing;
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
            btnCatalogAdd.Enabled = enable;
            btnDeptEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCatalogEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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

        private void OpenDeptSetting(EventFormInfo eventInfo, dt313_DepartmentSetting setting)
        {
            if (eventInfo != EventFormInfo.Create && setting == null)
            {
                return;
            }

            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_DepartmentSetting_Info(module, setting == null ? null : module.CloneDepartmentSetting(setting)))
            {
                form.eventInfo = eventInfo;
                form.formName = "部門設定";
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void OpenCatalog(EventFormInfo eventInfo, dt313_AbnormalCatalog catalog)
        {
            if (eventInfo != EventFormInfo.Create && catalog == null)
            {
                return;
            }

            if (!module.IsManager313)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            using (var form = new f313_AbnormalCatalog_Info(module, catalog == null ? null : module.CloneAbnormalCatalog(catalog)))
            {
                form.eventInfo = eventInfo;
                form.formName = "異常項目";
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnDeptAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenDeptSetting(EventFormInfo.Create, null);
        }

        private void btnDeptEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenDeptSetting(EventFormInfo.View, GetFocusedDept()?.Entity);
        }

        private void btnCatalogAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenCatalog(EventFormInfo.Create, null);
        }

        private void btnCatalogEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenCatalog(EventFormInfo.View, GetFocusedCatalog()?.Entity);
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvDept_DoubleClick(object sender, EventArgs e)
        {
            OpenDeptSetting(EventFormInfo.View, GetFocusedDept()?.Entity);
        }

        private void gvCatalog_DoubleClick(object sender, EventArgs e)
        {
            OpenCatalog(EventFormInfo.View, GetFocusedCatalog()?.Entity);
        }

        private void gvDept_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && e.Menu != null)
            {
                e.Menu.Items.Add(itemViewDeptInfo);
            }
        }

        private void gvCatalog_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && e.Menu != null)
            {
                e.Menu.Items.Add(itemViewCatalogInfo);
            }
        }

        private void ItemViewDeptInfo_Click(object sender, EventArgs e)
        {
            OpenDeptSetting(EventFormInfo.View, GetFocusedDept()?.Entity);
        }

        private void ItemViewCatalogInfo_Click(object sender, EventArgs e)
        {
            OpenCatalog(EventFormInfo.View, GetFocusedCatalog()?.Entity);
        }
    }
}
