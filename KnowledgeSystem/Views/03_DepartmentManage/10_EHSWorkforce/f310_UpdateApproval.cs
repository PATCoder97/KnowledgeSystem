using BusinessLayer;
using DataAccessLayer;
using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce.f310_UpdateLeaveUser_Info;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UpdateApproval : DevExpress.XtraEditors.XtraForm
    {
        public f310_UpdateApproval()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idDataUpdate = -1;
        dt310_UpdateLeaveUser updateLeaveUser = new dt310_UpdateLeaveUser();

        private void InitializeIcon()
        {
            btnApproval.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Close;
            btnClose.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private object GetSignatureInfo(dt310_UpdateLeaveUser_detail detail, List<dm_User> users)
        {
            if (detail == null || string.IsNullOrWhiteSpace(detail.IdUser))
                return null;

            var user = users.FirstOrDefault(x => x.Id == detail.IdUser);
            if (user == null)
                return null;

            bool isReject = !string.IsNullOrWhiteSpace(detail.Description);
            string status = isReject ? "已退回" : "已簽名";

            return new
            {
                name = user.DisplayName,
                time = detail.TimeSubmit?.ToString("yyyy/MM/dd HH:mm") ?? "",
                isreject = isReject ? "true" : "false",
                status = status,
                statusColor = isReject ? "red" : "green",
                reason = detail.Description ?? ""
            };
        }

        private object GetCreateSignatureInfo(dt310_UpdateLeaveUser record, List<dm_User> users)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.CreateBy))
                return null;

            var user = users.FirstOrDefault(x => x.Id == record.CreateBy);
            if (user == null)
                return null;

            return new
            {
                name = user.DisplayName,
                time = record.CreateAt.ToString("yyyy/MM/dd HH:mm"),
                isreject = "false",
                status = "已送出",
                statusColor = "green",
                reason = ""
            };
        }

        private object GetEmptySignatureInfo()
        {
            return new
            {
                name = "",
                time = "",
                isreject = "false",
                status = "",
                statusColor = "green",
                reason = ""
            };
        }

        private static string NormalizeChangeAction(string actionType)
        {
            if (string.Equals(actionType, "Create", StringComparison.OrdinalIgnoreCase))
            {
                return "Create";
            }

            if (string.Equals(actionType, "Delete", StringComparison.OrdinalIgnoreCase))
            {
                return "Delete";
            }

            return "Update";
        }

        private static string GetChangeActionText(string actionType)
        {
            switch (NormalizeChangeAction(actionType))
            {
                case "Create":
                    return "新增";
                case "Delete":
                    return "刪除";
                default:
                    return "修改";
            }
        }

        private static string FormatUserDisplay(string userId, List<dm_User> users)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "";
            }

            dm_User user = users.FirstOrDefault(u => u.Id == userId);
            return user == null ? userId : $"{userId} {user.DisplayName}";
        }

        private static string BuildDeptRoleText(string deptId, int? roleId, List<dt310_Role> roles)
        {
            string roleName = roles.FirstOrDefault(r => r.Id == roleId)?.DisplayName ?? "";
            return $"{deptId}：{roleName}".Trim('：');
        }

        private static string BuildDeptFunctionText(string deptId, int? functionId, List<dt310_Function> funcs)
        {
            string funcName = funcs.FirstOrDefault(r => r.Id == functionId)?.DisplayName ?? "";
            return $"{deptId}：{funcName}".Trim('：');
        }

        private static string BuildAreaContextText(string deptId, int? areaId, List<dt310_Area5S> areas)
        {
            string areaName = areas.FirstOrDefault(r => r.Id == areaId)?.DisplayName ?? "";
            return $"{deptId}：{areaName}".Trim('：');
        }

        private static string BuildAreaPeopleSummary(string employeeId, string agentId, string bossId, List<dm_User> users)
        {
            List<string> parts = new List<string>();

            string employee = FormatUserDisplay(employeeId, users);
            string agent = FormatUserDisplay(agentId, users);
            string boss = FormatUserDisplay(bossId, users);

            if (!string.IsNullOrWhiteSpace(employee))
            {
                parts.Add($"責任:{employee}");
            }

            if (!string.IsNullOrWhiteSpace(agent))
            {
                parts.Add($"代理:{agent}");
            }

            if (!string.IsNullOrWhiteSpace(boss))
            {
                parts.Add($"主管:{boss}");
            }

            return string.Join(" / ", parts);
        }

        private static string BuildUnitDisplayText(UpdateLeaveUserData item, List<dt310_Role> roles)
        {
            string action = NormalizeChangeAction(item?.ActionType);
            string oldText = item?.UnitEHSOrgData != null
                ? BuildDeptRoleText(item.UnitEHSOrgData.DeptId, item.UnitEHSOrgData.RoleId, roles)
                : "";
            string newText = BuildDeptRoleText(
                item?.DeptId ?? item?.UnitEHSOrgData?.DeptId ?? "",
                item?.RoleId ?? item?.UnitEHSOrgData?.RoleId,
                roles);

            switch (action)
            {
                case "Create":
                    return $"[{GetChangeActionText(action)}] {newText}";
                case "Delete":
                    return $"[{GetChangeActionText(action)}] {oldText}";
                default:
                    return oldText != newText
                        ? $"[{GetChangeActionText(action)}] {oldText} -> {newText}"
                        : $"[{GetChangeActionText(action)}] {newText}";
            }
        }

        private static string BuildFuncDisplayText(UpdateLeaveUserData item, List<dt310_Function> funcs)
        {
            string action = NormalizeChangeAction(item?.ActionType);
            string oldText = item?.EHSFunctionData != null
                ? BuildDeptFunctionText(item.EHSFunctionData.DeptId, item.EHSFunctionData.FunctionId, funcs)
                : "";
            string newText = BuildDeptFunctionText(
                item?.DeptId ?? item?.EHSFunctionData?.DeptId ?? "",
                item?.FunctionId ?? item?.EHSFunctionData?.FunctionId,
                funcs);

            switch (action)
            {
                case "Create":
                    return $"[{GetChangeActionText(action)}] {newText}";
                case "Delete":
                    return $"[{GetChangeActionText(action)}] {oldText}";
                default:
                    return oldText != newText
                        ? $"[{GetChangeActionText(action)}] {oldText} -> {newText}"
                        : $"[{GetChangeActionText(action)}] {newText}";
            }
        }

        private static List<object> BuildAreaTemplateRows(List<UpdateLeaveUserData> areaList, List<dm_User> users, List<dt310_Area5S> areas)
        {
            return areaList.Select(item =>
            {
                string action = NormalizeChangeAction(item?.ActionType);
                bool isLegacySingleFieldChange =
                    item?.Area5SResponsibleData != null
                    && !string.IsNullOrWhiteSpace(item.FieldName)
                    && !string.IsNullOrWhiteSpace(item.UserId)
                    && string.IsNullOrWhiteSpace(item.EmployeeId)
                    && string.IsNullOrWhiteSpace(item.AgentId)
                    && string.IsNullOrWhiteSpace(item.BossId);

                if (isLegacySingleFieldChange)
                {
                    string oldUserId = item.FieldName == "EmployeeId"
                        ? item.Area5SResponsibleData.EmployeeId
                        : item.Area5SResponsibleData.AgentId;

                    return (object)new
                    {
                        oldemp = FormatUserDisplay(oldUserId, users),
                        reciver = FormatUserDisplay(item.UserId, users),
                        colname = item.ColName,
                        area = item.Desc,
                        areacode = item.Area5SResponsibleData?.AreaCode ?? "",
                        areaname = item.Area5SResponsibleData?.AreaName ?? "",
                    };
                }

                string oldContext = item?.Area5SResponsibleData != null
                    ? BuildAreaContextText(item.Area5SResponsibleData.DeptId, item.Area5SResponsibleData.AreaId, areas)
                    : "";
                string newContext = BuildAreaContextText(
                    item?.DeptId ?? item?.Area5SResponsibleData?.DeptId ?? "",
                    item?.AreaId ?? item?.Area5SResponsibleData?.AreaId,
                    areas);

                string areaText = action == "Create"
                    ? $"[{GetChangeActionText(action)}] {newContext}"
                    : action == "Delete"
                        ? $"[{GetChangeActionText(action)}] {oldContext}"
                        : oldContext != newContext
                            ? $"[{GetChangeActionText(action)}] {oldContext} -> {newContext}"
                            : $"[{GetChangeActionText(action)}] {newContext}";

                return (object)new
                {
                    oldemp = action == "Create"
                        ? ""
                        : BuildAreaPeopleSummary(
                            item?.Area5SResponsibleData?.EmployeeId,
                            item?.Area5SResponsibleData?.AgentId,
                            item?.Area5SResponsibleData?.BossId,
                            users),
                    reciver = action == "Delete"
                        ? ""
                        : BuildAreaPeopleSummary(
                            item?.EmployeeId ?? item?.Area5SResponsibleData?.EmployeeId,
                            item?.AgentId ?? item?.Area5SResponsibleData?.AgentId,
                            item?.BossId ?? item?.Area5SResponsibleData?.BossId,
                            users),
                    colname = GetChangeActionText(action),
                    area = areaText,
                    areacode = item?.AreaCode ?? item?.Area5SResponsibleData?.AreaCode ?? "",
                    areaname = item?.AreaName ?? item?.Area5SResponsibleData?.AreaName ?? "",
                };
            }).ToList();
        }

        private void f310_UpdateApproval_Load(object sender, EventArgs e)
        {
            var users = dm_UserBUS.Instance.GetList();
            var roles = dt310_RoleBUS.Instance.GetList();
            var funcs = dt310_FunctionBUS.Instance.GetList();
            var areas = dt310_Area5SBUS.Instance.GetList();
            updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            var dataDetail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdData(idDataUpdate);
            bool isEHSAssign = updateLeaveUser.DataType == "EHSAssign";

            var userLeaveInfo = users.FirstOrDefault(r => r.Id == updateLeaveUser?.IdUserLeave);
            if (isEHSAssign)
            {
                Text = "審核安衛環人員異動申請";
            }
            else if (userLeaveInfo != null)
            {
                Text = $"審核離職人員權限轉移 - {updateLeaveUser.IdUserLeave} {userLeaveInfo.DisplayName}";
            }

            // Nếu đã hoàn thành hoặc đã trả về, chỉ hiển thị nút Confirm
            if (updateLeaveUser.IdGroupProcess == -1 || updateLeaveUser.IsCancel)
            {
                btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            else
            {
                var currentUserGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
                bool hasPermission = currentUserGroups.Any(g => g.IdGroup == updateLeaveUser.IdGroupProcess);

                if (!hasPermission)
                {
                    btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                else
                {
                    btnClose.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }

            string json = updateLeaveUser?.DataJson ?? "{}";
            var data = JsonConvert.DeserializeObject<UpdateLeaveUserJson>(json);

            // Sử dụng data
            var unitList = data?.UnitEHSOrg ?? new List<UpdateLeaveUserData>();
            var funcList = data?.EHSFunction ?? new List<UpdateLeaveUserData>();
            var areaList = data?.Area5SResponsible ?? new List<UpdateLeaveUserData>();

            userLeaveInfo = users.FirstOrDefault(r => r.Id == updateLeaveUser?.IdUserLeave);
            var userLeaveName = userLeaveInfo != null ? $"{updateLeaveUser.IdUserLeave} {userLeaveInfo.DisplayName}" : "";
            var signApplicant = isEHSAssign
                ? GetCreateSignatureInfo(updateLeaveUser, users)
                : GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == 0), users);
            var signLevel2 = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 0 : 1)), users);
            var signPsm = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 1 : 2)), users);
            var signLevel1 = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 2 : 3)), users);

            var templateData = new
            {
                userleave = userLeaveName,
                createby = $"{updateLeaveUser.CreateBy} {users.FirstOrDefault(u => u.Id == updateLeaveUser.CreateBy)?.DisplayName ?? ""}",
                unitlist = unitList.Select(r => new
                {
                    oldemp = NormalizeChangeAction(r.ActionType) == "Create"
                        ? ""
                        : FormatUserDisplay(r.UnitEHSOrgData?.EmployeeId, users),
                    reciver = NormalizeChangeAction(r.ActionType) == "Delete"
                        ? ""
                        : FormatUserDisplay(r.UserId, users),
                    displaydata = BuildUnitDisplayText(r, roles)
                }).ToList(),
                funclist = funcList.Select(r => new
                {
                    oldemp = NormalizeChangeAction(r.ActionType) == "Create"
                        ? ""
                        : FormatUserDisplay(r.EHSFunctionData?.EmployeeId, users),
                    reciver = NormalizeChangeAction(r.ActionType) == "Delete"
                        ? ""
                        : FormatUserDisplay(r.UserId, users),
                    displaydata = BuildFuncDisplayText(r, funcs)
                }).ToList(),
                arealist = BuildAreaTemplateRows(areaList, users, areas),
                sign1st = signApplicant ?? GetEmptySignatureInfo(),
                sign2nd = signLevel2 ?? GetEmptySignatureInfo(),
                sign3th = signPsm ?? GetEmptySignatureInfo(),
                sign4th = signLevel1 ?? GetEmptySignatureInfo(),
            };

            string templateFile = isEHSAssign ? "310_updateehsassign.html" : "310_updateleaveruser.html";
            var templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, templateFile));

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);

            webViewUpdateData.DocumentText = pageContent;
        }

        private void ApplyUnitChange(UpdateLeaveUserData item)
        {
            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId) || !item.RoleId.HasValue || string.IsNullOrWhiteSpace(item.UserId))
                    {
                        return;
                    }

                    dt310_UnitEHSOrgBUS.Instance.Add(new dt310_UnitEHSOrg
                    {
                        DeptId = item.DeptId,
                        RoleId = item.RoleId.Value,
                        EmployeeId = item.UserId,
                        StartDate = item.StartDate ?? DateTime.Now.Date,
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.UnitEHSOrgData != null)
                    {
                        dt310_UnitEHSOrgBUS.Instance.RemoveById(item.UnitEHSOrgData.Id, TPConfigs.LoginUser.Id);
                    }
                    break;
                default:
                    if (item?.UnitEHSOrgData == null)
                    {
                        return;
                    }

                    var unitEHSOrg = dt310_UnitEHSOrgBUS.Instance.GetItemById(item.UnitEHSOrgData.Id);
                    if (unitEHSOrg == null)
                    {
                        return;
                    }

                    unitEHSOrg.DeptId = item.DeptId ?? unitEHSOrg.DeptId;
                    unitEHSOrg.RoleId = item.RoleId ?? unitEHSOrg.RoleId;
                    unitEHSOrg.EmployeeId = item.UserId ?? unitEHSOrg.EmployeeId;
                    unitEHSOrg.StartDate = item.StartDate ?? unitEHSOrg.StartDate;
                    dt310_UnitEHSOrgBUS.Instance.AddOrUpdate(unitEHSOrg);
                    break;
            }
        }

        private void ApplyFuncChange(UpdateLeaveUserData item)
        {
            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId) || !item.FunctionId.HasValue || string.IsNullOrWhiteSpace(item.UserId))
                    {
                        return;
                    }

                    dt310_EHSFunctionBUS.Instance.Add(new dt310_EHSFunction
                    {
                        DeptId = item.DeptId,
                        FunctionId = item.FunctionId.Value,
                        EmployeeId = item.UserId,
                        StartDate = item.StartDate ?? DateTime.Now.Date,
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.EHSFunctionData != null)
                    {
                        dt310_EHSFunctionBUS.Instance.RemoveById(item.EHSFunctionData.Id, TPConfigs.LoginUser.Id);
                    }
                    break;
                default:
                    if (item?.EHSFunctionData == null)
                    {
                        return;
                    }

                    var ehsFunction = dt310_EHSFunctionBUS.Instance.GetItemById(item.EHSFunctionData.Id);
                    if (ehsFunction == null)
                    {
                        return;
                    }

                    ehsFunction.DeptId = item.DeptId ?? ehsFunction.DeptId;
                    ehsFunction.FunctionId = item.FunctionId ?? ehsFunction.FunctionId;
                    ehsFunction.EmployeeId = item.UserId ?? ehsFunction.EmployeeId;
                    ehsFunction.StartDate = item.StartDate ?? ehsFunction.StartDate;
                    dt310_EHSFunctionBUS.Instance.AddOrUpdate(ehsFunction);
                    break;
            }
        }

        private void ApplyAreaChange(UpdateLeaveUserData item)
        {
            if (item?.Area5SResponsibleData != null && !string.IsNullOrWhiteSpace(item.FieldName) && !string.IsNullOrWhiteSpace(item.UserId))
            {
                var legacyArea = dt310_Area5SResponsibleBUS.Instance.GetItemById(item.Area5SResponsibleData.Id);
                if (legacyArea == null)
                {
                    return;
                }

                if (item.FieldName == "EmployeeId")
                {
                    legacyArea.EmployeeId = item.UserId;
                }
                else if (item.FieldName == "AgentId")
                {
                    legacyArea.AgentId = item.UserId;
                }

                dt310_Area5SResponsibleBUS.Instance.AddOrUpdate(legacyArea);
                return;
            }

            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId)
                        || !item.AreaId.HasValue
                        || string.IsNullOrWhiteSpace(item.EmployeeId)
                        || string.IsNullOrWhiteSpace(item.AgentId)
                        || string.IsNullOrWhiteSpace(item.BossId))
                    {
                        return;
                    }

                    dt310_Area5SResponsibleBUS.Instance.Add(new dt310_Area5SResponsible
                    {
                        DeptId = item.DeptId,
                        AreaId = item.AreaId.Value,
                        EmployeeId = item.EmployeeId,
                        AgentId = item.AgentId,
                        BossId = item.BossId,
                        AreaCode = item.AreaCode ?? "",
                        AreaName = item.AreaName ?? "",
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.Area5SResponsibleData != null)
                    {
                        dt310_Area5SResponsibleBUS.Instance.RemoveById(item.Area5SResponsibleData.Id);
                    }
                    break;
                default:
                    if (item?.Area5SResponsibleData == null)
                    {
                        return;
                    }

                    var area5S = dt310_Area5SResponsibleBUS.Instance.GetItemById(item.Area5SResponsibleData.Id);
                    if (area5S == null)
                    {
                        return;
                    }

                    area5S.DeptId = item.DeptId ?? area5S.DeptId;
                    area5S.AreaId = item.AreaId ?? area5S.AreaId;
                    area5S.EmployeeId = item.EmployeeId ?? area5S.EmployeeId;
                    area5S.AgentId = item.AgentId ?? area5S.AgentId;
                    area5S.BossId = item.BossId ?? area5S.BossId;
                    area5S.AreaCode = item.AreaCode ?? area5S.AreaCode;
                    area5S.AreaName = item.AreaName ?? area5S.AreaName;
                    dt310_Area5SResponsibleBUS.Instance.AddOrUpdate(area5S);
                    break;
            }
        }

        private void btnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var detail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndIdGroup(updateLeaveUser.Id, (int)updateLeaveUser.IdGroupProcess);
            detail.IdUser = TPConfigs.LoginUser.Id;
            detail.TimeSubmit = DateTime.Now;
            dt310_UpdateLeaveUser_detailBUS.Instance.AddOrUpdate(detail);

            var nextStep = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndStep(updateLeaveUser.Id, detail.IndexStep + 1);

            if (nextStep == null)
            {
                string json = updateLeaveUser?.DataJson ?? "{}";
                var data = JsonConvert.DeserializeObject<UpdateLeaveUserJson>(json);

                if (data != null)
                {
                    if (data.UnitEHSOrg != null)
                    {
                        foreach (var item in data.UnitEHSOrg)
                        {
                            ApplyUnitChange(item);
                        }
                    }

                    if (data.EHSFunction != null)
                    {
                        foreach (var item in data.EHSFunction)
                        {
                            ApplyFuncChange(item);
                        }
                    }

                    if (data.Area5SResponsible != null)
                    {
                        foreach (var item in data.Area5SResponsible)
                        {
                            ApplyAreaChange(item);
                        }
                    }
                }

                updateLeaveUser.IsProcess = false;
                updateLeaveUser.IdGroupProcess = -1;
                dt310_UpdateLeaveUserBUS.Instance.AddOrUpdate(updateLeaveUser);
                XtraMessageBox.Show("審核完成！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                updateLeaveUser.IdGroupProcess = nextStep.IdGroup;
                dt310_UpdateLeaveUserBUS.Instance.AddOrUpdate(updateLeaveUser);
                XtraMessageBox.Show("已送交下一級審核！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string reason = result.ToString();
            if (string.IsNullOrWhiteSpace(reason))
            {
                XtraMessageBox.Show("請輸入退回原因！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var detail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndIdGroup(updateLeaveUser.Id, (int)updateLeaveUser.IdGroupProcess);
            detail.IdUser = TPConfigs.LoginUser.Id;
            detail.TimeSubmit = DateTime.Now;
            detail.Description = reason;
            dt310_UpdateLeaveUser_detailBUS.Instance.AddOrUpdate(detail);

            var nextStep = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndStep(updateLeaveUser.Id, 0);

            updateLeaveUser.IdGroupProcess = nextStep.IdGroup;
            updateLeaveUser.IsCancel = true;
            dt310_UpdateLeaveUserBUS.Instance.AddOrUpdate(updateLeaveUser);

            XtraMessageBox.Show("已退回！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
