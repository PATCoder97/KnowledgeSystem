using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._02_JFEnCSCDocs
{
    public partial class uc202_JFEnCSCBase : DevExpress.XtraEditors.XtraUserControl
    {
        public uc202_JFEnCSCBase()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;

        List<dm_User> lsUser = new List<dm_User>();
        List<dt202_Type> types;

        List<dt202_Attach> attachments;
        List<dm_Attachment> attachmentsInfo;

        private bool IsCanEdit = false;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("編輯", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemViewFile = new DXMenuItem("讀取", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewFile.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            int idFile = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdFile));

            var fileInfo = attachmentsInfo.FirstOrDefault(r => r.Id == idFile);

            if (fileInfo == null) return;

            string source = Path.Combine(TPConfigs.Folder202, fileInfo.EncryptionName);
            string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {fileInfo.ActualName}");
            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(source, dest, true);

            f00_VIewFile viewFile = new f00_VIewFile(dest);
            viewFile.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            string idBase = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId).ToString();

            f202_DocInfo fInfo = new f202_DocInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "文件";
            fInfo.idBase202 = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                // Check quyền hạn có thể sửa văn kiện
                IsCanEdit = AppPermission.Instance.CheckAppPermission(AppPermission.EditDoc202);

                helper.SaveViewInfo();

                var lsBases = dt202_BaseBUS.Instance.GetList();
                lsUser = dm_UserBUS.Instance.GetList();
                types = dt202_TypeBUS.Instance.GetList();

                var lsBasesDisplay = (from data in lsBases
                                      join typeOf in types on data.TypeOf equals typeOf.Id
                                      join usrUpload in lsUser on data.UsrUpload equals usrUpload.Id
                                      select new
                                      {
                                          Id = data.Id,
                                          data.DisplayName,
                                          data.RequestUsr,
                                          TypeDoc = typeOf.DisplayName,
                                          data.Keyword,
                                          data.UploadTime,
                                          UsrUpload = usrUpload.DisplayName,
                                          data.IdFile
                                      }).ToList();

                sourceBases.DataSource = lsBasesDisplay;
                helper.LoadViewInfo();

                attachments = dt202_AttachBUS.Instance.GetListByListBases(lsBases.Select(r => r.Id).ToList());
                attachmentsInfo = dm_AttachmentBUS.Instance.GetListByThread("202");

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                if (IsCanEdit) e.Menu.Items.Add(itemViewInfo);
               
                e.Menu.Items.Add(itemViewFile);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f202_DocInfo fInfo = new f202_DocInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "文件";
            fInfo.ShowDialog();

            LoadData();
        }

        private void uc202_JFEnCSCBase_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();
            gvAttachment.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            string idBase = view.GetRowCellValue(e.RowHandle, gColId)?.ToString();
            var reports = attachments.Where(r => r.IdBase == idBase).Select(r => r.Id).ToList();

            e.IsEmpty = reports.Count == 0;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            string idBase = view.GetRowCellValue(e.RowHandle, gColId).ToString();

            e.ChildList = (from data in attachments.Where(r => r.IdBase == idBase)
                           join att in attachmentsInfo on data.IdAttach equals att.Id
                           select att).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "表單";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvAttachment_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            string actualName = view.GetRowCellValue(view.FocusedRowHandle, gColActualName).ToString();
            string encryptName = view.GetRowCellValue(view.FocusedRowHandle, gColEncryptName).ToString();

            string source = Path.Combine(TPConfigs.Folder202, encryptName);
            string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {actualName}");
            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(source, dest, true);

            f00_VIewFile viewFile = new f00_VIewFile(dest);
            viewFile.ShowDialog();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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
    }
}
