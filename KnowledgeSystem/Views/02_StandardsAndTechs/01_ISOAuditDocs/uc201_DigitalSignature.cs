using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_DigitalSignature : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_DigitalSignature()
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
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            List<dt201_Base> baseData = dt201_BaseBUS.Instance.GetList();


            baseForm = dt201_FormsBUS.Instance.GetListProcessing();
            users = dm_UserBUS.Instance.GetList();

            var dataInfo = (from data in baseForm
                            join usr in users on data.UploadUser equals usr.Id
                            join category in baseData on data.IdBase equals category.Id
                            select new
                            {
                                category,
                                data,
                                usr
                            } into dt
                            group dt by dt.category.IdParent into dtg
                            select new
                            {
                                Key = dtg.Key,
                                category = baseData.FirstOrDefault(r => r.Id == dtg.Key),
                                detailData = dtg.Select(r => new
                                {
                                    UsrUploadName = $"{r.usr.Id.Substring(5)} {r.usr.DisplayName}",
                                    data = r.data,
                                    usr = r.usr
                                }).ToList()
                            }).ToList();

            sourceForm.DataSource = dataInfo;
            gcData.LevelTree.Nodes.Add("detailData", gvDetail);

            gvData.BestFitColumns();

            if (gvData.RowCount > 0)
                gvData.ExpandMasterRow(0, 0);
        }

        private void uc201_DigitalSignature_Load(object sender, EventArgs e)
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

            f201_SignDoc_Info fInfo = new f201_SignDoc_Info();
            fInfo.idBase = idForm;
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
