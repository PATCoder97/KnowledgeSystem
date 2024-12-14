using BusinessLayer;
using DataAccessLayer;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Drawing.Charts;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_UploadExcelDB : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_UploadExcelDB()
        {
            InitializeComponent();
        }

        private void uc402_UploadExcelDB_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private DataSet OpenFile()
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

            return ds;
        }

        List<dt201_Base> bases = new List<dt201_Base>();

        private void btnOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //bases = new List<dt201_Base>();
            //DataSet ds = OpenFile();

            //System.Data.DataTable dt03 = ds.Tables[0];

            //foreach (DataRow item in dt03.Rows)
            //{
            //    dt201_Base data = new dt201_Base();

            //    data.IdDept = "7730";
            //    data.NotifyCycle = 1;
            //    data.DocType = "";
            //    data.DocCode = item[5].ToString();

            //    data.IdParent = Convert.ToInt16(item[10].ToString().Trim());
            //    var Name = item[6].ToString().Trim().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            //    if (Name.Count() > 2)
            //    {
            //        data.IsDel = true;
            //    }

            //    if (data.DocCode.Length != 14)
            //    {
            //        data.IsDel = true;
            //    }

            //    data.DisplayName = Name[1];
            //    data.DisplayNameVN = Name[0];
            //    data.IdRecordCode = Convert.ToInt16(item[9].ToString().Trim());

            //    bases.Add(data);
            //}

            //gcData.DataSource = bases;
        }

        private void btnUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //foreach (var item in bases)
            //{
            //    dt201_BaseBUS.Instance.Add(item);
            //}

            //XtraMessageBox.Show("OK");
        }

    }
}
