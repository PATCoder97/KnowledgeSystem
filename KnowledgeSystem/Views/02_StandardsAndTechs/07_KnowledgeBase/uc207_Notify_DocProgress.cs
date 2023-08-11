using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
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
        public uc207_Notify_DocProgress()
        {
            InitializeComponent();
        }

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

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                var lsDocProgresses = db.dt207_DocProgress.ToList();
                var lsDocProgressInfos = db.DocProgressInfoes.ToList();
                var lsKnowledgeBases = db.dt207_Base.ToList();
                var lsUsers = db.Users.ToList();

                var lsDocNotSuccess = (from data in db.DocProgressInfoes
                                       group data by data.IdDocProgress into g
                                       select new
                                       {
                                           IdDocProgress = g.Key,
                                           IndexStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IndexStep).FirstOrDefault(),
                                           TimeStep = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.TimeStep).FirstOrDefault(),
                                           IdUserProcess = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault(),
                                           Descriptions = g.OrderByDescending(dpi => dpi.TimeStep).Select(dpi => dpi.Descriptions).FirstOrDefault(),
                                           IdUserNow = g.OrderBy(dpi => dpi.TimeStep).Select(dpi => dpi.IdUserProcess).FirstOrDefault(),
                                       }).ToList();

                var lsDataApproval = (from data in lsDocProgresses
                                      join infos in lsDocNotSuccess on data.Id equals infos.IdDocProgress
                                      join bases in lsKnowledgeBases on data.IdKnowledgeBase equals bases.Id
                                      join users in lsUsers on infos.IdUserProcess equals users.Id
                                      select new
                                      {
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
            }

            gvData.BestFitColumns();
        }

        private void uc207_Notify_DocProgress_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();

            LoadData();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            string IdKnowledgeBase = gvData.GetFocusedRowCellValue(gvColIdKnowledgeBase).ToString();

            f207_Document_Info document_Info = new f207_Document_Info(IdKnowledgeBase);
            document_Info.ShowDialog();

            LoadData();
        }

        private void gvData_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (!gvData.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }

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
