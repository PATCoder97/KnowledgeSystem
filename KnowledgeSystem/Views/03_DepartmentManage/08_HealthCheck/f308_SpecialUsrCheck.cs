using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_SpecialUsrCheck : DevExpress.XtraEditors.XtraForm
    {
        public f308_SpecialUsrCheck()
        {
            InitializeComponent();

            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        List<dm_User> usrs;
        List<dm_User> oldUsrs;

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            f308_UserData fData = new f308_UserData();
            fData.UsersInput = usrs;
            fData.ShowDialog();

            if (fData.UsersOutput == null) return;

            usrs.AddRange(fData.UsersOutput);

            gvData.RefreshData();
        }

        private void btnDelUser_Click(object sender, EventArgs e)
        {
            gvData.GetSelectedRows()
                .Select(r => gvData.GetRow(r) as dm_User)
                .Where(u => u != null)
                .ToList()
                .ForEach(u => usrs.Remove(u));

            gvData.ClearSelection();
            gvData.RefreshData();
        }

        private void f308_SpecialUsrCheck_Load(object sender, EventArgs e)
        {
            // Lấy danh sách nhân viên là dùng tạm IP để làm mốc là nhân viên cũ (từ cơ sở dữ liệu lên)
            var usrIds = dt308_SpecialUsrCheckBUS.Instance.GetList().Select(r => r.Id).ToList();
            usrs = dm_UserBUS.Instance.GetList().Where(r => usrIds.Contains(r.Id) && r.IdDepartment.StartsWith(TPConfigs.idDept2word)).Select(u => { u.IPAddress = "1"; return u; }).ToList();
            oldUsrs = dm_UserBUS.Instance.GetList().Where(r => usrIds.Contains(r.Id) && r.IdDepartment.StartsWith(TPConfigs.idDept2word)).Select(u => { u.IPAddress = "1"; return u; }).ToList();

            gcData.DataSource = usrs;
            gvData.BestFitColumns();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // So sánh sự khác biệt dựa trên Id và điều kiện IPAddress
            var addedUsers = usrs.Where(u => !oldUsrs.Any(o => o.Id == u.Id) && u.IPAddress != "1").ToList();
            var removedUsers = oldUsrs.Where(o => !usrs.Any(u => u.Id == o.Id)).ToList();

            // Thêm mới
            foreach (var item in addedUsers)
            {
                var result = dt308_SpecialUsrCheckBUS.Instance.Add(new dt308_SpecialUsrCheck() { Id = item.Id });
            }

            // Xóa
            foreach (var item in removedUsers)
            {
                dt308_SpecialUsrCheckBUS.Instance.RemoveById(item.Id);
            }

            Close();
        }
    }
}