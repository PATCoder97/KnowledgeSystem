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

        List<dt301_Base> dt301_Bases = new List<dt301_Base>();

        List<dt301_CertReqSetting> dt301_CertReqs = new List<dt301_CertReqSetting>();

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


            DataTable dt = ds.Tables[2];

            dt301_CertReqs = new List<dt301_CertReqSetting>();

            foreach (DataRow row in dt.Rows)
            {
                dt301_CertReqSetting req = new dt301_CertReqSetting();

                req.IdDept = "77";
                req.IdJobTitle = row["machuvu"].ToString().Trim();
                req.IdCourse = row["mabaihoc"].ToString().Trim(); //new actual  req

                req.NewHeadcount = Convert.ToInt16(row["new"].ToString().Trim());
                req.ActualHeadcount = Convert.ToInt16(row["actual"].ToString().Trim());
                req.ReqQuantity = Convert.ToInt16(row["req"].ToString().Trim());

                dt301_CertReqs.Add(req);
            }





            //dt301_Bases = new List<dt301_Base>();
            //var dtCourse = dt301_CourseBUS.Instance.GetList();

            //foreach (DataRow row in dt.Rows)
            //{
            //    dt301_Base dt301 = new dt301_Base();
            //    dt301.IdDept = "77";
            //    dt301.IdUser = row["人員代號"].ToString().Trim();
            //    dt301.IdJobTitle = row["新職務代號"].ToString().Trim();
            //    dt301.IdCourse = row["課程代號"].ToString().Trim();

            //    var dateReceipt = row["NgayLayBang"].ToString().Trim();
            //    dt301.DateReceipt = string.IsNullOrEmpty(dateReceipt) ? default : Convert.ToDateTime(dateReceipt);

            //    int yearAdd = dtCourse.First(r => r.Id == dt301.IdCourse).Duration ?? 1000;
            //    dt301.ExpDate = yearAdd != 0 ? dt301.DateReceipt.AddYears(yearAdd) : (DateTime?)null;

            //    var aaa = dt.Columns[24].ColumnName;

            //    var value1 = row["應取證\n人員"].ToString().Trim();
            //    dt301.ValidLicense = value1 == "Y";

            //    var value2 = row["備援\n證照"].ToString().Trim();
            //    dt301.BackupLicense = value2 == "Y";

            //    var value3 = row["無效\n證照"].ToString().Trim();
            //    dt301.InvalidLicense = value3 == "Y";

            //    dt301.Describe = row["備註"].ToString().Trim();

            //    dt301_Bases.Add(dt301);
            //}



            // Data
            //lsUser = new List<dm_User>();

            //foreach (DataRow row in dt.Rows)
            //{
            //    dm_User usr = new dm_User();
            //    usr.Id = row["人員代號"].ToString().Trim();
            //    usr.DisplayName = row["中文姓名"].ToString().Trim();
            //    usr.DisplayNameVN = row["越文姓名"].ToString().Trim();

            //    var dateCrate = row["到職日"].ToString().Trim();

            //    usr.DateCreate = string.IsNullOrEmpty(dateCrate) ? default : Convert.ToDateTime(dateCrate);
            //    usr.Status = string.IsNullOrEmpty(dateCrate) ? 1 : 0;


            //    usr.IdDepartment = row["部門代號"].ToString().Trim();
            //    usr.JobCode = row["職務代號"].ToString().Trim();
            //    usr.CitizenID = row["身份證/護照號碼"].ToString().Trim();

            //    var dateOB = row["出生日期"].ToString().Trim();
            //    usr.DOB = string.IsNullOrEmpty(dateOB) ? default : Convert.ToDateTime(dateOB);

            //    usr.Nationality = row["國籍"].ToString().Trim();

            //    lsUser.Add(usr);
            //}

            gridControl1.DataSource = dt301_CertReqs;
        }

        private void btnUploadDB_Click(object sender, EventArgs e)
        {
            // 11111111111111111
            //List<dm_User> dm_Users = dm_UserBUS.Instance.GetList();

            //foreach (var item in lsUser)
            //{
            //    if (item.Id == "VNW00004589")
            //    {

            //    }

            //    if (dm_Users.Any(r => r.Id == item.Id))
            //    {


            //        dm_User dm_UserUpdate = dm_Users.First(r => r.Id == item.Id);

            //        dm_UserUpdate.DisplayName = item.DisplayName;
            //        dm_UserUpdate.DisplayNameVN = item.DisplayNameVN;
            //        dm_UserUpdate.DateCreate = item.DateCreate;
            //        dm_UserUpdate.Status = item.Status;
            //        dm_UserUpdate.IdDepartment = item.IdDepartment;
            //        dm_UserUpdate.JobCode = item.JobCode;
            //        dm_UserUpdate.CitizenID = item.CitizenID;
            //        dm_UserUpdate.DOB = item.DOB;
            //        dm_UserUpdate.Nationality = item.Nationality;

            //        dm_UserBUS.Instance.AddOrUpdate(dm_UserUpdate);
            //    }
            //    else
            //    {
            //        dm_UserBUS.Instance.AddOrUpdate(item);
            //    }
            //}


            // 2222222222

            //foreach (var item in dt301_Bases)
            //{
            //    dt301_BaseBUS.Instance.Add(item);
            //}

            foreach (var item in dt301_CertReqs)
            {
                dt301_CertReqSetBUS.Instance.Add(item);
            }

            MessageBox.Show("ok");
        }
    }
}
