using BusinessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class f312_QuesMgmt : DevExpress.XtraEditors.XtraForm
    {
        public f312_QuesMgmt(int _idGroup)
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvQues, "Id");

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;

            idGroup = _idGroup;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        RefreshHelper helper;
        BindingSource sourceQues = new BindingSource();

        int idGroup = -1;

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e, Color color)
        {
            e.Info.Appearance.Font = fontUI14;
            e.Info.Appearance.ForeColor = color;
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            //itemAddNode = CreateMenuItem("新增表單", ItemAddNote_Click, TPSvgimages.Add);
            //itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            //itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            //itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            //itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            //itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var ques = dt312_QuestionsBUS.Instance.GetListByIdGroup(idGroup);

                sourceQues.DataSource = ques;

                gcData.DataSource = sourceQues;
                gvQues.BestFitColumns();
                gvQues.CollapseAllDetails();
            }
        }

        private void LoadQues(string idJob)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var ques = dt312_QuestionsBUS.Instance.GetListByIdGroup(idGroup);

                sourceQues.DataSource = ques;
                helper.LoadViewInfo();

                gvQues.BestFitColumns();
                gvQues.CollapseAllDetails();
            }
        }

        private void f312_QuesMgmt_Load(object sender, EventArgs e)
        {
            gvQues.ReadOnlyGridView();
            gvQues.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvQues.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvQues.OptionsDetail.AllowZoomDetail = false;

            gvAns.ReadOnlyGridView();
            gvAns.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvAns.OptionsDetail.AllowZoomDetail = false;

            LoadData();
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "HaveImg" && e.IsGetData)
            {
                string imgName = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "ImageName")?.ToString();

                e.Value = !string.IsNullOrWhiteSpace(imgName);
            }
        }
    }
}