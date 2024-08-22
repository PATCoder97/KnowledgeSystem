using BusinessLayer;
using DataAccessLayer;
using DevExpress.Xpo.DB;
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
    public partial class f307_UsersData : DevExpress.XtraEditors.XtraForm
    {
        public f307_UsersData()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public List<dm_User> UsersInput { get; set; }
        public List<dm_User> UsersOutput { get; set; }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void LoadData()
        {
            var idUsrs = UsersInput.Select(r => r.Id).ToList();

            // Lấy danh sách các phòng ban và chức danh công việc
            var depts = dm_DeptBUS.Instance.GetList();
            var jobs = dm_JobTitleBUS.Instance.GetList();

            // Lọc danh sách người dùng theo điều kiện
            var usrs = dm_UserBUS.Instance.GetListByDept("7")
                .Where(r => r.Status == 0 &&
                            !idUsrs.Contains(r.Id) &&
                            r.ActualJobCode != null &&
                            r.ActualJobCode.EndsWith("J"))
                .ToList();

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

            gcData.DataSource = datas;
            gvData.BestFitColumns();
        }

        private void f307_UsersData_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var rows = gvData.GetSelectedRows();

            List<dm_User> usrsOutput = new List<dm_User>();
            foreach (var item in rows)
            {
                var data = gvData.GetRow(item);

                if (data != null)
                {
                    // Extract the original dm_User object from the anonymous type
                    var usr = ((dynamic)data).usr as dm_User;
                    usrsOutput.Add(usr);
                }
            }

            UsersOutput = usrsOutput;
            Close();
        }
    }
}