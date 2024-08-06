using BusinessLayer;
using DataAccessLayer;
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
    public partial class f402_UserSigns : DevExpress.XtraEditors.XtraForm
    {
        public f402_UserSigns()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.View;
        public string idUsr = "";

        BindingSource _sourceAllSign = new BindingSource();
        BindingSource _sourceSelectSign = new BindingSource();

        List<dm_Sign> signs = new List<dm_Sign>();
        List<dm_Sign> selectSigns = new List<dm_Sign>();

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

        private void f402_UserSigns_Load(object sender, EventArgs e)
        {
            gvAllSign.ReadOnlyGridView();
            gvSelectSign.ReadOnlyGridView();

            gcAllSign.DataSource = _sourceAllSign;
            gcSelectSign.DataSource = _sourceSelectSign;

            signs = dm_SignBUS.Instance.GetList();
            _sourceAllSign.DataSource = signs;
            _sourceSelectSign.DataSource = selectSigns;

            switch (eventInfo)
            {
                case EventFormInfo.View:
                    var userRoles = dm_SignUsersBUS.Instance.GetListByUID(idUsr).Select(r => r.IdSign).ToList();
                    selectSigns.AddRange(signs.Where(a => userRoles.Exists(b => b == a.Id)));
                    signs.RemoveAll(a => userRoles.Exists(b => b == a.Id));

                    gcAllSign.RefreshDataSource();
                    gcSelectSign.RefreshDataSource();
                    break;
                case EventFormInfo.Update:
                    break;
            }

            LockControl();
        }

        private void gvAllSign_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (!(info.InRow || info.InRowCell) || eventInfo != EventFormInfo.Update) return;

            dm_Sign sign = view.GetRow(view.FocusedRowHandle) as dm_Sign;

            signs.Remove(sign);
            view.RefreshData();
            selectSigns.Add(sign);
            gvSelectSign.RefreshData();
        }

        private void gvSelectSign_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (!(info.InRow || info.InRowCell) || eventInfo != EventFormInfo.Update) return;

            dm_Sign sign = view.GetRow(view.FocusedRowHandle) as dm_Sign;

            selectSigns.Remove(sign);
            view.RefreshData();
            signs.Add(sign);
            gvAllSign.RefreshData();
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
                List<dm_SignUsers> userSignsAdd = selectSigns.Select(r => new dm_SignUsers { IdUser = idUsr, IdSign = r.Id }).ToList();
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