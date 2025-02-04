using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_Watermark : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc402_Watermark()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "ID");
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        BindingSource sourceData = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemModifyUsr;

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("更新浮水印", ItemViewInfo_Click, TPSvgimages.View);
            //itemModifyUsr = CreateMenuItem("設定權限", ItemModifyUsr_Click, TPSvgimages.Edit);
        }

        private void ItemModifyUsr_Click(object sender, EventArgs e)
        {
            int idSign = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));

            f402_SignUsers FSignUsr = new f402_SignUsers();
            FSignUsr._eventInfo = EventFormInfo.View;
            FSignUsr.idSign = idSign;
            FSignUsr.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            int idSign = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            dm_Watermark vmSelect = dm_WatermarkBUS.Instance.GetItemById(idSign);

            f402_Watermark_Info fInfo = new f402_Watermark_Info();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "浮水印";
            fInfo.watermark = vmSelect;
            fInfo.ShowDialog();

            LoadData();
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
            helper.SaveViewInfo();

            List<dm_Watermark> watermarks = dm_WatermarkBUS.Instance.GetList();

            //var signInfos = (from data in signs
            //                 join typeImg in signTypes on data.ImgType equals typeImg.Key
            //                 select new SignInfo
            //                 {
            //                     Id = data.Id,
            //                     DisplayName = data.DisplayName,
            //                     ImgName = data.ImgName,
            //                     ImgType = data.ImgType,
            //                     SignType = typeImg.Value,
            //                     WidImg = data.WidImg,
            //                     HgtImg = data.HgtImg,
            //                     X = data.X,
            //                     Y = data.Y,
            //                     FontName = data.FontName,
            //                     FontSize = data.FontSize,
            //                     FontType = data.FontType,
            //                     FontColor = data.FontColor,
            //                     Prioritize = data.Prioritize
            //                 }).ToList();

            sourceData.DataSource = watermarks;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        private void uc402_Watermark_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceData;

            LoadData();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_Watermark_Info fInfo = new f402_Watermark_Info();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "浮水印";
            fInfo.ShowDialog();

            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                e.Menu.Items.Add(itemViewInfo);
                //e.Menu.Items.Add(itemModifyUsr);
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }
    }
}
