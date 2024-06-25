using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin
{
    public partial class uc402_SignManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc402_SignManage()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        Dictionary<int, string> signTypes = TPConfigs.signTypes;
        BindingSource sourceSigns = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemModifyUsr;

        private class SignInfo : dm_Sign
        {
            public string SignType { get; set; }
        }

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnTestSign.ImageOptions.SvgImage = TPSvgimages.Approval;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("更新簽名", ItemViewInfo_Click, TPSvgimages.View);
            itemModifyUsr = CreateMenuItem("設定權限", ItemModifyUsr_Click, TPSvgimages.Edit);
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
            dm_Sign signSelect = dm_SignBUS.Instance.GetItemById(idSign);

            f402_SignInfo fInfo = new f402_SignInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "簽名";
            fInfo.signInfo = signSelect;
            fInfo.ShowDialog();

            LoadSign();
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

        private void LoadSign()
        {
            helper.SaveViewInfo();

            List<dm_Sign> signs = dm_SignBUS.Instance.GetList();

            var signInfos = (from data in signs
                             join typeImg in signTypes on data.ImgType equals typeImg.Key
                             select new SignInfo
                             {
                                 Id = data.Id,
                                 DisplayName = data.DisplayName,
                                 ImgName = data.ImgName,
                                 ImgType = data.ImgType,
                                 SignType = typeImg.Value,
                                 WidImg = data.WidImg,
                                 HgtImg = data.HgtImg,
                                 X = data.X,
                                 Y = data.Y,
                                 FontName = data.FontName,
                                 FontSize = data.FontSize,
                                 FontType = data.FontType,
                                 FontColor = data.FontColor,
                                 Prioritize = data.Prioritize
                             }).ToList();

            sourceSigns.DataSource = signInfos;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        private void uc402_SignManage_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceSigns;

            LoadSign();
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_SignInfo fInfo = new f402_SignInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "簽名";
            fInfo.ShowDialog();

            LoadSign();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ////f00_PdfTools frm = new f00_PdfTools(@"E:\01. DEV\02. KnowledgeSystem\Test\Blank.pdf");
            //f00_PdfTools frm = new f00_PdfTools(@"C:\Users\ANHTUAN\Desktop\New folder\Blank.pdf");
            //frm.ShowDialog();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemModifyUsr);
            }
        }

        private void btnTestSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF (*.pdf)|*.pdf";
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string sourcePath = ofd.FileName;
            string sourceFolder = Path.GetDirectoryName(sourcePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"TestSign_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_PdfTools pdfTools = new f00_PdfTools(destPath, sourceFolder);
            pdfTools.ShowDialog();
        }
    }
}
