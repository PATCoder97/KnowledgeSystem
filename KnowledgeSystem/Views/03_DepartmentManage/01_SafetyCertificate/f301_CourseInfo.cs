using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate
{
    public partial class f301_CourseInfo : DevExpress.XtraEditors.XtraForm
    {
        public f301_CourseInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string _formName = string.Empty;
        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public dt301_Course _course = null;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDisplayName.Enabled = _enable;
            txbDuration.Enabled = _enable;
            cbbCategory.Enabled = _enable;
            cbbTypeOf.Enabled = _enable;
        }

        private void LockControl()
        {
            txbId.Enabled = false;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    txbId.Enabled = true;

                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController();
                    break;
                case EventFormInfo.Delete:
                    Text = $"刪除{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

                    EnabledController(false);
                    break;
                case EventFormInfo.View:
                    Text = $"{_formName}信息";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f301_CourseInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            List<string> lsCategories = new List<string>() { "第一類", "第二類", "第三類", "第四類", "第五類", "第六類", "其他" };
            cbbCategory.Properties.Items.AddRange(lsCategories);

            List<string> typeOfs = new List<string>() { "勞動安全衛生證照及職業執照訓練", "輻射操作人員安全訓練", "化學藥劑安全教育訓練", "消防及救難救護業務" };
            cbbTypeOf.Properties.Items.AddRange(typeOfs);

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _course = new dt301_Course();
                    cbbCategory.SelectedIndex = 0;
                    break;
                case EventFormInfo.View:
                    txbId.EditValue = _course.Id;
                    txbDisplayName.EditValue = _course.DisplayName;
                    txbDuration.EditValue = _course.Duration;
                    cbbCategory.EditValue = _course.Category;
                    cbbTypeOf.EditValue = _course.TypeOf;
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

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string newId = txbId.EditValue?.ToString();
            string newDisplayName = txbDisplayName.EditValue?.ToString();
            int duration = Convert.ToInt16(txbDuration.EditValue?.ToString());
            string category = cbbCategory.EditValue?.ToString();
            string typeOf = cbbTypeOf.EditValue?.ToString();

            if (string.IsNullOrEmpty(newId) || string.IsNullOrEmpty(newDisplayName))
            {
                XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                _course.Id = newId;
                _course.DisplayName = newDisplayName;
                _course.Duration = duration;
                _course.Category = category;
                _course.TypeOf = typeOf;

                msg = $"{_course.Id} {_course.DisplayName}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        result = dt301_CourseBUS.Instance.Add(_course);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        result = dt301_CourseBUS.Instance.AddOrUpdate(_course);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"您確認要刪除{_formName}: {_course.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;
                        result = dt301_CourseBUS.Instance.Remove(_course.Id);
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
    }
}