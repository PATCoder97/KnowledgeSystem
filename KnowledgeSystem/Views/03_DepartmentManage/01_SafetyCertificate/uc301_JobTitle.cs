using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Browsing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using OfficeOpenXml;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class uc301_JobTitle : DevExpress.XtraEditors.XtraUserControl
    {
        public uc301_JobTitle()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<dm_JobTitle> lsJobTitles = new List<dm_JobTitle>();
        BindingSource sourceJobTitles = new BindingSource();

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            sourceJobTitles.DataSource = lsJobTitles;

            gvData.BestFitColumns();
        }

        private void uc301_JobTitle_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();

            gcData.DataSource = sourceJobTitles;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f301_JobTitleInfo fInfo = new f301_JobTitleInfo();
            fInfo._eventInfo = EventFormInfo.Create;
            fInfo._formName = "職務";
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
            dm_JobTitle _jobSelect = view.GetRow(view.FocusedRowHandle) as dm_JobTitle;

            f301_JobTitleInfo fInfo = new f301_JobTitleInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo._formName = "用戶";
            fInfo._jobTitle = _jobSelect;
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string dataPath = "";
            DataSet ds;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel files (*.xls, *.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataPath = openFileDialog.FileName;
                }
            }

            string extension = Path.GetExtension(dataPath);
            using (var stream = File.Open(dataPath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                if (extension == "*.xls")
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close();
            }

            List<dm_JobTitle> lsJobTitles = new List<dm_JobTitle>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lsJobTitles.Add(new dm_JobTitle() { Id = row[0]?.ToString().Trim(), DisplayName = row[1]?.ToString().Trim() });
            }

            foreach (var item in lsJobTitles)
            {
                dm_JobTitleBUS.Instance.AddOrUpdate(item);
            }
        }
    }
}
