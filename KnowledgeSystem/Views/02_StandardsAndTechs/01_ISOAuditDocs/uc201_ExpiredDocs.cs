﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_ExpiredDocs : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_ExpiredDocs()
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
            users = dm_UserBUS.Instance.GetList();

            var baseRecords = dt201_BaseBUS.Instance.GetList();

            // Step 1: Identify records with IsFinalNode = true and get the max Id for each IdParent
            var maxIdForFinalNodes = baseRecords
                .Where(r => r.IsFinalNode == true)
                .GroupBy(r => r.IdParent)
                .Select(g => g.OrderByDescending(r => r.Id).First().Id)
                .ToHashSet();

            // Step 2: Get all records excluding those with IsFinalNode = true and a lower Id for the same IdParent
            var resultBase = baseRecords
                .Where(r => r.IsFinalNode != true || maxIdForFinalNodes.Contains(r.Id))
                .ToList();

            // Step 3: Lấy danh sach bản năm
            var Yearlys = resultBase.Where(r => r.IsFinalNode == true).ToList();

            // Step 4: Lấy tất cả các form theo baseIds là Bản năm mới nhất
            var formRecords = dt201_FormsBUS.Instance.GetListByBaseIds(Yearlys.Select(x => x.Id).ToList());

            // Step 5: Lấy danh sách biểu đơn mới nhất
            var resultForm = formRecords.Where(r => r.IsCancel != true).GroupBy(r => r.IdBase).Select(g => g.OrderByDescending(r => r.Id).First()).ToList();

            // Step 6: Lấy ra chu kỳ của từng văn kiện để xác định có thông báo không
            var cycles = (from id in Yearlys
                          join basedata in resultBase on id.IdParent equals basedata.Id
                          select new
                          {
                              Id = id.Id,
                              NotifyCycle = basedata.NotifyCycle
                          }).ToList();

            var resultNotifys = (from data in resultForm
                                 join cycle in cycles on data.IdBase equals cycle.Id
                                 where cycle.NotifyCycle.HasValue &&
                                       data.UploadTime.AddMonths(cycle.NotifyCycle.Value - 1) <= DateTime.Today &&
                                       data.IsProcessing != true
                                 select data).ToList();

            // Step 7: Lấy ra cha của biểu đơn đó để người đưa lên biết vị trí nằm ở đâu
            var dataInfo = (from data in resultNotifys
                            join usr in users on data.UploadUser equals usr.Id
                            join category in baseRecords on data.IdBase equals category.Id
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
                                category = baseRecords.FirstOrDefault(r => r.Id == dtg.Key),
                                detailData = dtg.Select(r => new
                                {
                                    Year = dtg.First().category.DisplayName,
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

        private void uc201_ExpiredDocs_Load(object sender, EventArgs e)
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
            gvData.ExpandMasterRow(handle, 0);
        }

        private void gvDetail_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColBaseId));
            var currentData = dt201_BaseBUS.Instance.GetItemById(idBase);

            int year = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColYear));
            if (year < DateTime.Today.Year)
            {
                if (XtraMessageBox.Show("您正在更新文件至舊年版，系統將自動創建今年的版本。點擊「是」以繼續。如果您想更新至舊年版，請進入「稽核文件」手動更新。", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                var nextYear = new dt201_Base()
                {
                    DisplayName = DateTime.Today.Year.ToString(),
                    IdParent = currentData.IdParent,
                    DocCode = currentData.DocCode,
                    IdDept = currentData.IdDept,
                    IsFinalNode = currentData.IsFinalNode,
                    IsPaperType = currentData.IsPaperType,
                };

                idBase = dt201_BaseBUS.Instance.Add(nextYear);
            }

            currentData = dt201_BaseBUS.Instance.GetItemById(idBase);
            var parentData = dt201_BaseBUS.Instance.GetItemById(currentData.IdParent);

            f201_AddAttachment fAtt = new f201_AddAttachment()
            {
                eventInfo = EventFormInfo.Create,
                formName = "表單",
                currentData = currentData,
                parentData = parentData
            };
            fAtt.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}