using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_FixedProgMain : DevExpress.XtraEditors.XtraForm
    {
        public f00_FixedProgMain()
        {
            InitializeComponent();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> users = new List<dm_User>();
        List<dm_FixedProgress> bases;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                // Kiểm tra xem phải sysAdmin không, nếu là admin thì cho xem tất cả lưu trình
                bool IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
                if (IsSysAdmin)
                {
                    bases = dm_FixedProgressBUS.Instance.GetList();
                }
                else
                {
                    bases = dm_FixedProgressBUS.Instance.GetListByOwner(TPConfigs.LoginUser.Id);
                }

                users = dm_UserBUS.Instance.GetList();
                var basesDisplay = (from data in bases
                                    join urs in users on data.Owner equals urs.Id into userGroup
                                    from urs in userGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        data,
                                        urs,
                                        DisplayName = urs != null
                                            ? $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}"
                                            : ""
                                    }).ToList();


                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void f00_FixedProgMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f00_FixedProgInfo fInfo = new f00_FixedProgInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "流程";
            fInfo.ShowDialog();

            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int forcusRow = view.FocusedRowHandle;
            if (forcusRow < 0) return;

            int idFixedProg = (int)view.GetRowCellValue(forcusRow, gColId);
            dm_FixedProgress dataRow = dm_FixedProgressBUS.Instance.GetItemById(idFixedProg);

            f00_FixedProgInfo fInfo = new f00_FixedProgInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "流程";
            fInfo.prog = dataRow;
            fInfo.ShowDialog();

            LoadData();
        }
    }
}
