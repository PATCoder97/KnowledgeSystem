using BusinessLayer;
using DataAccessLayer;
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
            helper = new RefreshHelper(gvData, "Id");
        }

        Dictionary<int, string> signTypes = TPConfigs.signTypes;
        BindingSource sourceSigns = new BindingSource();

        private class SignInfo : dm_Sign
        {
            public string SignType { get; set; }
        }

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
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
                                 FontColor = data.FontColor
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

        private void gcData_DoubleClick(object sender, EventArgs e)
        {

        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            dm_Sign signSelect = view.GetRow(view.FocusedRowHandle) as dm_Sign;

            f402_SignInfo fInfo = new f402_SignInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "簽名";
            fInfo.signInfo = signSelect;
            fInfo.ShowDialog();

            LoadSign();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f00_PdfTools frm = new f00_PdfTools(@"E:\01. DEV\02. KnowledgeSystem\Test\Blank.pdf");
            //f00_PdfTools frm = new f00_PdfTools(@"C:\Users\ANHTUAN\Desktop\New folder\Blank.pdf");
            frm.ShowDialog();
        }
    }
}
