using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_UnitEHSOrg : DevExpress.XtraEditors.XtraUserControl
    {
        public uc310_UnitEHSOrg()
        {
            InitializeComponent();
            InitializeIcon();
        }

        BindingSource sourceFunc = new BindingSource();
        BindingSource sourceOrg = new BindingSource();

        List<dm_Departments> depts;
        List<dt310_UnitEHSOrg> unitEHSOrgs;
        List<dt310_Role> roles;
        List<dm_User> users;
        List<UnitEHSOrgCustom> unitEHSOrgCustoms;

        private class UnitEHSOrgCustom
        {
            public int Id { get; set; }
            public int IdParent { get; set; }
            public string DeptName { get; set; }
            public string Role { get; set; }
            public string Emp { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSummaryTable.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        /// <summary>
        /// Hàm đệ quy thêm Dept và các node con vào unitEHSOrgCustoms
        /// </summary>
        void AddDepartmentRecursive(
            dm_Departments dept,
            int parentId,
            ref int index,
            List<UnitEHSOrgCustom> unitEHSOrgCustoms,
            List<dm_Departments> allDepts,
            List<dt310_UnitEHSOrg> unitEHSOrgs,
            List<dt310_Role> roles,
            List<dm_User> users)
        {
            // Add Dept
            var deptNode = new UnitEHSOrgCustom
            {
                Id = index,
                IdParent = parentId,
                DeptName = $"{dept.Id} {dept.DisplayName}"
            };
            index++;

            unitEHSOrgCustoms.Add(deptNode);

            // Add EHS orgs trong Dept này
            var orgs = unitEHSOrgs.Where(r => r.DeptId == dept.Id).ToList();
            foreach (var org in orgs)
            {
                var roleObj = roles.FirstOrDefault(r => r.Id == Convert.ToInt32(org.Role));
                var userObj = users.FirstOrDefault(r => r.Id == org.EmployeeId);

                unitEHSOrgCustoms.Add(new UnitEHSOrgCustom
                {
                    Id = index,
                    IdParent = deptNode.Id,
                    Emp = $"{userObj?.IdDepartment} {userObj?.DisplayName} {userObj?.DisplayNameVN}",
                    Role = roleObj?.DisplayName,
                    DeptName = $"↪ {dept.Id}：{roleObj?.DisplayName}"
                });
                index++;
            }

            // Lấy children của Dept này
            var children = allDepts.Where(r => r.IdParent == dept.IdChild).ToList();
            foreach (var child in children)
            {
                AddDepartmentRecursive(child, deptNode.Id, ref index, unitEHSOrgCustoms, allDepts, unitEHSOrgs, roles, users);
            }
        }

        private void LoadData()
        {
            roles = dt310_RoleBUS.Instance.GetList();
            unitEHSOrgs = dt310_UnitEHSOrgBUS.Instance.GetList();
            //depts = dm_DeptBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true).ToList();
            users = dm_UserBUS.Instance.GetList();

            unitEHSOrgCustoms = new List<UnitEHSOrgCustom>();

            int index = 1;
            List<dm_Departments> startParents = depts.Where(r => r.IdParent == -1).ToList();

            foreach (var parent in startParents)
            {
                AddDepartmentRecursive(parent, 0, ref index, unitEHSOrgCustoms, depts, unitEHSOrgs, roles, users);
            }

            sourceFunc.DataSource = unitEHSOrgCustoms;

            var now = DateTime.Now;
            var result = unitEHSOrgs.Select(u =>
            {
                var roleObj = roles.FirstOrDefault(r => r.Id == Convert.ToInt32(u.Role));
                var userObj = users.FirstOrDefault(r => r.Id == u.EmployeeId);

                return new
                {
                    u.Id,
                    Emp = $"{userObj?.IdDepartment} {userObj?.DisplayName} {userObj?.DisplayNameVN}",
                    Role = roleObj?.DisplayName,
                    u.StartDate,
                    ThamNien = (now.Year - u.StartDate.Year) -
                               (now.Month < u.StartDate.Month ||
                               (now.Month == u.StartDate.Month && now.Day < u.StartDate.Day) ? 1 : 0)
                };
            }).ToList();


            sourceOrg.DataSource = result;
        }

        private void uc310_UnitEHSOrg_Load(object sender, EventArgs e)
        {
            LoadData();

            treeFunctions.DataSource = sourceFunc;
            treeFunctions.KeyFieldName = "Id";
            treeFunctions.ParentFieldName = "IdParent";
            treeFunctions.CheckBoxFieldName = "Status";
            treeFunctions.BestFitColumns();
            treeFunctions.ReadOnlyTreelist();

            TreeListNode node = treeFunctions.GetNodeByVisibleIndex(0);
            node.Expanded = !node.Expanded;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gcData.DataSource = sourceOrg;
            gvData.BestFitColumns();
            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void treeFunctions_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "DeptName") // chỉ áp dụng cho cột DeptName
            {
                // Get the value from the node for the current column
                var nodeValue = e.Node.GetValue(e.Column)?.ToString();
                if (!string.IsNullOrEmpty(nodeValue) && nodeValue.Contains("↪")) // điều kiện căn phải
                {
                    e.Appearance.ForeColor = DXSkinColors.ForeColors.Question;
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
