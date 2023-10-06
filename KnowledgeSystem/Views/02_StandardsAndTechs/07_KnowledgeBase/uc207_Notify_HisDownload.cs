using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
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
    public partial class uc207_Notify_HisDownload : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_Notify_HisDownload()
        {
            InitializeComponent();
        }

        dt207_HistoryGetFileBUS _dt207_HistoryGetFileBUS = new dt207_HistoryGetFileBUS();
        dt207_BaseBUS _dt207_BaseBUS = new dt207_BaseBUS();

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
            var lsHisRaws = _dt207_HistoryGetFileBUS.GetListByUserId(TPConfigs.LoginUser.Id);
            var ls207Base = _dt207_BaseBUS.GetList();

            // Truy vấn và lấy danh sách các tệp tin lịch sử kiến thức dựa trên ID người dùng, kết hợp các bảng liên quan.
            var query = (from data in lsHisRaws
                         join names in ls207Base on data.IdKnowledgeBase equals names.Id
                         select new
                         {
                             data.TimeGet,
                             TypeGetFile = data.idTypeHisGetFile,
                             data.IdKnowledgeBase,
                             data.KnowledgeAttachmentName,
                             names.DisplayName
                         }).ToList();

            gcData.DataSource = query;
            gvData.BestFitColumns();
        }

        private void uc207_Notify_HisDownload_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();

            LoadData();
        }

        private void gvData_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
