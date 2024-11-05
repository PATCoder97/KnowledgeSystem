using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public dt201_Base baseData = null;
        public dt201_Base baseParent = null;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void EnabledController(bool _enable = true)
        {
            cbbDept.Enabled = baseParent == null && _enable;
            ckPaperType.Enabled = baseParent == null || baseParent.IsPaperType != true;
        }

        private void LockControl()
        {
            if (baseParent != null)
            {
                ckPaperType.Checked = baseParent.IsPaperType == true;
                cbbDept.EditValue = baseParent.IdDept;
            }

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    Text = $"新增{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    EnabledController();
                    break;
                case EventFormInfo.Update:
                    Text = $"更新{formName}";

                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    EnabledController(false);
                    break;
                default:
                    break;
            }
        }

        private void f201_AddNode_Load(object sender, EventArgs e)
        {
            var depts = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName("ISO組");

            var deptsCbb = (from data in depts
                            join grp in groups on data.Id equals grp.IdDept
                            select data.Id).ToList();

            cbbDept.Properties.Items.AddRange(deptsCbb);
            cbbDept.SelectedIndex = 0;

            var articles = TPConfigs.Articles201.Split(';').ToList();
            txbArticles.Properties.DataSource = articles;

            cbbDocType.Properties.Items.AddRange(TPConfigs.DocTypes201.Split(';').ToList());

            LockControl();

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    baseData = new dt201_Base();
                    break;
                case EventFormInfo.Update:

                    txbDocCode.EditValue = baseData.DocCode;
                    txbDisplayName.EditValue = baseData.DisplayName;
                    txbDisplayNameVN.EditValue = baseData.DisplayNameVN;
                    txbArticles.EditValue = baseData.Articles;
                    cbbDept.EditValue = baseData.IdDept;
                    ckPaperType.Checked = baseData.IsPaperType == true;
                    txbNotifyCycle.EditValue = baseData.NotifyCycle;
                    cbbDocType.EditValue = baseData.DocType;

                    break;
                default:
                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var result = false;
            string msg = "";
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                baseData.DocCode = txbDocCode.Text;
                baseData.DisplayName = txbDisplayName.Text.Trim();
                baseData.DisplayNameVN = txbDisplayNameVN.Text.Trim();
                baseData.Articles = txbArticles.EditValue?.ToString() ?? "";
                baseData.IdDept = cbbDept.EditValue.ToString();
                baseData.IsPaperType = ckPaperType.Checked;
                baseData.NotifyCycle = Convert.ToInt16(txbNotifyCycle.EditValue?.ToString() ?? "0");
                baseData.DocType = cbbDocType.Text;

                msg = $"{baseData.DocCode} {baseData.DisplayName}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        baseData.IdParent = baseParent?.Id ?? -1;

                        result = dt201_BaseBUS.Instance.Add(baseData) > 0;
                        break;
                    case EventFormInfo.Update:
                        result = dt201_BaseBUS.Instance.AddOrUpdate(baseData);
                        break;
                    default:
                        break;
                }
            }

            if (result)
            {
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }
    }
}