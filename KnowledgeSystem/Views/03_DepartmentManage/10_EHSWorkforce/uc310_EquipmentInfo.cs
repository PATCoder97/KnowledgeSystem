using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_EquipmentInfo : XtraUserControl
    {
        private readonly BindingSource sourceData = new BindingSource();

        private List<vw310_EquipmentInfo> equipmentInfos = new List<vw310_EquipmentInfo>();
        private List<dm_GroupUser> userGroups = new List<dm_GroupUser>();
        private List<string> editableDeptIds = new List<string>();
        private bool isEHSAdmin = false;
        private bool canManage = false;

        public uc310_EquipmentInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                equipmentInfos = vw310_EquipmentInfoBUS.Instance.GetList();

                var displayData = equipmentInfos
                    .Select(r => new
                    {
                        r.Id,
                        r.Code,
                        r.DisplayNameVN,
                        r.DisplayNameTW,
                        DeptDisplay = string.Join(" ", new[] { r.DeptId, r.DeptNameTW }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        ManagerDisplay = string.Join(" ", new[] { r.ManagerId, r.ManagerNameTW, r.ManagerNameVN }.Where(x => !string.IsNullOrWhiteSpace(x))),
                        r.Note
                    })
                    .ToList();

                sourceData.DataSource = displayData;
                gcData.DataSource = sourceData;

                gvData.BestFitColumns();
            }
        }

        private void ResolvePermission()
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            List<dm_Group> ehsGroups = dm_GroupBUS.Instance.GetListContainName("安衛環");
            dm_Group adminGroup = ehsGroups.FirstOrDefault(r => string.Equals(r.DisplayName?.Trim(), "安衛環7", StringComparison.Ordinal));
            isEHSAdmin = adminGroup != null && userGroups.Any(r => r.IdGroup == adminGroup.Id);

            editableDeptIds = ehsGroups
                .Where(r => userGroups.Any(gu => gu.IdGroup == r.Id))
                .Select(r => (r.DisplayName ?? string.Empty).Replace("安衛環", string.Empty).Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r) && r != "7" && r.All(char.IsDigit))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            canManage = isEHSAdmin || editableDeptIds.Count > 0;
        }

        private void uc310_EquipmentInfo_Load(object sender, EventArgs e)
        {
            ResolvePermission();

            if (!canManage)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsView.ShowGroupPanel = false;

            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!canManage)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.Create,
                formName = "設備"
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view?.CalcHitInfo(view.GridControl.PointToClient(Control.MousePosition));

            if (hitInfo == null || !hitInfo.InRowCell)
            {
                return;
            }

            int idBase = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, gColId) ?? -1);
            if (idBase <= 0)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.View,
                formName = "設備",
                idBase = idBase
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }
    }
}
