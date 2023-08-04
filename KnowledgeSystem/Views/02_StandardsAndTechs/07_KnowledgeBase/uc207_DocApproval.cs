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

        List<DocProgress> lsDocProgresses = new List<DocProgress>();
        List<DocProgressInfo> lsDocProgressInfos = new List<DocProgressInfo>();

        #endregion

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsDocProgresses = db.DocProgresses.Where(r => !r.IsSuccessful).ToList();
                lsDocProgressInfos = db.DocProgressInfoes.ToList();
                var lsKnowledgeBases = db.KnowledgeBases.ToList();
                var lsUsers = db.Users.ToList();


                var queryDocNotSuccess = (from infos in lsDocProgressInfos
                                          group infos by infos.IdDocProgress into pg
                                          select new
                                          {
                                              Id = pg.Key,
                                              TimeStep = lsDocProgressInfos.Where(r => r.IdDocProgress == pg.Key).OrderByDescending(r => r.TimeStep).First().TimeStep,
                                              IndexStep = lsDocProgressInfos.Where(r => r.IdDocProgress == pg.Key).OrderByDescending(r => r.TimeStep).First().IndexStep,
                                              IdUserProcess = lsDocProgressInfos.Where(r => r.IdDocProgress == pg.Key).OrderBy(r => r.TimeStep).First().IdUserProcess,
                                          }).ToList();

                var lsDataApproval = (from data in lsDocProgresses
                                      join infos in queryDocNotSuccess on data.Id equals infos.Id
                                      join bases in lsKnowledgeBases on data.IdKnowledgeBase equals bases.Id
                                      join users in lsUsers on infos.IdUserProcess equals users.Id
                                      select new
                                      {
                                          data.IdKnowledgeBase,
                                          data.IdProgress,
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
            lbInfo.Text = "※ 請審查下列資料。<br>※ 若同意上傳請按<color=red>「核准」</color>按鈕<br>※ 不同意上傳請按<color=red>「退回」</color>按鈕<br>※ 若要修改請按<color=red>「編輯」</color>按鈕";
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
        }
    }
}
