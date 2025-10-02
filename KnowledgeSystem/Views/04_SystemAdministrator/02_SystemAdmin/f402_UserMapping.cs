using BusinessLayer;
using DataAccessLayer;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_UserMapping : DevExpress.XtraEditors.XtraForm
    {
        class DataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public string IdDept { get; set; }
        }

        public f402_UserMapping()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.View;

        List<DataItem> allItems = new List<DataItem>();
        List<DataItem> selectedItems = new List<DataItem>();

        public string idUsr = "";
        public string mapData = "";

        BindingSource _sourceAll = new BindingSource();
        BindingSource _sourceSelect = new BindingSource();

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
        }

        private void LockControl()
        {
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            switch (eventInfo)
            {
                case EventFormInfo.View:
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
                case EventFormInfo.Update:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    break;
            }
        }

        private void f402_UserMapping_Load(object sender, EventArgs e)
        {
            Text += $" - {mapData}";

            gvAllData.ReadOnlyGridView();
            gvSelectData.ReadOnlyGridView();

            gcAllData.DataSource = _sourceAll;
            gcSelectData.DataSource = _sourceSelect;

            switch (mapData)
            {
                case "group":
                    allItems = dm_GroupBUS.Instance.GetList()
                        .Select(g => new DataItem
                        {
                            Id = g.Id,
                            Name = $"[{g.IdDept}] {g.DisplayName}",
                            Desc = g.Describe,
                            IdDept = g.IdDept
                        })
                        .ToList();

                    var userGroupIds = dm_GroupUserBUS.Instance.GetListByUID(idUsr)
                        .Select(x => x.IdGroup)
                        .ToList();

                    selectedItems.AddRange(allItems.Where(i => userGroupIds.Contains(i.Id)));
                    allItems.RemoveAll(i => userGroupIds.Contains(i.Id));
                    break;

                case "role":
                    allItems = dm_RoleBUS.Instance.GetList()
                        .Select(r => new DataItem
                        {
                            Id = r.Id,
                            Name = r.DisplayName,
                            Desc = r.Describe
                        })
                        .ToList();

                    var userRoleIds = dm_UserRoleBUS.Instance.GetListByUID(idUsr)
                        .Select(x => x.IdRole)
                        .ToList();

                    selectedItems.AddRange(allItems.Where(i => userRoleIds.Contains(i.Id)));
                    allItems.RemoveAll(i => userRoleIds.Contains(i.Id));
                    break;

                case "sign":
                    allItems = dm_SignBUS.Instance.GetList()
                        .Select(s => new DataItem
                        {
                            Id = s.Id,
                            Name = s.DisplayName
                        })
                        .ToList();

                    var userSignIds = dm_SignUsersBUS.Instance.GetListByUID(idUsr)
                        .Select(x => x.IdSign)
                        .ToList();

                    selectedItems.AddRange(allItems.Where(i => userSignIds.Contains(i.Id)));
                    allItems.RemoveAll(i => userSignIds.Contains(i.Id));
                    break;
            }

            _sourceAll.DataSource = allItems;
            _sourceSelect.DataSource = selectedItems;

            LockControl();
        }

        private void gvAllData_DoubleClick(object sender, EventArgs e)
        {
            HandleGridViewDoubleClick(gvAllData, gvSelectData, allItems, selectedItems, e);
        }

        private void gvSelectData_DoubleClick(object sender, EventArgs e)
        {
            HandleGridViewDoubleClick(gvSelectData, gvAllData, selectedItems, allItems, e);
        }

        private void HandleGridViewDoubleClick(GridView sourceView, GridView targetView, List<DataItem> sourceList, List<DataItem> targetList, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridHitInfo info = sourceView.CalcHitInfo(ea.Location);

            if (!(info.InRow || info.InRowCell) || eventInfo != EventFormInfo.Update) return;

            DataItem item = sourceView.GetRow(sourceView.FocusedRowHandle) as DataItem;
            if (item == null) return;

            sourceList.Remove(item);
            targetList.Add(item);

            sourceView.RefreshData();
            targetView.RefreshData();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                bool removed = false, added = false;

                switch (mapData)
                {
                    case "group":
                        var userGroups = selectedItems
                            .Select(r => new dm_GroupUser { IdUser = idUsr, IdGroup = r.Id })
                            .ToList();

                        removed = dm_GroupUserBUS.Instance.RemoveRangeByUID(idUsr);
                        added = userGroups.Count == 0 || dm_GroupUserBUS.Instance.AddRange(userGroups);
                        break;

                    case "sign":
                        var userSigns = selectedItems
                            .Select(r => new dm_SignUsers { IdUser = idUsr, IdSign = r.Id })
                            .ToList();

                        removed = dm_SignUsersBUS.Instance.RemoveRangeByUID(idUsr);
                        added = userSigns.Count == 0 || dm_SignUsersBUS.Instance.AddRange(userSigns);
                        break;

                    case "role":
                        var userRoles = selectedItems
                            .Select(r => new dm_UserRole { IdUser = idUsr, IdRole = r.Id })
                            .ToList();

                        removed = dm_UserRoleBUS.Instance.RemoveRangeByUID(idUsr);
                        added = userRoles.Count == 0 || dm_UserRoleBUS.Instance.AddRange(userRoles);
                        break;
                }

                if (removed && added)
                {
                    Close();
                }
            }
        }
    }
}
