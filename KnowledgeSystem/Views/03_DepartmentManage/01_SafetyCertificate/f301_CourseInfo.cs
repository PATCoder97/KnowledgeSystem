﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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
            txbNewDisplayName.Enabled = _enable;
        }

        private void LockControl()
        {
            txbNewId.Enabled = false;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{_formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    txbNewId.Enabled = true;

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

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _course = new dt301_Course();
                    break;
                case EventFormInfo.View:
                    txbNewId.EditValue = _course.Id;
                    txbNewDisplayName.EditValue = _course.DisplayName;
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
            string newId = txbNewId.EditValue?.ToString();
            string newDisplayName = txbNewDisplayName.EditValue?.ToString();

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
                DefaultMsg.MsgErrorDB();
            }
        }
    }
}