using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_QuesMgmt : DevExpress.XtraEditors.XtraForm
    {
        public f312_QuesMgmt(int _idGroup)
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvQues, "Id");

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;

            idGroup = _idGroup;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        RefreshHelper helper;
        BindingSource sourceQues = new BindingSource();

        int idGroup = -1;

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
                var ques = dt312_QuestionsBUS.Instance.GetListByIdGroup(idGroup);

                sourceQues.DataSource = ques;

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

                var ques = dt312_QuestionsBUS.Instance.GetListByIdGroup(idGroup);

                sourceQues.DataSource = ques;
                helper.LoadViewInfo();

                gvQues.BestFitColumns();
                gvQues.CollapseAllDetails();
            }
        }

        private void f312_QuesMgmt_Load(object sender, EventArgs e)
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

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "HaveImg" && e.IsGetData)
            {
                string imgName = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "ImageName")?.ToString();

                e.Value = !string.IsNullOrWhiteSpace(imgName);
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        static void CleanDataTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] is string)
                    {
                        row[column] = CleanText(row[column].ToString());
                    }
                }
            }
        }

        static string CleanText(string input)
        {
            string cleaned = Regex.Replace(input, @"[\t\n\r\s]+", match =>
            {
                if (match.Value.Contains("\n"))
                {
                    return "\r\n";
                }
                else
                {
                    return " ";
                }
            }).Trim();

            return cleaned;
        }

        private void btnUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string excelPath = "", pathImages = "";
            DataSet ds;
            List<dt312_Questions> ques = new List<dt312_Questions>();
            List<dt312_Answers> answers = new List<dt312_Answers>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;
                excelPath = openFileDialog.FileName;
                pathImages = $@"{Path.GetDirectoryName(excelPath)}\Images";
            }

            string extension = Path.GetExtension(excelPath);
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
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

            DataTable dtBase = ds.Tables[0];
            CleanDataTable(dtBase);

            int currentQuesId = 0;
            int answerId = 1; // Biến đếm để theo dõi ID hiện tại cho câu trả lời

            for (int row = 0; row < dtBase.Rows.Count; row++)
            {
                int quesId = 0;
                int.TryParse(dtBase.Rows[row][0].ToString(), out quesId);

                if (quesId != currentQuesId && quesId != 0)
                {
                    // Thêm câu hỏi mới
                    ques.Add(new dt312_Questions()
                    {
                        DisplayName = dtBase.Rows[row][1].ToString(),
                        ImageName = string.IsNullOrEmpty(dtBase.Rows[row][2].ToString()) ? "" : Path.Combine(pathImages, dtBase.Rows[row][2].ToString()),
                    });
                    currentQuesId = quesId;
                    answerId = 1; // Đặt lại biến đếm answerId khi câu hỏi đổi
                }

                int resultAns = 0;
                int.TryParse(dtBase.Rows[row][6].ToString(), out resultAns);
                bool IsTrueAns = resultAns == 1;

                // Thêm câu trả lời mới
                answers.Add(new dt312_Answers()
                {
                    Id = answerId++, // Tăng ID mỗi khi thêm câu trả lời mới
                    QuesId = currentQuesId,
                    DisplayName = dtBase.Rows[row][4].ToString(),
                    ImageName = string.IsNullOrEmpty(dtBase.Rows[row][5].ToString()) ? "" : Path.Combine(pathImages, dtBase.Rows[row][5].ToString()),
                    TrueAns = IsTrueAns
                });
            }

            f307_ConfirmQues fConfirm = new f307_ConfirmQues();
            fConfirm.ques = ques.Select(r => new f307_ConfirmQues.CheckQuestion() { DisplayText = r.DisplayName, ImageName = r.ImageName }).ToList();
            fConfirm.answers = answers.Select(r => new f307_ConfirmQues.CheckAnswer() { QuesId = r.QuesId, Id = r.Id, DisplayText = r.DisplayName, ImageName = r.ImageName, TrueAns = r.TrueAns }).ToList();
            fConfirm.ShowDialog();

            // Nếu xác nhận thành công thì cập nhật vào CSDL
            bool IsConfirmed = fConfirm.IsConfirmed;
            if (!IsConfirmed) return;

            if (!Directory.Exists(TPConfigs.Folder307))
                Directory.CreateDirectory(TPConfigs.Folder307);

            int index = 1;
            foreach (var item in ques)
            {
                var data = new dt312_Questions()
                {
                    GroupId = idGroup,
                    DisplayName = item.DisplayName,
                    ImageName = string.IsNullOrEmpty(item.ImageName) ? ""
                    : $"{EncryptionHelper.EncryptionFileName(Path.GetFileName(item.ImageName))}{Path.GetExtension(item.ImageName)}"
                };

                int idQues = dt312_QuestionsBUS.Instance.Add(data);
                answers.Where(r => r.QuesId == index).ToList().ForEach(r => r.QuesId = idQues);
                index++;

                if (string.IsNullOrEmpty(item.ImageName)) continue;
                File.Copy(item.ImageName, Path.Combine(TPConfigs.Folder307, data.ImageName), true);
            }

            List<dt312_Answers> anwsAdd = new List<dt312_Answers>();
            foreach (var item in answers)
            {
                var data = new dt312_Answers()
                {
                    QuesId = item.QuesId,
                    DisplayName = item.DisplayName,
                    TrueAns = item.TrueAns,
                    ImageName = string.IsNullOrEmpty(item.ImageName) ? ""
                    : $"{EncryptionHelper.EncryptionFileName(Path.GetFileName(item.ImageName))}{Path.GetExtension(item.ImageName)}"
                };

                anwsAdd.Add(data);

                if (string.IsNullOrEmpty(item.ImageName)) continue;
                File.Copy(item.ImageName, Path.Combine(TPConfigs.Folder307, data.ImageName), true);
            }

            var b = dt312_AnswersBUS.Instance.AddRange(anwsAdd);

            Close();
        }
    }
}