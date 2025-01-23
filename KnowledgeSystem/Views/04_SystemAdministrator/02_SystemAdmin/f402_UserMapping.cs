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
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_UserMapping : DevExpress.XtraEditors.XtraForm
    {
        class DataItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
        }

        public f402_UserMapping()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<DataItem> items = new List<DataItem>();
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

            //switch (eventInfo)
            //{
            //    case EventFormInfo.View:
            //        btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //        break;
            //    case EventFormInfo.Update:
            //        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            //        break;
            //}
        }

        private void f402_UserMapping_Load(object sender, EventArgs e)
        {
            gvAllData.ReadOnlyGridView();
            gvSelectData.ReadOnlyGridView();

            gcAllData.DataSource = _sourceAll;
            gcSelectData.DataSource = _sourceSelect;

            switch (mapData)
            {
                case "group":

                    items = dm_GroupBUS.Instance.GetList().Select(r => new DataItem()
                    {
                        Id = r.Id,
                        Name = r.DisplayName,
                        Desc = r.Describe
                    }).ToList();

                    break;
                case "role":

                    items = dm_RoleBUS.Instance.GetList().Select(r => new DataItem()
                    {
                        Id = r.Id,
                        Name = r.DisplayName,
                        Desc = r.Describe
                    }).ToList();

                    break;

                case "sign":

                    items = dm_SignBUS.Instance.GetList().Select(r => new DataItem()
                    {
                        Id = r.Id,
                        Name = r.DisplayName,
                    }).ToList();

                    break;

                default:
                    break;
            }

            _sourceAll.DataSource = items;
            _sourceSelect.DataSource = selectedItems;
        }

        private void gvAllData_DoubleClick(object sender, EventArgs e)
        {
            HandleGridViewDoubleClick(gvAllData, gvSelectData, items, selectedItems, e);
        }

        private void gvSelectData_DoubleClick(object sender, EventArgs e)
        {
            HandleGridViewDoubleClick(gvSelectData, gvAllData, selectedItems, items, e);
        }

        private void HandleGridViewDoubleClick(GridView sourceView, GridView targetView, List<DataItem> sourceList, List<DataItem> targetList, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridHitInfo info = sourceView.CalcHitInfo(ea.Location);

            if (!(info.InRow || info.InRowCell)) return;

            DataItem item = sourceView.GetRow(sourceView.FocusedRowHandle) as DataItem;
            if (item == null) return;

            sourceList.Remove(item);
            targetList.Add(item);

            sourceView.RefreshData();
            targetView.RefreshData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                List<dm_SignUsers> userSignsAdd = selectedItems.Select(r => new dm_SignUsers { IdUser = idUsr, IdSign = r.Id }).ToList();
                var result1 = dm_SignUsersBUS.Instance.RemoveRangeByUID(idUsr);
                var result2 = dm_SignUsersBUS.Instance.AddRange(userSignsAdd);

                if (result1 && result2)
                {
                    Close();
                }
            }
        }
    }
}