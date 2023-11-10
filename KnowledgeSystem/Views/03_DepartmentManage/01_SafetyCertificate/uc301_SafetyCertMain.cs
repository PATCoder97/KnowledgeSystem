using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSpreadsheet.Model;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml.XPath;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.XtraSplashScreen;
using DevExpress.Security;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class uc301_SafetyCertMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_SafetyCertMain()
        {
            InitializeComponent();
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        List<BaseDisplay> lsBasesDisplay = new List<BaseDisplay>();
        BindingSource sourceBases = new BindingSource();

        List<dm_User> lsUser = new List<dm_User>();
        List<dm_User> lsAllUser;
        List<dm_Departments> lsDept;
        List<dm_JobTitle> lsJobs;
        List<dt301_Course> lsCourses;
        List<dt301_Base> lsData51;

        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
        double countValid = 0;
        double countBackup = 0;
        double countInvalid = 0;
        int quarter = 1;
        int year = 1;
        DateTime startOfQuarter = default(DateTime);
        DateTime endOfQuarter = default(DateTime);
        string pathDocument = "";

        Dictionary<int, string> mapQuater = new Dictionary<int, string>()
        {
            {1, "一"},
            {2, "二"},
            {3, "三"},
            {4, "四"}
        };

        private class BaseDisplay : dt301_Base
        {
            public string UserName { get; set; }
            public string JobName { get; set; }
            public string CourseName { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnFilter.ImageOptions.SvgImage = TPSvgimages.Filter;

            btnValidCert.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnBackCert.ImageOptions.SvgImage = TPSvgimages.Num2;
            btnInvalidCert.ImageOptions.SvgImage = TPSvgimages.Num3;
            btnWaitCert.ImageOptions.SvgImage = TPSvgimages.Num4;
            btnExpCert.ImageOptions.SvgImage = TPSvgimages.Num5;
            btnClearFilter.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            var lsBases = dt301_BaseBUS.Instance.GetList();
            lsUser = dm_UserBUS.Instance.GetList();
            lsJobs = dm_JobTitleBUS.Instance.GetList();
            lsCourses = dt301_CourseBUS.Instance.GetList();
            lsDept = dm_DeptBUS.Instance.GetList();

            lsBasesDisplay = (from data in lsBases
                              join urs in lsUser on data.IdUser equals urs.Id
                              join job in lsJobs on data.IdJobTitle equals job.Id
                              join course in lsCourses on data.IdCourse equals course.Id
                              select new BaseDisplay()
                              {
                                  Id = data.Id,
                                  IdDept = data.IdDept,
                                  IdUser = data.IdUser,
                                  IdJobTitle = data.IdJobTitle,
                                  IdCourse = data.IdCourse,
                                  DateReceipt = data.DateReceipt,
                                  ExpDate = data.ExpDate,
                                  ValidLicense = data.ValidLicense,
                                  BackupLicense = data.BackupLicense,
                                  InvalidLicense = data.InvalidLicense,
                                  Describe = data.Describe,
                                  UserName = urs.DisplayName,
                                  JobName = data.IdJobTitle + job.DisplayName,
                                  CourseName = data.IdCourse + course.DisplayName,
                              }).ToList();

            sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        // Xuất các phụ kiện
        private void ExportExcelFiles()
        {
            lsAllUser = dm_UserBUS.Instance.GetList();
            int idCounter = 1;
            // 附件04：各處提報訓練明細
            var lsDataFile4 = (from data in lsBasesDisplay
                               join usr in lsAllUser on data.IdUser equals usr.Id
                               //join dept in lsDept on usr.IdDepartment equals dept.Id
                               join course in lsCourses on data.IdCourse equals course.Id
                               select new
                               {
                                   部門代號 = usr.IdDepartment,
                                   //部門名稱 = dept.DisplayName,
                                   人員代號 = data.IdUser,
                                   人員姓名 = data.UserName,
                                   到職日 = $"{usr.DateCreate:yyyyMMdd}",
                                   職務代號 = data.IdJobTitle,
                                   職務名稱 = data.JobName.Replace(data.IdJobTitle, ""),
                                   課程代號 = data.IdCourse,
                                   課程名稱 = data.CourseName.Replace(data.IdCourse, ""),
                                   課程類別 = course.Category,
                                   上課日期 = $"{data.DateReceipt:yyyyMMdd}{(data.ExpDate.HasValue ? $"-{data.ExpDate:yyyyMMdd}" : "")}",
                                   應取證人員 = data.ValidLicense ? "Y" : "",
                                   備援證照 = data.BackupLicense ? "Y" : "",
                                   無效證照 = data.InvalidLicense ? "Y" : "",
                                   備註 = data.Describe
                               }).ToList();

            // 附件03：各廠提報資料
            idCounter = 1;

            var lsCertReqs = dt301_CertReqSetBUS.Instance.GetListByDept(idDept2word);

            var lsCountDataBase = (from data in lsBasesDisplay
                                   group data by new { data.IdJobTitle, data.IdCourse } into g
                                   select new
                                   {
                                       IdJobTitle = g.Key.IdJobTitle,
                                       IdCourse = g.Key.IdCourse,
                                       ValidLicense = g.Count(r => r.ValidLicense),
                                       BackupLicense = g.Count(r => r.BackupLicense),
                                       InvalidLicense = g.Count(r => r.InvalidLicense),
                                   }).ToList();

            var lsCountByCertReqs = (from data in lsCertReqs
                                     join countg in lsCountDataBase on new { data.IdJobTitle, data.IdCourse } equals new { countg.IdJobTitle, countg.IdCourse } into gj
                                     from sub in gj.DefaultIfEmpty()
                                     select new
                                     {
                                         data.IdDept,
                                         DeptName = "",
                                         data.IdCourse,
                                         CourseName = "",
                                         data.IdJobTitle,
                                         JobName = "",
                                         data.NewHeadcount,
                                         data.ActualHeadcount,
                                         data.ReqQuantity,
                                         ValidLicense = sub != null ? sub.ValidLicense : 0,
                                         BackupLicense = sub != null ? sub.BackupLicense : 0,
                                         InvalidLicense = sub != null ? sub.InvalidLicense : 0,
                                     }).ToList();

            var lsDataFile3 = (from dt in lsCountByCertReqs
                               join dept in lsDept on dt.IdDept equals dept.Id
                               join job in lsJobs on dt.IdJobTitle equals job.Id
                               join course in lsCourses on dt.IdCourse equals course.Id
                               select new
                               {
                                   Id = idCounter++,
                                   dt.IdDept,
                                   DeptName = dept.DisplayName,
                                   dt.IdCourse,
                                   CourseName = course.DisplayName,
                                   dt.IdJobTitle,
                                   JobName = job.DisplayName,
                                   dt.NewHeadcount,
                                   dt.ActualHeadcount,
                                   dt.ReqQuantity,
                                   ValidLicense = dt.ValidLicense,
                                   BackupLicense = dt.BackupLicense,
                                   InvalidLicense = dt.InvalidLicense,
                               }).ToList();

            // 附件05.2：複訓之提報需求人員名單
            idCounter = 1;

            var lsQueryFile52 = (from data in lsBasesDisplay
                                 where data.ValidLicense && (data.ExpDate.HasValue ? (data.ExpDate <= endOfQuarter) : false)
                                 join usr in lsUser on data.IdUser equals usr.Id
                                 join dept in lsDept on usr.IdDepartment equals dept.Id
                                 select new
                                 {
                                     Id = idCounter++,
                                     IdDept = usr.IdDepartment,
                                     DivisionName = "冶金部",
                                     DeptName = dept.DisplayName,
                                     UserID = usr.Id,
                                     UserNameTW = usr.DisplayName,
                                     UserNameVN = usr.DisplayNameVN,
                                     CitizenID = usr.CitizenID,
                                     DOB = usr.DOB,
                                     CourseName = data.CourseName,
                                     JobName = data.JobName,
                                     Nationality = usr.Nationality.Replace("TW", "台灣").Replace("VN", "越南").Replace("CN", "中國"),
                                     data.IdCourse
                                 }).ToList();

            // 附件05.1：初訓之提報需求人員名單
            idCounter = 1;

            var lsQueryFile51 = (from data in lsData51
                                 join usr in lsUser on data.IdUser equals usr.Id
                                 join dept in lsDept on usr.IdDepartment equals dept.Id
                                 join course in lsCourses on data.IdCourse equals course.Id
                                 join job in lsJobs on usr.JobCode equals job.Id
                                 select new
                                 {
                                     Id = idCounter++,
                                     IdDept = usr.IdDepartment,
                                     DivisionName = "冶金部",
                                     DeptName = dept.DisplayName,
                                     UserID = usr.Id,
                                     UserNameTW = usr.DisplayName,
                                     UserNameVN = usr.DisplayNameVN,
                                     CitizenID = usr.CitizenID,
                                     DOB = usr.DOB,
                                     CourseName = course.DisplayName,
                                     JobName = job.DisplayName,
                                     Nationality = usr.Nationality.Replace("TW", "台灣").Replace("VN", "越南").Replace("CN", "中國"),
                                     data.IdCourse
                                 }).ToList();

            // 附件02：工安類證照統計表
            idCounter = 1;

            var lsCountData51 = (from data in lsQueryFile51
                                 group data by data.IdCourse into g
                                 select new
                                 {
                                     IdCourse = g.Key,
                                     Count = g.Count()
                                 }).ToList();

            var lsCountData52 = (from data in lsQueryFile52
                                 group data by data.IdCourse into g
                                 select new
                                 {
                                     IdCourse = g.Key,
                                     Count = g.Count()
                                 }).ToList();

            var lsDataFile2 = (from data in lsDataFile3
                               group data by data.IdCourse into g
                               select new
                               {
                                   Id = idCounter++,
                                   JobName = g.First(r => r.IdCourse == g.Key).CourseName,
                                   IdCourse = g.Key,
                                   TotalReqQuantity = g.Sum(r => r.ReqQuantity),
                                   ValidLicense = g.Sum(r => r.ValidLicense),
                                   BackupLicense = g.Sum(r => r.BackupLicense),
                                   NewTrain = lsCountData51.FirstOrDefault(r => r.IdCourse == g.Key)?.Count ?? 0,
                                   ReTrain = lsCountData52.FirstOrDefault(r => r.IdCourse == g.Key)?.Count ?? 0,
                               }).ToList();

            // 附件01：工安類證照取得情形一覽表
            List<string> lsDataFile1 = new List<string>()
            {
                "1",
                "冶金部",
                "物理試驗處",
                lsDataFile2.Sum(r => r.TotalReqQuantity).ToString(),
                $"{lsDataFile2.Sum(r => r.ValidLicense)}",
                $"{lsDataFile2.Sum(r => r.NewTrain) + lsDataFile2.Sum(r => r.ReTrain)}"
            };

            // Xuất ra Excel
            // 附件01：工安類證照取得情形一覽表.xlsx
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "DFKai-SB";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = true;

                ws.Cells["A1"].Value = "工安類證照取得情形一覽表";
                ws.Cells["A1:I1"].Merge = true;

                ws.Cells["A2"].Value = "項次";
                ws.Cells["B2"].Value = "事業部";
                ws.Cells["C2"].Value = "單位";
                ws.Cells["D2"].Value = $"{DateTime.Today.Year}第{mapQuater[quarter]}季\r\n應取得張數\r\nA";
                ws.Cells["E2"].Value = "已取得張數\r\n\r\nB";
                ws.Cells["F2"].Value = "缺額\r\n\r\nC=A-B";
                ws.Cells["G2"].Value = "缺額張數說明原因";
                ws.Cells["G3"].Value = "人員待補";
                ws.Cells["H3"].Value = "尚未派訓";
                ws.Cells["I3"].Value = "其他\r\n(留職停薪)";

                ws.Cells["A5"].Value = "合計";
                ws.Cells["A6"].Value = "備註：\r\n1.人員待補：\r\n2.尚未派訓：\r\n3.其他：\r\n4.應取證更新說明:";

                ws.Cells["A2:A3"].Merge = true;
                ws.Cells["B2:B3"].Merge = true;
                ws.Cells["C2:C3"].Merge = true;
                ws.Cells["D2:D3"].Merge = true;
                ws.Cells["E2:E3"].Merge = true;
                ws.Cells["F2:F3"].Merge = true;
                ws.Cells["G2:I2"].Merge = true;
                ws.Cells["A5:C5"].Merge = true;
                ws.Cells["A6:I6"].Merge = true;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A4"].Value = lsDataFile1[0];
                ws.Cells["B4"].Value = lsDataFile1[1];
                ws.Cells["C4"].Value = lsDataFile1[2];
                ws.Cells["D4:D5"].Value = lsDataFile1[3];
                ws.Cells["E4:E5"].Value = lsDataFile1[4];
                ws.Cells["F4:F5"].Formula = "D4-E4";
                ws.Cells["G4:G5"].Value = lsDataFile1[5];
                ws.Cells["H4:H5"].Formula = "";
                ws.Cells["I4:I5"].Value = lsDataFile2.Sum(r => r.TotalReqQuantity);

                ws.Columns[01].Width = 20;
                ws.Columns[02].Width = 20;
                ws.Columns[03].Width = 20;
                ws.Columns[04].Width = 20;
                ws.Columns[05].Width = 20;
                ws.Columns[06].Width = 20;
                ws.Columns[07].Width = 20;
                ws.Columns[08].Width = 20;
                ws.Columns[09].Width = 20;

                ws.Row(1).Height = 40;
                ws.Row(6).Height = 120;

                ws.Row(1).Style.Font.Size = 18;

                ws.Row(1).Style.Font.Bold = true;
                ws.Row(5).Style.Font.Bold = true;
                ws.Row(6).Style.Font.Bold = true;
                ws.Row(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A1:I5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1:I5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1:I5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                string savePath = Path.Combine(pathDocument, $"附件01：工安類證照取得情形一覽表.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }

            // 附件02：工安類證照統計表.xlsx
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "DFKai-SB";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = false;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A2"].Value = "項次";
                ws.Cells["B2"].Value = "課程名稱";
                ws.Cells["C2"].Value = "課程代號";
                ws.Cells["D2"].Value = "應取得";
                ws.Cells["E2"].Value = "實際取得";
                ws.Cells["F2"].Value = "備援";
                ws.Cells["G2"].Value = "初訓";
                ws.Cells["H2"].Value = "複訓";
                ws.Cells["I2"].Value = "人員待補";
                ws.Cells["J2"].Value = "留職停薪";

                ws.Cells["A3"].LoadFromCollection(lsDataFile2, false);

                ws.Columns[01].Width = 20;
                ws.Columns[02].Width = 60;
                ws.Columns[03].Width = 20;
                ws.Columns[04].Width = 20;
                ws.Columns[05].Width = 20;
                ws.Columns[06].Width = 20;
                ws.Columns[07].Width = 20;
                ws.Columns[08].Width = 20;
                ws.Columns[09].Width = 20;
                ws.Columns[10].Width = 20;

                List<int> lsColAlignLefts = new List<int>() { 2 };
                foreach (var indexCol in lsColAlignLefts)
                    ws.Columns[indexCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Tạo bảng theo dữ liệu của Excel
                var dataRange = ws.Cells[ws.Dimension.Address];
                ExcelTable tab = ws.Tables.Add(dataRange, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                for (int i = 0; i < lsDataFile2.Count; i++)
                {
                    int indexRow = i + 3;
                    ws.Cells[$"I{indexRow}"].Formula = $"D{indexRow}-SUM(E{indexRow}:H{indexRow})";
                }

                ws.Cells["A1"].Value = "課程";
                ws.Cells["D1"].Value = idDept2word;
                ws.Cells["G1"].Value = "情形";

                ws.Cells["A1:C1"].Merge = true;
                ws.Cells["D1:F1"].Merge = true;
                ws.Cells["G1:J1"].Merge = true;

                tab.ShowTotal = true;
                for (int i = 3; i <= 9; i++)
                {
                    tab.Columns[i].TotalsRowFunction = RowFunctions.Sum;
                }
                ws.Calculate();

                string savePath = Path.Combine(pathDocument, $"附件02：工安類證照統計表.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }

            // 附件03：各廠提報資料.xlsx
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "DFKai-SB";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = false;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A5"].Value = "項次";
                ws.Cells["B5"].Value = "部門代號";
                ws.Cells["C5"].Value = "部門名稱";
                ws.Cells["D5"].Value = "訓練證照代號";
                ws.Cells["E5"].Value = "訓練證照名稱";
                ws.Cells["F5"].Value = "職務代號";
                ws.Cells["G5"].Value = "職務名稱";
                ws.Cells["H5"].Value = "編制人數";
                ws.Cells["I5"].Value = "實際人數";
                ws.Cells["J5"].Value = "應取證張數";
                ws.Cells["K5"].Value = "實際取證張數";
                ws.Cells["L5"].Value = "備援證照張數";
                ws.Cells["M5"].Value = "無效證照張數";

                ws.Cells["A6"].LoadFromCollection(lsDataFile3, false);

                ws.Columns[01].Width = 20;
                ws.Columns[02].Width = 20;
                ws.Columns[03].Width = 20;
                ws.Columns[04].Width = 20;
                ws.Columns[05].Width = 60;
                ws.Columns[06].Width = 20;
                ws.Columns[07].Width = 30;
                ws.Columns[08].Width = 20;
                ws.Columns[09].Width = 20;
                ws.Columns[10].Width = 20;
                ws.Columns[11].Width = 20;
                ws.Columns[12].Width = 20;
                ws.Columns[13].Width = 20;

                List<int> lsColAlignLefts = new List<int>() { 5, 7 };
                foreach (var indexCol in lsColAlignLefts)
                    ws.Columns[indexCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Tạo bảng theo dữ liệu của Excel
                var dataRange = ws.Cells[ws.Dimension.Address];
                ExcelTable tab = ws.Tables.Add(dataRange, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                tab.ShowTotal = true;
                for (int i = 7; i <= 12; i++)
                {
                    tab.Columns[i].TotalsRowFunction = RowFunctions.Sum;
                }
                ws.Calculate();

                ws.Cells["A1"].Value = "廠處代號";
                ws.Cells["A2"].Value = "廠處名稱";
                ws.Cells["A3"].Value = "編制人數";
                ws.Cells["A4"].Value = "實際人數";

                ws.Cells["B1"].Value = idDept2word;
                ws.Cells["B2"].Value = "";
                ws.Cells["B3"].Value = "";
                ws.Cells["B4"].Value = "";

                ws.Cells["B1:M1"].Merge = true;
                ws.Cells["B2:M2"].Merge = true;
                ws.Cells["B3:M3"].Merge = true;
                ws.Cells["B4:M4"].Merge = true;

                string savePath = Path.Combine(pathDocument, $"附件03：各廠提報資料.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }

            // 附件04：各處提報訓練明細.xlsx
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "DFKai-SB";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = false;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A1"].LoadFromCollection(lsDataFile4, true);

                ws.Columns[01].Width = 20;
                ws.Columns[02].Width = 20;
                ws.Columns[03].Width = 20;
                ws.Columns[04].Width = 20;
                ws.Columns[05].Width = 20;
                ws.Columns[06].Width = 20;
                ws.Columns[07].Width = 20;
                ws.Columns[08].Width = 20;
                ws.Columns[09].Width = 20;
                ws.Columns[10].Width = 20;
                ws.Columns[11].Width = 20;
                ws.Columns[12].Width = 20;
                ws.Columns[13].Width = 20;

                List<int> lsColAlignLefts = new List<int>() { 5, 7 };
                foreach (var indexCol in lsColAlignLefts)
                    ws.Columns[indexCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Tạo bảng theo dữ liệu của Excel
                var dataRange = ws.Cells[ws.Dimension.Address];
                ExcelTable tab = ws.Tables.Add(dataRange, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                tab.ShowTotal = true;
                for (int i = 10; i <= 12; i++)
                {
                    //  tab.Columns[i].TotalsRowFunction = RowFunctions.Custom;
                    tab.Columns[i].TotalsRowFormula = $"COUNTIF([{tab.Columns[i].Name}],\"Y\")";
                }
                ws.Calculate();
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                string savePath = Path.Combine(pathDocument, $"附件04：各處提報訓練明細.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }

            // 附件05.1：初訓之提報需求人員名單.xlsx, 附件05.2：.複訓之提報需求人員名單.xlsx
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "DFKai-SB";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = true;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A2"].Value = "TT\r\n項次";
                ws.Cells["B2"].Value = "Mã BP\r\n部門代號";
                ws.Cells["C2"].Value = "Bộ phận\r\n事業部";
                ws.Cells["D2"].Value = "Xưởng\r\n廠處";
                ws.Cells["E2"].Value = "Mã nhân viên\r\n人員代號";
                ws.Cells["F2"].Value = "Tên tiếng trung\r\n人員中文姓名";
                ws.Cells["G2"].Value = "Tên tiếng việt\r\n人員越/英文姓名";
                ws.Cells["H2"].Value = "CMND\r\n身份證號";
                ws.Cells["I2"].Value = "Năm sinh\r\n出生日期";
                ws.Cells["J2"].Value = "Loại chứng chỉ\r\n證照類別";
                ws.Cells["K2"].Value = "Chức vụ\r\n職務";
                ws.Cells["L2"].Value = "Quốc tịch \r\n國籍";
                ws.Cells["M2"].Value = "Thời gian\r\n時期";
                ws.Cells["N2"].Value = "Địa điểm học\r\n地點";

                ws.Row(1).Height = 40;
                ws.Columns[1].Width = 20;
                ws.Columns[2].Width = 20;
                ws.Columns[3].Width = 20;
                ws.Columns[4].Width = 40;
                ws.Columns[5].Width = 20;
                ws.Columns[6].Width = 30;
                ws.Columns[7].Width = 30;
                ws.Columns[8].Width = 20;
                ws.Columns[9].Width = 20;
                ws.Columns[10].Width = 80;
                ws.Columns[11].Width = 35;
                ws.Columns[12].Width = 20;
                ws.Columns[13].Width = 20;
                ws.Columns[14].Width = 20;

                ws.Cells["A3"].LoadFromCollection(lsQueryFile52, false);
                var rangeTab = ws.Cells[$"A2:{ws.Dimension.End.Address}"];

                List<int> lsColAlignLefts = new List<int>() { 4, 6, 7, 10, 11 };
                foreach (var indexCol in lsColAlignLefts)
                    ws.Columns[indexCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A1"].Value = "冶金部人員證照受訓需求表(初訓)";
                ws.Cells["A1"].Style.Font.Size = 28;
                ws.Cells["A1:N1"].Merge = true;
                ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set the column format to DateTime
                ws.Column(9).Style.Numberformat.Format = "yyyy/MM/dd";

                // Ading a table to a Range
                ExcelTable tab = ws.Tables.Add(rangeTab, "Table1");
                tab.TableStyle = TableStyles.Medium2;

                string savePath = Path.Combine(pathDocument, $"附件05.2：.複訓之提報需求人員名單.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);

                ws.DeleteRow(3, lsQueryFile52.Count - lsQueryFile51.Count);
                ws.Cells["A3"].LoadFromCollection(lsQueryFile51, false);

                //// Define the data range on the source sheet, Ading a table to a Range
                //rangeTab = ws.Cells[$"A2:{ws.Dimension.End.Address}"];
                //tab = ws.Tables.Add(rangeTab, "Table1");
                //tab.TableStyle = TableStyles.Medium2;

                savePath = Path.Combine(pathDocument, $"附件05.1：初訓之提報需求人員名單.xlsx");
                excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }

            // 附件06：派訓數量統計
        }

        private void uc301_SafetyCertMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.CustomSummaryCalculate += GvData_CustomSummaryCalculate;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void GvData_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.IsTotalSummary)
            {
                GridSummaryItem item = e.Item as GridSummaryItem;

                switch (e.SummaryProcess)
                {
                    case CustomSummaryProcess.Start:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                countValid = 0;
                                break;
                            case "BackupLicense":
                                countBackup = 0;
                                break;
                            case "InvalidLicense":
                                countInvalid = 0;
                                break;
                        }
                        break;
                    case CustomSummaryProcess.Calculate:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "ValidLicense")) countValid++;
                                break;
                            case "BackupLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "BackupLicense")) countBackup++;
                                break;
                            case "InvalidLicense":
                                if ((bool)view.GetRowCellValue(e.RowHandle, "InvalidLicense")) countInvalid++;
                                break;
                        }
                        break;
                    case CustomSummaryProcess.Finalize:
                        switch (item.FieldName)
                        {
                            case "ValidLicense":
                                e.TotalValue = countValid;
                                break;
                            case "BackupLicense":
                                e.TotalValue = countBackup;
                                break;
                            case "InvalidLicense":
                                e.TotalValue = countInvalid;
                                break;
                            case "Describe":
                                e.TotalValue = countValid + countBackup + countInvalid;
                                break;
                        }
                        break;
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f301_CertInfo fInfo = new f301_CertInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "證照";
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dt301_Base _base = view.GetRow(view.FocusedRowHandle) as dt301_Base;
            if (_base == null) return;

            f301_CertInfo fInfo = new f301_CertInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "證照";
            fInfo._base = _base;
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc301_SelectOutputFile ucInfo = new uc301_SelectOutputFile();
            if (XtraDialog.Show(ucInfo, "選導出表單", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            bool IsWrongData = ucInfo.IsWrongData;
            quarter = ucInfo.quarter;
            year = ucInfo.year;
            lsData51 = ucInfo.lsData51;

            if (IsWrongData)
            {
                MsgTP.MsgError("資料初訓不正確！");
                return;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                startOfQuarter = new DateTime(year, 3 * quarter - 2, 1);
                endOfQuarter = startOfQuarter.AddMonths(3).AddSeconds(-1);

                pathDocument = Path.Combine(TPConfigs.DocumentPath(), $"工安證照 {DateTime.Now:yyyyMMddHHmm}");
                if (!Directory.Exists(pathDocument))
                    Directory.CreateDirectory(pathDocument);

                ExportExcelFiles();
            }

            Process.Start(pathDocument);
        }

        private void SetFilter(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filterString = "";
            switch (e.Item.Caption)
            {
                case "應取證照":
                    filterString = "[ValidLicense] = True AND [BackupLicense] = False AND [InvalidLicense] = False";
                    break;
                case "備援證照":
                    filterString = "[ValidLicense] = False AND [BackupLicense] = True AND [InvalidLicense] = False";
                    break;
                case "無效證照":
                    filterString = "[ValidLicense] = False AND [BackupLicense] = False AND [InvalidLicense] = True";
                    break;
                case "在等證照":
                    filterString = "[ValidLicense] = False AND [BackupLicense] = False AND [InvalidLicense] = False";
                    break;
                case "過期證照":
                    filterString = $"[ValidLicense] = True  AND [ExpDate] IS NOT NULL AND [ExpDate] < '{DateTime.Today.ToShortDateString()}' ";
                    break;
            }

            gvData.ActiveFilterString = filterString;
        }
    }
}
