using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class f301_CertReqSetInfo : DevExpress.XtraEditors.XtraForm
    {
        public f301_CertReqSetInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public string _formName = "";
        public dt301_CertReqSetting _certReq = null;
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
            cbbJobTitle.Enabled = _enable;
            cbbCourse.Enabled = _enable;
            txbNewHeadcount.Enabled = _enable;
            txbActualHeadcount.Enabled = _enable;
            txbReqQuantity.Enabled = _enable;
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


        private void f301_CertReqSetInfo_Load(object sender, EventArgs e)
        {


            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments
            {
                Id = r.Id,
                DisplayName = $"{r.Id} {r.DisplayName}"
            }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList().Select(r => new dm_JobTitle() { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" });
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            var lsCourse = dt301_CourseBUS.Instance.GetList().Select(r => new dt301_Course() { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" });
            cbbCourse.Properties.DataSource = lsCourse;
            cbbCourse.Properties.DisplayMember = "DisplayName";
            cbbCourse.Properties.ValueMember = "Id";

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _certReq = new dt301_CertReqSetting();
                    cbbDept.EditValue = idDept2word;
                    break;
                case EventFormInfo.View:
                    cbbDept.EditValue = _certReq.IdDept;
                    cbbJobTitle.EditValue = _certReq.IdJobTitle;
                    cbbCourse.EditValue = _certReq.IdCourse;
                    txbNewHeadcount.EditValue = _certReq.NewHeadcount;
                    txbActualHeadcount.EditValue = _certReq.ActualHeadcount;
                    txbReqQuantity.EditValue = _certReq.ReqQuantity;

                    Hide();
                    cbbJobTitle.ShowPopup();
                    cbbCourse.ShowPopup();
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

            LockControl();
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
            string jobTitles = cbbJobTitle.EditValue?.ToString();
            string course = cbbCourse.EditValue?.ToString();

            if (string.IsNullOrEmpty(jobTitles) || string.IsNullOrEmpty(course))
            {
                XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                _certReq.IdDept = idDept2word;
                _certReq.IdJobTitle = jobTitles;
                _certReq.IdCourse = course;
                _certReq.NewHeadcount = Convert.ToInt16(txbNewHeadcount.EditValue);
                _certReq.ActualHeadcount = Convert.ToInt16(txbActualHeadcount.EditValue);
                _certReq.ReqQuantity = Convert.ToInt16(txbReqQuantity.EditValue);

                msg = $"{_certReq.IdDept} {_certReq.IdJobTitle} {_certReq.IdCourse} {_certReq.ReqQuantity}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dt301_CertReqSetBUS.Instance.Add(_certReq);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = dt301_CertReqSetBUS.Instance.AddOrUpdate(_certReq);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{_formName}: {_certReq.IdJobTitle} {_certReq.IdCourse}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt301_CertReqSetBUS.Instance.Remove(_certReq.Id);
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
    }
}