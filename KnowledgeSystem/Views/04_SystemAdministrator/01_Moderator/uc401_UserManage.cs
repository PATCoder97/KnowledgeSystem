using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table.PivotTable;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using DevExpress.Data.Browsing;
using ExcelDataReader;
using System.Threading;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using DevExpress.XtraGrid.Views.Items.ViewInfo;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_UserManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc401_UserManage()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        #region parameters

        bool IsSysAdmin = false;
        bool IsUploadUser = false;

        Font fontDFKaiSB12 = new Font("DFKai-SB", 12.0f, FontStyle.Regular);
        BindingSource sourceUsers = new BindingSource();
        string sheetName = "DataUser";
        List<dm_User> lsUserMs;

        DXMenuItem itemEditRole;
        DXMenuItem itemEditSign;

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

        private void InitializeMenuItems()
        {
            itemEditRole = CreateMenuItem("設定用色", ItemEditRole_Click, TPSvgimages.Num1);
            itemEditSign = CreateMenuItem("設定簽名", ItemEditSign_Click, TPSvgimages.Num2);
        }

        private void ItemEditSign_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            f402_UserSigns fSetting = new f402_UserSigns();
            fSetting.eventInfo = EventFormInfo.View;
            fSetting.idUsr = idUser;
            fSetting.ShowDialog();
        }

        private void ItemEditRole_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            f402_UserRoles fSetting = new f402_UserRoles();
            fSetting.eventInfo = EventFormInfo.View;
            fSetting.idUsr = idUser;
            fSetting.ShowDialog();
        }

        private class dmUserM : dm_User
        {
            public string RoleName { get; set; }
            public string DeptName { get; set; }
            public string JobName { get; set; }
            public string ActualJobName { get; set; }
            public string Describe { get; set; }
            public string SexName { get; set; }
            public string StatusName { get; set; }
        }

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeControl()
        {
            gColPCName.Visible = IsSysAdmin;
            gColIP.Visible = IsSysAdmin;
            gColLastUpdate.Visible = IsSysAdmin;
        }

        private void LoadUser()
        {
            helper.SaveViewInfo();

            string idDept2Word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
            List<dm_User> lsUsers = new List<dm_User>();
            if (TPConfigs.IdParentControl == AppPermission.SysAdmin || TPConfigs.IdParentControl == AppPermission.Mod)
            {
                lsUsers = dm_UserBUS.Instance.GetList();
            }
            else if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            {
                lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2Word);
            }

            List<dm_Departments> lsDepts = dm_DeptBUS.Instance.GetList();
            List<dm_Role> lsRoles = dm_RoleBUS.Instance.GetList();
            List<dm_JobTitle> lsJobTitles = dm_JobTitleBUS.Instance.GetList();

            var lsUserManage = (from data in lsUsers
                                join depts in lsDepts on data.IdDepartment equals depts.Id
                                join job in lsJobTitles on data.JobCode equals job.Id into dtg
                                from g in dtg.DefaultIfEmpty()
                                join actualJob in lsJobTitles on data.ActualJobCode equals actualJob.Id into atg
                                from a in atg.DefaultIfEmpty()
                                let displayName = $"{data.DisplayName}{(!string.IsNullOrEmpty(data.DisplayNameVN) ? $"\r\n{data.DisplayNameVN}" : "")}"
                                let deptName = $"{data.IdDepartment}\r\n{depts.DisplayName}"
                                let sexName = data.Sex == null ? "" : data.Sex.Value ? "男" : "女"
                                let statusName = data.Status == null ? "" : TPConfigs.lsUserStatus[data.Status.Value]
                                select new dmUserM()
                                {
                                    Id = data.Id,
                                    DisplayName = displayName,
                                    DisplayNameVN = data.DisplayNameVN,
                                    IdDepartment = data.IdDepartment,
                                    DateCreate = data.DateCreate,
                                    SecondaryPassword = data.SecondaryPassword,
                                    DeptName = deptName,
                                    DOB = data.DOB,
                                    CitizenID = data.CitizenID,
                                    Nationality = data.Nationality,
                                    PCName = data.PCName,
                                    IPAddress = data.IPAddress,
                                    JobCode = data.JobCode,
                                    JobName = g != null ? g.DisplayName : "",
                                    ActualJobCode = data.ActualJobCode,
                                    ActualJobName = a != null ? a.DisplayName : "",
                                    Addr = data.Addr,
                                    PhoneNum1 = data.PhoneNum1,
                                    PhoneNum2 = data.PhoneNum2,
                                    Sex = data.Sex,
                                    SexName = sexName,
                                    Status = data.Status,
                                    StatusName = statusName,
                                    LastUpdate = data.LastUpdate,
                                }).ToList();

            sourceUsers.DataSource = lsUserManage;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();

            IsUploadUser = false;
        }

        private void DownloadTempExcel(string savePath)
        {
            SplashScreenManager.ShowDefaultWaitForm();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage())
            {
                pck.Workbook.Properties.Author = "VNW0014732";
                pck.Workbook.Properties.Company = "FHS";
                pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);

                // Định dạng toàn Sheet
                ws.Cells.Style.Font.Name = "Times New Roman";
                ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells.Style.Font.Size = 14;
                ws.Cells.Style.WrapText = true;

                // Thêm dữ liệu từ Grid vào Excel
                ws.Cells["A1"].Value = "Mã bộ phận\n單位";
                ws.Cells["B1"].Value = "Mã nhân viên\n人員代號";
                ws.Cells["C1"].Value = "Tên tiếng trung\n中文名稱";
                ws.Cells["D1"].Value = "Tên tiếng việt\n越文名稱";
                ws.Cells["E1"].Value = "Ngày sinh\n出生日期";
                ws.Cells["F1"].Value = "CCCD\n護照號碼";
                ws.Cells["G1"].Value = "Quốc tịch\n國籍(VN/TW/CN)";
                ws.Cells["H1"].Value = "Mã chức vụ\n職務代號";
                ws.Cells["I1"].Value = "Ngày nhận việc\n到職日";

                int sumColumn = ws.Dimension.Columns;
                int sumRow = ws.Dimension.Rows;

                ws.Columns[1].Width = 20;
                ws.Columns[2].Width = 20;
                ws.Columns[3].Width = 20;
                ws.Columns[4].Width = 20;
                ws.Columns[5].Width = 20;
                ws.Columns[6].Width = 20;
                ws.Columns[7].Width = 20;
                ws.Columns[8].Width = 20;

                // Define the data range on the source sheet
                var dataRange = ws.Cells[ws.Dimension.Address];

                //Ading a table to a Range
                ExcelTable tab = ws.Tables.Add(dataRange, "Table1");

                //Formating the table style
                tab.TableStyle = TableStyles.Medium2;

                FileInfo excelFile = new FileInfo(savePath);
                pck.SaveAs(excelFile);
            }
            SplashScreenManager.CloseDefaultWaitForm();
        }

        #endregion

        private void f401_UserManager_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            InitializeControl();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceUsers;

            LoadUser();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "用戶";
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadUser();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            if (IsUploadUser) return;

            GridView view = sender as GridView;
            dm_User _userSelect = view.GetRow(view.FocusedRowHandle) as dm_User;

            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "用戶";
            fInfo.userInfo = _userSelect;
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnUploadList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var dialog = XtraMessageBox.Show("Bạn có muốn tải xuống tệp mẫu không?\n- Ấn \"OK\" để tải xuống\n- Ấn\"Cancel\" để bỏ qua", TPConfigs.SoftNameTW, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            string filePath = "";
            if (dialog == DialogResult.OK)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.AddExtension = true;
                    saveFileDialog.FileName = "ExcelTempUser " + DateTime.Now.ToString("yyyyMMddHHmmss");

                    DialogResult result = saveFileDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        filePath = saveFileDialog.FileName;

                        DownloadTempExcel(filePath);
                        Process.Start(filePath);
                        return;
                    }
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            DataSet ds;
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
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

            if (ds.Tables[0].TableName != sheetName)
            {
                XtraMessageBox.Show("Dữ liệu đầu vào không đúng", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IsUploadUser = true;
            lsUserMs = new List<dm_User>();

            SplashScreenManager.ShowDefaultWaitForm();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string idDept = row[0].ToString().Trim();
                string idUser = row[1].ToString().Trim();
                string nameTW = row[2].ToString().Trim();
                string nameVN = row[3].ToString().Trim();
                string _dob = row[4].ToString().Trim();
                string cccd = row[5].ToString().Trim();
                string national = row[6].ToString().Trim();
                string idJobTitle = row[7].ToString().Trim();
                string _startDate = row[8].ToString().Trim();
                string password = TPConfigs.DefaultPassword;
                string describe = "";



                if (string.IsNullOrEmpty(nameTW))
                {
                    string newLine = string.IsNullOrEmpty(describe) ? string.Empty : "\r\n";
                    describe += $"{newLine}Tên bị trống";
                }

                DateTime DOB = string.IsNullOrEmpty(_dob) ? default(DateTime) : DateTime.Parse(_dob);
                DateTime startDate = string.IsNullOrEmpty(_startDate) ? default(DateTime) : DateTime.Parse(_startDate);


                string userNameByDomain = DomainVNFPG.Instance.GetAccountName(idUser);
                if (!string.IsNullOrEmpty(userNameByDomain))
                {
                    string[] displayNameFHS = userNameByDomain.Split('/');
                    idDept = displayNameFHS[0].Replace("LG", string.Empty);
                    nameTW = displayNameFHS[1];
                    password = "";
                }


                dm_User usr = new dm_User()
                {
                    IdDepartment = idDept,
                    Id = idUser,
                    DisplayName = nameTW,
                    DisplayNameVN = nameVN,
                    CitizenID = cccd,
                    DOB = DOB,
                    Nationality = national,
                    JobCode = idJobTitle,
                    DateCreate = startDate,
                    SecondaryPassword = password,
                };

                if (string.IsNullOrEmpty(describe))
                {
                    lsUserMs.Add(usr);
                }
            }

            SplashScreenManager.CloseDefaultWaitForm();
            sourceUsers.DataSource = lsUserMs;
            gvData.BestFitColumns();
        }

        private void btnConfirmUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            foreach (var item in lsUserMs)
            {
                dm_UserBUS.Instance.AddOrUpdate(item);
            }
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && IsSysAdmin)
            {
                e.Menu.Items.Add(itemEditRole);
                e.Menu.Items.Add(itemEditSign);
            }
        }
    }
}