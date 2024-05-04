using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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
            //cbbDept.Enabled = false;
            //cbbJobTitle.Enabled = false;
            //cbbUser.Enabled = _enable;
            //cbbCertStatus.Enabled = _enable;
            //cbbCourse.Enabled = _enable;
            //txbDateReceipt.Enabled = _enable;
            //txbDuration.Enabled = _enable;
            //txbDescribe.Enabled = _enable;
        }

        private void LockControl()
        {
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
                    EnabledController();
                    break;
                default:
                    break;
            }
        }

        private void f201_AddNode_Load(object sender, EventArgs e)
        {
            LockControl();

            var articles = new List<string>() { "5.1", "6.2", "6.3", "7.4" };
            txbArticles.Properties.DataSource = articles;

            switch (eventInfo)
            {
                case EventFormInfo.Create:
                    baseData = new dt201_Base();
                    break;
                case EventFormInfo.Update:

                    txbDocCode.EditValue = baseData.DocCode;
                    txbDisplayName.EditValue = baseData.DisplayName;
                    txbArticles.EditValue = baseData.Articles;
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
                baseData.DisplayName = txbDisplayName.Text;
                baseData.Articles = txbArticles.EditValue.ToString();

                msg = $"{baseData.DocCode} {baseData.DisplayName}";
                switch (eventInfo)
                {
                    case EventFormInfo.Create:
                        baseData.IdParent = baseParent.Id;

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