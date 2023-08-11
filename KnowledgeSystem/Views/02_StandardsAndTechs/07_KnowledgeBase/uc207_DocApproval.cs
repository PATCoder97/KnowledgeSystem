using DevExpress.ClipboardSource.SpreadsheetML;
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

        BindingSource source = new BindingSource();

        List<dt207_DocProgress> lsDocProgresses = new List<dt207_DocProgress>();
        List<DocProgressInfo> lsDocProgressInfos = new List<DocProgressInfo>();

        #endregion

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsDocProgresses = db.dt207_DocProgress.Where(r => !r.IsComplete).ToList();
                lsDocProgressInfos = db.DocProgressInfoes.ToList();
                var lsKnowledgeBases = db.dt207_Base.ToList();
                var lsUsers = db.Users.ToList();

                var lsDocNotSuccess = (from data in db.DocProgressInfoes
                                       group data by data.IdDocProgress into g
                                       select new
                                       {
                                           IdDocProgress = g.Key,
                                           IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                                           TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                                           IdUserProcess = g.OrderBy(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault()
                                       }).ToList();

                var lsDataApproval = (from data in lsDocProgresses
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
                                      }).ToList();

                source.DataSource = lsDataApproval;
            }

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

            gcData.DataSource = source;

            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            string IdKnowledgeBase = gvData.GetFocusedRowCellValue(gvColIdKnowledgeBase).ToString();

            f207_Document_Info document_Info = new f207_Document_Info(IdKnowledgeBase);
            document_Info.ShowDialog();

            LoadData();
        }
    }
}
