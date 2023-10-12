using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_Notify_DocProgress : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;

        public uc207_Notify_DocProgress()
        {
            InitializeComponent();
        }

        dm_GroupUserBUS _dm_GroupUserBUS = new dm_GroupUserBUS();
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();
        dt207_DocProcessingBUS _dt207_DocProgressBUS = new dt207_DocProcessingBUS();
        dt207_DocProcessingInfoBUS _dt207_DocProgressInfoBUS = new dt207_DocProcessingInfoBUS();

        List<dt207_DocProcessingInfo> lsBaseProcessInfos;

        Font fontIndicator = new Font("Times New Roman", 12.0f, FontStyle.Italic);
        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e)
        {
            e.Info.Appearance.Font = fontIndicator;
            e.Info.Appearance.ForeColor = Color.FromArgb(16, 110, 190);
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gcolDescription,
                new DevExpress.Utils.AppearanceDefault() { BackColor = Color.Red },
                "StartsWith([Descriptions], \'退回\')");
            gvData.FormatRules.AddExpressionRule(gcolDescription,
              new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.BlueViolet },
              "StartsWith([Descriptions], \'取消\')");
            gvData.FormatRules.AddExpressionRule(gcolDescription,
              new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Green },
              "StartsWith([Descriptions], \'確認完畢\')");
        }

        private void LoadData()
        {
            helper.SaveViewInfo();

            var lsDocProgresses = _dt207_DocProgressBUS.GetListByUIDProcess(TPConfigs.LoginUser.Id);
            var lsKnowledgeBases = _dt207_BaseBUS.GetList();
            var lsUsers = dm_UserBUS.Instance.GetList();
            lsBaseProcessInfos = _dt207_DocProgressInfoBUS.GetList();

            var lsDocProgressInfosByLoginId =
                (from data in lsBaseProcessInfos
                 group data by data.IdDocProgress into g
                 select new
                 {
                     IdDocProgress = g.Key,
                     IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                     TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                     IdUserProcess = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault(),
                     Descriptions = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.Descriptions).FirstOrDefault(),
                 }).ToList();

            var lsDataApproval =
                (from data in lsDocProgresses
                 join infos in lsDocProgressInfosByLoginId on data.Id equals infos.IdDocProgress
                 join bases in lsKnowledgeBases on data.IdKnowledgeBase equals bases.Id
                 join users in lsUsers on infos.IdUserProcess equals users.Id
                 select new
                 {
                     data.Id,
                     data.IsComplete,
                     Reason = data.Descriptions,
                     data.IdKnowledgeBase,
                     data.IdProgress,
                     infos.TimeStep,
                     infos.IndexStep,
                     infos.Descriptions,
                     bases.DisplayName,
                     UserProcess = $"{users.IdDepartment} | {infos.IdUserProcess}/{users.DisplayName}",
                 }).OrderByDescending(r => r.TimeStep).ToList();

            gcData.DataSource = lsDataApproval;

            gvData.BestFitColumns();

            helper.LoadViewInfo();
        }

        private void uc207_Notify_DocProgress_Load(object sender, EventArgs e)
        {
            helper = new RefreshHelper(gvData, "Id");
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();

            CreateRuleGV();
            gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            int idDocProcess = Convert.ToInt32(gvData.GetFocusedRowCellValue(gColId));
            var docProcess = _dt207_DocProgressBUS.GetItemById(idDocProcess);

            int indexStep = lsBaseProcessInfos.OrderByDescending(r => r.TimeStep).FirstOrDefault(r => r.IdDocProgress == idDocProcess).IndexStep;
            if (indexStep == -1 && !(docProcess.IsComplete))
            {
                f207_Document_Info document_Info = new f207_Document_Info(docProcess.IdKnowledgeBase);
                document_Info._event207 = Event207DocInfo.Check;
                document_Info.ShowDialog();
            }
            else
            {
                f207_Document_ViewOnly document_Info = new f207_Document_ViewOnly(docProcess);
                document_Info.ShowDialog();
            }

            LoadData();
        }

        private void gvData_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (!gvData.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    e.Info.ImageIndex = e.RowHandle < 0 ? 0 : -1;
                    e.Info.DisplayText = e.RowHandle < 0 ? string.Empty : (e.RowHandle + 1).ToString();

                    IndicatorDraw(e);

                    SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                    int width = (int)size.Width + 20;

                    BeginInvoke(new MethodInvoker(() => cal(width, gvData)));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = $"[{e.RowHandle * -1}]";

                IndicatorDraw(e);

                SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                int width = (int)size.Width + 20;

                BeginInvoke(new MethodInvoker(() => cal(width, gvData)));
            }
        }
    }
}
