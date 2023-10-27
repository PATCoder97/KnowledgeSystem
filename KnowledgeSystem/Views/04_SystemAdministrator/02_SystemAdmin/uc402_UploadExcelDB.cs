using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using ExcelDataReader;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_UploadExcelDB : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_UploadExcelDB()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
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

            //List<dt301_CertReqSetting> lsCourse = new List<dt301_CertReqSetting>();

            //foreach (DataRow row in ds.Tables[0].Rows)
            //{
            //    lsCourse.Add(new dt301_CertReqSetting()
            //    {
            //        IdDept = "78",
            //        IdJobTitle = row["IdJob"].ToString().Trim(),
            //        IdCourse = row["IdCourse"].ToString().Trim(),
            //        NewHeadcount = Convert.ToInt16(row["New"].ToString().Trim()),
            //        ActualHeadcount = Convert.ToInt16(row["Actual"].ToString().Trim()),
            //        ReqQuantity = Convert.ToInt16(row["Req"].ToString().Trim()),
            //    });
            //}

            //foreach (var item in lsCourse)
            //{
            //    dt301_CertReqSetBUS.Instance.AddOrUpdate(item);
            //}



            // Data
            List<dt301_Base> lsCourse = new List<dt301_Base>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                lsCourse.Add(new dt301_Base()
                {
                    IdDept = "78",
                    IdUser = row["IdUser"].ToString().Trim(),
                    IdJobTitle = row["IdJob"].ToString().Trim(),
                    IdCourse = row["IdCourse"].ToString().Trim(),
                    DateReceipt = DateTime.Parse(row["DateReceipt"].ToString().Trim()),
                    ExpDate = DateTime.Parse(row["DateReceipt"].ToString().Trim()).AddYears(Convert.ToInt16(row["DateCount"].ToString().Trim())),
                    Describe = row["Describe"].ToString().Trim(),
                    ValidLicense = row["Valid"].ToString().Trim() == "Y",
                    BackupLicense = row["Backup"].ToString().Trim() == "Y",
                    InvalidLicense = row["Invalid"].ToString().Trim() == "Y",
                });
            }

            foreach (var item in lsCourse)
            {
                dt301_BaseBUS.Instance.AddOrUpdate(item);
            }

            MessageBox.Show($"{lsCourse.Count}");
        }
    }
}
