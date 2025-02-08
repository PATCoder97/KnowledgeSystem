using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_CheckData : DevExpress.XtraEditors.XtraForm
    {
        public f308_CheckData()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idDetail = -1;
        string idDept2word = TPConfigs.idDept2word;
        List<dt308_Disease> diseases;
        dt308_CheckDetail checkDetail = new dt308_CheckDetail();

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            txbDisease1.Enabled = _enable;
            txbDisease1.Enabled = _enable;
            txbDisease1.Enabled = _enable;
            radioType.Enabled = _enable;
        }

        private void LockControl()
        {
            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

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

                    EnabledController(false);
                    break;
                default:
                    break;
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

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;
            foreach (var item in lcImpControls)
            {
                if (string.IsNullOrEmpty(item.Control.Text)) IsValidate = false;
            }

            if (Convert.ToInt16(radioType.EditValue) == -1) IsValidate = false;

            if (!IsValidate)
            {
                MsgTP.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }

            // Sử dụng Regex để lấy các số trong dấu ngoặc đơn
            string ExtractDiseaseIds(string input)
            {
                return string.Join(",",
                    Regex.Matches(input ?? "", @"\((\d+)\)")
                         .Cast<Match>()
                         .Select(m => int.Parse(m.Groups[1].Value)) // Chuyển thành int
                         .Distinct()
                );
            }

            // Gán giá trị cho các trường Disease
            checkDetail.Disease1 = ExtractDiseaseIds(txbDisease1.EditValue?.ToString());
            checkDetail.Disease2 = ExtractDiseaseIds(txbDisease2.EditValue?.ToString());
            checkDetail.Disease3 = ExtractDiseaseIds(txbDisease3.EditValue?.ToString());

            checkDetail.HealthRating = Convert.ToInt16(radioType.EditValue);

            var result = dt308_CheckDetailBUS.Instance.AddOrUpdate(checkDetail);
            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void f308_CheckData_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>() { lcUser, lcType, lcDisease1, lcDisease2, lcDisease3 };
            lcImpControls = new List<LayoutControlItem>() { lcUser, lcType };

            foreach (var item in lcControls)
            {
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{item.Text}</color>";
            }

            var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            {
                Id = r.Id,
                IdDepartment = r.IdDepartment,
                DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
                DisplayNameVN = $"LG{r.IdDepartment} {r.DisplayName} {r.DisplayNameVN}",
                JobCode = r.JobCode
            }).ToList();
            cbbUsr.Properties.DataSource = lsUsers;
            cbbUsr.Properties.DisplayMember = "DisplayNameVN";
            cbbUsr.Properties.ValueMember = "Id";
            cbbUsr.Properties.BestFitWidth = 110;

            diseases = dt308_DiseaseBUS.Instance.GetList();
            var diseaseTypes = new[] { 1, 2, 3 };

            foreach (var diseaseType in diseaseTypes)
            {
                var tokens = diseases.Where(r => r.DiseaseType == diseaseType).ToList();
                var tokenEdit = diseaseType == 1 ? txbDisease1.Properties.Tokens :
                                diseaseType == 2 ? txbDisease2.Properties.Tokens :
                                txbDisease3.Properties.Tokens;

                foreach (var item in tokens)
                    tokenEdit.AddToken(new TokenEditToken($"({item.Id:D2}) {item.DisplayNameVN} / {item.DisplayNameTW}", item.Id));
            }

            // Create five items.
            object[] itemValues = new object[] { 1, 2, 3, 4, 5 };
            string[] itemDescriptions = new string[] { "I", "II", "III", "IV", "V" };
            for (int i = 0; i < itemValues.Length; i++)
            {
                radioType.Properties.Items.Add(new RadioGroupItem(itemValues[i], itemDescriptions[i]));
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:

                    checkDetail = dt308_CheckDetailBUS.Instance.GetItemById(idDetail);
                    cbbUsr.EditValue = checkDetail.EmpId;

                    radioType.EditValue = checkDetail.HealthRating;
                    txbDisease1.EditValue = string.Join(",", diseases
                        .Where(r => (checkDetail.Disease1?.Split(',') ?? Array.Empty<string>())
                        .Contains(r.Id.ToString()))
                        .Select(r => $"({r.Id:D2}) {r.DisplayNameVN} / {r.DisplayNameTW}"));

                    txbDisease2.EditValue = string.Join(",", diseases
                         .Where(r => (checkDetail.Disease2?.Split(',') ?? Array.Empty<string>())
                         .Contains(r.Id.ToString()))
                         .Select(r => $"({r.Id:D2}) {r.DisplayNameVN} / {r.DisplayNameTW}"));

                    txbDisease3.EditValue = string.Join(",", diseases
                        .Where(r => (checkDetail.Disease3?.Split(',') ?? Array.Empty<string>())
                        .Contains(r.Id.ToString()))
                        .Select(r => $"({r.Id:D2}) {r.DisplayNameVN} / {r.DisplayNameTW}"));

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

        }
    }
}