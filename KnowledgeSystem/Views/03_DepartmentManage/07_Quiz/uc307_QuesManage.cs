using BusinessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class uc307_QuesManage : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_QuesManage()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvQues, "Id");

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        RefreshHelper helper;
        BindingSource sourceQues = new BindingSource();

        string idJobSelect = "";

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e, Color color)
        {
            e.Info.Appearance.Font = fontUI14;
            e.Info.Appearance.ForeColor = color;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            //itemAddNode = CreateMenuItem("新增表單", ItemAddNote_Click, TPSvgimages.Add);
            //itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            //itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            //itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            //itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            //itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var baseData = dt307_JobQuesManageBUS.Instance.GetList();
                var jobs = dm_JobTitleBUS.Instance.GetList();
                var depts = dm_DeptBUS.Instance.GetList();

                var dataDisplays = (from data in baseData
                                    join job in jobs on data.JobId equals job.Id
                                    join dept in depts on data.IdDept equals dept.Id
                                    select new
                                    {
                                        Id = data.JobId,
                                        DisplayName = job.DisplayName,
                                        Dept = $"{dept.Id}\n{dept.DisplayName}",
                                    }).ToList();

                txbJob.DataSource = dataDisplays;
                txbJob.DisplayMember = "DisplayName";
                txbJob.ValueMember = "Id";

                gcData.DataSource = sourceQues;
                gvQues.BestFitColumns();
                gvQues.CollapseAllDetails();
            }
        }

        private void LoadQues(string idJob)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var ques = dt307_QuestionsBUS.Instance.GetListByJob(idJob);

                sourceQues.DataSource = ques;
                helper.LoadViewInfo();

                gvQues.BestFitColumns();
                gvQues.CollapseAllDetails();
            }
        }

        private void uc307_QuesManage_Load(object sender, EventArgs e)
        {
            gvQues.ReadOnlyGridView();
            gvQues.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvQues.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvQues.OptionsDetail.AllowZoomDetail = false;

            gvAns.ReadOnlyGridView();
            gvAns.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvAns.OptionsDetail.AllowZoomDetail = false;

            LoadData();
        }

        private void txbJob_EditValueChanged(object sender, EventArgs e)
        {
            SearchLookUpEdit editor = (SearchLookUpEdit)sender;
            idJobSelect = editor.EditValue.ToString();

            LoadQues(idJobSelect);
        }

        private void gvQues_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvQues_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idQues = (int)view.GetRowCellValue(e.RowHandle, gColId);

            var answers = dt307_AnswersBUS.Instance.GetListByQues(idQues);

            //var baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            //var childList = (from data in baseAtts
            //                 join urs in users on data.UsrCancel equals urs.Id into userGroup
            //                 from urs in userGroup.DefaultIfEmpty()
            //                 select new
            //                 {
            //                     data,
            //                     DisplayName = urs != null ? $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}" : null,
            //                     Status = data.IsCancel || isCancel ? "被退回" : isProgess ? "核簽中" : "核簽完畢"
            //                 }).ToList();

            e.ChildList = answers;
        }

        private void gvQues_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvQues_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "答案";
        }

        private void gvQues_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;

            Color color = view.Appearance.HeaderPanel.ForeColor;

            if (!view.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }
                    IndicatorDraw(e, color);
                    SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, fontUI14);
                    Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                    BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = string.Format("[{0}]", (e.RowHandle * -1));
                IndicatorDraw(e, color);
                SizeF _Size = e.Graphics.MeasureString(e.Info.DisplayText, fontUI14);
                Int32 _Width = Convert.ToInt32(_Size.Width) + 20;
                BeginInvoke(new MethodInvoker(delegate { cal(_Width, view); }));
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadQues(idJobSelect);
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "HaveImg" && e.IsGetData)
            {
                string imgName = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "ImageName")?.ToString();

                e.Value = !string.IsNullOrWhiteSpace(imgName);
            }
        }

        private void btnUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string dataPath = "";
            DataSet ds;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                dataPath = openFileDialog.FileName;
            }

            string extension = Path.GetExtension(dataPath);
            using (var stream = File.Open(dataPath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close();
            }

            DataTable dt = ds.Tables["EXCEL"];

            f307_UploadQues fUpload = new f307_UploadQues();
            fUpload.dtBase = dt;
            fUpload.pathImages = $@"{Path.GetDirectoryName(dataPath)}\Images";
            fUpload.ShowDialog();

            // DataTable questionsTable = new DataTable();
            //questionsTable.Columns.Add("Id", typeof(int));
            //questionsTable.Columns.Add("IdJob", typeof(string));
            //questionsTable.Columns.Add("DisplayText", typeof(string));
            //questionsTable.Columns.Add("ImageName", typeof(string));
            //questionsTable.Columns.Add("IsMultiAns", typeof(bool));

            //DataTable answersTable = new DataTable();
            //answersTable.Columns.Add("Id", typeof(int));
            //answersTable.Columns.Add("QuesId", typeof(int));
            //answersTable.Columns.Add("DisplayText", typeof(string));
            //answersTable.Columns.Add("ImageName", typeof(string));
            //answersTable.Columns.Add("TrueAns", typeof(bool));

            //int currentQuesId = -1;

            //for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            //{
            //    int quesId = worksheet.Cells[row, 1].GetValue<int>();
            //    if (quesId != currentQuesId)
            //    {
            //        questionsTable.Rows.Add(
            //            quesId,
            //            worksheet.Cells[row, 2].GetValue<string>(),
            //            worksheet.Cells[row, 3].GetValue<string>(),
            //            worksheet.Cells[row, 4].GetValue<string>(),
            //            worksheet.Cells[row, 5].GetValue<bool>()
            //        );
            //        currentQuesId = quesId;
            //    }

            //    answersTable.Rows.Add(
            //        worksheet.Cells[row, 6].GetValue<int>(),
            //        quesId,
            //        worksheet.Cells[row, 7].GetValue<string>(),
            //        worksheet.Cells[row, 8].GetValue<string>(),
            //        worksheet.Cells[row, 9].GetValue<bool>()
            //    );
            //}
        }

        private void gvQues_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            view.ExpandMasterRow(view.FocusedRowHandle, 0);
        }
    }
}
