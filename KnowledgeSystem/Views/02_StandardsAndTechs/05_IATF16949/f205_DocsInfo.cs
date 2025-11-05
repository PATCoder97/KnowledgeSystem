using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949
{
    public partial class f205_DocsInfo : DevExpress.XtraEditors.XtraForm
    {
        public f205_DocsInfo()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();
        }

        DXMenuItem itemDelDoc;
        DXMenuItem itemEditDoc;

        public dt205_Base currentData = new dt205_Base();
        public dt205_Base parentData = new dt205_Base();

        int idBase = -1;
        bool IsDisable = false;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnAddMultiFile.ImageOptions.SvgImage = TPSvgimages.Add2;
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
        }

        private void ItemDelDoc_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入您的工號以確認刪除表單",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

            GridView view = gvData;
            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            dt205_FormBUS.Instance.RemoveById(idForm, TPConfigs.LoginUser.Id);
            LoadData();
        }

        private void ItemEditDoc_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f205_AddAtts fAtt = new f205_AddAtts()
            {
                eventInfo = EventFormInfo.Update,
                formName = "編輯表單",
                idForm = idForm
            };
            fAtt.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            var displayDatas = (from data in dt205_FormBUS.Instance.GetListByIdBase(idBase)
                                join usr in dm_UserBUS.Instance.GetList() on data.CreateBy equals usr.Id
                                let UsrUploadName = $"{usr.Id.Substring(5)} {usr.DisplayName}"
                                select new { data, usr, UsrUploadName }).ToList();

            gcData.DataSource = displayDatas;
            gvData.BestFitColumns();
        }

        private void f205_DocsInfo_Load(object sender, EventArgs e)
        {
            idBase = currentData.Id;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f205_AddAtts fAtt = new f205_AddAtts()
            {
                eventInfo = EventFormInfo.Create,
                formName = "表單",
                currentData = currentData,
                parentData = parentData
            };
            fAtt.ShowDialog();

            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && !IsDisable)
            {
                GridView view = sender as GridView;
                var baseForm = (view.GetRow(view.FocusedRowHandle) as dynamic).data as dt201_Forms;

                e.Menu.Items.Add(itemEditDoc);
                e.Menu.Items.Add(itemDelDoc);
            }
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            DXMouseEventArgs ea = e as DXMouseEventArgs;
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(ea.Location);
            if (!(info.InRow || info.InRowCell)) return;

            int idForm = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            dt205_Form baseForm = dt205_FormBUS.Instance.GetItemById(idForm);

            int idAtt = (int)baseForm.AttId;
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder205, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(baseForm.DisplayName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile viewFile = new f00_VIewFile(destPath);
            viewFile.ShowDialog();

            //// Xem pdf 
            //var mainForm = f00_ViewMultiFile.Instance;

            //if (!mainForm.Visible)
            //    mainForm.Show();

            //mainForm.OpenFormInDocumentManager(destPath);
        }
    }
}