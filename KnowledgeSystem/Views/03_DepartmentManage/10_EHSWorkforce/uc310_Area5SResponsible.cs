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
using Newtonsoft.Json;
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
using UpdateLeaveUserJson = KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce.f310_UpdateLeaveUser_Info.UpdateLeaveUserJson;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_Area5SResponsible : DevExpress.XtraEditors.XtraUserControl
    {
        private const string PdfCoverCompanyName = "台塑河靜鋼鐵公司";
        private const string PdfCoverTitleSuffix = "責任區域配置圖及責任人員";
        private const string PdfCjkFontName = "DFKai-SB";
        private const string PdfLatinFontName = "Times New Roman";
        private const float PdfPageMargin = 28f;
        private const float PdfImageSectionMaxHeight = 340f;
        private const float PdfImageSectionMinHeight = 120f;
        private const float PdfImagePadding = 8f;
        private const float PdfSectionSpacing = 14f;
        private const float PdfSectionTitleHeight = 24f;
        private const float PdfHeaderRowHeight = 24f;
        private const float PdfDataRowHeight = 22f;
        private const float PdfCoverTitleBoxHeight = 150f;
        private const float PdfCoverSignatureTopOffset = 205f;
        private const float PdfCoverSignatureLineTopOffset = 58f;
        private const float PdfCoverSignatureLinePadding = 16f;

        private sealed class PdfCoverSignatureInfo
        {
            public string Label { get; set; }
            public string UserId { get; set; }
            public string Name { get; set; }
            public string DateText { get; set; }
        }

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
            if (!isEHSAdmin)
            {
                return;
            }

            f310_Area5SResponsible_Info fInfo = new f310_Area5SResponsible_Info() { isAddInfo = true, isEditorInfo = false };
            fInfo.ShowDialog(this);
            LoadData();
        }

        private void LoadData()
        {
            area5S = dt310_Area5SBUS.Instance.GetList();
            areaResps = dt310_Area5SResponsibleBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetList();
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
            List<dt310_Area5S> reportAreas = (area5S ?? new List<dt310_Area5S>())
                .Where(r => r.DeletedAt == null)
                .OrderBy(r => r.DisplayName)
                .ToList();

            reportAreas = reportAreas
                .Where(r => BuildAreaReportRows(r.Id).Count > 0)
                .ToList();

            if (reportAreas.Count == 0)
            {
                XtraMessageBox.Show("目前登入部門沒有可導出的 5S 區域資料。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.FileName = BuildAreaReportFileName();
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        ExportAreaReportPdf(reportAreas, saveFileDialog.FileName);
                    }

                    XtraMessageBox.Show($"已導出 PDF 報告：\n{saveFileDialog.FileName}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"導出 PDF 失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string BuildAreaReportFileName()
        {
            return $"[310] 5S Area Responsibility Report {DateTime.Now:yyyyMMdd_HHmm}.pdf";
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }

        private void ExportAreaReportPdf(List<dt310_Area5S> areas, string outputPath)
        {
            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

            var exportAreas = areas
                .Select(area => new
                {
                    Area = area,
                    ReportRows = BuildAreaReportRows(area.Id)
                })
                .Where(item => item.ReportRows.Count > 0)
                .ToList();

            if (exportAreas.Count == 0)
            {
                throw new InvalidOperationException("目前登入部門沒有可導出的 5S 區域資料。");
            }

            using (PdfDocument document = new PdfDocument())
            using (Font coverCompanyFontGdi = new Font(PdfCjkFontName, 24f, FontStyle.Bold))
            using (Font coverTitleFontGdi = new Font(PdfCjkFontName, 23f, FontStyle.Regular))
            using (Font coverSignLabelFontGdi = new Font(PdfCjkFontName, 13f, FontStyle.Regular))
            using (Font coverSignHintFontGdi = new Font(PdfCjkFontName, 9.5f, FontStyle.Regular))
            using (Font coverSignDateFontGdi = new Font(PdfLatinFontName, 7.5f, FontStyle.Regular))
            using (Font titleFontGdi = new Font(PdfCjkFontName, 16f, FontStyle.Bold))
            using (Font sectionFontGdi = new Font(PdfCjkFontName, 11f, FontStyle.Bold))
            using (Font contentFontGdi = new Font(PdfCjkFontName, 9.5f, FontStyle.Regular))
            using (Font smallFontGdi = new Font(PdfCjkFontName, 8.5f, FontStyle.Regular))
            using (PdfTrueTypeFont coverCompanyFont = new PdfTrueTypeFont(coverCompanyFontGdi, true))
            using (PdfTrueTypeFont coverTitleFont = new PdfTrueTypeFont(coverTitleFontGdi, true))
            using (PdfTrueTypeFont coverSignLabelFont = new PdfTrueTypeFont(coverSignLabelFontGdi, true))
            using (PdfTrueTypeFont coverSignHintFont = new PdfTrueTypeFont(coverSignHintFontGdi, true))
            using (PdfTrueTypeFont coverSignDateFont = new PdfTrueTypeFont(coverSignDateFontGdi, true))
            using (PdfTrueTypeFont titleFont = new PdfTrueTypeFont(titleFontGdi, true))
            using (PdfTrueTypeFont sectionFont = new PdfTrueTypeFont(sectionFontGdi, true))
            using (PdfTrueTypeFont contentFont = new PdfTrueTypeFont(contentFontGdi, true))
            using (PdfTrueTypeFont smallFont = new PdfTrueTypeFont(smallFontGdi, true))
            {
                document.PageSettings.Size = PdfPageSize.A4;
                document.DocumentInformation.Title = "5S 區域責任報告";
                document.DocumentInformation.Creator = TPConfigs.SoftNameEN ?? "KnowledgeSystem";
                document.DocumentInformation.Author = TPConfigs.LoginUser?.DisplayName ?? TPConfigs.LoginUser?.Id ?? "KnowledgeSystem";

                PdfNewPage coverPage = (PdfNewPage)document.Pages.Add();
                DrawCoverPage(coverPage, ResolveCoverDepartmentName(), ResolveCoverSignatures(), coverCompanyFont, coverTitleFont, coverSignLabelFont, coverSignHintFont, coverSignDateFont);

                for (int i = 0; i < exportAreas.Count; i++)
                {
                    dt310_Area5S area = exportAreas[i].Area;
                    List<object[]> reportRows = exportAreas[i].ReportRows;
                    PdfNewPage page = (PdfNewPage)document.Pages.Add();
                    float pageWidth = page.Canvas.ClientSize.Width;
                    float pageHeight = page.Canvas.ClientSize.Height;
                    float contentWidth = pageWidth - (PdfPageMargin * 2);
                    float y = PdfPageMargin;
                    float gridHeight = CalculateTableGridHeight(reportRows.Count);

                    y = DrawHeader(page, area, titleFont, contentFont, pageWidth, y);
                    float imageHeight = CalculateImageHeight(pageHeight, y, gridHeight);
                    y = DrawAreaImageSection(page, area, sectionFont, smallFont, contentWidth, y, imageHeight);
                    y += PdfSectionSpacing;
                    DrawTableSection(page, reportRows, sectionFont, contentFont, contentWidth, y, gridHeight);
                }

                document.SaveToFile(outputPath);
            }
        }

        private void DrawCoverPage(PdfNewPage page, string departmentName, List<PdfCoverSignatureInfo> signatures, PdfTrueTypeFont companyFont, PdfTrueTypeFont titleFont, PdfTrueTypeFont signLabelFont, PdfTrueTypeFont signHintFont, PdfTrueTypeFont signDateFont)
        {
            float pageWidth = page.Canvas.ClientSize.Width;
            float pageHeight = page.Canvas.ClientSize.Height;
            float contentWidth = pageWidth - (PdfPageMargin * 2);
            float y = 54f;

            page.Canvas.DrawString(PdfCoverCompanyName, companyFont, PdfBrushes.Black, new RectangleF(PdfPageMargin, y, contentWidth, 36f), CreateStringFormat(PdfTextAlignment.Center));
            y += 42f;
            page.Canvas.DrawLine(PdfPens.Black, PdfPageMargin + 120f, y, pageWidth - PdfPageMargin - 120f, y);

            RectangleF titleBounds = new RectangleF(PdfPageMargin, y + 110f, contentWidth, PdfCoverTitleBoxHeight);
            page.Canvas.DrawRectangle(PdfPens.Black, titleBounds);
            page.Canvas.DrawString($"{departmentName}{PdfCoverTitleSuffix}", titleFont, PdfBrushes.Black, titleBounds, CreateCenteredStringFormat());

            DrawCoverSignatureSection(page, signatures, signLabelFont, signHintFont, signDateFont, pageHeight);
        }

        private void DrawCoverSignatureSection(PdfNewPage page, List<PdfCoverSignatureInfo> signatures, PdfTrueTypeFont signLabelFont, PdfTrueTypeFont signHintFont, PdfTrueTypeFont signDateFont, float pageHeight)
        {
            List<PdfCoverSignatureInfo> displaySignatures = signatures ?? BuildDefaultCoverSignatures();
            float contentWidth = page.Canvas.ClientSize.Width - (PdfPageMargin * 2);
            float columnWidth = contentWidth / displaySignatures.Count;
            float top = pageHeight - PdfPageMargin - PdfCoverSignatureTopOffset;

            for (int i = 0; i < displaySignatures.Count; i++)
            {
                PdfCoverSignatureInfo signature = displaySignatures[i];
                float x = PdfPageMargin + (columnWidth * i);
                float lineY = top + PdfCoverSignatureLineTopOffset;
                float imageTop = top + 24f;
                float imageHeight = 28f;
                RectangleF imageBounds = new RectangleF(x + 10f, imageTop, columnWidth - 20f, imageHeight);
                bool hasImage = TryDrawCoverSignatureImage(page, signature.UserId, imageBounds);
                bool hasSignatureData = !string.IsNullOrWhiteSpace(signature.UserId) || !string.IsNullOrWhiteSpace(signature.DateText);

                page.Canvas.DrawString($"{signature.Label}：", signLabelFont, PdfBrushes.Black, new RectangleF(x, top, columnWidth, 26f), CreateStringFormat(PdfTextAlignment.Center));

                if (!hasImage)
                {
                    page.Canvas.DrawLine(PdfPens.Black, x + PdfCoverSignatureLinePadding, lineY, x + columnWidth - PdfCoverSignatureLinePadding, lineY);
                }

                if (!hasImage && !hasSignatureData)
                {
                    page.Canvas.DrawString("暫留簽名位置", signHintFont, PdfBrushes.DimGray, imageBounds, CreateStringFormat(PdfTextAlignment.Center));
                }

                if (hasSignatureData && !string.IsNullOrWhiteSpace(signature.DateText))
                {
                    page.Canvas.DrawString(
                        signature.DateText,
                        signDateFont,
                        PdfBrushes.Black,
                        new RectangleF(x + PdfCoverSignatureLinePadding, lineY + 2f, columnWidth - (PdfCoverSignatureLinePadding * 2), 12f),
                        CreateStringFormat(PdfTextAlignment.Right));
                }
            }
        }

        private float DrawHeader(PdfNewPage page, dt310_Area5S area, PdfTrueTypeFont titleFont, PdfTrueTypeFont contentFont, float pageWidth, float y)
        {
            float contentWidth = pageWidth - (PdfPageMargin * 2);

            page.Canvas.DrawString($"{area.DisplayName}區域責任報告", titleFont, PdfBrushes.Black, new RectangleF(PdfPageMargin, y, contentWidth, 24f), CreateStringFormat(PdfTextAlignment.Center));
            y += 28f;

            string desc = string.IsNullOrWhiteSpace(area.DESC) ? "無" : area.DESC.Trim();
            string meta = $"區域：{area.DisplayName}    備註：{desc}";
            page.Canvas.DrawString(meta, contentFont, PdfBrushes.DimGray, new RectangleF(PdfPageMargin, y, contentWidth, 18f), CreateStringFormat(PdfTextAlignment.Left));
            y += 24f;

            page.Canvas.DrawLine(PdfPens.DarkGray, PdfPageMargin, y, pageWidth - PdfPageMargin, y);
            return y + 10f;
        }

        private float DrawAreaImageSection(PdfNewPage page, dt310_Area5S area, PdfTrueTypeFont sectionFont, PdfTrueTypeFont contentFont, float contentWidth, float y, float imageHeight)
        {
            page.Canvas.DrawString("區域圖片", sectionFont, PdfBrushes.Black, PdfPageMargin, y);
            y += PdfSectionTitleHeight;

            RectangleF imageBounds = new RectangleF(PdfPageMargin, y, contentWidth, imageHeight);
            RectangleF imageInnerBounds = new RectangleF(
                imageBounds.X + PdfImagePadding,
                imageBounds.Y + PdfImagePadding,
                Math.Max(1f, imageBounds.Width - (PdfImagePadding * 2)),
                Math.Max(1f, imageBounds.Height - (PdfImagePadding * 2)));

            using (Image areaImage = LoadAreaImage(area))
            {
                if (areaImage == null)
                {
                    page.Canvas.DrawRectangle(PdfPens.Gray, imageBounds);
                    page.Canvas.DrawString("未找到區域圖片", contentFont, PdfBrushes.DarkGray, imageInnerBounds, CreateCenteredStringFormat());
                    return imageBounds.Bottom;
                }

                PdfImage pdfImage = PdfImage.FromImage(areaImage);
                float scale = Math.Min(imageInnerBounds.Width / areaImage.Width, imageInnerBounds.Height / areaImage.Height);
                float drawWidth = areaImage.Width * scale;
                float drawHeight = areaImage.Height * scale;
                float drawX = imageInnerBounds.X + ((imageInnerBounds.Width - drawWidth) / 2f);
                float drawY = imageInnerBounds.Y + ((imageInnerBounds.Height - drawHeight) / 2f);

                page.Canvas.DrawRectangle(PdfPens.LightGray, imageBounds);
                page.Canvas.DrawRectangle(PdfBrushes.White, imageInnerBounds);
                page.Canvas.DrawImage(pdfImage, drawX, drawY, drawWidth, drawHeight);

                return imageBounds.Bottom;
            }
        }

        private void DrawTableSection(PdfNewPage page, List<object[]> reportRows, PdfTrueTypeFont sectionFont, PdfTrueTypeFont contentFont, float contentWidth, float y, float gridHeight)
        {
            page.Canvas.DrawString("責任區劃分", sectionFont, PdfBrushes.Black, PdfPageMargin, y);
            y += PdfSectionTitleHeight;
            PdfGrid grid = BuildResponsibilityGrid(reportRows, contentFont, contentWidth);
            grid.Draw(page, new RectangleF(PdfPageMargin, y, contentWidth, gridHeight));
        }

        private PdfGrid BuildResponsibilityGrid(List<object[]> reportRows, PdfTrueTypeFont contentFont, float contentWidth)
        {
            PdfGrid grid = new PdfGrid
            {
                RepeatHeader = true,
                AllowCrossPages = false
            };

            grid.Style.CellPadding = new PdfPaddings(3, 3, 3, 3);
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
            headerRow.Height = PdfHeaderRowHeight;
            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Cells[i].Value = headers[i];
                headerRow.Cells[i].Style = CreateGridCellStyle(contentFont, PdfBrushes.LightGray, PdfTextAlignment.Center, true);
            }

            if (reportRows.Count == 0)
            {
                PdfGridRow emptyRow = grid.Rows.Add();
                emptyRow.Height = PdfDataRowHeight;
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
                row.Height = PdfDataRowHeight;
                for (int i = 0; i < rowData.Length; i++)
                {
                    row.Cells[i].Value = rowData[i] ?? "";
                    row.Cells[i].Style = CreateGridCellStyle(contentFont, PdfBrushes.White, i <= 2 ? PdfTextAlignment.Center : PdfTextAlignment.Left);
                }
            }

            return grid;
        }

        private float CalculateTableGridHeight(int rowCount)
        {
            int safeRowCount = Math.Max(rowCount, 1);
            return PdfHeaderRowHeight + (safeRowCount * PdfDataRowHeight);
        }

        private float CalculateImageHeight(float pageHeight, float initialY, float gridHeight)
        {
            float reservedHeight =
                PdfPageMargin +
                PdfSectionTitleHeight +
                gridHeight +
                PdfSectionSpacing +
                PdfSectionTitleHeight;

            float availableHeight = pageHeight - initialY - reservedHeight;
            return Math.Max(PdfImageSectionMinHeight, Math.Min(PdfImageSectionMaxHeight, availableHeight));
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

        private string ResolveCoverDepartmentName()
        {
            string loginDeptId = TPConfigs.LoginUser?.IdDepartment ?? "";
            string deptPrefix = GetExportDeptPrefix();

            dm_Departments displayDept =
                depts?.FirstOrDefault(r => r.Id == deptPrefix)
                ?? depts?.FirstOrDefault(r => r.Id == loginDeptId)
                ?? depts?.FirstOrDefault(r => !string.IsNullOrWhiteSpace(r.Id) && r.Id.StartsWith(deptPrefix));

            if (!string.IsNullOrWhiteSpace(displayDept?.DisplayName))
            {
                return displayDept.DisplayName.Trim();
            }

            return string.IsNullOrWhiteSpace(deptPrefix) ? "單位" : deptPrefix;
        }

        private List<PdfCoverSignatureInfo> ResolveCoverSignatures()
        {
            string deptPrefix = GetExportDeptPrefix();
            List<dm_User> localUsers = users ?? dm_UserBUS.Instance.GetList();
            var detailLookup = dt310_UpdateLeaveUser_detailBUS.Instance.GetList()
                .GroupBy(r => r.IdUpdateData)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.IndexStep).ToList());

            var latestRecord = dt310_UpdateLeaveUserBUS.Instance.GetList()
                .Where(r => r.DataType == "EHSAssign" && !r.IsProcess && !r.IsCancel)
                .Select(r => new
                {
                    Record = r,
                    Data = TryDeserializeUpdateLeaveUserJson(r.DataJson),
                    Details = detailLookup.ContainsKey(r.Id) ? detailLookup[r.Id] : new List<dt310_UpdateLeaveUser_detail>()
                })
                .Where(r => HasMatchingAreaDeptPrefix(r.Data, deptPrefix))
                .OrderByDescending(r => GetLatestCompletedTime(r.Record, r.Details))
                .ThenByDescending(r => r.Record.Id)
                .FirstOrDefault();

            if (latestRecord == null)
            {
                return BuildDefaultCoverSignatures();
            }

            return new List<PdfCoverSignatureInfo>
            {
                CreateCoverSignatureInfo("申請人", latestRecord.Record.CreateBy, latestRecord.Record.CreateAt, localUsers),
                CreateCoverSignatureInfo("二級主管", latestRecord.Details.FirstOrDefault(r => r.IndexStep == 0)?.IdUser, latestRecord.Details.FirstOrDefault(r => r.IndexStep == 0)?.TimeSubmit, localUsers),
                CreateCoverSignatureInfo("PSM專人", latestRecord.Details.FirstOrDefault(r => r.IndexStep == 1)?.IdUser, latestRecord.Details.FirstOrDefault(r => r.IndexStep == 1)?.TimeSubmit, localUsers),
                CreateCoverSignatureInfo("一級主管", latestRecord.Details.FirstOrDefault(r => r.IndexStep == 2)?.IdUser, latestRecord.Details.FirstOrDefault(r => r.IndexStep == 2)?.TimeSubmit, localUsers),
            };
        }

        private static UpdateLeaveUserJson TryDeserializeUpdateLeaveUserJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<UpdateLeaveUserJson>(json);
            }
            catch
            {
                return null;
            }
        }

        private static bool HasMatchingAreaDeptPrefix(UpdateLeaveUserJson data, string deptPrefix)
        {
            if (string.IsNullOrWhiteSpace(deptPrefix) || data?.Area5SResponsible == null || data.Area5SResponsible.Count == 0)
            {
                return false;
            }

            return data.Area5SResponsible.Any(r =>
                SafeLeft(r?.DeptId, 2) == deptPrefix
                || SafeLeft(r?.Area5SResponsibleData?.DeptId, 2) == deptPrefix
                || SafeLeft(ExtractDeptIdFromDesc(r?.Desc), 2) == deptPrefix);
        }

        private static string ExtractDeptIdFromDesc(string desc)
        {
            if (string.IsNullOrWhiteSpace(desc))
            {
                return string.Empty;
            }

            int splitIndex = desc.IndexOf('：');
            return splitIndex > 0 ? desc.Substring(0, splitIndex).Trim() : desc.Trim();
        }

        private static DateTime GetLatestCompletedTime(dt310_UpdateLeaveUser record, List<dt310_UpdateLeaveUser_detail> details)
        {
            DateTime? latestTime = details?
                .Where(r => r.TimeSubmit.HasValue)
                .Select(r => (DateTime?)r.TimeSubmit.Value)
                .OrderByDescending(r => r)
                .FirstOrDefault();

            return latestTime ?? record.CreateAt;
        }

        private static List<PdfCoverSignatureInfo> BuildDefaultCoverSignatures()
        {
            return new List<PdfCoverSignatureInfo>
            {
                new PdfCoverSignatureInfo { Label = "申請人" },
                new PdfCoverSignatureInfo { Label = "二級主管" },
                new PdfCoverSignatureInfo { Label = "PSM專人" },
                new PdfCoverSignatureInfo { Label = "一級主管" }
            };
        }

        private static PdfCoverSignatureInfo CreateCoverSignatureInfo(string label, string userId, DateTime? signTime, List<dm_User> localUsers)
        {
            dm_User user = localUsers?.FirstOrDefault(r => r.Id == userId);

            return new PdfCoverSignatureInfo
            {
                Label = label,
                UserId = userId ?? "",
                Name = user?.DisplayName ?? (userId ?? ""),
                DateText = signTime?.ToString("yyyy.MM.dd") ?? ""
            };
        }

        private bool TryDrawCoverSignatureImage(PdfNewPage page, string userId, RectangleF imageBounds)
        {
            using (Image signatureImage = LoadPreferredSignatureImage(userId))
            {
                if (signatureImage == null)
                {
                    return false;
                }

                PdfImage pdfImage = PdfImage.FromImage(signatureImage);
                float scale = Math.Min(imageBounds.Width / signatureImage.Width, imageBounds.Height / signatureImage.Height);
                float drawWidth = signatureImage.Width * scale;
                float drawHeight = signatureImage.Height * scale;
                float drawX = imageBounds.X + ((imageBounds.Width - drawWidth) / 2f);
                float drawY = imageBounds.Y + ((imageBounds.Height - drawHeight) / 2f);

                page.Canvas.DrawImage(pdfImage, drawX, drawY, drawWidth, drawHeight);
                return true;
            }
        }

        private Image LoadPreferredSignatureImage(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            List<dm_SignUsers> signUsers = dm_SignUsersBUS.Instance.GetListByUID(userId).ToList();
            List<int> signIds = signUsers
                .Select(r => r.IdSign)
                .Distinct()
                .ToList();

            if (signIds.Count == 0)
            {
                return null;
            }

            List<dm_Sign> userSigns = dm_SignBUS.Instance.GetListByIdSigns(signIds)
                .OrderBy(r => r.Prioritize)
                .ThenBy(r => r.Id)
                .ToList();

            if (!userSigns.Any(r => r.ImgType == 0))
            {
                return null;
            }

            dm_Sign selectedSign = userSigns.FirstOrDefault(r => r.ImgType == 0);

            if (selectedSign == null || string.IsNullOrWhiteSpace(selectedSign.ImgName))
            {
                return null;
            }
            string signPath = Path.Combine(TPConfigs.FolderSign, selectedSign.ImgName);
            if (!File.Exists(signPath))
            {
                return null;
            }

            using (Bitmap image = new Bitmap(signPath))
            {
                return ConvertImageToWhiteBackground(image);
            }
        }

        private List<object[]> BuildAreaReportRows(int areaId)
        {
            string exportDeptPrefix = GetExportDeptPrefix();

            return areaResps
                .Where(r => r.AreaId == areaId && r.DeletedAt == null)
                .Where(r => string.IsNullOrWhiteSpace(exportDeptPrefix) || SafeLeft(r.DeptId, 2) == exportDeptPrefix)
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

        private string GetExportDeptPrefix()
        {
            string loginDeptId = TPConfigs.LoginUser?.IdDepartment ?? "";
            string deptSource = !string.IsNullOrWhiteSpace(loginDeptId) ? loginDeptId : TPConfigs.idDept2word;
            return SafeLeft(deptSource, 2);
        }

        private string FormatUserInfo(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "";
            }

            dm_User user = users?.FirstOrDefault(u => u.Id == userId);
            return user != null
                ? user.DisplayName
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
                return ConvertImageToWhiteBackground(image);
            }
        }

        private static Bitmap ConvertImageToWhiteBackground(Image sourceImage)
        {
            Bitmap bitmap = new Bitmap(sourceImage.Width, sourceImage.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height);
            }

            return bitmap;
        }

        private static string SafeLeft(string value, int length)
        {
            if (string.IsNullOrWhiteSpace(value) || length <= 0)
            {
                return string.Empty;
            }

            return value.Length <= length ? value : value.Substring(0, length);
        }
    }
}
