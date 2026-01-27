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
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
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

        private void f310_UpdateApproval_Load(object sender, EventArgs e)
        {
            var users = dm_UserBUS.Instance.GetList();
            updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            var dataDetail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdData(idDataUpdate);

            var userLeaveInfo = users.FirstOrDefault(r => r.Id == updateLeaveUser?.IdUserLeave);
            if (userLeaveInfo != null)
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
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
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

            var templateData = new
            {
                userleave = userLeaveName,
                unitlist = unitList.Select(r => new
                {
                    reciver = $"{r.UserId} {users.FirstOrDefault(u => u.Id == r.UserId)?.DisplayName ?? ""}",
                    displaydata = r.Desc
                }),
                funclist = funcList.Select(r => new
                {
                    reciver = $"{r.UserId} {users.FirstOrDefault(u => u.Id == r.UserId)?.DisplayName ?? ""}",
                    displaydata = r.Desc
                }),
                arealist = areaList.Select(r => new
                {
                    reciver = $"{r.UserId} {users.FirstOrDefault(u => u.Id == r.UserId)?.DisplayName ?? ""}",
                    colname = r.ColName,
                    area = r.Desc,
                    areacode = r.Area5SResponsibleData?.AreaCode ?? "",
                    areaname = r.Area5SResponsibleData?.AreaName ?? "",
                }),
                sign1st = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == 0), users),
                sign2nd = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == 1), users),
                sign3th = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == 2), users),
            };

            var templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, "310_updateleaveruser.html"));

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);

            webViewUpdateData.DocumentText = pageContent;
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
                            if (!string.IsNullOrWhiteSpace(item.UserId) && item.UnitEHSOrgData != null)
                            {
                                var unitEHSOrg = dt310_UnitEHSOrgBUS.Instance.GetItemById(item.UnitEHSOrgData.Id);
                                if (unitEHSOrg != null)
                                {
                                    unitEHSOrg.EmployeeId = item.UserId;
                                    dt310_UnitEHSOrgBUS.Instance.AddOrUpdate(unitEHSOrg);
                                }
                            }
                        }
                    }

                    if (data.EHSFunction != null)
                    {
                        foreach (var item in data.EHSFunction)
                        {
                            if (!string.IsNullOrWhiteSpace(item.UserId) && item.EHSFunctionData != null)
                            {
                                var ehsFunction = dt310_EHSFunctionBUS.Instance.GetItemById(item.EHSFunctionData.Id);
                                if (ehsFunction != null)
                                {
                                    ehsFunction.EmployeeId = item.UserId;
                                    dt310_EHSFunctionBUS.Instance.AddOrUpdate(ehsFunction);
                                }
                            }
                        }
                    }

                    if (data.Area5SResponsible != null)
                    {
                        foreach (var item in data.Area5SResponsible)
                        {
                            if (!string.IsNullOrWhiteSpace(item.UserId) && item.Area5SResponsibleData != null)
                            {
                                var area5S = dt310_Area5SResponsibleBUS.Instance.GetItemById(item.Area5SResponsibleData.Id);
                                if (area5S != null)
                                {
                                    if (item.FieldName == "EmployeeId")
                                    {
                                        area5S.EmployeeId = item.UserId;
                                    }
                                    else if (item.FieldName == "AgentId")
                                    {
                                        area5S.AgentId = item.UserId;
                                    }
                                    dt310_Area5SResponsibleBUS.Instance.AddOrUpdate(area5S);
                                }
                            }
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
    }
}