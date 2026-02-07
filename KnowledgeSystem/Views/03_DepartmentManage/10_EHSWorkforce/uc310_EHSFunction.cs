using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
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
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_EHSFunction : DevExpress.XtraEditors.XtraUserControl
    {
        public uc310_EHSFunction()
        {
            InitializeComponent();
            InitializeIcon();
        }

        BindingSource sourceEHSFunc = new BindingSource();

        List<dm_Departments> depts;
        List<dt310_EHSFunction> EHSFuncs;
        List<dt310_Function> funcs;
        List<dm_User> users;
        List<EHSFuncCustom> EHSFuncsCustom;

        List<dm_GroupUser> userGroups;
        bool isEHSAdmin = false;

        private class EHSFuncCustom
        {
            public int Id { get; set; }
            public int? IdData { get; set; }
            public int IdParent { get; set; }
            public string DeptName { get; set; }
            public string DeptId { get; set; }
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
            List<EHSFuncCustom> EHSFuncCustoms,
            List<dm_Departments> allDepts,
            List<dt310_EHSFunction> EHSFuncs,
            List<dt310_Function> funcs,
            List<dm_User> users)
        {
            // Add Dept
            var deptNode = new EHSFuncCustom
            {
                Id = index,
                IdParent = parentId,
                DeptName = $"{dept.Id} {dept.DisplayName}"
            };
            index++;

            EHSFuncCustoms.Add(deptNode);

            // Add EHS orgs trong Dept này
            var orgs = EHSFuncs.Where(r => r.DeptId == dept.Id).ToList();
            foreach (var org in orgs)
            {
                var roleObj = funcs.FirstOrDefault(r => r.Id == org.FunctionId);
                var userObj = users.FirstOrDefault(r => r.Id == org.EmployeeId);

                EHSFuncCustoms.Add(new EHSFuncCustom
                {
                    Id = index,
                    IdData = org.Id,
                    IdParent = deptNode.Id,
                    Emp = $"{userObj?.IdDepartment} {userObj?.DisplayName} {userObj?.DisplayNameVN}",
                    Role = roleObj?.DisplayName,
                    DeptName = $"↪ {dept.Id}：{roleObj?.DisplayName}",
                    DeptId = dept.Id
                });
                index++;
            }

            // Lấy children của Dept này
            var children = allDepts.Where(r => r.IdParent == dept.IdChild).ToList();
            foreach (var child in children)
            {
                AddDepartmentRecursive(child, deptNode.Id, ref index, EHSFuncCustoms, allDepts, EHSFuncs, funcs, users);
            }
        }

        private void LoadData()
        {
            funcs = dt310_FunctionBUS.Instance.GetList();
            EHSFuncs = dt310_EHSFunctionBUS.Instance.GetList();
            //depts = dm_DeptBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && !TPConfigs.ExclusionDept310.Split(';').Contains(r.Id)).ToList();
            users = dm_UserBUS.Instance.GetList();

            EHSFuncsCustom = new List<EHSFuncCustom>();

            int index = 1;
            List<dm_Departments> startParents = depts.Where(r => r.IdParent == -1).ToList();

            foreach (var parent in startParents)
            {
                AddDepartmentRecursive(parent, 0, ref index, EHSFuncsCustom, depts, EHSFuncs, funcs, users);
            }

            sourceEHSFunc.DataSource = EHSFuncsCustom;
            treeFunctions.BestFitColumns();

            //var now = DateTime.Now;
            //var result = EHSFuncs.Select(u =>
            //{
            //    var roleObj = funcs.FirstOrDefault(r => r.Id == u.FunctionId);
            //    var userObj = users.FirstOrDefault(r => r.Id == u.EmployeeId);

            //    return new
            //    {
            //        u.Id,
            //        Emp = $"{userObj?.IdDepartment} {userObj?.DisplayName} {userObj?.DisplayNameVN}",
            //        Role = roleObj?.DisplayName,
            //        u.StartDate,
            //        ThamNien = (now.Year - u.StartDate.Year) -
            //                   (now.Month < u.StartDate.Month ||
            //                   (now.Month == u.StartDate.Month && now.Day < u.StartDate.Day) ? 1 : 0)
            //    };
            //}).ToList();


            //sourceOrg.DataSource = result;
            //gvData.BestFitColumns();
        }

        private void uc310_EHSFunction_Load(object sender, EventArgs e)
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            int groupId = dm_GroupBUS.Instance.GetItemByName($"安衛環7")?.Id ?? -1;
            isEHSAdmin = userGroups.Any(r => r.IdGroup == groupId);

            LoadData();

            treeFunctions.DataSource = sourceEHSFunc;
            treeFunctions.KeyFieldName = "Id";
            treeFunctions.ParentFieldName = "IdParent";
            treeFunctions.CheckBoxFieldName = "Status";
            treeFunctions.BestFitColumns();
            treeFunctions.ReadOnlyTreelist();

            TreeListNode node = treeFunctions.GetNodeByVisibleIndex(0);
            node.Expanded = !node.Expanded;

            //gvData.ReadOnlyGridView();
            //gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            //gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            //gcData.DataSource = sourceOrg;
            //gvData.BestFitColumns();
            //gvData.OptionsDetail.EnableMasterViewMode = true;
            //gvData.OptionsView.ShowGroupPanel = false;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f310_EHSFunc_Info finfo = new f310_EHSFunc_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "項目",
            };

            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
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

        private void treeFunctions_DoubleClick(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(treeFunctions))
            {

                var treeList = treeFunctions;
                var hit = treeList.CalcHitInfo(treeList.PointToClient(System.Windows.Forms.Control.MousePosition));

                // Must click on a cell
                if (hit.HitInfoType != HitInfoType.Cell)
                    return;

                var node = treeList.FocusedNode;
                if (node == null)
                    return;

                // Get bound data from the node
                var row = treeList.GetDataRecordByNode(node) as EHSFuncCustom;
                if (row == null || !row.IdData.HasValue)
                    return;

                if (!isEHSAdmin)
                {
                    int groupId = dm_GroupBUS.Instance.GetItemByName($"安衛環{row.DeptId}")?.Id ?? -1;
                    if (!userGroups.Any(r => r.IdGroup == groupId))
                        return;
                }

                using (var finfo = new f310_EHSFunc_Info
                {
                    eventInfo = EventFormInfo.View,
                    formName = "項目",
                    idBase = row.IdData.Value
                })
                {
                    finfo.ShowDialog(this);
                }
            }

            LoadData();
        }
    }
}
