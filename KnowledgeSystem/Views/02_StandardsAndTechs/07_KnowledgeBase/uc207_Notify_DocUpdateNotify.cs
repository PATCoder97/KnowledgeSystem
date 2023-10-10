using BusinessLayer;
using DataAccessLayer;
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

        dm_UserBUS _dm_UserBUS = new dm_UserBUS();
        dm_GroupUserBUS _dm_GroupUserBUS = new dm_GroupUserBUS();
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();
        dt207_DocProcessingBUS _dt207_DocProgressBUS = new dt207_DocProcessingBUS();
        dt207_DocProcessingInfoBUS _dt207_DocProgressInfoBUS = new dt207_DocProcessingInfoBUS();
        dt207_NotifyEditDocBUS _dt207_NotifyEditDocBUS = new dt207_NotifyEditDocBUS();

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
            var lsNotifyEditBases = _dt207_NotifyEditDocBUS.GetListByUID(TPConfigs.LoginUser.Id);
            var lsBaseProcessing = _dt207_DocProgressBUS.GetList();
            var lsBase207 = _dt207_BaseBUS.GetList();

            var query = (from data in lsNotifyEditBases
                         join idDoc in lsBaseProcessing on data.IdDocProcess equals idDoc.Id
                         join names in lsBase207 on idDoc.IdKnowledgeBase equals names.Id
                         orderby data.TimeNotify descending
                         select new
                         {
                             data.Id,
                             data.TimeNotify,
                             data.IsRead,
                             data.TimeNotifyNotes,
                             idDoc.IdKnowledgeBase,
                             names.DisplayName
                         }).ToList();

            gcData.DataSource = query;

            gvData.BestFitColumns();
        }

        private void uc207_Notify_DocUpdateNotify_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

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

            var notifyDocUpdate = _dt207_NotifyEditDocBUS.GetItemById(id);

            if (notifyDocUpdate != null)
            {
                notifyDocUpdate.IsRead = true;
                _dt207_NotifyEditDocBUS.AddOrUpdate(notifyDocUpdate);

                bool IsProcess = _dt207_DocProgressBUS.CheckItemProcessing(IdKnowledgeBase);
                if (IsProcess)
                {
                    XtraMessageBox.Show(TPConfigs.DocIsProcessing, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LoadData();
                    return;
                }
            }

            f207_Document_Info document_Info = new f207_Document_Info(IdKnowledgeBase);
            document_Info.ShowDialog();

            LoadData();
        }
    }
}
