using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.Xpo;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._07_Quiz
{
    public partial class f307_Exam_Info : DevExpress.XtraEditors.XtraForm
    {
        public f307_Exam_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<dm_User> usrs = new List<dm_User>();
        BindingSource sourceUser = new BindingSource();
        int MultiQuesLimit = 4;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnAddUsr.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRemoveUsr.ImageOptions.SvgImage = TPSvgimages.Remove;
        }

        private void LoadData()
        {
            var depts = dm_DeptBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();
            var ques = dt307_QuestionsBUS.Instance.GetList();
            var JobsSettings = dt307_JobQuesManageBUS.Instance.GetList();

            // Lấy ra danh sách nhân viên tham gia kỳ thì
            var datas = (from usr in usrs
                         join job in jobs on usr.ActualJobCode equals job.Id
                         join dept in depts on usr.IdDepartment equals dept.Id
                         let DeptName = $"{dept.Id}\r\n{dept.DisplayName}"
                         let DisplayName = $"{usr.DisplayName}\r\n{usr.DisplayNameVN}"
                         select new
                         {
                             usr,
                             job,
                             dept,
                             DeptName,
                             DisplayName
                         }).ToList();

            // Lấy ra tổng câu hỏi và câu hỏi nhiều lựa chọn
            var quesCount = from data in ques
                            group data by data.IdJob into dtg
                            select new
                            {
                                IdJob = dtg.Key,
                                QuestionCount = dtg.Count(),
                                MultiQuesCount = dtg.Count(r => r.IsMultiAns == true),
                            };

            // Check xem cài đặt của các chức vụ đã đạt chưa
            var JobsRemark = (from data in JobsSettings
                              join question in quesCount on data.JobId equals question.IdJob into dtg
                              from g in dtg.DefaultIfEmpty()
                              let Remark = String.Join("、", (new string[]
                              {
                                  data.QuesCount > (g != null ? g.QuestionCount : 0) ? "題目數量不夠" : "",
                                  MultiQuesLimit > (g != null ? g.MultiQuesCount : 0) ? "複選擇題數量不夠" : ""
                              }).Where(s => !string.IsNullOrEmpty(s)))
                              select new
                              {
                                  data,
                                  QuesCount = g != null ? g.QuestionCount : 0,
                                  MultiQuesCount = g != null ? g.MultiQuesCount : 0,
                                  Remark
                              }).ToList();

            // Tổng hợp dữ liệu đưa lên gridview
            var results = (from data in datas
                           join remark in JobsRemark on data.usr.ActualJobCode equals remark.data.JobId into dtg
                           from g in dtg.DefaultIfEmpty()
                           select new
                           {
                               data,
                               Remark = g != null ? g.Remark : "職務尚未設定題目"
                           }).ToList();

            sourceUser.DataSource = results;
            gvData.BestFitColumns();
        }

        private void f307_Exam_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceUser;
            LoadData();
        }

        private void btnAddUsr_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f307_UsersData fData = new f307_UsersData();
            fData.UsersInput = usrs;
            fData.ShowDialog();

            if (fData.UsersOutput == null) return;

            usrs.AddRange(fData.UsersOutput);

            LoadData();
        }

        private void btnRemoveUsr_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var rows = gvData.GetSelectedRows();

            foreach (var item in rows)
            {
                var data = gvData.GetRow(item);

                if (data != null)
                {
                    // Extract the original dm_User object from the anonymous type
                    var usr = ((dynamic)data).data.usr as dm_User;
                    usrs.Remove(usr);
                }
            }

            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var data = new dt307_ExamMgmt()
            {
                Code = "",
                DisplayName = txbExamName.Text.Trim(),
                CreateTime = DateTime.Now,
            };

            int idExam = dt307_ExamMgmtBUS.Instance.Add(data);

            string codeExam = dt307_ExamMgmtBUS.Instance.GetItemById(idExam).Code;

            List<dt307_ExamUser> examUsrs = usrs.Select(r => new dt307_ExamUser()
            {
                ExamCode = codeExam,
                IdJob = r.ActualJobCode,
                IdUser = r.Id,
                IsPass = false
            }).ToList();

            var result = dt307_ExamUserBUS.Instance.AddRange(examUsrs);
            Close();
        }
    }
}