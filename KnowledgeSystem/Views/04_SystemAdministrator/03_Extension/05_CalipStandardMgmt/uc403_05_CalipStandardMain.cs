using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._05_CalipStandardMgmt
{
    public partial class uc403_05_CalipStandardMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc403_05_CalipStandardMain()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
            System.Drawing.Font fontUI12 = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }
        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        bool isManager40305 = false;
        List<string> deptsManager40305;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dt403_05_Standard> dt403_05_Standards;
        List<dt403_05_StandardAtt> dt403_05_StandardAtts;
        List<dm_Attachment> attachments;
        string picImage = "";
        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdateVer;
        DXMenuItem itemConfirmdate;
        DXMenuItem itemFinishdate;
        DXMenuItem itemViewHistory;
        private class Attachment : dm_Attachment
        {
            public string PathFile { get; set; }
            public dm_Attachment BaseAttachment { get; set; } = new dm_Attachment();
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemUpdateVer = CreateMenuItem("更新版本", ItemUpdateVer_Click, TPSvgimages.UpLevel);
            itemConfirmdate = CreateMenuItem("確認日期", ItemConfirmdate_Click, TPSvgimages.Attach);
            itemFinishdate = CreateMenuItem("完成日期", ItemFinishdate_Click, TPSvgimages.Suspension);
            //itemViewHistory = CreateMenuItem("版本歷史", ItemViewHistory_Click, TPSvgimages.Progress);
        }

        private void ItemConfirmdate_Click(object sender, EventArgs e)
        {
            XtraMessageBoxArgs args = new XtraMessageBoxArgs()
            { Caption = "ConfirmDate", Text = "Bạn xác nhận đã cập nhật chuẩn ", Buttons = new DialogResult[] { DialogResult.Yes, DialogResult.No } };
        }

        private void ItemFinishdate_Click(object sender, EventArgs e)
        {
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f403_05_AddStandard fInfo = new f403_05_AddStandard()
            {
                eventInfo = EventFormInfo.View,
                idBase = idBase,
                Text = "規範"
            };
            fInfo.ShowDialog();
            LoadData();
        }
        private void ItemUpdateVer_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "PDF files (*.pdf)|*.pdf" })
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                GridView view = gvData;
                int idBase = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, gColId));
                // Lấy bản ghi StandardAtt nếu có
                var stdAtt = new dt403_05_StandardAtt { StandardId = idBase,UploadDate = DateTime.Now };

                // ---- Xử lý file ----
                string filePath = dlg.FileName;
                string actualName = Path.GetFileName(filePath);
                string encryptionName = EncryptionHelper.EncryptionFileName(actualName);

                dm_Attachment attachment = new dm_Attachment
                {
                    Thread = "40305",
                    EncryptionName = encryptionName,
                    ActualName = actualName
                };

                // Tạo thư mục nếu chưa có
                string destPath = Path.Combine(TPConfigs.Folder40305, encryptionName);
                Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                // Sao chép file
                File.Copy(filePath, destPath, true);

                // Lưu attachment → lấy Id
                stdAtt.AttId = dm_AttachmentBUS.Instance.Add(attachment);

                // Lưu StandardAtt
                dt403_05_StandardAttBUS.Instance.Add(stdAtt);

                LoadData();
            }
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
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }
        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }
        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();
                dt403_05_Standards = dt403_05_StandardBUS.Instance.GetList();
                dt403_05_StandardAtts = dt403_05_StandardAttBUS.Instance.GetList();
                attachments = dm_AttachmentBUS.Instance.GetListByThread("40305");
                var Display = from STD in dt403_05_Standards
                              select new
                              {
                                  Id = STD.Id,
                                  SN = STD.SN,
                                  DisplayNameTW = STD.DisplayNameTW,
                                  DisplayNameVN = STD.DisplayNameVN,
                              };
                //var Display = from STD in dt403_05_Standards
                //              join STDAtts in dt403_05_StandardAtts on STD.Id equals STDAtts.StandardId
                //              join ATT in attachments on STDAtts.AttId equals ATT.Id
                //              select new
                //              {
                //                  Id = STD.Id,
                //                  SN = STD.SN,
                //                  DisplayNameTW = STD.DisplayNameTW,
                //                  DisplayNameVN = STD.DisplayNameVN,
                //                  AttId = STDAtts.AttId,
                //                  ActualName = ATT.ActualName,
                //                  //EncryptionName = ATT.EncryptionName,
                //              };
                sourceBases.DataSource = Display;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }
        private void uc403_05_CalipStandardMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvForm.ReadOnlyGridView();
            gvForm.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f403_05_AddStandard fAdd = new f403_05_AddStandard()
            {
                eventInfo = EventFormInfo.Create,
                formName = "規範"
            };
            fAdd.ShowDialog();
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)// && isManager40305)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                //string idDept = view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString();

                //if (deptsManager40305.Any(r => r == idDept))
                //{
                //    e.Menu.Items.Add(itemViewInfo);
                //    e.Menu.Items.Add(itemUpdateVer);
                //}
                    e.Menu.Items.Add(itemViewInfo);
                    e.Menu.Items.Add(itemUpdateVer);
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "表單";
        }
        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = view.GetRowCellValue(e.RowHandle, gColId) != null ? (int)view.GetRowCellValue(e.RowHandle, gColId) : -1;

            e.IsEmpty = !dt403_05_StandardAtts.Any(r => r.StandardId == idBase);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);

            var DisplayGvFrom = from stdAtt in dt403_05_StandardAtts
                                join att in attachments on stdAtt.AttId equals att.Id
                                where stdAtt.StandardId == idBase
                                select new
                                {
                                    UploadDate = stdAtt.UploadDate,//.ToString("yyyy/MM/dd HH:mm:ss"),
                                    ConfirmDate = stdAtt.ConfirmDate,//?.ToString("yyyy/MM/dd HH:mm:ss"),
                                    FinishDate = stdAtt.FinishDate,//?.ToString("yyyy/MM/dd HH:mm:ss"),
                                    Name = att.ActualName,
                                    AttId = att.Id,
                                };
            e.ChildList = DisplayGvFrom.ToList();
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvForm_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(System.Windows.Forms.Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAttForm));
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder40305, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            var mainForm = f00_ViewMultiFile.Instance;
            if (!mainForm.Visible)
                mainForm.Show();

            mainForm.OpenFormInDocumentManager(destPath);
        }
    }
}
