using BusinessLayer;
using DataAccessLayer;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UpdateLeaveUser_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_UpdateLeaveUser_Info()
        {
            InitializeComponent();
        }

        string userId = "VNW0012950";

        BindingSource sourceUnitEHSOrg = new BindingSource();
        BindingSource sourceEHSFunction = new BindingSource();
        BindingSource sourceArea5SResponsible = new BindingSource();

        class TestData
        {
            public string UserId { get; set; }
            public string Desc { get; set; }
            public dt310_UnitEHSOrg UnitEHSOrgData { get; set; }
            public dt310_Role RoleData { get; set; }
            public dt310_EHSFunction EHSFunctionData { get; set; }
            public dt310_Function FuncData { get; set; }
            public dt310_Area5SResponsible Area5SResponsibleData { get; set; }
            public dt310_Area5S AreaData { get; set; }
            public string FieldName { get; set; }
            public string ColName { get; set; }
        }

        private void ConfigureGridEdit(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
            {
                // Kiểm tra FieldName (dùng OrdinalIgnoreCase để không phân biệt hoa thường cho chắc chắn)
                if (col.FieldName.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                {
                    col.OptionsColumn.AllowEdit = true;
                    // Nếu muốn cho phép sửa nhưng không cho focus vào các ô khác thì mở thêm dòng dưới
                    // col.OptionsColumn.AllowFocus = true; 
                }
                else
                {
                    col.OptionsColumn.AllowEdit = false;
                    // col.OptionsColumn.AllowFocus = false; // Tùy chọn: chặn click chuột vào ô
                }
            }
        }

        private void f310_UpdateLeaveUser_Info_Load(object sender, EventArgs e)
        {
            ConfigureGridEdit(gvUnitEHSOrg);
            ConfigureGridEdit(gvEHSFunction);
            ConfigureGridEdit(gvArea5SResponsible);

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0).Select(r => new
            {
                DisplayName = $"LG{r.IdDepartment}/{r.DisplayName}",
                Id = r.Id,
                DeptId = r.IdDepartment
            }).ToList();

            itemCbbUser01.DataSource = usrs;
            itemCbbUser01.DisplayMember = "DisplayName";
            itemCbbUser01.ValueMember = "Id";

            itemCbbUser02.DataSource = usrs;
            itemCbbUser02.DisplayMember = "DisplayName";
            itemCbbUser02.ValueMember = "Id";

            itemCbbUser03.DataSource = usrs;
            itemCbbUser03.DisplayMember = "DisplayName";
            itemCbbUser03.ValueMember = "Id";

            itemCbbUser01.NullText = itemCbbUser02.NullText = itemCbbUser03.NullText = "【請選擇接任人】";

            var UnitEHSOrgData = dt310_UnitEHSOrgBUS.Instance.GetListByUserId(userId);
            var EHSFunctionData = dt310_EHSFunctionBUS.Instance.GetListByUserId(userId);
            var Area5SResponsibleData = dt310_Area5SResponsibleBUS.Instance.GetListByUserId(userId);

            var roles = dt310_RoleBUS.Instance.GetList();
            var funcs = dt310_FunctionBUS.Instance.GetList();
            var areas = dt310_Area5SBUS.Instance.GetList();

            var UnitEHSOrgUpdate = (from data in UnitEHSOrgData
                                    join role in roles on data.RoleId equals role.Id
                                    select new TestData
                                    {
                                        UnitEHSOrgData = data,
                                        RoleData = role,
                                        Desc = $"{data.DeptId}：{role.DisplayName}"
                                    }).ToList();

            var EHSFunctionUpdate = (from data in EHSFunctionData
                                     join func in funcs on data.FunctionId equals func.Id
                                     select new TestData
                                     {
                                         EHSFunctionData = data,
                                         FuncData = func,
                                         Desc = $"{data.DeptId}：{func.DisplayName}"
                                     }).ToList();

            var Area5SResponsibleUpdate = (from data in Area5SResponsibleData
                                           join area in areas on data.AreaId equals area.Id
                                           let matchEmp = data.EmployeeId == userId
                                           let matchAgent = data.AgentId == userId
                                           where matchEmp || matchAgent
                                           select new TestData
                                           {
                                               Area5SResponsibleData = data,
                                               AreaData = area,
                                               Desc = $"{data.DeptId}：{area.DisplayName}",
                                               FieldName = matchEmp ? "EmployeeId" : "AgentId",
                                               ColName = matchEmp ? "責任人員" : "代理人",
                                           }).ToList();

            sourceUnitEHSOrg.DataSource = UnitEHSOrgUpdate;
            gcUnitEHSOrg.DataSource = sourceUnitEHSOrg;
            gvUnitEHSOrg.ReadOnlyGridView(false);
            gvUnitEHSOrg.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvUnitEHSOrg.BestFitColumns();

            sourceEHSFunction.DataSource = EHSFunctionUpdate;
            gcEHSFunction.DataSource = sourceEHSFunction;
            //gvEHSFunction.ReadOnlyGridView();
            gvEHSFunction.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEHSFunction.BestFitColumns();

            sourceArea5SResponsible.DataSource = Area5SResponsibleUpdate;
            gcArea5SResponsible.DataSource = sourceArea5SResponsible;
            //gvArea5SResponsible.ReadOnlyGridView();
            gvArea5SResponsible.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvArea5SResponsible.BestFitColumns();
        }
    }
}
