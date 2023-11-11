using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Svg;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class f402_FuncInfo : DevExpress.XtraEditors.XtraForm
    {
        public f402_FuncInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public string _formName = string.Empty;
        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public dm_FunctionM _function = null;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbIdParent.Enabled = _enable;
            txbFunction.Enabled = _enable;
            cbbControl.Enabled = _enable;
            txbPrioritize.Enabled = _enable;
            cbbPicture.Enabled = _enable;
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

        private void f402_FuncInfo_Load(object sender, EventArgs e)
        {
            LockControl();

            var lsControls = Assembly.GetExecutingAssembly().GetTypes().Where(a => a.BaseType == typeof(XtraForm) || a.BaseType == typeof(XtraUserControl)).Select(r => r.Name).ToList();
            cbbControl.Properties.Items.AddRange(lsControls);

            //var lsFuncEvents = Enum.GetValues(typeof(FuncEvent)).Cast<FuncEvent>().Select(r => r.ToString()).ToList();
            //cbbControl.Properties.Items.AddRange(lsFuncEvents);

            SvgImageCollection svgImages = new SvgImageCollection();
            string[] svgFiles = Directory.GetFiles(TPConfigs.ImagesPath, "*.svg");
            for (int i = 0; i < svgFiles.Count(); i++)
            {
                var pic = SvgImage.FromFile(svgFiles[i]);
                var nameSvg = Path.GetFileName(svgFiles[i]);
                svgImages.Add(pic);
                cbbPicture.Properties.Items.Add(new ImageComboBoxItem(nameSvg, i, i));
            }
            cbbPicture.Properties.SmallImages = svgImages;

            var lsFuncs = dm_FunctionBUS.Instance.GetList();
            lsFuncs.Add(new dm_FunctionM() { Id = -1, DisplayName = "Root" });

            cbbIdParent.Properties.DataSource = lsFuncs;
            cbbIdParent.Properties.DisplayMember = "DisplayName";
            cbbIdParent.Properties.ValueMember = "Id";

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _function = new dm_FunctionM();
                    break;
                case EventFormInfo.View:
                    txbId.EditValue = _function.Id;
                    cbbIdParent.EditValue = _function.IdParent;
                    txbFunction.EditValue = _function.DisplayName;
                    cbbControl.EditValue = _function.ControlName;
                    txbPrioritize.EditValue = _function.Prioritize;

                    var indexImage = cbbPicture.Properties.Items.ToList().FirstOrDefault(r => r.Description == _function.Images)?.ImageIndex;
                    cbbPicture.EditValue = indexImage;
                    break;
                case EventFormInfo.Update:
                    break;
                case EventFormInfo.Delete:
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
            MsgTP.MsgConfirmDel();

            _eventInfo = EventFormInfo.Delete;
            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                string _idParent = cbbIdParent.EditValue?.ToString();
                _function.IdParent = string.IsNullOrEmpty(_idParent) ? -1 : int.Parse(_idParent);
                _function.DisplayName = txbFunction.EditValue?.ToString();
                _function.ControlName = cbbControl.EditValue?.ToString();

                string _prioritize = txbPrioritize.EditValue?.ToString();
                _function.Prioritize = string.IsNullOrEmpty(_prioritize) ? -1 : int.Parse(_prioritize);
                _function.Images = cbbPicture.Text?.ToString();
                _function.Status = false;

                msg = $"{_function.Id} {_function.IdParent} {_function.DisplayName} {_function.ControlName} {_function.Images}";
                switch (_eventInfo)
                {
                    case EventFormInfo.Create:
                        _function.Id = dm_FunctionBUS.Instance.GetNewId();

                        result = dm_FunctionBUS.Instance.Add(_function);
                        break;
                    case EventFormInfo.View:
                        break;
                    case EventFormInfo.Update:
                        _function.Id = (int)txbId.EditValue;
                        result = dm_FunctionBUS.Instance.AddOrUpdate(_function);
                        break;
                    case EventFormInfo.Delete:
                        var dialogResult = XtraMessageBox.Show($"Bạn xác nhận muốn xoá {_formName}: {_function.DisplayName}", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dialogResult != DialogResult.Yes) return;

                        result = dm_FunctionBUS.Instance.Remove(_function.Id);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                //switch (_eventInfo)
                //{
                //    case EventInfo.Update:
                //        logger.Info(_eventInfo.ToString(), msg);
                //        break;
                //    case EventInfo.Delete:
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