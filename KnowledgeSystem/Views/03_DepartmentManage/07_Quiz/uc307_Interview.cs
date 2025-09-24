using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Design.ParameterEditor;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class uc307_Interview : DevExpress.XtraEditors.XtraUserControl
    {
        public uc307_Interview()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            //barCbbDept.EditValueChanged += CbbDept_EditValueChanged;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        string deptGetData = "";

        List<dm_User> users = new List<dm_User>();
        List<dm_Departments> depts;

        List<dt307_InterviewReport> reports;

        DXMenuItem itemViewInfo;
        DXMenuItem itemExportExcel;
        DXMenuItem itemCopyLink;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemExportExcel = CreateMenuItem("導出評核表", ItemExportExcel_Click, TPSvgimages.Excel);
            itemCopyLink = CreateMenuItem("複製評核路徑", ItemCopyLinkl_Click, TPSvgimages.Copy);
        }

        private void ItemCopyLinkl_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string idReport = view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString();
            string link = $"{TPConfigs.WebLink307.TrimEnd('/')}/{idReport}";

            Clipboard.SetText(link);
            XtraMessageBox.Show($"評核路徑已復製\r\n{link}");
        }

        private void ItemExportExcel_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string idReport = view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString();

            var reportInfos = dt307_InterviewScoreBUS.Instance.GetListByReportId(idReport);

            var excelDatas = (from data in reportInfos
                              join viewer in users on data.InterviewerId equals viewer.Id into viewerJoin
                              from viewer in viewerJoin.DefaultIfEmpty()
                              join viewee in users on data.IntervieweeId equals viewee.Id into vieweeJoin
                              from viewee in vieweeJoin.DefaultIfEmpty()
                              let vieweeName = viewee != null ? $"{viewee.Id} {viewee.DisplayName}" : data.IntervieweeId
                              let viewerName = viewer != null ? $"{viewer.Id} {viewer.DisplayName}" : data.InterviewerId
                              orderby viewee?.IdDepartment, vieweeName
                              select new
                              {
                                  部門 = viewee?.IdDepartment ?? "",
                                  受訪人 = vieweeName,
                                  委員名稱 = viewerName,
                                  ProfessionalSkill = data.ProfessionalSkill,
                                  ProfessionalSkillNote = data.ProfessionalSkillNote,
                                  Responsiveness = data.Responsiveness,
                                  ResponsivenessNote = data.ResponsivenessNote,
                                  Communication = data.Communication,
                                  CommunicationNote = data.CommunicationNote,
                                  ReportQuality = data.ReportQuality,
                                  ReportQualityNote = data.ReportQualityNote,
                                  Total = data.Total,
                              }).ToList();

            // Tính trung bình theo từng 受訪人 (vieweeName)
            var excelWithAvg = (from d in excelDatas
                                group d by d.受訪人 into g
                                from item in g
                                let avgScore = Math.Round((double)g.Average(x => x.Total), 1)
                                let pass = avgScore >= 80
                                select new
                                {
                                    item.部門,
                                    item.受訪人,
                                    item.委員名稱,
                                    item.ProfessionalSkill,
                                    item.ProfessionalSkillNote,
                                    item.Responsiveness,
                                    item.ResponsivenessNote,
                                    item.Communication,
                                    item.CommunicationNote,
                                    item.ReportQuality,
                                    item.ReportQualityNote,
                                    item.Total,
                                    avgScore = avgScore,
                                    pass = pass ? "合格" : "不合格"
                                }).ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"口試報告-{idReport} - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                ws.Cells["A1"].Value = "部門";
                ws.Cells["B1"].Value = "人員名稱";
                ws.Cells["C1"].Value = "委員名稱";
                ws.Cells["D1"].Value = "專業能力(40%)";
                ws.Cells["F1"].Value = "臨場及應對能力(30%)";
                ws.Cells["H1"].Value = "表達能力(20%)";
                ws.Cells["J1"].Value = "報告文書品質(10%)";
                ws.Cells["L1"].Value = "合計(100%)";
                ws.Cells["M1"].Value = "平均分數";
                ws.Cells["N1"].Value = "結果";

                ws.Cells["D2"].Value = "得分";
                ws.Cells["E2"].Value = "異常說明";
                ws.Cells["F2"].Value = "得分";
                ws.Cells["G2"].Value = "異常說明";
                ws.Cells["H2"].Value = "得分";
                ws.Cells["I2"].Value = "異常說明";
                ws.Cells["J2"].Value = "得分";
                ws.Cells["K2"].Value = "異常說明";

                ws.Cells["A1:A2"].Merge = true;
                ws.Cells["B1:B2"].Merge = true;
                ws.Cells["C1:C2"].Merge = true;
                ws.Cells["D1:E1"].Merge = true;
                ws.Cells["F1:G1"].Merge = true;
                ws.Cells["H1:I1"].Merge = true;
                ws.Cells["J1:K1"].Merge = true;
                ws.Cells["L1:L2"].Merge = true;
                ws.Cells["M1:M2"].Merge = true;
                ws.Cells["N1:N2"].Merge = true;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A3"].LoadFromCollection(excelWithAvg, false);

                int startRow = 3, endRow = excelDatas.Count + 2;
                List<int> cols = new List<int>() { 1, 2, 13 };

                for (int colIndex = 0; colIndex < cols.Count; colIndex++)
                {
                    int col = cols[colIndex];
                    int mergeStart = startRow;

                    for (int row = startRow + 1; row <= endRow + 1; row++)
                    {
                        var curr = ws.Cells[row, col].Value?.ToString();
                        var prev = ws.Cells[mergeStart, col].Value?.ToString();

                        if (curr != prev || row == endRow + 1)
                        {
                            if (row - 1 > mergeStart)
                            {
                                ws.Cells[mergeStart, col, row - 1, col].Merge = true;

                                if (col == 13)
                                    ws.Cells[mergeStart, col + 1, row - 1, col + 1].Merge = true;
                            }

                            mergeStart = row;
                        }
                    }
                }


                //ws.Column(1).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                // Căn giữa và kẻ ô toàn bảng
                var fullRange = ws.Cells[ws.Dimension.Address];
                fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullRange.Style.Border.Top.Style = fullRange.Style.Border.Bottom.Style =
                    fullRange.Style.Border.Left.Style = fullRange.Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string idReport = view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString();
            f307_Interview_Info fInfo = new f307_Interview_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "報告",
                idBase = idReport,
                idDeptGetData = deptGetData
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                users = dm_UserBUS.Instance.GetList();

                reports = dt307_InterviewReportBUS.Instance.GetList();

                sourceBases.DataSource = reports;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void uc307_Interview_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            //// Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            //var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            //var departments = dm_DeptBUS.Instance.GetList();
            //var groups = dm_GroupBUS.Instance.GetListByName("機邊庫");

            //var accessibleGroups = groups
            //    .Where(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id))
            //    .ToList();

            //var departmentItems = departments
            //    .Where(dept => accessibleGroups.Any(group => group.IdDept == dept.Id))
            //    .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
            //    .ToArray();

            //cbbDept.Items.AddRange(departmentItems);
            //barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_Interview_Info finfo = new f307_Interview_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "報告",
                idDeptGetData = deptGetData
            };

            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemCopyLink);
                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemExportExcel);
            }
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "INFO";
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                GridView view = sender as GridView;
                dt307_InterviewReport report = view.GetRow(e.RowHandle) as dt307_InterviewReport;
                string idReport = report.Id;

                if (report != null)
                {
                    List<string> interviewees_id = JsonConvert.DeserializeObject<List<string>>(report.Interviewees);
                    //var interviewers = users.Where(r => interviewers_id.Any(x => x == r.Id)).ToList();

                    var reportInfos = dt307_InterviewScoreBUS.Instance.GetListByReportId(idReport);

                    // Lấy list InterviewerId đã có trong reportInfos
                    var existIds = reportInfos.Select(r => r.IntervieweeId).ToList();

                    // Tạo list bổ sung cho những Interviewer chưa có
                    var missing = interviewees_id
                        .Where(id => !existIds.Contains(id))
                        .Select(id => new dt307_InterviewScore
                        {
                            Id = 0,                        // giá trị mặc định
                            ReportId = idReport,
                            InterviewerId = "---",         // theo yêu cầu
                            IntervieweeId = id,            // id người bị phỏng vấn
                            ProfessionalSkill = 0,
                            ProfessionalSkillNote = null,
                            Responsiveness = 0,
                            ResponsivenessNote = null,
                            Communication = 0,
                            CommunicationNote = null,
                            ReportQuality = 0,
                            ReportQualityNote = null,
                            Total = null
                        }).ToList();

                    // Gộp cả 2 list
                    var datas = reportInfos.Concat(missing).ToList();

                    var displayDatas = (from data in datas
                                        join viewer in users on data.InterviewerId equals viewer.Id into viewerJoin
                                        from viewer in viewerJoin.DefaultIfEmpty()
                                        join viewee in users on data.IntervieweeId equals viewee.Id into vieweeJoin
                                        from viewee in vieweeJoin.DefaultIfEmpty()
                                        select new
                                        {
                                            data,
                                            ViewerName = viewer != null ? viewer.DisplayName : "---",
                                            VieweeName = viewee != null ? viewee.DisplayName : "---"
                                        }).ToList();

                    e.ChildList = displayDatas;
                }
            }
        }

        private (decimal avgScore, bool isPass, int count) GetRowCounts(GridView view, int rowHandle)
        {
            decimal totalScore = 0;
            int count = 0;

            int childrenCount = view.GetChildRowCount(rowHandle);
            for (int i = 0; i < childrenCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(rowHandle, i);

                if (view.IsGroupRow(childRowHandle))
                {
                    // Đệ quy xuống group con
                    var (childAvg, childPass, childCount) = GetRowCounts(view, childRowHandle);

                    // Ghép kết quả vào tổng
                    // childAvg là trung bình, muốn cộng dồn thì phải nhân lại với số lượng dòng
                    // nên ta lấy tất cả leaf rows trong group con
                    var leafCount = view.GetChildRowCount(childRowHandle);
                    totalScore += childAvg * leafCount;
                    count += leafCount;
                }
                else
                {
                    object cellValue = view.GetRowCellValue(childRowHandle, "data.Total");
                    if (cellValue != null && decimal.TryParse(cellValue.ToString(), out decimal val))
                    {
                        totalScore += val;
                        count++;
                    }
                }
            }

            decimal avg = count > 0 ? Math.Round(totalScore / count, 1) : 0;
            bool pass = avg >= 80;

            return (avg, pass, count);
        }

        private void gvInfo_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            var view = (GridView)sender;
            var info = (GridGroupRowInfo)e.Info;
            var caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
            {
                caption = info.Column.ToString();
            }

            var groupInfo = info.RowKey as GroupRowInfo;

            var (total, pass, count) = GetRowCounts(view, e.RowHandle);
            string colorName = pass ? "Green" : "Red";
            string groupValue = pass ? "合格" : "不合格";

            info.GroupText = $" <color={colorName}>{groupValue}</color>：{info.GroupValueText}《<color=Blue>{total}分</color>-<color=Red>{count}位委員</color>》";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
