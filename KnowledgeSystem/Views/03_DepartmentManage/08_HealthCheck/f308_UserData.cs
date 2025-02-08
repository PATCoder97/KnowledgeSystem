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
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_UserData : DevExpress.XtraEditors.XtraForm
    {
        public f308_UserData()
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

            // Lọc danh sách người dùng theo điều kiện
            var usrs = dm_UserBUS.Instance.GetListByDept(TPConfigs.idDept2word)
                .Where(r => r.Status == 0 && !idUsrs.Contains(r.Id)).ToList();

            gcData.DataSource = usrs;
            gvData.BestFitColumns();
        }

        private void f308_UserData_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var rows = gvData.GetSelectedRows();

            List<dm_User> usrsOutput = new List<dm_User>();
            foreach (var item in rows)
            {
                var data = gvData.GetRow(item) as dm_User;
                usrsOutput.Add(data);
            }

            UsersOutput = usrsOutput;
            Close();
        }
    }
}