using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_DocApproval : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_DocApproval()
        {
            InitializeComponent();
        }

        #region parameters

        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();


        BindingSource source = new BindingSource();

        List<dt207_DocProcessing> lsDocProgresses = new List<dt207_DocProcessing>();
        List<dt207_DocProcessingInfo> lsDocProgressInfos = new List<dt207_DocProcessingInfo>();

        #endregion

        #region methods

        private void LoadData()
        {
            // Xử lý lấy tất cả các văn kiện cần trình ký
            lsDocProgresses = dt207_DocProcessingBUS.Instance.GetListNotComplete();
            lsDocProgressInfos = dt207_DocProcessingInfoBUS.Instance.GetList();
            var lsKnowledgeBases = _dt207_BaseBUS.GetList();
            var lsUsers = dm_UserBUS.Instance.GetList();
            var lsGroupUserByUID = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var lsStepProgress = dm_StepProgressBUS.Instance.GetList();

            var lsDocNotSuccess =
                (from data in lsDocProgressInfos
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
            var lsGroupIn = (from data in lsGroupUserByUID
                             join progresses in lsStepProgress on data.IdGroup equals progresses.IdGroup
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


            source.DataSource = lsDisplays;

            gcData.RefreshDataSource();
        }

        #endregion

        private void uc207_DocApproval_Load(object sender, EventArgs e)
        {
            lbInfo.Text = "※ 請審查下列資料。<br>※ 若同意上傳請按<color=red>「核准」</color>按鈕<br>※ 不同意上傳請按<color=red>「退回」</color>按鈕";
            lbInfo.AllowHtmlString = true;
            lbInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lbInfo.Appearance.Options.UseTextOptions = true;
            lbInfo.AutoSizeMode = LabelAutoSizeMode.Vertical;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = source;

            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            string IdKnowledgeBase = gvData.GetFocusedRowCellValue(gvColIdKnowledgeBase).ToString();

            f207_Document_Info document_Info = new f207_Document_Info(IdKnowledgeBase);
            document_Info._event207 = Event207DocInfo.Approval;
            document_Info.ShowDialog();

            LoadData();
        }
    }
}
