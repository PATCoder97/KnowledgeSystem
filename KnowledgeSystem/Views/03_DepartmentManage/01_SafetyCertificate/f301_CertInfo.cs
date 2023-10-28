using BusinessLayer;
using DataAccessLayer;
using DevExpress.Internal;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class f301_CertInfo : DevExpress.XtraEditors.XtraForm
    {
        public f301_CertInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public string _formName = "";
        public dt301_Base _base = null;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        private enum CertStatus
        {
            應取證照,
            備援證照,
            無效證照,
            在等證照
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDept.Enabled = false;
            cbbJobTitle.Enabled = false;
            cbbUser.Enabled = _enable;
            cbbCertStatus.Enabled = _enable;
            cbbCourse.Enabled = _enable;
            txbDateReceipt.Enabled = _enable;
            txbDuration.Enabled = _enable;
            txbDescribe.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{_formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f301_CertInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            {
                Id = r.Id,
                DisplayName = $"{r.Id} {r.DisplayName}"
            }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            {
                Id = r.Id,
                IdDepartment = r.IdDepartment,
                DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
                JobCode = r.JobCode
            }).ToList();
            cbbUser.Properties.DataSource = lsUsers;
            cbbUser.Properties.DisplayMember = "DisplayName";
            cbbUser.Properties.ValueMember = "Id";
            cbbUser.Properties.BestFitWidth = 110;

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Select(r => new dm_JobTitle() { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" });
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            cbbCourse.Properties.DisplayMember = "DisplayName";
            cbbCourse.Properties.ValueMember = "Id";

            cbbCertStatus.Properties.Items.AddRange((CertStatus[])Enum.GetValues(typeof(CertStatus)));
            cbbCertStatus.SelectedIndex = 0;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _base = new dt301_Base();
                    cbbDept.EditValue = idDept2word;
                    txbDateReceipt.EditValue = DateTime.Today;
                    break;
                case EventFormInfo.View:
                    cbbDept.EditValue = _base.IdDept;
                    cbbUser.EditValue = _base.IdUser;
                    cbbJobTitle.EditValue = _base.IdJobTitle;
                    cbbCourse.EditValue = _base.IdCourse;
                    txbDateReceipt.EditValue = _base.DateReceipt;
                    if (_base.ValidLicense)
                    {
                        cbbCertStatus.EditValue = EnumHelper.GetDescription(CertStatus.應取證照);
                    }
                    else if (_base.BackupLicense)
                    {
                        cbbCertStatus.EditValue = EnumHelper.GetDescription(CertStatus.備援證照);
                    }
                    else if (_base.InvalidLicense)
                    {
                        cbbCertStatus.EditValue = EnumHelper.GetDescription(CertStatus.無效證照);
                    }
                    else
                    {
                        cbbCertStatus.EditValue = EnumHelper.GetDescription(CertStatus.在等證照);
                    }

                    int duration = _base.ExpDate.HasValue ? _base.ExpDate.Value.Year - _base.DateReceipt.Year : 0;
                    txbDuration.EditValue = duration;
                    txbDescribe.EditValue = _base.Describe;
                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
                    break;
                case EventFormInfo.ViewOnly:
                    break;
                default:
                    break;
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DefaultMsg.MsgConfirmDel();

            _eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string ursId = cbbUser.EditValue?.ToString();
            string course = cbbCourse.EditValue?.ToString();

            if (string.IsNullOrEmpty(ursId) || string.IsNullOrEmpty(course))
            {
                XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                _base.IdDept = idDept2word;
                _base.IdUser = ursId;
                _base.IdJobTitle = cbbJobTitle.EditValue?.ToString();
                _base.IdCourse = course;
                _base.DateReceipt = Convert.ToDateTime(txbDateReceipt.EditValue);

                int duration = Convert.ToInt16(txbDuration?.EditValue);
                _base.ExpDate = duration == 0 ? null : (DateTime?)_base.DateReceipt.AddYears(duration);

                _base.ValidLicense = false;
                _base.BackupLicense = false;
                _base.InvalidLicense = false;
                var status = EnumHelper.GetEnumByDescription<CertStatus>(cbbCertStatus.EditValue?.ToString());
                switch (status)
                {
                    case CertStatus.應取證照:
                        _base.ValidLicense = true;
                        break;
                    case CertStatus.備援證照:
                        _base.BackupLicense = true;
                        break;
                    case CertStatus.無效證照:
                        _base.InvalidLicense = true;
                        break;
                }

                _base.Describe = txbDescribe.EditValue?.ToString();

                msg = $"{_base.IdDept} {_base.IdJobTitle} {_base.IdCourse} {_base.DateReceipt}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dt301_BaseBUS.Instance.Add(_base);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = dt301_BaseBUS.Instance.AddOrUpdate(_base);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{_formName}: {_base.IdJobTitle} {_base.IdCourse} {_base.DateReceipt}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt301_BaseBUS.Instance.Remove(_base.Id);
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
                DefaultMsg.MsgErrorDB();
            }
        }

        private void cbbUser_EditValueChanged(object sender, EventArgs e)
        {
            dm_User urs = cbbUser.GetSelectedDataRow() as dm_User;
            if (urs == null) return;

            if (_eventInfo == EventFormInfo.Create)
            {
                cbbJobTitle.EditValue = urs.JobCode;
            };


        }

        private void cbbCourse_EditValueChanged(object sender, EventArgs e)
        {
            var course = dt301_CourseBUS.Instance.GetItemById(cbbCourse.EditValue?.ToString());
            if (course == null) return;

            txbDuration.EditValue = course.Duration;
        }

        private void cbbJobTitle_EditValueChanged(object sender, EventArgs e)
        {
            string idJobTitle = cbbJobTitle.EditValue?.ToString();
            var lsCertReqSets = dt301_CertReqSetBUS.Instance.GetListByJobAndDept(idJobTitle, idDept2word);
            var lsCourses = (from certReq in lsCertReqSets
                             join course in dt301_CourseBUS.Instance.GetList() on certReq.IdCourse equals course.Id
                             select new dt301_Course
                             {
                                 Id = course.Id,
                                 DisplayName = $"{course.Id} {course.DisplayName}"
                             }).ToList();

            cbbCourse.Properties.DataSource = lsCourses;
            cbbCourse.EditValue = null;
        }
    }
}