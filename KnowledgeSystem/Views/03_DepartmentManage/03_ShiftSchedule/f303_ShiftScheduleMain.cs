using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;


namespace KnowledgeSystem.Views._03_DepartmentManage._03_ShiftSchedule
{
    public partial class f303_ShiftScheduleMain : DevExpress.XtraEditors.XtraForm
    {
        public f303_ShiftScheduleMain()
        {
            InitializeComponent();
        }

        // Import the mouse_event function from user32.dll
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        // Constants for mouse event flags
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ExStyle |= 0x08000000;
                return param;
            }
        }

        int numDaysInMonth = 0;
        Dictionary<string, string> refTableDatas;
        Dictionary<string, string> shiftDatas;
        string pathShiftData = "";
        int indexUsr = 1;

        string userFocus = "";

        List<string> usrs = new List<string>();
        List<dm_User> usersDB;

        static string GenerateERPString(string original, int numTab)
        {
            string last40Chars = original.Substring(original.Length - 40);
            string firstChars = original.Substring(0, original.Length - 40);
            string tabString = string.Join("", new string[numTab].Select(_ => "{Tab}"));

            return $"{firstChars}{tabString}{last40Chars}";
        }

        private void f303_ShiftScheduleMain_Load(object sender, EventArgs e)
        {
            Text = "人員排班";
            TopMost = true;

            StartPosition = FormStartPosition.Manual;
            int x = Screen.PrimaryScreen.WorkingArea.Right - Width;
            int y = Screen.PrimaryScreen.WorkingArea.Top;
            Location = new Point(x, y);

            btnExcel.Enabled = false;
            cbbSheet.Enabled = false;
            btnDel.Enabled = false;
            btnAdd.Enabled = false;
            btnPrevious.Enabled = false;
            btnNext.Enabled = false;
            btnSave.Enabled = false;
            //btnEdit.Enabled = false;

            lbUser.Text = "";

            usersDB = dm_UserBUS.Instance.GetList();
        }

        private void btnPdf_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            string pathRefTable = openFileDialog.FileName;

            string line = "";
            using (PdfReader reader = new PdfReader(pathRefTable))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    line += Regex.Replace(PdfTextExtractor.GetTextFromPage(reader, i), @"\s", "");
                }
            }

            refTableDatas = new Dictionary<string, string>();
            Regex regex = new Regex(@"D[5-8][休中早夜日]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            MatchCollection matchCollection = regex.Matches(line);
            foreach (Match match in matchCollection)
            {
                string key = match.Value.Substring(0, 2);
                string value = match.Value.Replace(key, "").Replace("夜", "夜班").Replace("早", "早班").Replace("中", "中班").Replace("日", "日班").Replace("休", "休班");

                numDaysInMonth = value.Length;
                refTableDatas.Add(key, value);
            }

            btnExcel.Enabled = refTableDatas.Count != 0;
            cbbSheet.Enabled = refTableDatas.Count != 0;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            pathShiftData = openFileDialog.FileName;

            List<string> dataUser = new List<string>();
            Dictionary<string, string> datasShift = new Dictionary<string, string>();

            FileInfo newFile = new FileInfo(pathShiftData);
            //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                var sheetsName = pck.Workbook.Worksheets.Select(r => r.Name).ToList();
                cbbSheet.Properties.Items.Clear();
                cbbSheet.Properties.Items.AddRange(sheetsName);
                if (sheetsName.Count != 0)
                    cbbSheet.SelectedIndex = 0;
            }
        }

        private void cbbSheet_SelectedIndexChanged(object sender, EventArgs e)
        {
            shiftDatas = new Dictionary<string, string>();
            indexUsr = 1;

            FileInfo newFile = new FileInfo(pathShiftData);
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                var wsShift = pck.Workbook.Worksheets[cbbSheet.EditValue?.ToString()];

                var dimension = wsShift.Dimension.Address;
                ExcelRange range = wsShift.Cells[dimension];
                var numRows = range.Rows;

                for (int i = 0; i < numRows; i++)
                {
                    string line = string.Join("", range.TakeSingleRow(i)
                                                       .Select(r => r.Text.Trim().Replace("休", "FB").Replace("早", "早班").Replace("中", "中班").Replace("日", "日班").Replace("夜", "夜班"))
                                                       .ToList());

                    Regex regex = new Regex(@"VNW\d{7}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
                    var matchCollection = regex.Matches(line);
                    if (matchCollection.Count == 0) continue;

                    string userID = matchCollection[0].ToString();

                    regex = new Regex(@"[休中早夜日班GOFB]{40,}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
                    matchCollection = regex.Matches(line);
                    if (matchCollection.Count == 0) continue;

                    string match = matchCollection[0].ToString();

                    if (match.Length < numDaysInMonth) continue;
                    string shift = match.Substring(match.Length - numDaysInMonth, numDaysInMonth);

                    string outputString = "";
                    int numChar = shift.Length / 2;
                    int numTab = 31 - numChar;

                    // Kiểm tra từng ký tự của chuỗi dữ liệu
                    for (int j = 0; j < numChar; j++)
                    {
                        string currentString = $"{shift[j * 2]}{shift[j * 2 + 1]}";
                        string result = refTableDatas.FirstOrDefault(kvp => currentString == $"{kvp.Value[j * 2]}{kvp.Value[j * 2 + 1]}").Key ?? "";

                        outputString += string.IsNullOrEmpty(result) ? currentString : result;
                    }

                    outputString = GenerateERPString(outputString, numTab);

                    //// Chuyển ca 日班 thành GO khi có nhân viên đi ca về ca 日
                    //outputString = outputString.Replace("日班", "GO");

                    if (!outputString.Contains("班"))
                        shiftDatas.Add(userID, outputString);
                }

                usrs = shiftDatas.Keys.ToList();
                if (usrs.Count == 0)
                {
                    txbIndex.EditValue = $"0/{shiftDatas.Count}";
                    lbUser.Text = "";

                    btnExcel.Enabled = true;
                    btnPdf.Enabled = true;
                    btnAdd.Enabled = false;
                    btnDel.Enabled = false;
                    btnPrevious.Enabled = false;
                    btnNext.Enabled = false;
                    btnSave.Enabled = false;
                    btnEdit.Enabled = false;
                }
                else
                {
                    txbIndex.EditValue = $"{indexUsr}/{shiftDatas.Count}";

                    string userID = usrs[indexUsr - 1];
                    string userName = usersDB.FirstOrDefault(r => r.Id == userID)?.DisplayName;
                    lbUser.Text = $"{userID} {userName}";
                    userFocus = userID;

                    btnExcel.Enabled = false;
                    btnPdf.Enabled = false;
                    btnAdd.Enabled = true;
                    btnDel.Enabled = true;
                    btnPrevious.Enabled = true;
                    btnNext.Enabled = true;
                    btnSave.Enabled = true;
                    btnEdit.Enabled = true;
                }
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (indexUsr <= 1) return;

            indexUsr--;
            txbIndex.EditValue = $"{indexUsr}/{shiftDatas.Count}";

            string userID = usrs[indexUsr - 1];
            string userName = usersDB.FirstOrDefault(r => r.Id == userID)?.DisplayName;
            lbUser.Text = $"{userID} {userName}";
            userFocus = userID;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (shiftDatas == null || indexUsr >= shiftDatas.Count) return;

            indexUsr++;
            txbIndex.EditValue = $"{indexUsr}/{shiftDatas.Count}";

            string userID = usrs[indexUsr - 1];
            string userName = usersDB.FirstOrDefault(r => r.Id == userID)?.DisplayName;
            lbUser.Text = $"{userID} {userName}";
            userFocus = userID;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string user = userFocus;
            string data = string.Join("{Tab}", new string[31].Select(_ => "{DEL}"));

            SendKeys.SendWait($"LG{user}");
            Thread.Sleep(1000);
            SendKeys.SendWait("{Enter}");

            SendKeys.SendWait(data);

            Thread.Sleep(1000);
            SendKeys.SendWait("%(a)");
            SendKeys.SendWait("S");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string user = userFocus;
            string data = shiftDatas[user];

            SendKeys.SendWait($"LG{user}");
            Thread.Sleep(1000);

            SendKeys.SendWait("{Enter}");
            SendKeys.SendWait(data);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            SendKeys.SendWait("%(a)");
            SendKeys.SendWait("S");

            btnNext_Click(sender, e);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string user = userFocus;
            string data = shiftDatas[user];

            SendKeys.SendWait($"LG{user}");
            Thread.Sleep(1000);
            SendKeys.SendWait("{Enter}");

            string inputString = Regex.Replace(data, @"\{[^}]*\}", "{}");
            var result = Enumerable.Range(0, inputString.Length / 2)
            .Select(i => inputString.Substring(i * 2, 2).Replace("{}", "")).ToList();

            int today = DateTime.Today.Day;
            int indexStart = today < 21 ? today + 11 : today - 20;
            if (indexStart >= 31) return;

            int flag = 0;
            for (int j = 0; j < 3; j++)
            {
                int x = j == 0 ? 168 : 113;
                int y = 290 + j * 85;

                for (int i = 0; i < 10; i++)
                {
                    flag++;
                    string shift = (j == 0 && i < indexStart - 1) ? "" : result[flag];
                    SendMouseAndKey(x, y, shift);
                    x += 55;
                }
            }
        }

        static void SendMouseAndKey(int x, int y, string shift = "")
        {
            Cursor.Position = new System.Drawing.Point(x, y);
            if (string.IsNullOrEmpty(shift)) return;

            // Simulate left mouse
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            for (int i = 0; i < 4; i++)
            {
                SendKeys.SendWait("{LEFT}");
                Thread.Sleep(200);
            }

            for (int i = 0; i < 3; i++)
            {
                SendKeys.SendWait("{DEL}");
                Thread.Sleep(200);
            }

            SendKeys.SendWait(shift);
            Thread.Sleep(200);
        }
    }
}
