using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_AuditDoc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f201_AuditDoc_Info()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
        }

        DXMenuItem itemDelDoc;
        DXMenuItem itemEditDoc;
        DXMenuItem itemApprovalHis;

        public int idBase = -1;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
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

        private void InitializeMenuItems()
        {
            itemEditDoc = CreateMenuItem("更新表單", ItemEditDoc_Click, TPSvgimages.Edit);
            itemDelDoc = CreateMenuItem("刪除表單", ItemDelDoc_Click, TPSvgimages.Close);
            itemApprovalHis = CreateMenuItem("核簽歷史", ItemApprovalHis_Click, TPSvgimages.Approval);
        }

        private void ItemApprovalHis_Click(object sender, EventArgs e)
        {

        }

        private void ItemDelDoc_Click(object sender, EventArgs e)
        {

        }

        private void ItemEditDoc_Click(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            var formByBaseDatas = dt201_FormsBUS.Instance.GetListByIdBase(idBase);

            var displayDatas = (from data in formByBaseDatas
                                join usr in dm_UserBUS.Instance.GetList() on data.UploadUser equals usr.Id
                                let UsrUploadName = $"{usr.Id.Substring(5)} {usr.DisplayName}"
                                select new { data, usr, UsrUploadName }).ToList();

            gcData.DataSource = displayDatas;
            gvData.BestFitColumns();
        }

        private void f201_AuditDoc_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemEditDoc);
                e.Menu.Items.Add(itemDelDoc);
                e.Menu.Items.Add(itemApprovalHis);
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f201_AddAttachment fAtt = new f201_AddAttachment();
            fAtt.eventInfo = EventFormInfo.Create;
            fAtt.formName = "表單";
            fAtt.idBase = idBase;
            fAtt.ShowDialog();

            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            dt201_Forms baseForm = dt201_FormsBUS.Instance.GetItemById(idForm);

            if (baseForm.IsProcessing == true) return;

            int idAtt = (int)baseForm.AttId;

            string filePath = dm_AttachmentBUS.Instance.GetItemById(idAtt).EncryptionName;

            string sourcePath = Path.Combine(TPConfigs.Folder201, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath);
            fView.ShowDialog();
        }
    }
}