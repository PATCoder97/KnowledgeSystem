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

        List<dm_User> lsUser = new List<dm_User>();

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

            DataTable dt = ds.Tables[0];

            // Data
            lsUser = new List<dm_User>();

            foreach (DataRow row in dt.Rows)
            {
                dm_User usr = new dm_User();
                usr.Id = row["人員代號"].ToString().Trim();
                usr.DisplayName = row["中文姓名"].ToString().Trim();
                usr.DisplayNameVN = row["越文姓名"].ToString().Trim();

                var dateCrate = row["到職日"].ToString().Trim();

                usr.DateCreate = string.IsNullOrEmpty(dateCrate) ? default : Convert.ToDateTime(dateCrate);
                usr.Status = string.IsNullOrEmpty(dateCrate) ? 1 : 0;


                usr.IdDepartment = row["部門代號"].ToString().Trim();
                usr.JobCode = row["職務代號"].ToString().Trim();
                usr.CitizenID = row["身份證/護照號碼"].ToString().Trim();

                var dateOB = row["出生日期"].ToString().Trim();
                usr.DOB = string.IsNullOrEmpty(dateOB) ? default : Convert.ToDateTime(dateOB);

                usr.Nationality = row["國籍"].ToString().Trim();

                lsUser.Add(usr);
            }

            gridControl1.DataSource = lsUser;
        }

        private void btnUploadDB_Click(object sender, EventArgs e)
        {
            List<dm_User> dm_Users = dm_UserBUS.Instance.GetList();

            foreach (var item in lsUser)
            {
                if (item.Id == "VNW00004589")
                {

                }

                if (dm_Users.Any(r => r.Id == item.Id))
                {
                    

                    dm_User dm_UserUpdate = dm_Users.First(r => r.Id == item.Id);

                    dm_UserUpdate.DisplayName = item.DisplayName;
                    dm_UserUpdate.DisplayNameVN = item.DisplayNameVN;
                    dm_UserUpdate.DateCreate = item.DateCreate;
                    dm_UserUpdate.Status = item.Status;
                    dm_UserUpdate.IdDepartment = item.IdDepartment;
                    dm_UserUpdate.JobCode = item.JobCode;
                    dm_UserUpdate.CitizenID = item.CitizenID;
                    dm_UserUpdate.DOB = item.DOB;
                    dm_UserUpdate.Nationality = item.Nationality;

                    dm_UserBUS.Instance.AddOrUpdate(dm_UserUpdate);
                }
                else
                {
                    dm_UserBUS.Instance.AddOrUpdate(item);
                }
            }

            MessageBox.Show("ok");
        }
    }
}
