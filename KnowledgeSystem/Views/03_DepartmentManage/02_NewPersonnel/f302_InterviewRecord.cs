﻿using BusinessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class f302_InterviewRecord : DevExpress.XtraEditors.XtraForm
    {
        public f302_InterviewRecord()
        {
            InitializeComponent();
        }

        string idDept2word;

        private void f302_InterviewRecord_Load(object sender, EventArgs e)
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;

            idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

            var users = dm_UserBUS.Instance.GetListByDept(idDept2word).Where(r => r.Status == 0).ToList();
            cbbBossLv2.Properties.DataSource = users;
            cbbBossLv2.Properties.DisplayMember = "DisplayName";
            cbbBossLv2.Properties.ValueMember = "Id";

            var lsJobTitles = dm_JobTitleBUS.Instance.GetList();
            cbbJobTitle.Properties.DataSource = lsJobTitles;
            cbbJobTitle.Properties.DisplayMember = "DisplayName";
            cbbJobTitle.Properties.ValueMember = "Id";

            cbbRecordNo.Properties.Items.AddRange(new[] { "第一次", "第二次", "第三次", "第四次", "第五次" });
        }

        private void txbUserId_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            var dmUsers = dm_UserBUS.Instance.GetItemById(txbUserId.EditValue?.ToString());
            if (dmUsers == null) return;

            txbUserNameTW.EditValue = dmUsers.DisplayName?.Trim();
            txbDept.EditValue = dmUsers.IdDepartment;
            cbbJobTitle.EditValue = dmUsers.ActualJobCode;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string salt = Guid.NewGuid().ToString();
            int seed = salt.GetHashCode();
            Random rng = new Random(seed);
            int randomNumber = rng.Next(1, 6);

            List<int> scores = Enumerable.Repeat(8, randomNumber).ToList();
            scores.AddRange(Enumerable.Repeat(10, 10 - randomNumber).ToList());

            int n = scores.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = scores[k];
                scores[k] = scores[n];
                scores[n] = value;
            }

            string interviewId = txbUserId.EditValue.ToString();
            string interviewName = HttpUtility.UrlEncode(txbUserNameTW.Text).Replace("+", "%20").ToUpper();
            string dept = txbDept.Text;
            string jobId = cbbJobTitle.EditValue.ToString();
            string jobName = HttpUtility.UrlEncode(cbbJobTitle.Text).Replace("+", "%20").ToUpper();
            string bossLv2 = cbbBossLv2.EditValue.ToString();
            string dateRecord = txbDateRecord.DateTime.ToString("yyyy-MM-dd");
            string noRecord = HttpUtility.UrlEncode(cbbRecordNo.Text).Replace("+", "%20").ToUpper();
            string scoreRecord = string.Join("vkv", scores);
            string remark = HttpUtility.UrlEncode(txbRemark.Text.Trim()).Replace("+", "%20").ToUpper();

            string url = $"https://www.fhs.com.tw/ads/api/Furnace/rest/json/hr/s26/{interviewId}vkv{interviewName}vkvLG{dept}vkv{jobId}{jobName}vkv{bossLv2}vkv{dateRecord}vkv{noRecord}vkv{scoreRecord}vkv90-100%E5%88%86(%E5%84%AA%E7%AD%89)vkv{remark}vkv{TPConfigs.LoginUser.Id}";

            using (WebClient client = new WebClient())
            {
                //try
                //{
                //    string response = client.DownloadString(url);
                //    if (response == "ok")
                //    {
                //        XtraMessageBox.Show("上傳成功，請二級主管上FPGFlow執行核准！", TPConfigs.SoftNameTW);
                //        return;
                //    }
                //}
                //catch { }
            }

            XtraMessageBox.Show("系統發生問題！", TPConfigs.SoftNameTW);
        }
    }
}