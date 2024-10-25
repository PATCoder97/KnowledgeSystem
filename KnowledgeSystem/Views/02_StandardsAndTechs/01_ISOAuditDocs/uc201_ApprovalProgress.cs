using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_ApprovalProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_ApprovalProgress()
        {
            InitializeComponent();
            InitializeIcon();

            gvData.MasterRowExpanded += (s, e) =>
            {
                GridView masterView = s as GridView;
                int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
                GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

                detailView.BestFitColumns();
            };
        }

        List<dt201_Forms> baseForm;
        List<dm_User> users;

        BindingSource sourceForm = new BindingSource();

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            List<dt201_Base> baseData = dt201_BaseBUS.Instance.GetList();

            baseForm = dt201_FormsBUS.Instance.GetListProcessing().Where(r => r.UploadUser == TPConfigs.LoginUser.Id).ToList();
            users = dm_UserBUS.Instance.GetList();

            var dataInfo = (from data in baseForm
                            join category in baseData on data.IdBase equals category.Id
                            join nextStep in users on data.NextStepProg equals nextStep.Id into dtg
                            from nextStep in dtg.DefaultIfEmpty() 
                            select new
                            {
                                category,
                                data,
                                nextStep
                            } into dt
                            group dt by dt.category.IdParent into dtg
                            select new
                            {
                                Key = dtg.Key,
                                category = baseData.FirstOrDefault(r => r.Id == dtg.Key),
                                detailData = dtg.Select(r => new
                                {
                                    NextStepProg = r.nextStep != null ? $"{r.nextStep.Id.Substring(5)} {r.nextStep.IdDepartment}/{r.nextStep.DisplayName}" : null,
                                    data = r.data,
                                }).ToList()
                            }).ToList();

            sourceForm.DataSource = dataInfo;
            gcData.LevelTree.Nodes.Add("detailData", gvDetail);

            gvData.BestFitColumns();

            if (gvData.RowCount > 0)
                gvData.ExpandMasterRow(0, 0);
        }

        private void uc201_ApprovalProgress_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvDetail.ReadOnlyGridView();
            gvDetail.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gcData.DataSource = sourceForm;
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            int handle = gvData.FocusedRowHandle;
            if (gvData.GetMasterRowExpanded(handle))
            {
                gvData.CollapseMasterRow(handle, 0);
            }
            else
            {
                gvData.ExpandMasterRow(handle, 0);
            }
        }

        private void gvDetail_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f201_SignProg_Detail fDetail = new f201_SignProg_Detail();
            fDetail.idBase = idForm;
            fDetail.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
