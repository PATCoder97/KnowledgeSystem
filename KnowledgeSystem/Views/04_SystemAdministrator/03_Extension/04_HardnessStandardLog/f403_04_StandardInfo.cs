using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._09_SparePart;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._04_HardnessStandardLog
{
    public partial class f403_04_StandardInfo : DevExpress.XtraEditors.XtraForm
    {
        public f403_04_StandardInfo()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        public string deptGetData = "";

        List<dt403_04_StandardInfo> standardInfos;

        DXMenuItem itemViewInfo;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, TPSvgimages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, TPSvgimages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, TPSvgimages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, TPSvgimages.Num4);
            //itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, TPSvgimages.Num5);

            //itemMultiselect = CreateMenuItem("啟用多選", ItemMultiselect_Click, TPSvgimages.CheckedRadio);
            //itemPrintStamp = CreateMenuItem("執行列印", ItemPrintStamp_Click, TPSvgimages.Print);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string sn = view.GetRowCellValue(view.FocusedRowHandle, gColSN).ToString();
            f403_04_Standard_Info fInfo = new f403_04_Standard_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "標準試片",
                snBase = sn,
                idDeptGetData = deptGetData
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                standardInfos = dt403_04_StandardInfoBUS.Instance.GetList().Where(r => r.IdDept == deptGetData).ToList();

                sourceBases.DataSource = standardInfos;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void f403_04_StandarInfo_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f403_04_Standard_Info finfo = new f403_04_Standard_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "標準試片",
                idDeptGetData = deptGetData,
            };

            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
            }
        }
    }
}