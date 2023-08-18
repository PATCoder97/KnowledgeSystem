using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_Notify_DocUpdateNotify : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_Notify_DocUpdateNotify()
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
                var query = (from data in db.dt207_NotifyEditDoc
                             where data.IdUserNotify == TempDatas.LoginId
                             join idDoc in db.dt207_DocProgress on data.IdDocProcess equals idDoc.Id
                             join names in db.dt207_Base on idDoc.IdKnowledgeBase equals names.Id
                             orderby data.TimeNotify descending
                             select new
                             {
                                 data.Id,
                                 data.TimeNotify,
                                 data.IsRead,
                                 data.TimeNotyfiNotes,
                                 idDoc.IdKnowledgeBase,
                                 names.DisplayName
                             }).ToList();

                gcData.DataSource = query;
            }

            gvData.BestFitColumns();
        }

        private void uc207_Notify_DocUpdateNotify_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();

            LoadData();

            gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
        }

        private void gvData_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            string IdKnowledgeBase = gvData.GetFocusedRowCellValue(gvColIdKnowledgeBase).ToString();
            int id = Convert.ToInt32(gvData.GetFocusedRowCellValue(gvColId));

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var notifyDocUpdate = db.dt207_NotifyEditDoc.First(r => r.Id == id);

                if (notifyDocUpdate != null)
                {
                    notifyDocUpdate.IsRead = true;

                    db.dt207_NotifyEditDoc.AddOrUpdate(notifyDocUpdate);
                    db.SaveChanges();

                    bool IsProcess = db.dt207_DocProgress.Any(r => r.IdKnowledgeBase == IdKnowledgeBase && !r.IsComplete);
                    if (IsProcess)
                    {
                        XtraMessageBox.Show(TempDatas.DocIsProcessing, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        LoadData();
                        return;
                    }
                }
            }

            f207_Document_Info document_Info = new f207_Document_Info(IdKnowledgeBase);
            document_Info.ShowDialog();

            LoadData();
        }
    }
}
