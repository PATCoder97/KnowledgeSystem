using BusinessLayer;
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
using System.Reflection;
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
            InitializeIcon();
            helper = new RefreshHelper(gvData, "Id");
        }

        #region parameters

        // Khai báo BUS
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();
        dt207_TypeHisGetFileBUS _dt207_TypeHisGetFileBUS = new dt207_TypeHisGetFileBUS();
        dt207_SecurityBUS _dt207_SecurityBUS = new dt207_SecurityBUS();


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

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnSearch.ImageOptions.SvgImage = TPSvgimages.Search;
            btnSumNotifyApproval.ImageOptions.SvgImage = TPSvgimages.Approval;
        }

        private void LoadData()
        {
            // Lấy các thông tin liên quan
            string _userLogin = TPConfigs.LoginUser.Id;
            bool _haveKeyword = checkUseKeyword.CheckState == CheckState.Checked;
            gvColKeyword.Visible = _haveKeyword;
            helper.SaveViewInfo();

            // Khai báo các danh sách liên quan
            List<dm_User> lsUsers = dm_UserBUS.Instance.GetList();
            List<dt207_Type> lsKnowledgeTypes = dt207_TypeBUS.Instance.GetList();
            List<dt207_TypeHisGetFile> lsTypeHisGetFile = _dt207_TypeHisGetFileBUS.GetList();
            List<dt207_Base> lsKnowledgeBase = new List<dt207_Base>();
            List<dt207_Base> ls207Base = _haveKeyword ? _dt207_BaseBUS.GetList() : _dt207_BaseBUS.GetListWithoutKeyword();
            List<dt207_Security> lsSecurities = _dt207_SecurityBUS.GetList();
            List<dm_Group> lsGroups = dm_GroupBUS.Instance.GetList();
            List<dm_GroupUser> lsGroupUsers = dm_GroupUserBUS.Instance.GetList();

            // Lấy danh sách các giá trị IdKnowledgeBase mà chưa hoàn thành lưu trình trình ký
            var lsIdBaseRemove = dt207_DocProcessingBUS.Instance.GetListNotComplete().Select(r => r.IdKnowledgeBase).ToList();

            // Kiểm tra xem phải sysAdmin không, nếu là admin thì cho xem tất cả văn kiện
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            if (IsSysAdmin)
            {
                lsKnowledgeBase = ls207Base.Where(r => !lsIdBaseRemove.Contains(r.Id)).ToList();
                goto GET_DISPLAY_LIST;
            }

            // Mặc định sẽ hiển thị danh sách Base mà UserLogin là người đưa lên hoặc yêu cầu
            var lsIdBaseUserUploaded = ls207Base.Where(r => r.UserUpload == _userLogin || r.UserProcess == _userLogin).Select(r => r.Id).ToList();

            // Join group với GroupUser để lấy Prioritize
            var lsGroupWithPioritze = (from gUser in lsGroupUsers
                                       join g in lsGroups on gUser.IdGroup equals g.Id
                                       select new
                                       {
                                           gUser.IdGroup,
                                           gUser.IdUser,
                                           g.Prioritize
                                       }).ToList();

            // Danh sách mà user có quyền tìm kiếm
            var lsCanSearchs = (from ks in lsSecurities
                                join gu in lsGroupWithPioritze on ks.IdGroup equals gu.IdGroup into gj
                                from subGu in gj.DefaultIfEmpty()
                                where ks.IdUser == _userLogin || (subGu != null ? subGu.IdUser == _userLogin : false)
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
            lsIdDisplay.AddRange(lsIdBaseUserUploaded);
            lsIdDisplay.RemoveAll(r => lsIdBaseRemove.Contains(r));
            lsIdDisplay = lsIdDisplay.Distinct().ToList();

            lsIdCanReads.AddRange(lsIdBaseUserUploaded);
            lsIdCanReads.RemoveAll(r => lsIdBaseRemove.Contains(r));

            lsKnowledgeBase = ls207Base.Where(r => lsIdDisplay.Contains(r.Id)).ToList();

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
                                      Keyword = _haveKeyword ? data.Keyword : null,
                                      UserProcessName = userProcess_.DisplayName,
                                      UploadDate = data.UploadDate,
                                      nonUnicodeDisplayName = convertToUnSign3(data.DisplayName),
                                      nonUnicodeKeyword = _haveKeyword ? null : convertToUnSign3(data.Keyword),
                                  })
                                  .ToList();

            // Đặt nguồn dữ liệu của sourceKnowledge là danh sách các đối tượng DataDisplay
            sourceKnowledge.DataSource = lsDataDisplays;

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
                var lsDocProgresses = db.dt207_DocProcessing.Where(r => !(r.IsComplete)).ToList();
                var lsDocProgressInfos = db.dt207_DocProcessingInfo.ToList();
                var lsKnowledgeBases = db.dt207_Base.ToList();
                var lsUsers = db.dm_User.ToList();

                var lsDocNotSuccess =
                    (from data in db.dt207_DocProcessingInfo
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
                var lsGroupIn = (from data in db.dm_GroupUser.Where(r => r.IdUser == TPConfigs.LoginUser.Id).ToList()
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
                    emptySpaceRight.MinSize = new Size(85, 10);
                    emptySpaceRight.MinSize = new Size(85, 0);
                    emptySpaceRight.Size = new Size(85, 40);

                    lcSumApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    emptySpaceRight.MinSize = new Size(225, 10);
                    emptySpaceRight.MinSize = new Size(225, 0);
                    emptySpaceRight.Size = new Size(225, 40);

                    lcSumApproval.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                btnSumNotifyApproval.Text = $"{sumNotify}待審查";
            }
        }

        #endregion

        private void uc207_Search_Load(object sender, EventArgs e)
        {
            txbKeywords.Properties.NullValuePrompt = "請輸入您要查找的信息";

            gcData.DataSource = sourceKnowledge;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

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
                MsgTP.MsgNoPermission();
                return;
            }

            f207_Document_Info fDocumentInfo = new f207_Document_Info(idDocument);
            fDocumentInfo._event207 = Event207DocInfo.View;
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
