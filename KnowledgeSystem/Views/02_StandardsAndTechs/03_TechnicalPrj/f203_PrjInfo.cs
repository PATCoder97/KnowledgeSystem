using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using KnowledgeSystem.Helpers;
using MiniSoftware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._03_TechnicalPrj
{
    public partial class f203_PrjInfo : DevExpress.XtraEditors.XtraForm
    {
        public f203_PrjInfo()
        {
            InitializeComponent();
        }

        public string formName = string.Empty;
        public EventFormInfo eventInfo = EventFormInfo.Create;
        //public int idBase302 = -1;
        string idDept2word;

        dt302_Base personBase;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;

        List<dm_User> users;
        List<dm_JobTitle> jobs;

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f203_PrjInfo_Load(object sender, EventArgs e)
        {
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            //lcControls = new List<LayoutControlItem>() { lcUserID, lcUserName, lcDept, lcJob, lcSupervisor, lcSchool, lcMajor, lcEnterDate };
            //lcImpControls = new List<LayoutControlItem>() { lcUserID, lcSupervisor, lcSchool, lcMajor };
            //foreach (var item in lcControls)
            //{
            //    item.AllowHtmlStringInCaption = true;
            //    item.Text = $"<color=#FFFFFF>{item.Text}</color>";
            //}

            //LockControl();

            //var lsDepts = dm_DeptBUS.Instance.GetList().Where(r => r.Id.Length == 4 && r.Id.StartsWith(idDept2word))
            //    .Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5}{r.DisplayName}" }).ToList();
            //cbbDept.Properties.DataSource = lsDepts;
            //cbbDept.Properties.DisplayMember = "DisplayName";
            //cbbDept.Properties.ValueMember = "Id";

            //var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            //cbbJobTitle.Properties.DataSource = lsJobTitles;
            //cbbJobTitle.Properties.DisplayMember = "DisplayName";
            //cbbJobTitle.Properties.ValueMember = "Id";
            jobs = dm_JobTitleBUS.Instance.GetList();

            users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            tokenUserID.Properties.DataSource = users;
            tokenUserID.Properties.DisplayMember = "DisplayName";
            tokenUserID.Properties.ValueMember = "Id";
            tokenUserID.EditValue = TPConfigs.LoginUser.Id;

            var typeOf = new List<string>() { "系統流程優化", "資材成本降低", "產品品質提升", "生產製程改善", "檢驗技術開發", "客戶服務品質提升", "人事管理優化", "降低工安事故發生" };
            cbbTypeOf.Properties.Items.AddRange(typeOf);

            //switch (eventInfo)
            //{
            //    case EventFormInfo.Create:
            //        personBase = new dt302_Base();
            //        break;
            //    case EventFormInfo.View:
            //        personBase = dt302_BaseBUS.Instance.GetItemById(idBase302);

            //        cbbSupervisor.EditValue = personBase.Supervisor;
            //        txbSchool.EditValue = personBase.School;
            //        txbMajor.EditValue = personBase.Major;

            //        var dmUsers = dm_UserBUS.Instance.GetItemById(personBase.IdUser);

            //        txbUserId.EditValue = personBase.IdUser;
            //        txbUserNameVN.EditValue = $"{dmUsers.DisplayName?.Trim()} {dmUsers.DisplayNameVN?.Trim()}";
            //        cbbDept.EditValue = dmUsers.IdDepartment;
            //        cbbJobTitle.EditValue = dmUsers.JobCode;
            //        txbDateStart.EditValue = dmUsers.DateCreate;

            //        break;
            //}
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string PATH_TEMPLATE = @"C:\Users\ANHTUAN\Desktop\Working\TechnicalPrjTemp.docx";
            string PATH_EXPORT = $@"C:\Users\ANHTUAN\Desktop\Working\TechnicalPrjTemp\Jishu_{DateTime.Now.ToString("yyyyMMddHHmmss")}.docx";

            var tokens = tokenUserID.GetTokenList();
            string userName = string.Join("\r\n", tokens.Select(r => r.Description as string).ToList());

            var userIds = tokens.Select(r => r.Value as string).ToList();

            var jobNames = (from data in users
                            where userIds.Contains(data.Id)
                            join name in jobs on data.JobCode equals name.Id into joinedJobs
                            from subName in joinedJobs.DefaultIfEmpty()
                            orderby userIds.IndexOf(data.Id)
                            select subName?.DisplayName).ToList();

            var jobNamesString = string.Join("\r\n", jobNames);

            var charsUserId = Enumerable.Range(0, 10).Select(i => string.Join("\r\n", userIds.Select(chuoi => chuoi.ElementAtOrDefault(i)))).ToList();

            string subject = txbSubject.EditValue?.ToString();
            string typeOf = cbbTypeOf.EditValue?.ToString();

            var a1Array = txbA1.EditValue?.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var a1 = a1Array.Select((value1, index) => a1Array.Length > 1 ? $"1.{index + 1}. {value1}" : value1).ToArray();

            var value = new
            {
                user_name = userName,
                v0 = charsUserId[0],
                v1 = charsUserId[1],
                v2 = charsUserId[2],
                v3 = charsUserId[3],
                v4 = charsUserId[4],
                v5 = charsUserId[5],
                v6 = charsUserId[6],
                v7 = charsUserId[7],
                v8 = charsUserId[8],
                v9 = charsUserId[9],
                job = jobNamesString,
                subject = subject,
                type = typeOf,
                dept = $"LG{TPConfigs.LoginUser.IdDepartment}",
                a1 = a1,
                a2 = txbA2.EditValue?.ToString(),
                b1 = txbB2.EditValue?.ToString(),
                b2 = txbB2.EditValue?.ToString(),
                c1 = txbC1.EditValue?.ToString(),
                c2 = txbC2.EditValue?.ToString(),
                c3 = txbC3.EditValue?.ToString(),
            };

            MiniWord.SaveAsByTemplate(PATH_EXPORT, PATH_TEMPLATE, value);

            Process.Start(PATH_EXPORT);
        }
    }
}