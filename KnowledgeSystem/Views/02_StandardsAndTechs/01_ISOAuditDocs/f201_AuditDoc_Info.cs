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
using System.Text.RegularExpressions;
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
        dt201_Base baseData;
        bool IsDisable = false;

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
            itemEditDoc = CreateMenuItem("編輯表單", ItemEditDoc_Click, TPSvgimages.Edit);
            itemDelDoc = CreateMenuItem("刪除表單", ItemDelDoc_Click, TPSvgimages.Close);
            itemApprovalHis = CreateMenuItem("核簽歷史", ItemApprovalHis_Click, TPSvgimages.Approval);
        }

        private void ItemApprovalHis_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f201_SignProg_Detail fDetail = new f201_SignProg_Detail();
            fDetail.idBase = idForm;
            fDetail.ShowDialog();
        }

        private void ItemDelDoc_Click(object sender, EventArgs e)
        {

        }

        private void ItemEditDoc_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f201_AddAttachment fAtt = new f201_AddAttachment();
            fAtt.eventInfo = EventFormInfo.Update;
            fAtt.formName = "編輯表單";
            fAtt.baseForm = dt201_FormsBUS.Instance.GetItemById(idForm);
            fAtt.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            baseData = dt201_BaseBUS.Instance.GetItemById(idBase);
            var allchildren = dt201_BaseBUS.Instance.GetListByParentId(baseData.IdParent);

            // Kiểm tra xem danh sách file có quyền CRUD không
            IsDisable = baseData?.IsDisable == true || baseData.Id != allchildren.Last().Id;

            var displayDatas = (from data in dt201_FormsBUS.Instance.GetListByIdBase(idBase)
                                join usr in dm_UserBUS.Instance.GetList() on data.UploadUser equals usr.Id
                                let UsrUploadName = $"{usr.Id.Substring(5)} {usr.DisplayName}"
                                select new { data, usr, UsrUploadName }).ToList();

            gcData.DataSource = displayDatas;
            gvData.BestFitColumns();

            if (IsDisable)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
        }

        private void f201_AuditDoc_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            var baseForm = (view.GetRow(view.FocusedRowHandle) as dynamic).data as dt201_Forms;

            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && !IsDisable)
            {
                if (baseForm.IsProcessing != true)
                {
                    e.Menu.Items.Add(itemEditDoc);
                    e.Menu.Items.Add(itemDelDoc);
                }

                if (baseForm.DigitalSign == true)
                {
                    e.Menu.Items.Add(itemApprovalHis);
                }
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
            fAtt.formName = "新增表單";
            fAtt.idBase = idBase;
            fAtt.ShowDialog();

            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            bool IsCancel = Convert.ToBoolean(view.GetRowCellValue(view.FocusedRowHandle, gColIsCancel));
            if (IsCancel) return;

            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            dt201_Forms baseForm = dt201_FormsBUS.Instance.GetItemById(idForm);
            if (baseForm.IsProcessing == true) return;

            int idAtt = (int)baseForm.AttId;
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder201, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(baseForm.DisplayName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath);
            fView.ShowDialog();
        }
    }
}