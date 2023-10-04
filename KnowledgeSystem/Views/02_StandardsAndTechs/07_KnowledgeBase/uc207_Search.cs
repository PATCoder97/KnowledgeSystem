using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_Search : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc207_Search()
        {
            InitializeComponent();
            helper = new RefreshHelper(gvData, "Id");
        }

        #region parameters

        BindingSource sourceKnowledge = new BindingSource();

        List<string> lsIdCanReads = new List<string>();
        bool IsSysAdmin = false;

        private class DataDisplay
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string UserUpload { get; set; }
            public string UserUploadName { get; set; }
            public string TypeName { get; set; }
            public string Keyword { get; set; }
            public string UserProcessName { get; set; }
            public DateTime? UploadDate { get; set; }
            public string nonUnicodeDisplayName { get; set; }
            public string nonUnicodeKeyword { get; set; }
        }

        #endregion

        #region methods

        private void LoadData()
        {
            string userLogin = TempDatas.LoginId;

            List<dm_User> lsUsers = new List<dm_User>();
            List<dt207_Base> lsKnowledgeBase = new List<dt207_Base>();
            List<dt207_Type> lsKnowledgeTypes = new List<dt207_Type>();

            // Kiểm tra xem có lấy Keyword để hiện lên view không
            bool IsSimple = checkUseKeyword.CheckState == CheckState.Unchecked;

            // Lưu thông tin liên quan đến giao diện
            helper.SaveViewInfo();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Lấy danh sách Users, lsKnowledgeTypes, lsTypeHisGetFile từ cơ sở dữ liệu
                var lsTypeHisGetFile = db.dt207_TypeHisGetFile.ToList();
                lsKnowledgeTypes = db.dt207_Type.ToList();
                lsUsers = db.dm_User.ToList();

                // Gán các 說明 quyền vào class temp
                TempDatas.typeViewFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 1);
                TempDatas.typeSaveFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 2);
                TempDatas.typePrintFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 3);

                // Lấy danh sách các giá trị IdKnowledgeBase mà IsComplete là false
                var lsIdBaseRemove = db.dt207_DocProgress.Where(r => !(r.IsComplete ?? false)).Select(r => r.IdKnowledgeBase).ToList();

                // Kiểm tra xem phải sysAdmin không
                IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
                if (IsSysAdmin)
                {
                    lsKnowledgeBase = db.dt207_Base.Where(r => !lsIdBaseRemove.Contains(r.Id) && !(r.IsDelete ?? false)).ToList();
                    goto GET_DISPLAY_LIST;
                }

                // Danh sách Doc đưa lên hoặc yêu cầu
                var lsDocUserUpload = db.dt207_Base.Where(r => r.UserUpload == userLogin || r.UserProcess == userLogin).ToList();
                var lsIdDocUserUpload = lsDocUserUpload.Select(r => r.Id).ToList();

                // Join group với GroupUser để lấy Prioritize
                var lsGroupP = (from data in db.dm_GroupUser.ToList()
                                join p in db.dm_Group.ToList() on data.IdGroup equals p.Id
                                select new
                                {
                                    data.IdGroup,
                                    data.IdUser,
                                    p.Prioritize
                                }).ToList();

                // Danh sách mà user có quyền tìm kiếm
                var lsCanSearchs = (from ks in db.dt207_Security.ToList()
                                    join gu in lsGroupP on ks.IdGroup equals gu.IdGroup into gj
                                    from subGu in gj.DefaultIfEmpty()
                                    where ks.IdUser == userLogin || (subGu != null ? subGu.IdUser == userLogin : false)
                                    select new
                                    {
                                        ks.IdKnowledgeBase,
                                        ks.IdGroup,
                                        ks.IdUser,
                                        UserG = subGu?.IdUser,
                                        ks.SearchInfo,
                                        ks.ReadInfo,
                                        Prioritize = subGu?.Prioritize ?? -1
                                    } into dtData
                                    group dtData by dtData.IdKnowledgeBase into dtg
                                    select new
                                    {
                                        dtg.Key,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.Prioritize,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.SearchInfo,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.ReadInfo,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.IdUser,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.UserG,
                                    }).ToList();

                var lsIsCanSearchs = lsCanSearchs.Where(r => r.SearchInfo == true).Select(r => r.Key).ToList();
                lsIdCanReads = lsCanSearchs.Where(r => r.SearchInfo == true && r.ReadInfo == true).Select(r => r.Key).ToList();

                // Gom tất cả các Id có quyền tìm kiếm, đưa lên và yêu cầu sau đó xoá đi các Id đang trong quá trình ký
                List<string> lsIdDisplay = new List<string>();

                lsIdDisplay.AddRange(lsIsCanSearchs);
                lsIdDisplay.AddRange(lsIdDocUserUpload);
                lsIdDisplay.RemoveAll(r => lsIdBaseRemove.Contains(r));
                lsIdDisplay = lsIdDisplay.Distinct().ToList();

                lsIdCanReads.AddRange(lsIdDocUserUpload);
                lsIdCanReads.RemoveAll(r => lsIdBaseRemove.Contains(r));

                lsKnowledgeBase = db.dt207_Base.Where(r => lsIdDisplay.Contains(r.Id) && !(r.IsDelete ?? false)).ToList();
            }

        GET_DISPLAY_LIST:
            // Tạo danh sách các đối tượng DataDisplay bằng cách kết hợp lsKnowledgeBase, lsUsers, và lsKnowledgeTypes
            // Chọn các thuộc tính cụ thể để điền vào các đối tượng DataDisplay
            var lsDataDisplays = (from data in lsKnowledgeBase
                                  join userUpload_ in lsUsers on data.UserUpload equals userUpload_.Id
                                  join userProcess_ in lsUsers on data.UserProcess equals userProcess_.Id
                                  join type_ in lsKnowledgeTypes on data.IdTypes equals type_.Id
                                  select new DataDisplay
                                  {
                                      Id = data.Id,
                                      DisplayName = data.DisplayName,
                                      UserUpload = data.UserUpload,
                                      UserUploadName = userUpload_.DisplayName,
                                      TypeName = type_.DisplayName,
                                      Keyword = IsSimple ? null : data.Keyword,
                                      UserProcessName = userProcess_.DisplayName,
                                      UploadDate = data.UploadDate,
                                      nonUnicodeDisplayName = convertToUnSign3(data.DisplayName),
                                      nonUnicodeKeyword = IsSimple ? null : convertToUnSign3(data.Keyword),
                                  })
                                  .ToList();

            // Đặt nguồn dữ liệu của sourceKnowledge là danh sách các đối tượng DataDisplay
            sourceKnowledge.DataSource = lsDataDisplays;

            // Đặt tính hiển thị của cột gvColKeyword dựa trên giá trị của biến IsSimple
            gvColKeyword.Visible = !IsSimple;

            // Điều chỉnh độ rộng các cột của gvData
            gvData.BestFitColumns();

            // Tải thông tin liên quan đến giao diện
            helper.LoadViewInfo();

            GetNotifyApproval();
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        private void GetNotifyApproval()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Xử lý lấy tất cả các văn kiện cần trình ký
                var lsDocProgresses = db.dt207_DocProgress.Where(r => !(r.IsComplete ?? false)).ToList();
                var lsDocProgressInfos = db.dt207_DocProgressInfo.ToList();
                var lsKnowledgeBases = db.dt207_Base.ToList();
                var lsUsers = db.dm_User.ToList();

                var lsDocNotSuccess =
                    (from data in db.dt207_DocProgressInfo
                     group data by data.IdDocProgress into g
                     select new
                     {
                         IdDocProgress = g.Key,
                         IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                         TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                         IdUserProcess = g.OrderBy(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault()
                     }).ToList();

                var lsDataApproval =
                    (from data in lsDocProgresses
                     join infos in lsDocNotSuccess on data.Id equals infos.IdDocProgress
                     join bases in lsKnowledgeBases on data.IdKnowledgeBase equals bases.Id
                     join users in lsUsers on infos.IdUserProcess equals users.Id
                     select new
                     {
                         data.IdKnowledgeBase,
                         data.IdProgress,
                         data.Descriptions,
                         infos.TimeStep,
                         infos.IndexStep,
                         bases.DisplayName,
                         UserProcess = $"{users.IdDepartment} | {infos.IdUserProcess}/{users.DisplayName}",
                         ApprovalStep = $"{data.IdProgress}-{infos.IndexStep + 1}",
                     }).ToList();

                // Xử lý phân quyền nhưng user nằm trong group sẽ nhìn thấy
                var lsGroupIn = (from data in db.dm_GroupUser.Where(r => r.IdUser == TempDatas.LoginId).ToList()
                                 join progresses in db.dm_StepProgress.ToList() on data.IdGroup equals progresses.IdGroup
                                 select new
                                 {
                                     progresses.IdProgress,
                                     progresses.IdGroup,
                                     progresses.IndexStep,
                                     ApprovalStep = $"{progresses.IdProgress}-{progresses.IndexStep}",
                                 }).ToList();

                var lsDisplays = (from data in lsDataApproval
                                  join progresses in lsGroupIn on data.ApprovalStep equals progresses.ApprovalStep
                                  select data).ToList();

                int sumNotify = lsDisplays.Count;
                if (sumNotify > 0)
                {
                    emptySpaceRight.MinSize = new Size(145, 10);
                    emptySpaceRight.MinSize = new Size(145, 0);
                    emptySpaceRight.Size = new Size(145, 40);

                    lcSumApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    emptySpaceRight.MinSize = new Size(225, 10);
                    emptySpaceRight.MinSize = new Size(225, 0);
                    emptySpaceRight.Size = new Size(225, 40);

                    lcSumApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                btnSumNotifyApproval.Text = sumNotify.ToString();
            }
        }

        #endregion

        private void uc207_Search_Load(object sender, EventArgs e)
        {
            txbKeywords.Properties.NullValuePrompt = "請輸入您要查找的信息";

            gcData.DataSource = sourceKnowledge;

            gvData.ReadOnlyGridView();

            LoadData();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
            gvData.RefreshData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gvData.FindFilterText = convertToUnSign3(txbKeywords.Text.Trim());
        }

        private void txbKeywords_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txbKeywords.Text = string.Empty;
            gvData.FindFilterText = txbKeywords.Text.Trim();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            int focusedRow = gvData.FocusedRowHandle;
            if (focusedRow < 0)
                return;

            DataDisplay dataRow = gvData.GetRow(focusedRow) as DataDisplay;
            string idDocument = dataRow?.Id;

            if (!lsIdCanReads.Contains(idDocument) && !IsSysAdmin)
            {
                XtraMessageBox.Show(TempDatas.NoPermission, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            f207_Document_Info fDocumentInfo = new f207_Document_Info(idDocument);
            fDocumentInfo.ShowDialog();

            LoadData();
            gcData.RefreshDataSource();
        }

        private void btnSumNotifyApproval_Click(object sender, EventArgs e)
        {
            GetNotifyApproval();
        }
    }
}
