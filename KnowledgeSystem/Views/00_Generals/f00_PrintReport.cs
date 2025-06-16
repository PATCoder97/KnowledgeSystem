using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_PrintReport : DevExpress.XtraEditors.XtraForm
    {
        public f00_PrintReport()
        {
            InitializeComponent();
        }
        public DocumentViewer ViewerReport => viewerReport;

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

        private void f00_PrintReport_Load(object sender, EventArgs e)
        {
            var printers = GetPrinters();
            cbbPrinter.Items.AddRange(printers);
            barCbbPrinter.EditValue = printers.FirstOrDefault() ?? string.Empty;
        }

        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var report = ViewerReport.DocumentSource as XtraReport;
                ReportPrintTool tool = new ReportPrintTool(report);
                string print_selected = barCbbPrinter.EditValue.ToString();
                tool.PrinterSettings.PrinterName = print_selected;
                tool.PrintingSystem.ShowPrintStatusDialog = true;
                tool.PrintingSystem.Document.Name = "Label";
                tool.Print();
            }
        }
    }
}