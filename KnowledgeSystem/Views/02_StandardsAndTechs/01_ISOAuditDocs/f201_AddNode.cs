using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AddNode : DevExpress.XtraEditors.XtraForm
    {
        public f201_AddNode()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public string _formName = "";
        public dt301_Base _base = null;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

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
                default:
                    break;
            }
        }

        private void f201_AddNode_Load(object sender, EventArgs e)
        {
            LockControl();

            //var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            //{
            //    Id = r.Id,
            //    DisplayName = $"{r.Id} {r.DisplayName}"
            //}).ToList();
            //cbbDept.Properties.DataSource = lsDepts;
            //cbbDept.Properties.DisplayMember = "DisplayName";
            //cbbDept.Properties.ValueMember = "Id";

            //var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            //{
            //    Id = r.Id,
            //    IdDepartment = r.IdDepartment,
            //    DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
            //    JobCode = r.JobCode
            //}).ToList();
            //cbbUser.Properties.DataSource = lsUsers;
            //cbbUser.Properties.DisplayMember = "DisplayName";
            //cbbUser.Properties.ValueMember = "Id";
            //cbbUser.Properties.BestFitWidth = 110;

            //var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Select(r => new dm_JobTitle() { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" });
            //cbbJobTitle.Properties.DataSource = lsJobTitles;
            //cbbJobTitle.Properties.DisplayMember = "DisplayName";
            //cbbJobTitle.Properties.ValueMember = "Id";

            //cbbCourse.Properties.DisplayMember = "DisplayName";
            //cbbCourse.Properties.ValueMember = "Id";

            //cbbCertStatus.Properties.Items.AddRange((CertStatus[])Enum.GetValues(typeof(CertStatus)));
            //cbbCertStatus.SelectedIndex = 0;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _base = new dt301_Base();
                    cbbDept.EditValue = idDept2word;
                    txbDateReceipt.EditValue = DateTime.Today;
                    break;
                case EventFormInfo.Update:
                    cbbDept.EditValue = _base.IdDept;
                    cbbUser.EditValue = _base.IdUser;
                    cbbJobTitle.EditValue = _base.IdJobTitle;
                    cbbCourse.EditValue = _base.IdCourse;
                    txbDateReceipt.EditValue = _base.DateReceipt;

                    int duration = _base.ExpDate.HasValue ? _base.ExpDate.Value.Year - _base.DateReceipt.Year : 0;
                    txbDuration.EditValue = duration;
                    txbDescribe.EditValue = _base.Describe;
                    break;
                default:
                    break;
            }
        }
    }
}