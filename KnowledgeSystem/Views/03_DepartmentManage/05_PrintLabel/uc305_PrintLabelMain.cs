using DevExpress.XtraEditors;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DocumentFormat.OpenXml.Spreadsheet;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraEditors.Controls;
using System.Management;
using SelectQuery = System.Management.SelectQuery;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using ExcelDataReader;
using System.IO;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel
{
    public partial class uc305_PrintLabelMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc305_PrintLabelMain()
        {
            InitializeComponent();
        }

        System.Drawing.Font fontUI14 = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);

        List<string> printers = new List<string>();

        List<dm_Departments> depts;

        public static DataSet ExcelToDataSet(string filePath)
        {
            string text = System.IO.Path.GetExtension(filePath).ToLower();
            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader excelDataReader = ((!(text.ToLower() == ".xls")) ? ExcelReaderFactory.CreateOpenXmlReader(fileStream) : ExcelReaderFactory.CreateBinaryReader(fileStream));
                DataSet result = excelDataReader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = (IExcelDataReader _) => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });
                excelDataReader.Close();
                return result;
            };
        }

        private List<string> GetPrinters()
        {
            List<string> printers = new List<string>();

            ManagementScope objMS = new ManagementScope(ManagementPath.DefaultPath);
            objMS.Connect();

            SelectQuery objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher(objMS, objQuery);
            ManagementObjectCollection objMOC = objMOS.Get();

            foreach (ManagementObject Printers in objMOC)
            {
                string printer_name = Printers["Name"].ToString();
                bool IsOnline = Printers["WorkOffline"].ToString().ToLower().Equals("false");

                if (IsOnline)
                {
                    printers.Add(printer_name);
                }
            }

            return printers;
        }

        private void btnFixedAssets_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDF Files|*.pdf";
            dialog.Title = "Select data";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var lsFixedAssests = new List<object>();
                List<string> lsPatterns = new List<string>
                {
                    @"使用部門(.*)",
                    @"資產編號(.*)",
                    @"資產中文名稱(.*)",
                    @"資產越文名稱(.*)",
                    @"廠牌規格(.*)",
                    @"製造產地(.*)",
                    @"取得日期(.*)"
                };

                try
                {
                    using (PdfReader reader = new PdfReader(dialog.FileName))
                    {
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                            string line = PdfTextExtractor.GetTextFromPage(reader, i);

                            var dataLine = new
                            {
                                Dept = Regex.Match(line, lsPatterns[0]).Groups[1].Value,
                                AssetId = Regex.Match(line, lsPatterns[1]).Groups[1].Value,
                                NameTW = Regex.Match(line, lsPatterns[2]).Groups[1].Value,
                                NameVN = Regex.Match(line, lsPatterns[3]).Groups[1].Value,
                                Format = Regex.Match(line, lsPatterns[4]).Groups[1].Value,
                                MadeBy = Regex.Match(line, lsPatterns[5]).Groups[1].Value,
                                DateGet = Regex.Match(line, lsPatterns[6]).Groups[1].Value,
                            };

                            lsFixedAssests.Add(dataLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex}");
                    return;
                }

                var report = new rpFixedAssets();
                report.DataSource = lsFixedAssests;
                report.CreateDocument();
                report.PrintingSystem.ShowMarginsWarning = false;
                docViewerLabel.DocumentSource = report;
            }
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var docLabel = docViewerLabel.DocumentSource;
            if (docLabel == null) return;

            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "列印記";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選擇列印記</font>";
            args.DefaultButtonIndex = 0;
            ComboBoxEdit editor = new ComboBoxEdit();

            editor.Properties.Items.AddRange(printers);
            args.Editor = editor;

            editor.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.AppearanceDropDown.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            editor.Properties.NullText = "";

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string printerName = result?.ToString() ?? "";

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                ReportPrintTool tool = new ReportPrintTool(docViewerLabel.DocumentSource as IReport);
                string print_selected = printerName;
                tool.PrinterSettings.PrinterName = print_selected;
                tool.PrintingSystem.ShowPrintStatusDialog = true;
                tool.PrintingSystem.Document.Name = "Label";
                tool.Print();
            }
        }

        private void uc305_PrintLabelMain_Load(object sender, EventArgs e)
        {
            printers = GetPrinters();

            depts = dm_DeptBUS.Instance.GetList();
        }

        private void btnDeviceManagement_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dataDefault = new
            {
                NameVN = "{NameVN}",
                NameTW = "{NameTW}",
                Code = "{Code}",
                Dept = "{Dept}",
                UserVN = "{UserVN}",
                UserTW = "{UserTW}"
            };

            var devices = new List<object>();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.Title = "Select data";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                devices.Add(dataDefault);
            }
            else
            {
                DataTable data = ExcelToDataSet(dialog.FileName).Tables[0];

                foreach (DataRow row in data.Rows)
                {
                    var dataLine = new
                    {
                        NameVN = row["NameVN"] != DBNull.Value ? row["NameVN"].ToString() : null,
                        NameTW = row["NameTW"] != DBNull.Value ? row["NameTW"].ToString() : null,
                        Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : null,
                        Dept = row["Dept"] != DBNull.Value ? row["Dept"].ToString() : null,
                        UserVN = row["UserVN"] != DBNull.Value ? row["UserVN"].ToString() : null,
                        UserTW = row["UserTW"] != DBNull.Value ? row["UserTW"].ToString() : null
                    };
                    devices.Add(dataLine);
                }
            }

            var report = new rpDeviceManagement();
            report.DataSource = devices;
            report.CreateDocument();
            report.PrintingSystem.ShowMarginsWarning = false;
            docViewerLabel.DocumentSource = report;
        }

        private void btn5SAreaDivision_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dataDefault = new
            {
                Code = "{Code}",
                DeptName = "{DeptName}",
                Manager = "{Manager}",
                Agent = "{Agent}",
                Boss = "{Boss}"
            };

            var devices = new List<object>();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.Title = "Select data";
            dialog.Multiselect = false;

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                devices.Add(dataDefault);
            }
            else
            {
                DataTable data = ExcelToDataSet(dialog.FileName).Tables[0];

                foreach (DataRow row in data.Rows)
                {
                    var dataLine = new
                    {
                        Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : null,
                        DeptName = row["DeptName"] != DBNull.Value ? row["DeptName"].ToString() : null,
                        Manager = row["Manager"] != DBNull.Value ? row["Manager"].ToString() : null,
                        Agent = row["Agent"] != DBNull.Value ? row["Agent"].ToString() : null,
                        Boss = row["Boss"] != DBNull.Value ? row["Boss"].ToString() : null
                    };
                    devices.Add(dataLine);
                }
            }

            var report = new rp5SAreaDivision();
            report.DataSource = devices;
            report.CreateDocument();
            report.PrintingSystem.ShowMarginsWarning = false;
            docViewerLabel.DocumentSource = report;
        }

        private void btnWasteLabel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = TPConfigs.SoftNameTW;
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請輸入數量</font>";
            args.DefaultButtonIndex = 0;
            TextEdit editor = new TextEdit();
            editor.Properties.DisplayFormat.FormatString = "n0";
            editor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            editor.Properties.MaskSettings.Set("mask", "n0");

            args.Editor = editor;

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            editor.Properties.NullText = "";

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            int numLabel = Convert.ToInt16(result?.ToString() ?? "1");

            object[] labels = Enumerable.Range(0, numLabel)
                                     .Select(_ => new
                                     {
                                         NameVN = TPConfigs.LoginUser.DisplayNameVN,
                                         NameTW = TPConfigs.LoginUser.DisplayName,
                                         Dept = TPConfigs.LoginUser.IdDepartment,
                                     }).ToArray();

            var report = new rpWasteLabel();
            report.DataSource = labels;
            report.CreateDocument();
            report.PrintingSystem.ShowMarginsWarning = false;
            docViewerLabel.DocumentSource = report;
        }

        private void btnCabinetManage_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = TPConfigs.SoftNameTW;
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請輸入數量</font>";
            args.DefaultButtonIndex = 0;
            TextEdit editor = new TextEdit();
            editor.Properties.DisplayFormat.FormatString = "n0";
            editor.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.NumericMaskManager));
            editor.Properties.MaskSettings.Set("mask", "n0");

            args.Editor = editor;

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            editor.Properties.NullText = "";

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            int numLabel = Convert.ToInt16(result?.ToString() ?? "1");

            object[] labels = Enumerable.Range(0, numLabel)
                                     .Select(_ => new
                                     {
                                         NameVN = TPConfigs.LoginUser.DisplayNameVN,
                                         NameTW = TPConfigs.LoginUser.DisplayName,
                                         Dept = depts.FirstOrDefault(r => r.Id == TPConfigs.LoginUser.IdDepartment)?.DisplayName,
                                     }).ToArray();

            var report = new rpCabinetManage();
            report.DataSource = labels;
            report.CreateDocument();
            report.PrintingSystem.ShowMarginsWarning = false;
            docViewerLabel.DocumentSource = report;
        }
    }
}
