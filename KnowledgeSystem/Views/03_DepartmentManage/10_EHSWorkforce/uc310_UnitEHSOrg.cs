using BusinessLayer;
using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
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
        List<dm_JobTitle> jobTitles;

        List<dm_GroupUser> userGroups;
        bool isEHSAdmin = false;

        private class UnitEHSOrgCustom
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
                var roleObj = roles.FirstOrDefault(r => r.Id == org.RoleId);
                var userObj = users.FirstOrDefault(r => r.Id == org.EmployeeId);

                unitEHSOrgCustoms.Add(new UnitEHSOrgCustom
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
                AddDepartmentRecursive(child, deptNode.Id, ref index, unitEHSOrgCustoms, allDepts, unitEHSOrgs, roles, users);
            }
        }

        private void LoadData()
        {
            roles = dt310_RoleBUS.Instance.GetList();
            unitEHSOrgs = dt310_UnitEHSOrgBUS.Instance.GetList();
            //depts = dm_DeptBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetAllChildren(0).Where(r => r.IsGroup != true && !TPConfigs.ExclusionDept310.Split(';').Contains(r.Id)).ToList();
            users = dm_UserBUS.Instance.GetList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();

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
                var roleObj = roles.FirstOrDefault(r => r.Id == u.RoleId);
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
            treeFunctions.BestFitColumns();
            gvData.BestFitColumns();
        }

        private void uc310_UnitEHSOrg_Load(object sender, EventArgs e)
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            int groupId = dm_GroupBUS.Instance.GetItemByName($"安衛環7")?.Id ?? -1;
            isEHSAdmin = userGroups.Any(r => r.IdGroup == groupId);

            if (!isEHSAdmin)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            LoadData();

            treeFunctions.DataSource = sourceFunc;
            treeFunctions.KeyFieldName = "Id";
            treeFunctions.ParentFieldName = "IdParent";
            treeFunctions.CheckBoxFieldName = "Status";
            treeFunctions.BestFitColumns();
            treeFunctions.ReadOnlyTreelist();

            TreeListNode node = treeFunctions.GetNodeByVisibleIndex(0);
            if (node != null)
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
            if (!isEHSAdmin)
            {
                return;
            }

            f310_UnitEHSOrg_Info finfo = new f310_UnitEHSOrg_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "組織",
            };

            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
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
                var row = treeList.GetDataRecordByNode(node) as UnitEHSOrgCustom;
                if (row == null || !row.IdData.HasValue)
                    return;

                using (var finfo = new f310_UnitEHSOrg_Info
                {
                    eventInfo = EventFormInfo.View,
                    formName = "組織",
                    idBase = row.IdData.Value
                })
                {
                    finfo.ShowDialog(this);
                }
            }

            LoadData();
        }

        private void btnSummaryTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(treeFunctions))
            {
                // ── 1. Chuẩn bị dữ liệu ──────────────────────────────────────────────
                // Hàm lấy tổ tiên của dept (từ root đến dept cha trực tiếp)
                List<dm_Departments> GetAncestors(dm_Departments dept)
                {
                    var ancestors = new List<dm_Departments>();
                    var current = dept;
                    while (current != null && current.IdParent != -1 && current.IdParent != null)
                    {
                        var parent = depts.FirstOrDefault(r => r.IdChild == current.IdParent);
                        if (parent == null) break;
                        ancestors.Insert(0, parent);
                        current = parent;
                    }
                    return ancestors;
                }

                // Lấy tên dept theo cấp dựa vào số ký tự của DeptId:
                //   1 ký tự → tên dept vào 事業部, 廠處 & 課組 trống
                //   2 ký tự → tên dept vào 廠處, 事業部 = cha, 課組 trống
                //   4 ký tự → tên dept vào 課組, 廠處 = cha, 事業部 = ông
                var exportRows = unitEHSOrgs
                    .Select(org =>
                    {
                        var roleObj = roles.FirstOrDefault(r => r.Id == org.RoleId);
                        var userObj = users.FirstOrDefault(r => r.Id == org.EmployeeId);
                        var deptObj = depts.FirstOrDefault(r => r.Id == org.DeptId);

                        var ancestors = deptObj != null ? GetAncestors(deptObj) : new List<dm_Departments>();

                        int deptLen = org.DeptId?.Length ?? 0;
                        string colDiv   = ""; // 事業部
                        string colPlant = ""; // 廠處
                        string colSect  = ""; // 課組

                        if (deptLen <= 1)
                        {
                            // 1 ký tự: chỉ điền vào 事業部
                            colDiv = deptObj?.DisplayName ?? "";
                        }
                        else if (deptLen == 2)
                        {
                            // 2 ký tự: cha → 事業部, self → 廠處
                            colDiv   = ancestors.Count >= 1 ? ancestors[0].DisplayName : "";
                            colPlant = deptObj?.DisplayName ?? "";
                        }
                        else
                        {
                            // 4 ký tự: ông → 事業部, cha → 廠處, self → 課組
                            colDiv   = ancestors.Count >= 1 ? ancestors[0].DisplayName : "";
                            colPlant = ancestors.Count >= 2 ? ancestors[1].DisplayName : "";
                            colSect  = deptObj?.DisplayName ?? "";
                        }

                        // 備註: chức vụ thực tế của nhân viên từ dmJob
                        var jobTitle = jobTitles.FirstOrDefault(j => j.Id == userObj?.JobCode);

                        return new
                        {
                            事業部 = colDiv,
                            廠處   = colPlant,
                            課組   = colSect,
                            部門代號 = org.DeptId,
                            人員代號 = userObj?.Id ?? "",
                            人員名稱 = userObj?.DisplayName ?? "",
                            職務名稱 = roleObj?.DisplayName ?? "",
                            備註   = jobTitle?.DisplayName ?? ""
                        };
                    })
                    .OrderBy(r => r.部門代號)
                    .ToList();

                // ── 2. Lưu file ─────────────────────────────────────────────────────
                string documentsPath = TPConfigs.DocumentPath();
                if (!Directory.Exists(documentsPath))
                    Directory.CreateDirectory(documentsPath);

                string filePath = Path.Combine(documentsPath, $"安衛環人力表-{DateTime.Now:yyyyMMddHHmmss}.xlsx");

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var pck = new ExcelPackage(new FileInfo(filePath)))
                {
                    var ws = pck.Workbook.Worksheets.Add("安衛環人力表");

                    string fontName = "Microsoft JhengHei";
                    int totalCols   = 9; // A~I

                    // ── Row 1: Tiêu đề (merge A1:I1) ─────────────────────────────
                    ws.Cells[1, 1, 1, totalCols].Merge = true;
                    ws.Cells[1, 1].Value = $"河靜廠區安衛環及專職安督員人力表  {DateTime.Now:yyyy/M/d}";
                    ws.Cells[1, 1].Style.Font.Name = fontName;
                    ws.Cells[1, 1].Style.Font.Size = 18;
                    ws.Cells[1, 1].Style.Font.Bold = true;
                    ws.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1].Style.VerticalAlignment   = ExcelVerticalAlignment.Center;
                    ws.Row(1).Height = 30;

                    // ── Row 2: Header ────────────────────────────────────────────
                    string[] headers = { "項目", "事業部", "廠處", "課組", "部門代號", "人員代號", "人員名稱", "職務名稱", "備註" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = ws.Cells[2, c + 1];
                        cell.Value = headers[c];
                        cell.Style.Font.Name  = fontName;
                        cell.Style.Font.Size  = 12;
                        cell.Style.Font.Bold  = true;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment   = ExcelVerticalAlignment.Center;
                        cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        cell.Style.Border.Top.Style    = ExcelBorderStyle.Thin;
                        cell.Style.Border.Left.Style   = ExcelBorderStyle.Thin;
                        cell.Style.Border.Right.Style  = ExcelBorderStyle.Thin;
                    }
                    ws.Row(2).Height = 24;

                    // ── Row 3+: Dữ liệu ─────────────────────────────────────────
                    for (int i = 0; i < exportRows.Count; i++)
                    {
                        int row  = i + 3;
                        var data = exportRows[i];

                        ws.Cells[row, 1].Value = i + 1;
                        ws.Cells[row, 2].Value = data.事業部;
                        ws.Cells[row, 3].Value = data.廠處;
                        ws.Cells[row, 4].Value = data.課組;
                        ws.Cells[row, 5].Value = data.部門代號;
                        ws.Cells[row, 6].Value = data.人員代號;
                        ws.Cells[row, 7].Value = data.人員名稱;
                        ws.Cells[row, 8].Value = data.備註;
                        ws.Cells[row, 9].Value = data.職務名稱;

                        // Style toàn bộ row dữ liệu
                        var rowRange = ws.Cells[row, 1, row, totalCols];
                        rowRange.Style.Font.Name = fontName;
                        rowRange.Style.Font.Size = 12;
                        rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Border nhẹ
                        for (int c = 1; c <= totalCols; c++)
                        {
                            var bc = ws.Cells[row, c].Style.Border;
                            bc.Bottom.Style = ExcelBorderStyle.Thin;
                            bc.Top.Style    = ExcelBorderStyle.Thin;
                            bc.Left.Style   = ExcelBorderStyle.Thin;
                            bc.Right.Style  = ExcelBorderStyle.Thin;
                        }

                        // Căn giữa cột số thứ tự và mã
                        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    }

                    // ── Độ rộng cột ──────────────────────────────────────────────
                    ws.Column(1).Width  = 5;   // 項目
                    ws.Column(2).Width  = 10;  // 事業部
                    ws.Column(3).Width  = 10;  // 廠處
                    ws.Column(4).Width  = 18;  // 課組
                    ws.Column(5).Width  = 9;   // 部門代號
                    ws.Column(6).Width  = 13;  // 人員代號
                    ws.Column(7).Width  = 10;  // 人員名稱
                    ws.Column(8).Width  = 16;  // 職務名稱
                    ws.Column(9).Width  = 14;  // 備註

                    pck.Save();
                }

                Process.Start(filePath);
            }
        }
    }
}
