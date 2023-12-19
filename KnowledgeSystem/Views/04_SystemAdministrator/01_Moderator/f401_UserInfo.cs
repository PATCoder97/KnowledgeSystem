using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using OfficeOpenXml;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using System.Xml.XPath;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class f401_UserInfo : DevExpress.XtraEditors.XtraForm
    {
        public f401_UserInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        public dm_User userInfo = null;
        private string oldUserInfoJson = "";
        string idDept2word;
        bool IsSysAdmin = false;

        private UpdateEvent _eventUpdate = UpdateEvent.Normal;

        List<dm_Role> lsAllRoles;
        List<dm_Role> lsChooseRoles = new List<dm_Role>();
        List<dt301_Course> lsCourses;

        BindingSource _sourceAllRole = new BindingSource();
        BindingSource _sourceChooseRole = new BindingSource();

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private enum UpdateEvent
        {
            [Description("在職")]
            Normal,
            [Description("留職停薪")]
            Suspension,
            [Description("調至")]
            DeptChange,
            [Description("離職")]
            Resign,
            [Description("復職")]
            ResumeWork,
            [Description("晉升")]
            JobChange
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            btnSuspension.ImageOptions.SvgImage = TPSvgimages.Suspension;
            btnDeptChange.ImageOptions.SvgImage = TPSvgimages.Transfer;
            btnResign.ImageOptions.SvgImage = TPSvgimages.Resign;
            btnResumeWork.ImageOptions.SvgImage = TPSvgimages.Conferred;
            btnJobChange.ImageOptions.SvgImage = TPSvgimages.UpLevel;
            btnPersonnelChanges.ImageOptions.SvgImage = TPSvgimages.PersonnelChanges;
        }

        private void EnabledController(bool _enable = true)
        {
            txbUserNameVN.Enabled = _enable;
            txbUserNameTW.Enabled = _enable;
            txbDOB.Enabled = _enable;
            txbCCCD.Enabled = _enable;
            cbbNationality.Enabled = _enable;
            cbbSex.Enabled = _enable;
            txbAddr.Enabled = _enable;
            txbPhone1.Enabled = _enable;
            txbPhone2.Enabled = _enable;
            txbDateStart.Enabled = _enable;
        }

        private void LockControl()
        {
            txbUserId.Enabled = false;
            txbUserId.ReadOnly = false;
            cbbDept.Enabled = false;
            cbbStatus.Enabled = false;
            cbbJobTitle.Enabled = false;

            btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            Size = new Size(600, 358);

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    txbUserId.Enabled = true;
                    cbbDept.Enabled = true;
                    cbbStatus.Enabled = true;
                    cbbJobTitle.Enabled = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    txbUserId.Enabled = true;
                    txbUserId.ReadOnly = true;

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    if (IsSysAdmin)
                    {
                        lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        Size = new Size(600, 658);
                    }

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    if (userInfo.Status == 0)
                    {
                        btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else if (userInfo.Status == 2)
                    {
                        btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    else
                    {
                        btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    }

                    EnabledController(false);
                    break;
                default:
                    break;
            }

            bool role301Main = AppPermission.Instance.CheckAppPermission(AppPermission.SafetyCertMain);
            bool roleEditUserJobAndDept = AppPermission.Instance.CheckAppPermission(AppPermission.EditUserJobAndDept);

            if (!(role301Main && roleEditUserJobAndDept && TPConfigs.IdParentControl == AppPermission.SafetyCertMain))
            {
                btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            int x = screenBounds.Width / 2 - Width / 2;
            int y = screenBounds.Height / 2 - Height / 2;
            Location = new Point(x, y);
        }

        private void f401_UserInfo_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            lsCourses = dt301_CourseBUS.Instance.GetList();

            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            cbbNationality.Properties.Items.AddRange(new string[] { "VN", "TW", "CN" });

            lsAllRoles = dm_RoleBUS.Instance.GetList();
            _sourceAllRole.DataSource = lsAllRoles;
            gcAllRole.DataSource = _sourceAllRole;

            cbbStatus.Properties.Items.AddRange(TPConfigs.lsUserStatus.Select(r => r.Value).ToList());
            cbbSex.Properties.Items.AddRange(new List<string>() { "男", "女" });

            _sourceChooseRole.DataSource = lsChooseRoles;
            gcChooseRole.DataSource = _sourceChooseRole;

            gvAllRole.ReadOnlyGridView();
            gvChooseRole.ReadOnlyGridView();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    userInfo = new dm_User();
                    break;
                case EventFormInfo.View:
                    userInfo.DisplayName = userInfo.DisplayName.Split('\n')[0];
                    userInfo.DateCreate = DateTime.Parse(userInfo.DateCreate.ToShortDateString());

                    txbUserId.EditValue = userInfo.Id;
                    txbUserNameVN.EditValue = userInfo.DisplayNameVN?.Trim();
                    txbUserNameTW.EditValue = userInfo.DisplayName.Split('\n')[0]?.Trim();
                    cbbDept.EditValue = userInfo.IdDepartment;
                    cbbJobTitle.EditValue = userInfo.JobCode;
                    txbDOB.EditValue = userInfo.DOB;
                    txbCCCD.EditValue = userInfo.CitizenID;
                    cbbNationality.EditValue = userInfo.Nationality;

                    txbPhone1.EditValue = userInfo.PhoneNum1;
                    txbPhone2.EditValue = userInfo.PhoneNum2;
                    txbAddr.EditValue = userInfo.Addr;
                    cbbSex.EditValue = userInfo.Sex == null ? "" : userInfo.Sex.Value ? "男" : "女";
                    cbbStatus.EditValue = userInfo.Status == null ? "" : TPConfigs.lsUserStatus[userInfo.Status.Value];
                    txbDateStart.EditValue = userInfo.DateCreate;

                    oldUserInfoJson = JsonConvert.SerializeObject(userInfo);
                    idDept2word = userInfo.IdDepartment.Substring(0, 2);

                    // Lấy quyền hạn và chuyển các quyền mà user có sang gcChooseRoles
                    var lsUserRoles = dm_UserRoleBUS.Instance.GetListByUID(userInfo.Id).Select(r => r.IdRole).ToList();
                    lsChooseRoles.AddRange(lsAllRoles.Where(a => lsUserRoles.Exists(b => b == a.Id)));
                    lsAllRoles.RemoveAll(a => lsUserRoles.Exists(b => b == a.Id));

                    gcAllRole.RefreshDataSource();
                    gcChooseRole.RefreshDataSource();
                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // Lấy các thông tin người dùng từ giao diện
                userInfo.DisplayName = txbUserNameTW.EditValue?.ToString().Trim();
                userInfo.DisplayNameVN = txbUserNameVN.EditValue?.ToString().Trim();
                userInfo.IdDepartment = cbbDept.EditValue?.ToString();
                userInfo.JobCode = cbbJobTitle.EditValue?.ToString();
                userInfo.DOB = txbDOB.DateTime;
                userInfo.CitizenID = txbCCCD.EditValue?.ToString();
                userInfo.Nationality = cbbNationality.EditValue?.ToString();

                userInfo.DateCreate = txbDateStart.DateTime;
                userInfo.PhoneNum1 = txbPhone1.EditValue?.ToString();
                userInfo.PhoneNum2 = txbPhone2.EditValue?.ToString();
                userInfo.Addr = txbAddr.EditValue?.ToString();
                userInfo.Sex = cbbSex.EditValue?.ToString() == "男";
                userInfo.Status = TPConfigs.lsUserStatus.FirstOrDefault(r => r.Value == cbbStatus.EditValue?.ToString()).Key;

                string newUserInfoJson = JsonConvert.SerializeObject(userInfo);

                msg = $"{userInfo.Id} {userInfo.DisplayName} {userInfo.DisplayNameVN}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        userInfo.Id = txbUserId.EditValue?.ToString();
                        result = dm_UserBUS.Instance.Add(userInfo);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = true;
                        var resultUpdate = false;

                        if (oldUserInfoJson != newUserInfoJson)
                        {
                            resultUpdate = dm_UserBUS.Instance.AddOrUpdate(userInfo);
                            result = !resultUpdate ? false : result;
                        }

                        // Xử lý cập nhật các thông tin đặc biệt có ảnh hưởng đến các thông tin khác như: Chứng chỉ an toàn 301
                        var certificatesToInvalidate = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
                        switch (_eventUpdate)
                        {
                            case UpdateEvent.Suspension:
                                // Bảo lưu chức vụ: Đánh dấu chứng chỉ là "無效", lưu trạng thái bằng tạm lưu để phục chức sau này
                                foreach (var cert in certificatesToInvalidate)
                                {
                                    cert.ValidLicense = false;
                                    cert.InvalidLicense = true;
                                    cert.CertSuspended = true;
                                    cert.Describe = EnumHelper.GetDescription(_eventUpdate);

                                    dt301_BaseBUS.Instance.AddOrUpdate(cert);
                                }

                                break;
                            case UpdateEvent.ResumeWork:
                                // Phục chức: chuyển chứng chỉ đang bị tạm ngừng về trạng thái còn hạn
                                var suspendedCerts = dt301_BaseBUS.Instance.GetListByUIDAndCertSuspended(userInfo.Id);
                                foreach (var cert in suspendedCerts)
                                {
                                    cert.ValidLicense = true;
                                    cert.InvalidLicense = false;
                                    cert.CertSuspended = false;
                                    cert.Describe = "";

                                    dt301_BaseBUS.Instance.AddOrUpdate(cert);
                                }

                                break;
                            case UpdateEvent.DeptChange:
                                // Thay đổi bộ phận: chuyển tất cả chứng chỉ về trạng thái hết hạn
                                dm_User oldUserData = JsonConvert.DeserializeObject<dm_User>(oldUserInfoJson);
                                if (userInfo.IdDepartment.StartsWith(oldUserData.IdDepartment.Substring(0, 2))) break;

                                foreach (var cert in certificatesToInvalidate)
                                {
                                    cert.ValidLicense = false;
                                    cert.InvalidLicense = true;

                                    //dt301_BaseBUS.Instance.AddOrUpdate(cert);
                                }

                                var lsDepts = dm_DeptBUS.Instance.GetList();
                                List<dt301_Course> courses = dt301_CourseBUS.Instance.GetList();

                                // Xuất ra html của mail gửi Notes
                                int indexCounter = 1;
                                var certsTable = (from cert in certificatesToInvalidate
                                                  join course in courses on cert.IdCourse equals course.Id
                                                  select new
                                                  {
                                                      index = indexCounter++,
                                                      id = course.Id,
                                                      name = course.DisplayName,
                                                      date = cert.DateReceipt.ToString("yyyy/MM/dd"),
                                                      exp = cert.ExpDate?.ToString("yyyy/MM/dd")
                                                  }).ToList();

                                var templateData = new
                                {
                                    deptfrom = $"{oldUserData.IdDepartment}{lsDepts.FirstOrDefault(r => r.Id == oldUserData.IdDepartment).DisplayName}",
                                    deptto = $"{userInfo.IdDepartment}{lsDepts.FirstOrDefault(r => r.Id == userInfo.IdDepartment).DisplayName}",
                                    user = $"{userInfo.DisplayName}/{userInfo.Id}",
                                    total = certificatesToInvalidate.Count(),
                                    certs = certsTable
                                };

                                var templateContentSigner = System.IO.File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, $"f301_NotifyPersonnelTransfer.html"));
                                var templateSigner = Template.Parse(templateContentSigner);

                                var pageContent = templateSigner.Render(templateData);
                                string subject = $"{lsDepts.FirstOrDefault(r => r.Id == oldUserData.IdDepartment).DisplayName}有{userInfo.DisplayName}同仁調任至您部門，請點選新任職務工安證照需求";
                                int idGroup = dm_GroupBUS.Instance.GetItemByName($"處務室{userInfo.IdDepartment.Substring(0, 2)}")?.Id ?? -1;
                                var usersInGroup = dm_GroupUserBUS.Instance.GetListByIdGroup(idGroup);
                                string toUsers = string.Join(",", usersInGroup.Select(r => $"{r.IdUser}@VNFPG"));

                                //using (ExcelPackage pck = new ExcelPackage())
                                //{
                                //    pck.Workbook.Properties.Author = "VNW0014732";
                                //    pck.Workbook.Properties.Company = "FHS";
                                //    pck.Workbook.Properties.Title = "Exported by Phan Anh Tuan";
                                //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("DATA");

                                //    var exportData = from data in certificatesToInvalidate
                                //                     join course in courses on data.IdCourse equals course.Id
                                //                     select  new
                                //                     {
                                //                         課程代號= data.IdCourse,
                                //                         課程名稱 = course.DisplayName,
                                //                         證照期限 = data.ExpDate
                                //                     }

                                //    ws.Cells["A1"].LoadFromCollection(lsQueryFile52, false);
                                //    string savePath = Path.Combine(pathDocument, $"附件05.2：.複訓之提報需求人員名單.xlsx");
                                //    FileInfo excelFile = new FileInfo(savePath);
                                //    pck.SaveAs(excelFile);
                                //}

                                // Lưu vào bảng Mail để service gửi Notes
                                var mail = new sys_NotesMail()
                                {
                                    Thread = "301",
                                    Subjects = subject,
                                    Content = pageContent,
                                    ToUsers = toUsers,
                                };
                                sys_NotesMailBUS.Instance.Add(mail);

                                break;
                            case UpdateEvent.Resign:
                                // Nghi việc: chuyển tất cả chứng chỉ về trạng thái hết hạn
                                foreach (var cert in certificatesToInvalidate)
                                {
                                    cert.ValidLicense = false;
                                    cert.InvalidLicense = true;

                                    dt301_BaseBUS.Instance.AddOrUpdate(cert);
                                }

                                break;
                            case UpdateEvent.JobChange:
                                // Thay đổi chức vụ: Giữ chứng chỉ còn hạn cho chức vụ mới, hết hạn chứng chỉ không sử dụng ở chức vụ mới
                                // Lấy danh sách chứng chỉ hợp lệ của người dùng
                                var validCertificates = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);

                                // Lấy danh sách các bộ yêu cầu chứng chỉ dựa trên công việc và bộ phận
                                var certReqSets = dt301_CertReqSetBUS.Instance.GetListByJobAndDept(userInfo.JobCode, idDept2word);

                                // Chuyển các chứng chỉ có hiệu lực về chức vụ mới
                                var certsToNewJob = validCertificates
                                    .Join(certReqSets, data => data.IdCourse, req => req.IdCourse, (data, req) => data)
                                    .ToList();

                                foreach (var certificate in certsToNewJob)
                                {
                                    certificate.IdJobTitle = userInfo.JobCode;
                                    dt301_BaseBUS.Instance.AddOrUpdate(certificate);
                                }

                                // Vô hiệu hóa chứng chỉ của chức vụ cũ không cần thiết ở chức vụ mới
                                var certsToInvalid = validCertificates
                                    .Where(data => !certsToNewJob.Contains(data))
                                    .ToList();

                                foreach (var certificate in certsToInvalid)
                                {
                                    certificate.ValidLicense = false;
                                    certificate.InvalidLicense = true;
                                    dt301_BaseBUS.Instance.AddOrUpdate(certificate);
                                }

                                break;
                        }

                        // Nếu là admin thì có thể cài quyền hạn luôn trong này
                        if (IsSysAdmin)
                        {
                            var resultDel = dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);

                            var lsUserRolesAdd = lsChooseRoles.Select(r => new dm_UserRole { IdUser = userInfo.Id, IdRole = r.Id }).ToList();
                            var resultAdd = dm_UserRoleBUS.Instance.AddRange(lsUserRolesAdd);
                        }
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認刪除人員: {userInfo.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_UserBUS.Instance.Remove(userInfo.Id);
                        dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventFormInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventFormInfo.Delete:
                //        logger.Warning(_eventInfo.ToString(), msg);
                //        break;
                //}
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void gvAllRole_DoubleClick(object sender, EventArgs e)
        {
            if (eventInfo != EventFormInfo.Update) return;

            GridView view = gvAllRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsAllRoles.Remove(_role);
            view.RefreshData();
            lsChooseRoles.Add(_role);
            gvChooseRole.RefreshData();
        }

        private void gvChooseRole_DoubleClick(object sender, EventArgs e)
        {
            if (eventInfo != EventFormInfo.Update) return;

            GridView view = gvChooseRole;
            dm_Role _role = view.GetRow(view.FocusedRowHandle) as dm_Role;

            lsChooseRoles.Remove(_role);
            view.RefreshData();
            lsAllRoles.Add(_role);
            gvAllRole.RefreshData();
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG || txbUserId.Enabled == false) return;

            string userNameByDomain = DomainVNFPG.Instance.GetAccountName(txbUserId.Text.ToUpper());
            if (string.IsNullOrEmpty(userNameByDomain)) return;

            string[] displayNameFHS = userNameByDomain.Split('/');
            cbbDept.EditValue = displayNameFHS[0].Replace("LG", string.Empty);
            txbUserNameTW.EditValue = displayNameFHS[1];
        }

        private void cbbDept_AutoSearch(object sender, DevExpress.XtraEditors.Controls.LookUpEditAutoSearchEventArgs e)
        {
            e.ClearHighlight();
        }

        private void btnSuspension_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.Suspension;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[2];

            // Lấy ds chứng chỉ sẽ chuyển về 無效 khi nghỉ lưu chức
            var lsValidCertByUsers = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
            var lsValidCerts = (from data in lsValidCertByUsers
                                join course in lsCourses on data.IdCourse equals course.Id
                                select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>以下共{lsValidCertByUsers.Count()}證書將返回狀態「無效」，當該員復職時將返回「應取」:</color></br>{string.Join("\r\n", lsValidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert);
        }

        private void btnResumeWork_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.ResumeWork;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[0];

            // Lấy ds chứng chỉ sẽ chuyển về 應取 khi phục chức
            var suspendedCerts = dt301_BaseBUS.Instance.GetListByUIDAndCertSuspended(userInfo.Id);
            var lsValidCerts = (from data in suspendedCerts
                                join course in lsCourses on data.IdCourse equals course.Id
                                select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>以下共{suspendedCerts.Count()}證書將返回狀態「應取」:</color></br>{string.Join("\r\n", lsValidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert);
        }

        private void btnResign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.Resign;
            LockControl();

            cbbStatus.EditValue = TPConfigs.lsUserStatus[1];

            // Lấy ds chứng chỉ sẽ chuyển về 無效 khi nghỉ việc
            var lsValidCertByUsers = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);
            var lsValidCerts = (from data in lsValidCertByUsers
                                join course in lsCourses on data.IdCourse equals course.Id
                                select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>以下共{lsValidCertByUsers.Count()}證書將返回狀態「無效」:</color></br>{string.Join("\r\n", lsValidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert);
        }

        private void btnJobChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "晉升";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選職務</font>";
            args.DefaultButtonIndex = 0;
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();
            GridColumn gcol1 = new GridColumn() { Caption = "職務代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "職務名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Where(r => r.Id != userInfo.JobCode).ToList();
            editor.Properties.DataSource = lsJobTitles;
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";
            args.Editor = editor;

            editView.Appearance.HeaderPanel.Font = fontUI14;
            editView.Appearance.HeaderPanel.ForeColor = Color.Black;
            editView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            editView.Appearance.Row.Font = fontUI12;
            editView.Appearance.Row.ForeColor = Color.Black;
            editView.Columns.AddRange(new GridColumn[] { gcol1, gcol2 });

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = Color.Black;
            editor.Properties.NullText = "";
            editor.Properties.PopupView = editView;

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string idJobChange = result?.ToString() ?? "";

            cbbJobTitle.EditValue = idJobChange;

            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.JobChange;
            LockControl();

            // Thông báo ra các chứng chỉ được giữ lại và chứng chỉ sẽ chuyển vào 無效 khi thay đổi chức vụ
            var lsValidCerts = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);

            var lsCertReqSets = dt301_CertReqSetBUS.Instance.GetListByJobAndDept(idJobChange, idDept2word);

            // Lấy ds chứng chỉ còn được dùng đến ở chức vụ mới
            var lsNewValidCerts = (from data in lsValidCerts
                                   join req in lsCertReqSets on data.IdCourse equals req.IdCourse
                                   join course in lsCourses on data.IdCourse equals course.Id
                                   select $"{data.IdCourse} {course.DisplayName}").ToList();

            // Lấy ds các chứng chỉ ở chức vụ cũ còn hạn sẽ bị huỷ ở chức vụ mới
            var lsNewInvalidCerts = (from data in lsValidCerts
                                     join course in lsCourses on data.IdCourse equals course.Id
                                     where !lsNewValidCerts.Contains($"{data.IdCourse} {course.DisplayName}")
                                     select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>應取證照：</color></br>{string.Join("\r\n", lsNewValidCerts)}</font>";
            string msgInvalidCert = $"<font='Microsoft JhengHei UI' size=14><color=red>無效證照：</color></br>{string.Join("\r\n", lsNewInvalidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert + "\r\n" + msgInvalidCert);
        }

        private void btnDeptChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "晉升";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選單位</font>";
            args.DefaultButtonIndex = 0;
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();
            GridColumn gcol1 = new GridColumn() { Caption = "部門代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "部門名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            {
                Id = r.Id,
                DisplayName = $"{r.Id,-5}{r.DisplayName}"
            }).Where(r => r.Id != userInfo.IdDepartment && r.Id.Length == 4).ToList();

            editor.Properties.DataSource = lsDepts;
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";
            args.Editor = editor;

            editor.Properties.Appearance.Font = fontUI14;
            editor.Properties.Appearance.ForeColor = Color.Black;
            editor.Properties.NullText = "";
            editor.Properties.PopupView = editView;

            editView.Appearance.HeaderPanel.Font = fontUI14;
            editView.Appearance.HeaderPanel.ForeColor = Color.Black;
            editView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            editView.Appearance.Row.Font = fontUI12;
            editView.Appearance.Row.ForeColor = Color.Black;
            editView.Columns.AddRange(new GridColumn[] { gcol1, gcol2 });

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string idDeptChange = result?.ToString() ?? "";

            cbbDept.EditValue = idDeptChange;

            eventInfo = EventFormInfo.Update;
            LockControl();

            if (idDeptChange.StartsWith(userInfo.IdDepartment.Substring(0, 2))) return;

            _eventUpdate = UpdateEvent.DeptChange;

            var lsValidCertByUsers = dt301_BaseBUS.Instance.GetListByUIDAndValidCert(userInfo.Id);

            // Lấy ds chứng chỉ sẽ chuyển về 無效 khi chuyển bộ phận. Lưu vào notify để thông báo cho bên bp mới có các bằng còn hạn có dùng lại không
            var lsValidCerts = (from data in lsValidCertByUsers
                                join course in lsCourses on data.IdCourse equals course.Id
                                select $"{data.IdCourse} {course.DisplayName}").ToList();

            string msgValidCert = $"<font='Microsoft JhengHei UI' size=14><color=blue>以下共{lsValidCertByUsers.Count()}證書將返回狀態「無效」並通知新部門處務室：</color></br>{string.Join("\r\n", lsValidCerts)}</font>";

            MsgTP.MsgShowInfomation(msgValidCert);
        }

        private void f401_UserInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.Shift && e.KeyCode == Keys.T)
            {
                string password = EncryptionHelper.DecryptPass(userInfo.SecondaryPassword);
                MessageBox.Show(password);
            }
        }
    }
}