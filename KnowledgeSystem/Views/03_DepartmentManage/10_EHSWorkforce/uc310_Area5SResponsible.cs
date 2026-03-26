using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList.Nodes;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_Area5SResponsible : DevExpress.XtraEditors.XtraUserControl
    {
        private const float PdfPageMargin = 28f;
        private const float PdfImageSectionMaxHeight = 340f;
        private const float PdfSectionSpacing = 14f;

        public uc310_Area5SResponsible()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        BindingSource source5sArea = new BindingSource();
        BindingSource sourceResp = new BindingSource();

        DXMenuItem itemViewInfo;

        List<dt310_Area5S> area5S;
        List<dt310_Area5SResponsible> areaResps;

        List<dm_Departments> depts;
        List<dt310_Role> roles;
        List<dm_User> users;

        List<dm_GroupUser> userGroups;
        bool isEHSAdmin = false;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSummaryTable.ImageOptions.SvgImage = TPSvgimages.Print;
            btnSummaryTable.Caption = "導出PDF報告";
            btnSummaryTable.Visibility = BarItemVisibility.Always;
            btnSummaryTable.ItemClick -= btnSummaryTable_ItemClick;
            btnSummaryTable.ItemClick += btnSummaryTable_ItemClick;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看信息", ItemViewInfo_Click, TPSvgimages.View);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idArea = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "區域",
                idBase = idArea,
                isAddInfo = false
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info() { isAddInfo = true, isEditorInfo = false };
            fInfo.ShowDialog(this);
            LoadData();
        }

        private void LoadData()
        {
            area5S = dt310_Area5SBUS.Instance.GetList();
            areaResps = dt310_Area5SResponsibleBUS.Instance.GetList();
            users = dm_UserBUS.Instance.GetList();

            source5sArea.DataSource = area5S;

            var joinData = from resp in areaResps
                           join area in area5S on resp.AreaId equals area.Id
                           where resp.DeletedAt == null && area.DeletedAt == null
                           let emp = users.FirstOrDefault(r => r.Id == resp.EmployeeId)
                           let agent = users.FirstOrDefault(r => r.Id == resp.AgentId)
                           let boss = users.FirstOrDefault(r => r.Id == resp.BossId)
                           select new
                           {
                               EmployeeId = resp.EmployeeId,
                               EmpName = emp != null ? $"LG{emp.IdDepartment}/{emp.DisplayName}" : $"Unknown-{resp.EmployeeId}",
                               AgentName = agent != null ? $"LG{agent.IdDepartment}/{agent.DisplayName}" : $"Unknown-{resp.AgentId}",
                               BossName = boss != null ? $"LG{boss.IdDepartment}/{boss.DisplayName}" : $"Unknown-{resp.BossId}",
                               AreaId = area.Id,
                               AreaName = area.DisplayName,
                               AreaDesc = area.DESC,
                               RespAreaCode = resp.AreaCode,
                               RespAreaName = resp.AreaName,
                           };

            // Root node: EmployeeId
            var rootNodes = joinData
                .GroupBy(x => x.EmpName)
                .Select(x => new
                {
                    Id = x.Key,
                    IdParent = (string)null,
                    Name = x.Key,
                    EmpName = (string)null,
                    AgentName = (string)null,
                    BossName = (string)null,
                    RespAreaCode = (string)null,
                    RespAreaName = (string)null
                });

            // Child node: Areas
            var childNodes = joinData
                .Select(x => new
                {
                    Id = $"{x.EmployeeId}_{x.AreaId}_{Guid.NewGuid().ToString("N").Substring(5)}",
                    IdParent = x.EmpName,
                    Name = x.AreaName,      // tên hiển thị node con
                    EmpName = x.EmpName,
                    AgentName = x.AgentName,
                    BossName = x.BossName,
                    RespAreaCode = x.RespAreaCode,
                    RespAreaName = x.RespAreaName
                });

            // Merge
            var treeListSource = rootNodes
                .Union(childNodes)
                .ToList();

            // Gán TreeList
            sourceResp.DataSource = treeListSource;

            treeResps.BestFitColumns();
            gvData.BestFitColumns();
        }

        private void uc310_Area5SResponsible_Load(object sender, EventArgs e)
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            int groupId = dm_GroupBUS.Instance.GetItemByName($"安衛環7")?.Id ?? -1;
            isEHSAdmin = userGroups.Any(r => r.IdGroup == groupId);

            if (!isEHSAdmin)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            LoadData();

            treeResps.DataSource = sourceResp;
            treeResps.KeyFieldName = "Id";
            treeResps.ParentFieldName = "IdParent";
            treeResps.BestFitColumns();
            treeResps.ReadOnlyTreelist();
            treeResps.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;

            TreeListNode node = treeResps.GetNodeByVisibleIndex(0);
            if (node != null)
                node.Expanded = !node.Expanded;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = source5sArea;
            gvData.BestFitColumns();
            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;

            gvResponsibleEmp.ReadOnlyGridView();
            gvResponsibleEmp.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvResponsibleEmp.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idArea = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));
            e.IsEmpty = !areaResps.Any(r => r.AreaId == idArea);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idArea = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, gColId));
            e.ChildList = areaResps
                .Where(r => r.AreaId == idArea)
                .Select(r =>
                {
                    var emp = users.FirstOrDefault(u => u.Id == r.EmployeeId);
                    var agent = users.FirstOrDefault(u => u.Id == r.AgentId);
                    var boss = users.FirstOrDefault(u => u.Id == r.BossId);
                    return new
                    {
                        r.AreaCode,
                        r.AreaName,
                        r.DeptId,
                        EmpName = emp != null ? $"LG{emp.IdDepartment}/{emp.DisplayName}" : $"Unknown-{r.EmployeeId}",
                        AgentName = agent != null ? $"LG{agent.IdDepartment}/{agent.DisplayName}" : $"Unknown-{r.AgentId}",
                        BossName = boss != null ? $"LG{boss.IdDepartment}/{boss.DisplayName}" : $"Unknown-{r.BossId}"
                    };
                }).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Responsible";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnSummaryTable_ItemClick(object sender, ItemClickEventArgs e)
        {
            dt310_Area5S selectedArea = GetFocusedArea();
            if (selectedArea == null)
            {
                XtraMessageBox.Show("請先選擇一筆區域資料後再導出 PDF。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.FileName = BuildAreaReportFileName(selectedArea);
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        ExportAreaReportPdf(selectedArea, saveFileDialog.FileName);
                    }

                    XtraMessageBox.Show($"已導出 PDF 報告：\n{saveFileDialog.FileName}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"導出 PDF 失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private dt310_Area5S GetFocusedArea()
        {
            int rowHandle = gvData.FocusedRowHandle;
            if (rowHandle < 0 && gvData.RowCount > 0)
            {
                rowHandle = 0;
            }

            return rowHandle >= 0 ? gvData.GetRow(rowHandle) as dt310_Area5S : null;
        }

        private string BuildAreaReportFileName(dt310_Area5S area)
        {
            string areaName = string.IsNullOrWhiteSpace(area?.DisplayName) ? "Area5S" : area.DisplayName.Trim();
            string safeAreaName = SanitizeFileName(areaName);
            return $"[310] {safeAreaName} Area Responsibility Report {DateTime.Now:yyyyMMdd_HHmm}.pdf";
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }

        private void ExportAreaReportPdf(dt310_Area5S area, string outputPath)
        {
            List<object[]> reportRows = BuildAreaReportRows(area.Id);

            using (PdfDocument document = new PdfDocument())
            using (Font titleFontGdi = new Font("Microsoft JhengHei UI", 16f, FontStyle.Bold))
            using (Font sectionFontGdi = new Font("Microsoft JhengHei UI", 11f, FontStyle.Bold))
            using (Font contentFontGdi = new Font("Microsoft JhengHei UI", 9.5f, FontStyle.Regular))
            using (Font smallFontGdi = new Font("Microsoft JhengHei UI", 8.5f, FontStyle.Regular))
            using (PdfTrueTypeFont titleFont = new PdfTrueTypeFont(titleFontGdi, true))
            using (PdfTrueTypeFont sectionFont = new PdfTrueTypeFont(sectionFontGdi, true))
            using (PdfTrueTypeFont contentFont = new PdfTrueTypeFont(contentFontGdi, true))
            using (PdfTrueTypeFont smallFont = new PdfTrueTypeFont(smallFontGdi, true))
            {
                document.PageSettings.Size = PdfPageSize.A4;
                document.DocumentInformation.Title = $"{area.DisplayName} 區域責任報告";
                document.DocumentInformation.Creator = TPConfigs.SoftNameEN ?? "KnowledgeSystem";
                document.DocumentInformation.Author = TPConfigs.LoginUser?.DisplayName ?? TPConfigs.LoginUser?.Id ?? "KnowledgeSystem";

                PdfNewPage page = (PdfNewPage)document.Pages.Add();
                float pageWidth = page.Canvas.ClientSize.Width;
                float pageHeight = page.Canvas.ClientSize.Height;
                float contentWidth = pageWidth - (PdfPageMargin * 2);
                float y = PdfPageMargin;

                y = DrawHeader(page, area, titleFont, contentFont, pageWidth, y);
                y = DrawAreaImageSection(page, area, sectionFont, smallFont, contentWidth, pageHeight, y);
                y += PdfSectionSpacing;
                DrawTableSection(page, reportRows, sectionFont, contentFont, contentWidth, pageHeight, y);

                document.SaveToFile(outputPath);
            }
        }

        private float DrawHeader(PdfNewPage page, dt310_Area5S area, PdfTrueTypeFont titleFont, PdfTrueTypeFont contentFont, float pageWidth, float y)
        {
            float contentWidth = pageWidth - (PdfPageMargin * 2);

            page.Canvas.DrawString($"{area.DisplayName} 區域責任報告", titleFont, PdfBrushes.Black, new RectangleF(PdfPageMargin, y, contentWidth, 24f), CreateStringFormat(PdfTextAlignment.Center));
            y += 28f;

            string desc = string.IsNullOrWhiteSpace(area.DESC) ? "無" : area.DESC.Trim();
            string meta = $"區域：{area.DisplayName}    備註：{desc}";
            page.Canvas.DrawString(meta, contentFont, PdfBrushes.DimGray, new RectangleF(PdfPageMargin, y, contentWidth, 18f), CreateStringFormat(PdfTextAlignment.Left));
            y += 24f;

            page.Canvas.DrawLine(PdfPens.DarkGray, PdfPageMargin, y, pageWidth - PdfPageMargin, y);
            return y + 10f;
        }

        private float DrawAreaImageSection(PdfNewPage page, dt310_Area5S area, PdfTrueTypeFont sectionFont, PdfTrueTypeFont contentFont, float contentWidth, float pageHeight, float y)
        {
            page.Canvas.DrawString("區域圖片", sectionFont, PdfBrushes.Black, PdfPageMargin, y);
            y += 24f;

            float availableHeight = Math.Min(PdfImageSectionMaxHeight, Math.Max(200f, (pageHeight * 0.46f) - y));
            RectangleF imageBounds = new RectangleF(PdfPageMargin, y, contentWidth, availableHeight);

            using (Image areaImage = LoadAreaImage(area))
            {
                if (areaImage == null)
                {
                    page.Canvas.DrawRectangle(PdfPens.Gray, imageBounds);
                    page.Canvas.DrawString("未找到區域圖片", contentFont, PdfBrushes.DarkGray, imageBounds, CreateCenteredStringFormat());
                    return imageBounds.Bottom;
                }

                PdfImage pdfImage = PdfImage.FromImage(areaImage);
                float scale = Math.Min(imageBounds.Width / areaImage.Width, imageBounds.Height / areaImage.Height);
                float drawWidth = areaImage.Width * scale;
                float drawHeight = areaImage.Height * scale;
                float drawX = imageBounds.X + ((imageBounds.Width - drawWidth) / 2f);
                float drawY = imageBounds.Y + ((imageBounds.Height - drawHeight) / 2f);

                page.Canvas.DrawRectangle(PdfPens.LightGray, imageBounds);
                page.Canvas.DrawImage(pdfImage, drawX, drawY, drawWidth, drawHeight);

                return imageBounds.Bottom;
            }
        }

        private void DrawTableSection(PdfNewPage page, List<object[]> reportRows, PdfTrueTypeFont sectionFont, PdfTrueTypeFont contentFont, float contentWidth, float pageHeight, float y)
        {
            page.Canvas.DrawString("責任區劃分", sectionFont, PdfBrushes.Black, PdfPageMargin, y);
            y += 24f;

            PdfGrid grid = BuildResponsibilityGrid(reportRows, contentFont, contentWidth);
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat
            {
                Break = PdfLayoutBreakType.FitPage,
                Layout = PdfLayoutType.Paginate
            };

            float availableHeight = Math.Max(80f, pageHeight - y - PdfPageMargin);
            grid.Draw(page, new RectangleF(PdfPageMargin, y, contentWidth, availableHeight), layoutFormat);
        }

        private PdfGrid BuildResponsibilityGrid(List<object[]> reportRows, PdfTrueTypeFont contentFont, float contentWidth)
        {
            PdfGrid grid = new PdfGrid
            {
                RepeatHeader = true,
                AllowCrossPages = true
            };

            grid.Style.CellPadding = new PdfPaddings(4, 4, 4, 4);
            grid.Style.CellSpacing = 0.5f;
            grid.Style.Font = contentFont;

            grid.Columns.Add(6);
            float[] widths = { 0.11f, 0.13f, 0.14f, 0.21f, 0.21f, 0.20f };
            for (int i = 0; i < widths.Length; i++)
            {
                grid.Columns[i].Width = contentWidth * widths[i];
            }

            string[] headers = { "編號", "區域", "負責部門", "責任人員", "代理人", "督導主管" };
            PdfGridRow headerRow = grid.Headers.Add(1)[0];
            headerRow.Height = 24f;
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Cells[i].Value = headers[i];
                headerRow.Cells[i].Style = CreateGridCellStyle(contentFont, PdfBrushes.LightGray, PdfTextAlignment.Center, true);
            }

            if (reportRows.Count == 0)
            {
                PdfGridRow emptyRow = grid.Rows.Add();
                emptyRow.Cells[0].Value = "-";
                emptyRow.Cells[1].Value = "無責任區資料";
                emptyRow.Cells[2].Value = "";
                emptyRow.Cells[3].Value = "";
                emptyRow.Cells[4].Value = "";
                emptyRow.Cells[5].Value = "";

                for (int i = 0; i < 6; i++)
                {
                    emptyRow.Cells[i].Style = CreateGridCellStyle(contentFont, PdfBrushes.White, i == 0 ? PdfTextAlignment.Center : PdfTextAlignment.Left);
                }

                return grid;
            }

            foreach (object[] rowData in reportRows)
            {
                PdfGridRow row = grid.Rows.Add();
                for (int i = 0; i < rowData.Length; i++)
                {
                    row.Cells[i].Value = rowData[i] ?? "";
                    row.Cells[i].Style = CreateGridCellStyle(contentFont, PdfBrushes.White, i <= 2 ? PdfTextAlignment.Center : PdfTextAlignment.Left);
                }
            }

            return grid;
        }

        private PdfGridCellStyle CreateGridCellStyle(PdfTrueTypeFont font, PdfBrush backgroundBrush, PdfTextAlignment alignment, bool isHeader = false)
        {
            PdfGridCellStyle style = new PdfGridCellStyle
            {
                Font = font,
                BackgroundBrush = backgroundBrush,
                TextBrush = PdfBrushes.Black,
                Borders = new PdfBorders { All = PdfPens.Gray },
                StringFormat = new PdfStringFormat
                {
                    Alignment = alignment,
                    LineAlignment = PdfVerticalAlignment.Middle
                }
            };

            if (isHeader)
            {
                style.TextBrush = PdfBrushes.Black;
            }

            return style;
        }

        private static PdfStringFormat CreateStringFormat(PdfTextAlignment alignment)
        {
            return new PdfStringFormat
            {
                Alignment = alignment,
                LineAlignment = PdfVerticalAlignment.Middle
            };
        }

        private static PdfStringFormat CreateCenteredStringFormat()
        {
            return CreateStringFormat(PdfTextAlignment.Center);
        }

        private List<object[]> BuildAreaReportRows(int areaId)
        {
            return areaResps
                .Where(r => r.AreaId == areaId && r.DeletedAt == null)
                .OrderBy(r => r.AreaCode)
                .ThenBy(r => r.AreaName)
                .Select(r => new object[]
                {
                    r.AreaCode ?? "",
                    r.AreaName ?? "",
                    r.DeptId ?? "",
                    FormatUserInfo(r.EmployeeId),
                    FormatUserInfo(r.AgentId),
                    FormatUserInfo(r.BossId)
                })
                .ToList();
        }

        private string FormatUserInfo(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "";
            }

            dm_User user = users?.FirstOrDefault(u => u.Id == userId);
            return user != null
                ? $"LG{user.IdDepartment}/{user.DisplayName}"
                : userId;
        }

        private Image LoadAreaImage(dt310_Area5S area)
        {
            if (area == null || string.IsNullOrWhiteSpace(area.FileName))
            {
                return null;
            }

            string imagePath = Path.Combine(TPConfigs.Folder310, area.FileName);
            if (!File.Exists(imagePath))
            {
                return null;
            }

            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Image image = Image.FromStream(fileStream))
            {
                return new Bitmap(image);
            }
        }
    }
}
