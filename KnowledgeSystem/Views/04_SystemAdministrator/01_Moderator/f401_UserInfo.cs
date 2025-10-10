using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using OfficeOpenXml;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

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
        DateTime? resignDate;

        private UpdateEvent _eventUpdate = UpdateEvent.Normal;

        List<dt301_Course> lsCourses;

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

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
            [Description("更新實編制職務")]
            JobChange,
            [Description("更新實際職務")]
            ActualJobChange,
            [Description("預報離職")]
            ResignPlan,
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
            btnJobChange.ImageOptions.SvgImage = TPSvgimages.Info;
            btnActualJobChange.ImageOptions.SvgImage = TPSvgimages.UpLevel;
            btnPersonnelChanges.ImageOptions.SvgImage = TPSvgimages.PersonnelChanges;
            btnResignPlan.ImageOptions.SvgImage = TPSvgimages.Schedule;
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
            cbbActualJob.Enabled = false;
            txbPCName.Enabled = false;

            btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnResignPlan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnActualJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    txbUserId.Enabled = true;
                    cbbDept.Enabled = true;
                    cbbStatus.Enabled = true;
                    cbbJobTitle.Enabled = true;
                    cbbActualJob.Enabled = true;

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
                        btnResignPlan.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                        if (userInfo.ResignPlan != null)
                        {
                            btnResignPlan.Caption = "取消預報離職";
                        }
                        else
                        {
                            btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                            btnActualJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        }
                    }
                    else if (userInfo.Status == 2 || userInfo.Status == 1)
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

            // Kiểm tra quyền admin
            if (!IsSysAdmin)
            {
                lcRole.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcSign.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            // Kiêm tra xem có quyền quản lý chứng chỉ an toàn hay không, nếu có thì mới có chức năng này
            bool roleSafetyCertMain = AppPermission.Instance.CheckAppPermission(AppPermission.SafetyCertMain);

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groupEditDeptAndJob = dm_GroupBUS.Instance.GetListByName("人事變動【人員管理】");

            bool roleEditDeptAndJob = groupEditDeptAndJob.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));

            if (!(roleEditDeptAndJob && roleSafetyCertMain))
            {
                btnPersonnelChanges.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                btnSuspension.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnDeptChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResumeWork.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnActualJobChange.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnResignPlan.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            foreach (var item in lcControls)
            {
                string colorHex = item.Control.Enabled ? "000000" : "000000";
                item.Text = item.Text.Replace("000000", colorHex);
            }

            // Các thông tin phải điền có thêm dấu * màu đỏ
            foreach (var item in lcImpControls)
            {
                if (item.Control.Enabled)
                {
                    item.Text += "<color=red>*</color>";
                }
                else
                {
                    item.Text = item.Text.Replace("<color=red>*</color>", "");
                }
            }
        }

        private void f401_UserInfo_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcUserId, lcUserNameVN, lcUserNameTW, lcDept, lcActualJob, lcJobTitle, lcNationality, lcStatus, lcSex, lcDateStart, lcDOB, lcCCCD, lcPhone1, lcPhone2, lcAddr, lcPCName };
            lcImpControls = new List<LayoutControlItem>() { lcUserId, lcUserNameVN, lcUserNameTW, lcDept, lcActualJob, lcJobTitle, lcNationality, lcStatus, lcSex, lcDateStart, lcDOB, lcCCCD, lcPhone1, lcAddr };
            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            lcUserInfo.SelectedTabPageIndex = 0;

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

            cbbActualJob.Properties.DataSource = lsJobTitles;
            cbbActualJob.Properties.DisplayMember = "DisplayName";
            cbbActualJob.Properties.ValueMember = "Id";

            cbbNationality.Properties.Items.AddRange(new string[] { "VN", "TW", "CN" });
            cbbStatus.Properties.Items.AddRange(TPConfigs.lsUserStatus.Select(r => r.Value).ToList());
            cbbSex.Properties.Items.AddRange(new List<string>() { "男", "女" });

            gvSign.ReadOnlyGridView();
            gvRole.ReadOnlyGridView();
            gvGroup.ReadOnlyGridView();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    userInfo = new dm_User();
                    break;
                case EventFormInfo.View:
                    userInfo.DisplayName = userInfo.DisplayName.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    userInfo.DateCreate = DateTime.Parse(userInfo.DateCreate.ToShortDateString());

                    txbUserId.EditValue = userInfo.Id;
                    txbUserNameVN.EditValue = userInfo.DisplayNameVN?.Trim();
                    txbUserNameTW.EditValue = userInfo.DisplayName.Split('\n')[0]?.Trim();
                    cbbDept.EditValue = userInfo.IdDepartment;
                    cbbJobTitle.EditValue = userInfo.JobCode;
                    cbbActualJob.EditValue = userInfo.ActualJobCode;
                    txbDOB.EditValue = userInfo.DOB;
                    txbCCCD.EditValue = userInfo.CitizenID;
                    cbbNationality.EditValue = userInfo.Nationality;
                    txbPCName.EditValue = userInfo.PCName;

                    txbPhone1.EditValue = userInfo.PhoneNum1;
                    txbPhone2.EditValue = userInfo.PhoneNum2;
                    txbAddr.EditValue = userInfo.Addr;
                    cbbSex.EditValue = userInfo.Sex == null ? "" : userInfo.Sex.Value ? "男" : "女";
                    cbbStatus.EditValue = userInfo.Status == null ? "" : TPConfigs.lsUserStatus[userInfo.Status.Value];
                    txbDateStart.EditValue = userInfo.DateCreate;

                    resignDate = userInfo.ResignPlan;
                    oldUserInfoJson = JsonConvert.SerializeObject(userInfo);
                    idDept2word = userInfo.IdDepartment.Count() > 2 ? userInfo.IdDepartment.Substring(0, 2) : "00";

                    // Lấy quyền hạn và chuyển các quyền mà user có sang gcChooseRoles
                    var usrRoles = dm_UserRoleBUS.Instance.GetListByUID(userInfo.Id).Select(r => r.IdRole).ToList();
                    var roles = dm_RoleBUS.Instance.GetList().Where(a => usrRoles.Exists(b => b == a.Id));
                    gcRole.DataSource = roles;

                    Dictionary<int, string> signTypes = TPConfigs.signTypes;
                    var signUsrs = dm_SignUsersBUS.Instance.GetListByUID(userInfo.Id).ToList();
                    var idSigns = signUsrs.Select(r => r.IdSign).ToList();
                    var signs = dm_SignBUS.Instance.GetListByIdSigns(idSigns).OrderBy(r => r.Prioritize).ToList();
                    var signInfos = (from data in signs
                                     join typeImg in signTypes on data.ImgType equals typeImg.Key
                                     select new
                                     {
                                         Id = data.Id,
                                         DisplayName = data.DisplayName,
                                         SignType = typeImg.Value,
                                     }).ToList();

                    gcSign.DataSource = signInfos;

                    // Xem đang nằm ở các nhóm nào
                    var groupUsers = dm_GroupUserBUS.Instance.GetListByUID(userInfo.Id);
                    var groups = (from data in dm_GroupBUS.Instance.GetList()
                                  join userGroup in groupUsers on data.Id equals userGroup.IdGroup
                                  select new
                                  {
                                      DisplayName = data.DisplayName,
                                      Desc = data.Describe,
                                      Dept = data.IdDept
                                  }).ToList();
                    gcGroup.DataSource = groups;

                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (userInfo.Status != 0) return;

            eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!IsSysAdmin)
            {
                XtraMessageBox.Show("Vui lòng liên hệ nhân viên quản lý hệ thống!", TPConfigs.SoftNameTW);
                return;
            }

            MsgTP.MsgConfirmDel();

            eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()) && item.Control.Enabled)
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                TextInfo textInfo = new CultureInfo("vi-VN", false).TextInfo;

                // Lấy các thông tin người dùng từ giao diện
                userInfo.DisplayName = txbUserNameTW.EditValue?.ToString().Trim();
                userInfo.DisplayNameVN = textInfo.ToTitleCase(txbUserNameVN.EditValue?.ToString().Trim().ToLower() ?? "");
                userInfo.IdDepartment = cbbDept.EditValue?.ToString();
                userInfo.JobCode = cbbJobTitle.EditValue?.ToString();
                userInfo.DOB = txbDOB.DateTime;
                userInfo.CitizenID = txbCCCD.EditValue?.ToString();
                userInfo.Nationality = cbbNationality.EditValue?.ToString();
                userInfo.ActualJobCode = cbbActualJob.EditValue?.ToString();

                userInfo.DateCreate = txbDateStart.DateTime;
                userInfo.PhoneNum1 = txbPhone1.EditValue?.ToString();
                userInfo.PhoneNum2 = txbPhone2.EditValue?.ToString();
                userInfo.Addr = txbAddr.EditValue?.ToString();
                userInfo.Sex = cbbSex.EditValue?.ToString() == "男";
                userInfo.Status = TPConfigs.lsUserStatus.FirstOrDefault(r => r.Value == cbbStatus.EditValue?.ToString()).Key;
                userInfo.ResignPlan = resignDate;

                string newUserInfoJson = JsonConvert.SerializeObject(userInfo);

                // Tạo thông tin thay đổi nhân sự cho module ISO [201]
                var updateReq = new dt201_UpdateUsrReq()
                {
                    DateCreate = DateTime.Now,
                    IdDept = userInfo.IdDepartment,
                    IdUsr = userInfo.Id
                };

                msg = $"{userInfo.Id} {userInfo.DisplayName} {userInfo.DisplayNameVN}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        userInfo.Id = txbUserId.EditValue?.ToString();
                        result = dm_UserBUS.Instance.Add(userInfo);

                        // [201]
                        if (result)
                        {
                            updateReq = new dt201_UpdateUsrReq()
                            {
                                DateCreate = DateTime.Now,
                                IdDept = userInfo.IdDepartment,
                                IdUsr = userInfo.Id,
                                TypeChange = "新進"
                            };
                            dt201_UpdateUsrReqBUS.Instance.Add(updateReq);
                        }

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

                                    dt301_BaseBUS.Instance.AddOrUpdate(cert);
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

                                var templateContentSigner = System.IO.File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, $"f301_NotifyPersonnelTransfer.html"));
                                var templateSigner = Template.Parse(templateContentSigner);

                                var pageContent = templateSigner.Render(templateData);
                                string subject = $"{lsDepts.FirstOrDefault(r => r.Id == oldUserData.IdDepartment).DisplayName}有{userInfo.DisplayName}同仁調任至您部門，請點選新任職務工安證照需求";
                                int idGroup = dm_GroupBUS.Instance.GetItemByName($"處務室{userInfo.IdDepartment.Substring(0, 2)}")?.Id ?? -1;
                                var usersInGroup = dm_GroupUserBUS.Instance.GetListByIdGroup(idGroup);
                                string toUsers = string.Join(",", usersInGroup.Select(r => $"{r.IdUser}@VNFPG"));

                                // Lưu vào bảng Mail để service gửi Notes
                                var mail = new sys_NotesMail()
                                {
                                    Thread = "301",
                                    Subjects = subject,
                                    Content = pageContent,
                                    ToUsers = toUsers,
                                };
                                sys_NotesMailBUS.Instance.Add(mail);

                                // [309] Thông báo người quản lý kho  chuyển bộ phận
                                idGroup = dm_GroupBUS.Instance.GetListByName("機邊庫").Where(r => r.IdDept == oldUserData.IdDepartment).FirstOrDefault()?.Id ?? -1;
                                usersInGroup = dm_GroupUserBUS.Instance.GetListByIdGroup(idGroup);
                                toUsers = string.Join(",", usersInGroup.Select(r => $"{r.IdUser}@VNFPG"));
                                mail = new sys_NotesMail()
                                {
                                    Thread = "309",
                                    Subjects = $"機邊庫系統通知：偵測到「{oldUserData.IdDepartment}/{oldUserData.DisplayName}」已離開原部門，請確認物料管理權限",
                                    Content = $"{oldUserData.IdDepartment}/{oldUserData.DisplayName}",
                                    ToUsers = toUsers,
                                };
                                sys_NotesMailBUS.Instance.Add(mail);

                                // [201] Thông báo về sự biến động nhân sự của ISO
                                updateReq.TypeChange = "新進";
                                updateReq.Describe = $"調任從「{templateData.deptfrom}」到「{templateData.deptto}」";
                                dt201_UpdateUsrReqBUS.Instance.Add(updateReq);

                                updateReq.IdDept = oldUserData.IdDepartment;
                                updateReq.TypeChange = "離職";
                                updateReq.Describe = $"調任從「{templateData.deptfrom}」到「{templateData.deptto}」";
                                dt201_UpdateUsrReqBUS.Instance.Add(updateReq);

                                break;
                            case UpdateEvent.Resign:
                                // Nghi việc: chuyển tất cả chứng chỉ về trạng thái hết hạn
                                foreach (var cert in certificatesToInvalidate)
                                {
                                    cert.ValidLicense = false;
                                    cert.InvalidLicense = true;

                                    dt301_BaseBUS.Instance.AddOrUpdate(cert);
                                }

                                // [309] Thông báo người quản lý kho nghỉ việc
                                idGroup = dm_GroupBUS.Instance.GetListByName("機邊庫").Where(r => r.IdDept == userInfo.IdDepartment).FirstOrDefault()?.Id ?? -1;
                                usersInGroup = dm_GroupUserBUS.Instance.GetListByIdGroup(idGroup);
                                toUsers = string.Join(",", usersInGroup.Select(r => $"{r.IdUser}@VNFPG"));
                                mail = new sys_NotesMail()
                                {
                                    Thread = "309",
                                    Subjects = $"機邊庫系統通知：偵測到「{userInfo.IdDepartment}/{userInfo.DisplayName}」已離開原部門，請確認物料管理權限",
                                    Content = $"{userInfo.IdDepartment}/{userInfo.DisplayName}",
                                    ToUsers = toUsers,
                                };
                                sys_NotesMailBUS.Instance.Add(mail);

                                // [201]
                                updateReq.TypeChange = "離職";
                                dt201_UpdateUsrReqBUS.Instance.Add(updateReq);

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

                            case UpdateEvent.ActualJobChange:

                                // [201]
                                oldUserData = JsonConvert.DeserializeObject<dm_User>(oldUserInfoJson);
                                updateReq.TypeChange = "異動";
                                updateReq.Describe = $"調任從「{oldUserData.ActualJobCode}」到「{userInfo.ActualJobCode}」";
                                dt201_UpdateUsrReqBUS.Instance.Add(updateReq);

                                break;

                            case UpdateEvent.ResignPlan:

                                break;
                        }

                        //// Nếu là admin thì có thể cài quyền hạn luôn trong này
                        //if (IsSysAdmin)
                        //{
                        //    var resultDel = dm_UserRoleBUS.Instance.RemoveRangeByUID(userInfo.Id);

                        //    var lsUserRolesAdd = lsChooseRoles.Select(r => new dm_UserRole { IdUser = userInfo.Id, IdRole = r.Id }).ToList();
                        //    var resultAdd = dm_UserRoleBUS.Instance.AddRange(lsUserRolesAdd);
                        //}
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

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string idUsr = txbUserId.Text.ToUpper();
                string url = $"https://www.fhs.com.tw/ads/api/Furnace/rest/json/hr/s10/{idUsr}";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    try
                    {
                        string response = client.DownloadString(url);

                        if (!string.IsNullOrEmpty(response))
                        {
                            var data = response.Replace("o|o", "").Split('|');

                            txbDOB.EditValue = DateTime.ParseExact(data[2], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                            txbDateStart.EditValue = DateTime.ParseExact(data[3], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                            txbAddr.EditValue = data[18];
                            txbPhone1.Text = data[19];
                            txbPhone2.Text = data[20];

                            txbUserNameVN.EditValue = new CultureInfo("vi-VN", false).TextInfo.ToTitleCase(data[1].ToLower());
                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine($"Failed to fetch data: {ex.Message}");
                    }
                }

                if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG || txbUserId.Enabled == false) return;

                string userNameByDomain = DomainVNFPG.Instance.GetAccountName(txbUserId.Text.ToUpper());
                if (string.IsNullOrEmpty(userNameByDomain)) return;

                string[] displayNameFHS = userNameByDomain.Split('/');
                cbbDept.EditValue = displayNameFHS[0].Replace("LG", string.Empty);
                txbUserNameTW.EditValue = displayNameFHS[1];
            }
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
            MsgTP.MsgShowInfomation("<font='Microsoft JhengHei UI' size=14>更新編制職務會影響到證照管理，請您確認並注意！</font>");

            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "更新編制職務";
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

        private void btnActualJobChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
            args.Caption = "更新編制職務";
            args.Prompt = $"<font='Microsoft JhengHei UI' size=14>請選職務</font>";
            args.DefaultButtonIndex = 0;
            SearchLookUpEdit editor = new SearchLookUpEdit();
            GridView editView = new GridView();
            GridColumn gcol1 = new GridColumn() { Caption = "職務代號", FieldName = "Id", Visible = true, VisibleIndex = 0 };
            GridColumn gcol2 = new GridColumn() { Caption = "職務名稱", FieldName = "DisplayName", Visible = true, VisibleIndex = 1 };

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Where(r => r.Id != userInfo.ActualJobCode).ToList();
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

            cbbActualJob.EditValue = idJobChange;

            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.ActualJobChange;
            LockControl();
        }

        private void btnResignPlan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (resignDate != null)
            {
                var result = XtraInputBox.Show(new XtraInputBoxArgs
                {
                    Caption = TPConfigs.SoftNameTW,
                    AllowHtmlText = DevExpress.Utils.DefaultBoolean.False,
                    Prompt = "請輸入您的工號以確認取消預報離職",
                    DefaultButtonIndex = 0,
                    Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                    DefaultResponse = ""
                })?.ToString().ToUpper();

                if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

                resignDate = null;
            }
            else
            {
                var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

                // Thiết lập mask để buộc nhập đúng định dạng
                editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
                editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
                editor.Properties.MaskSettings.Set("useAdvancingCaret", true);
                editor.Properties.UseMaskAsDisplayFormat = true;

                var result = XtraInputBox.Show(new XtraInputBoxArgs
                {
                    Caption = TPConfigs.SoftNameTW,
                    AllowHtmlText = DevExpress.Utils.DefaultBoolean.False,
                    Prompt = "輸入核准預報離職時間",
                    Editor = editor,
                    DefaultButtonIndex = 0,
                    DefaultResponse = DateTime.Now.ToString("yyyy/MM/dd") // Định dạng mặc định
                });

                if (string.IsNullOrEmpty(result?.ToString())) return;

                // Xử lý kết quả nhập
                if (!DateTime.TryParse(result.ToString(), out DateTime respTime))
                {
                    XtraMessageBox.Show($"{result}\r\n時間格式不正確，請重新輸入！");
                    return;
                }

                if (respTime < DateTime.Now)
                {
                    XtraMessageBox.Show("預報離職時間無效！");
                    return;
                }

                resignDate = respTime;
            }

            eventInfo = EventFormInfo.Update;
            _eventUpdate = UpdateEvent.ResignPlan;
            LockControl();
        }
    }
}