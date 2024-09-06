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
using KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel;
using BusinessLayer;
using KnowledgeSystem.Helpers;
using System.Diagnostics;
using System.IO;

namespace KnowledgeSystem.Views._03_DepartmentManage._03_ShiftSchedule
{
    public partial class uc303_SickLeaveData : DevExpress.XtraEditors.XtraUserControl
    {
        private class SickData
        {
            public string Id { get; set; }
            public List<string> Data { get; set; }
            public double TotalTime { get; set; }
            public double Count { get; set; }
        }

        public uc303_SickLeaveData()
        {
            InitializeComponent();
            InitializeIcon();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select a PDF file";
                openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                FOLDER_PATH = openFileDialog.FileName;
            }
        }

        string FOLDER_PATH;

        private void InitializeIcon()
        {
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void uc303_SickLeaveData_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FOLDER_PATH))
            {
                MsgTP.MsgError("請選檔案！");
                return;
            }

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvDocs.ReadOnlyGridView();

            string line = "";
            using (PdfReader reader = new PdfReader(FOLDER_PATH))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    line += PdfTextExtractor.GetTextFromPage(reader, i);
                }
            }

            string[] lines = line.Split(new string[] { "VNW" }, StringSplitOptions.None);

            List<SickData> sickDatas = new List<SickData>();

            foreach (var item in lines)
            {
                // Điều chỉnh regex như đã nêu ở trên
                Regex regex = new Regex(@"\d{8}\s+0[1|2]\s+\d{4}\s+\d{4}\s+[\d.]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
                MatchCollection matchCollection = regex.Matches(item);

                Regex regex1 = new Regex(@"\b\d{7}\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
                MatchCollection matchCollection1 = regex1.Matches(item);

                SickData data = new SickData();

                // Nếu không có kết quả khớp, bỏ qua vòng lặp
                if (matchCollection1.Count <= 0 || matchCollection.Count <= 0) continue;

                // Lấy ID
                data.Id = "VNW" + matchCollection1.Cast<Match>().FirstOrDefault()?.Value;

                // Tính tổng các số cuối cùng trong matchCollection
                double total = matchCollection
                    .Cast<Match>()
                    .Select(s => s.Value.Trim())               // Xóa các khoảng trắng không cần thiết
                    .Select(s => s.Substring(s.LastIndexOf(' ') + 1)) // Lấy phần tử cuối cùng sau dấu cách
                    .Where(lastPart => double.TryParse(lastPart, out _))  // Kiểm tra xem có thể chuyển đổi thành số không
                    .Select(lastPart => double.Parse(lastPart))           // Chuyển thành số thực
                    .Sum();

                // Lưu kết quả
                data.Data = matchCollection.Cast<Match>().Select(x => x.Value).ToList();
                data.TotalTime = total;
                data.Count = matchCollection.Count;


                // Thêm vào danh sách datas
                sickDatas.Add(data);

            }

            var usrs = dm_UserBUS.Instance.GetList();

            var results = (from data in sickDatas
                           join usr in usrs on data.Id equals usr.Id into dtg
                           from g in dtg.DefaultIfEmpty()
                           select new
                           {
                               data,
                               g
                           }).ToList();

            gcData.DataSource = results;
            gvData.BestFitColumns();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = System.IO.Path.Combine(documentsPath, $"病假統計 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
