﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
        string typeOfs = "", halfYear = "";
        DateTime startOfTimeExport = default(DateTime);
        DateTime endOfTimeExport = default(DateTime);
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
            public string TypeOf { get; set; }
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
            subBtnExp.ImageOptions.SvgImage = TPSvgimages.Num5;
            btnExpQuater.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnExpHaflYear.ImageOptions.SvgImage = TPSvgimages.Num2;
            btnExpYear.ImageOptions.SvgImage = TPSvgimages.Num3;
            btnClearFilter.ImageOptions.SvgImage = TPSvgimages.Close;

            btnInvalidateExpCert.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnSpecialFunctions.ImageOptions.SvgImage = TPSvgimages.Progress;
        }

        private void LoadData()
        {
            helper.SaveViewInfo();
            var lsBases = dt301_BaseBUS.Instance.GetListByDept(idDept2word);
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
                                  CertSuspended = data.CertSuspended,
                                  TypeOf = course.TypeOf
                              }).ToList();

            sourceBases.DataSource = lsBasesDisplay;
            helper.LoadViewInfo();

            gvData.BestFitColumns();
        }

        // Xuất các phụ kiện
        private void ExportExcelFiles(string typeOf)
        {
            lsAllUser = dm_UserBUS.Instance.GetList();
            //var typeOfCourses = lsCourses.Where(r => !string.IsNullOrEmpty(r.TypeOf)).Select(r => r.TypeOf).Distinct().ToList();

            //foreach (var typeOf in typeOfCourses)
            //{
            string pathExcel = Path.Combine(pathDocument, typeOf);
            if (!Directory.Exists(pathExcel))
                Directory.CreateDirectory(pathExcel);

            var dataRawStatics = lsBasesDisplay.Where(r => r.TypeOf == typeOf).ToList();
            var courseByType = lsCourses.Where(r => r.TypeOf == typeOf).ToList();

            // xử lý theo từng loại bằng an toàn
            int idCounter = 1;
            // 附件04：各處提報訓練明細
            var lsDataFile4 = (from data in dataRawStatics
                               join usr in lsAllUser on data.IdUser equals usr.Id
                               //join dept in lsDept on usr.IdDepartment equals dept.Id
                               join course in courseByType on data.IdCourse equals course.Id
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

            var lsCountDataBase = (from data in dataRawStatics
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
                               join course in courseByType on dt.IdCourse equals course.Id
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

            var lsQueryFile52 = (from data in dataRawStatics
                                 where data.ValidLicense && (data.ExpDate.HasValue ? (data.ExpDate <= endOfTimeExport) : false)
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
                                     data.IdCourse,
                                     IdData = data.Id
                                 }).ToList();

            // 附件05.1：初訓之提報需求人員名單
            idCounter = 1;

            var lsQueryFile51 = (from data in lsData51
                                 join usr in lsUser on data.IdUser equals usr.Id
                                 join dept in lsDept on usr.IdDepartment equals dept.Id
                                 join course in courseByType on data.IdCourse equals course.Id
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

            var lsCountData51 = (from data in lsData51
                                 group data by data.IdCourse into g
                                 select new
                                 {
                                     IdCourse = g.Key,
                                     Count = g.Count(),
                                     FTrainNewU = g.Where(r => !r.ValidLicense).Count(),
                                     FTrainOldU = g.Where(r => r.ValidLicense).Count()
                                 }).ToList();

            var lsCountData52 = (from data in lsQueryFile52
                                 group data by data.IdCourse into g
                                 select new
                                 {
                                     IdCourse = g.Key,
                                     Count = g.Count()
                                 }).ToList();

            var lsDataFile2 = (from data in lsDataFile3.Where(r => !lsQueryFile52.Select(x => x.IdData).Contains(r.Id))
                               group data by data.IdCourse into g
                               select new
                               {
                                   Id = idCounter++,
                                   JobName = g.First(r => r.IdCourse == g.Key).CourseName,
                                   IdCourse = g.Key,
                                   TotalReqQuantity = g.Sum(r => r.ReqQuantity),
                                   ValidLicense = g.Sum(r => r.ValidLicense),
                                   BackupLicense = g.Sum(r => r.BackupLicense),
                                   FirstTrain = lsCountData51.FirstOrDefault(r => r.IdCourse == g.Key)?.Count ?? 0,
                                   ReTrain = lsCountData52.FirstOrDefault(r => r.IdCourse == g.Key)?.Count ?? 0,
                               }).ToList();

            // 附件01：工安類證照取得情形一覽表
            List<string> lsDataFile1 = new List<string>()
                {
                    "1",
                    "冶金部",
                    lsDept.FirstOrDefault(r => r.Id == idDept2word).DisplayName,
                    lsDataFile2.Sum(r => r.TotalReqQuantity).ToString(),
                    $"{lsDataFile2.Sum(r => r.ValidLicense)}",
                    $"{lsDataFile2.Sum(r => r.FirstTrain) + lsDataFile2.Sum(r => r.ReTrain)}",
                    $"{dataRawStatics.Count(r=>r.CertSuspended)}",
                };

            // 附件06：派訓數量統計.xlsx
            var lsCountBAKCertExp = (from data in dataRawStatics
                                     where data.BackupLicense && (data.ExpDate.HasValue ? (data.ExpDate <= endOfTimeExport) : false)
                                     group data by data.IdCourse into g
                                     select new
                                     {
                                         IdCourse = g.Key,
                                         Count = g.Count()
                                     }).ToList();

            var lsDataFile6 = (from data in lsDataFile2
                               join course in courseByType on data.IdCourse equals course.Id
                               select new // Lấy các thông tin học lần đầu, học lại, số người thiếu
                               {
                                   data.IdCourse,
                                   course.DisplayName,
                                   data.FirstTrain,
                                   data.ReTrain,
                                   data.BackupLicense,
                                   SumUserMissing = data.TotalReqQuantity - (data.ValidLicense + data.FirstTrain + data.ReTrain)
                               } into dt1
                               join f51 in lsCountData51 on dt1.IdCourse equals f51.IdCourse into dtg1
                               from g1 in dtg1.DefaultIfEmpty()
                               select new // Lấy thông tin học lần đầu là nhân viên mới hay là chuyển vị trí làm việc
                               {
                                   dt1.IdCourse,
                                   dt1.DisplayName,
                                   FTrainNewU = g1 != null ? g1.FTrainNewU : 0,
                                   FTrainOldU = g1 != null ? g1.FTrainOldU : 0,
                                   dt1.ReTrain,
                                   dt1.SumUserMissing
                               } into dt2
                               join bak in lsCountBAKCertExp on dt2.IdCourse equals bak.IdCourse into dtg2
                               from g2 in dtg2.DefaultIfEmpty()
                               select new // Lấy thông tin các bằng dự phòng hết hạn
                               {
                                   dt2.DisplayName,
                                   dt2.FTrainNewU,
                                   dt2.FTrainOldU,
                                   dt2.ReTrain,
                                   BackupLicense = g2 != null ? g2.Count : 0,
                                   dt2.SumUserMissing
                               }).ToList();

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
                ws.Cells["G4:G5"].Formula = "F4-H4-I4";
                ws.Cells["H4:H5"].Value = lsDataFile1[5];
                ws.Cells["I4:I5"].Value = lsDataFile1[6];

                ws.Columns[01].Width = 20;
                ws.Columns[02].Width = 20;
                ws.Columns[03].Width = 25;
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

                string savePath = Path.Combine(pathExcel, $"附件01：工安類證照取得情形一覽表.xlsx");
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
                ws.Cells["I2"].Value = "缺額張數";
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
                    ws.Cells[$"I{indexRow}"].Formula = $"D{indexRow}-E{indexRow}";
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

                string savePath = Path.Combine(pathExcel, $"附件02：工安類證照統計表.xlsx");
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
                ws.Cells["B2"].Value = lsDataFile3.FirstOrDefault().DeptName;
                ws.Cells["B3"].Value = lsUser.Where(r => r.Status == 0).Count(r => r.IdDepartment.StartsWith(idDept2word));
                ws.Cells["B4"].Value = lsUser.Where(r => r.Status == 0).Count(r => r.IdDepartment.StartsWith(idDept2word));

                ws.Cells["B1:M1"].Merge = true;
                ws.Cells["B2:M2"].Merge = true;
                ws.Cells["B3:M3"].Merge = true;
                ws.Cells["B4:M4"].Merge = true;

                string savePath = Path.Combine(pathExcel, $"附件03：各廠提報資料.xlsx");
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

                string savePath = Path.Combine(pathExcel, $"附件04：各處提報訓練明細.xlsx");
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
                ws.Columns[7].Width = 40;
                ws.Columns[8].Width = 20;
                ws.Columns[9].Width = 20;
                ws.Columns[10].Width = 80;
                ws.Columns[11].Width = 35;
                ws.Columns[12].Width = 20;
                ws.Columns[13].Width = 20;
                ws.Columns[14].Width = 20;

                ws.Cells["A3"].LoadFromCollection(lsQueryFile52, false);
                var rangeTab = ws.Cells[$"A2:N{ws.Dimension.End.Row}"];

                List<int> lsColAlignLefts = new List<int>() { 4, 6, 7, 10, 11 };
                foreach (var indexCol in lsColAlignLefts)
                    ws.Columns[indexCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A1"].Value = "冶金部人員證照受訓需求表(複訓)";
                ws.Cells["A1"].Style.Font.Size = 28;
                ws.Cells["A1:N1"].Merge = true;
                ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Set the column format to DateTime
                ws.Column(9).Style.Numberformat.Format = "yyyy/MM/dd";

                // Ading a table to a Range
                string tableName = "Table1";
                ExcelTable tab = ws.Tables.Add(rangeTab, tableName);
                tab.TableStyle = TableStyles.Medium2;

                // Xoá 2 cột dư
                ws.DeleteColumn(13, 2);

                string savePath = Path.Combine(pathExcel, $"附件05.2：.複訓之提報需求人員名單.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);

                // Nếu có dữ liệu File5.1 thì xuất theo Format file5.2
                if (lsQueryFile51.Count != 0)
                {
                    ws.Cells["A1"].Value = "冶金部人員證照受訓需求表(初訓)";

                    ws.Tables.Delete(tableName);
                    ws.Cells["A3"].LoadFromCollection(lsQueryFile51, false);

                    rangeTab = ws.Cells[$"A2:N{lsQueryFile51.Count + 2}"];
                    tab = ws.Tables.Add(rangeTab, tableName);
                    tab.TableStyle = TableStyles.Medium2;

                    // Xoá 2 cột dư
                    ws.DeleteColumn(13, 2);

                    savePath = Path.Combine(pathExcel, $"附件05.1：初訓之提報需求人員名單.xlsx");
                    excelFile = new FileInfo(savePath);
                    pck.SaveAs(excelFile);
                }
            }

            // 附件06：派訓數量統計.xlsx
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
                ws.Cells["A1"].Value = "單位";
                ws.Cells["B1"].Value = idDept2word;
                ws.Cells["A2"].Value = "證照項目";
                ws.Cells["B2"].Value = "應初訓";
                ws.Cells["D2"].Value = "應複訓";
                ws.Cells["E2"].Value = "備援證照到期";
                ws.Cells["F2"].Value = "人力增補";

                ws.Cells["B3"].Value = "新進";
                ws.Cells["C3"].Value = "異動";
                ws.Cells["D3"].Value = $"證照第{mapQuater[quarter]}季到期";

                ws.Cells["B1:F1"].Merge = true;
                ws.Cells["A2:A3"].Merge = true;
                ws.Cells["B2:C2"].Merge = true;
                ws.Cells["E2:E3"].Merge = true;
                ws.Cells["F2:F3"].Merge = true;

                ws.Columns[01].Width = 60;
                ws.Columns[02].Width = 20;
                ws.Columns[03].Width = 20;
                ws.Columns[04].Width = 20;
                ws.Columns[05].Width = 20;
                ws.Columns[06].Width = 20;
                ws.Columns[07].Width = 20;
                ws.Columns[08].Width = 20;
                ws.Columns[09].Width = 20;
                ws.Columns[10].Width = 20;

                ws.Cells["A4"].LoadFromCollection(lsDataFile6, false);

                int endRow = ws.Dimension.End.Row + 1;
                ws.Cells[$"A{endRow}"].Value = "上述證照小計";
                ws.Cells[$"B{endRow}"].Formula = $"=SUM(B4:B{endRow - 1})";
                ws.Cells[$"C{endRow}"].Formula = $"=SUM(C4:C{endRow - 1})";
                ws.Cells[$"D{endRow}"].Formula = $"=SUM(D4:D{endRow - 1})";
                ws.Cells[$"E{endRow}"].Formula = $"=SUM(E4:E{endRow - 1})";
                ws.Cells[$"F{endRow}"].Formula = $"=SUM(F4:F{endRow - 1})";

                ws.Cells[$"A{endRow + 1}"].Value = "派訓總計";
                ws.Cells[$"B{endRow + 1}"].Formula = $"=SUM(B4:E{endRow - 1})";
                ws.Cells[$"F{endRow + 1}"].Formula = $"=SUM(F4:F{endRow - 1})";

                ws.Cells[$"B{endRow + 1}:E{endRow + 1}"].Merge = true;

                var rangeData = ws.Cells[$"A1:F{endRow + 1}"];
                rangeData.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                rangeData.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                rangeData.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                rangeData.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                List<int> lsRowBolds = new List<int>() { 1, 2, 3, endRow, endRow + 1 };
                foreach (var indexRow in lsRowBolds)
                    ws.Row(indexRow).Style.Font.Bold = true;

                string savePath = Path.Combine(pathExcel, $"附件06：派訓數量統計.xlsx");
                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }
            //}
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
            halfYear = ucInfo.HalfYear;
            typeOfs = ucInfo.TypeOf;
            lsData51 = ucInfo.lsData51;

            if (IsWrongData)
            {
                MsgTP.MsgError("資料初訓不正確！");
                return;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                int indexTypeOf = TPConfigs.typeOf301.FirstOrDefault(r => r.Value == typeOfs).Key;
                switch (indexTypeOf)
                {
                    case 1:
                        startOfTimeExport = new DateTime(year, 1, 1);
                        endOfTimeExport = startOfTimeExport.AddMonths(12).AddSeconds(-1);
                        break;
                    case 2:
                        startOfTimeExport = new DateTime(year, halfYear == "上半年" ? 1 : 7, 1);
                        endOfTimeExport = startOfTimeExport.AddMonths(6).AddSeconds(-1);
                        break;
                    default:
                        startOfTimeExport = new DateTime(year, 3 * quarter - 2, 1);
                        endOfTimeExport = startOfTimeExport.AddMonths(3).AddSeconds(-1);
                        break;
                }

                pathDocument = Path.Combine(TPConfigs.DocumentPath(), $"工安證照 {DateTime.Now:yyyyMMddHHmm}");
                if (!Directory.Exists(pathDocument))
                    Directory.CreateDirectory(pathDocument);

                ExportExcelFiles(typeOfs);
            }

            Process.Start(pathDocument);
        }

        private void SetFilter(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string filterString = "";
            DateTime today = DateTime.Today;
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
                case "季":
                    DateTime endOfQuarter = today.AddMonths(3 - (today.Month - 1) % 3).AddDays(-today.Day + 1).AddDays(-1);

                    filterString = $"[ValidLicense] = True  AND [ExpDate] IS NOT NULL AND [ExpDate] < '{endOfQuarter}' ";
                    break;
                case "半年":
                    DateTime endOfHalfYear = (new DateTime(today.Year, today.Month < 7 ? 1 : 7, 1)).AddMonths(6).AddSeconds(-1);

                    filterString = $"[ValidLicense] = True  AND [ExpDate] IS NOT NULL AND [ExpDate] < '{endOfHalfYear}' ";
                    break;
                case "年":
                    DateTime endOfYear = (new DateTime(today.Year, 1, 1)).AddMonths(12).AddSeconds(-1);

                    filterString = $"[ValidLicense] = True  AND [ExpDate] IS NOT NULL AND [ExpDate] < '{endOfYear}' ";
                    break;
            }

            gvData.ActiveFilterString = filterString;
        }

        private void btnInvalidateExpCert_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime today = DateTime.Today;
            DateTime endOfQuarter = today.AddMonths(3 - (today.Month - 1) % 3).AddDays(-today.Day + 1).AddDays(-1);

            var expCerts = lsBasesDisplay.Where(r => r.ValidLicense && r.ExpDate < endOfQuarter).ToList();

            string msg = $"系統會作廢過期證照，請您確認：</br>1、已檢查過期證照清單（{expCerts.Count}張）</br>2、已導出本季附件";
            var dialog = MsgTP.MsgYesNoQuestion(msg);
            if (dialog != DialogResult.Yes) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                foreach (var cert in expCerts)
                {
                    var certUpdate = dt301_BaseBUS.Instance.GetItemById(cert.Id);
                    if (certUpdate == null) continue;

                    certUpdate.ValidLicense = false;
                    certUpdate.InvalidLicense = true;
                    certUpdate.Describe = $"自動作廢過期證照{DateTime.Today:yyy.MM.dd}";

                    dt301_BaseBUS.Instance.AddOrUpdate(certUpdate);
                }
            }

            LoadData();
        }
    }
}
