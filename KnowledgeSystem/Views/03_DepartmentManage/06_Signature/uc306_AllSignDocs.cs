using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Wizards;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraSplashScreen;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_AllSignDocs : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_AllSignDocs()
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
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> users = new List<dm_User>();
        List<dm_JobTitle> jobs;

        List<dt306_Base> bases;
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;
        DXMenuItem itemSaveFile;
        DXMenuItem itemSaveAllFile;
        DXMenuItem itemEditInfo;

        const string NAME_ISPROGRESS = "核簽中";
        const string NAME_ISCANCEL = "被退回";
        const string NAME_ISCOMPLETE = "核簽完畢";

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("核簽進度", ItemViewInfo_Click, TPSvgimages.View);
            itemViewFile = CreateMenuItem("查看文件", ItemViewFile_Click, TPSvgimages.View);
            itemSaveFile = CreateMenuItem("保存檔案", ItemSaveFile_Click, TPSvgimages.Attach);
            itemSaveAllFile = CreateMenuItem("保存所有檔案", ItemSaveAllFile_Click, TPSvgimages.Attach);
            itemEditInfo = CreateMenuItem("更新訊息", ItemEditInfo_Click, TPSvgimages.Edit);
        }

        private bool SaveFileWithProtect(string source, string dest)
        {
            using (PdfDocumentProcessor pdfDocumentProcessor = new PdfDocumentProcessor())
            {
                // Load a PDF document.
                pdfDocumentProcessor.LoadDocument(source);

                PdfEncryptionOptions encryptionOptions = new PdfEncryptionOptions();
                encryptionOptions.ModificationPermissions = PdfDocumentModificationPermissions.NotAllowed;
                encryptionOptions.InteractivityPermissions = PdfDocumentInteractivityPermissions.NotAllowed;

                // Specify the owner and user passwords for the document.  
                encryptionOptions.OwnerPasswordString = "fhspdf";
                //encryptionOptions.UserPasswordString = "UserPassword";

                // Specify the 256-bit AES encryption algorithm.
                encryptionOptions.Algorithm = PdfEncryptionAlgorithm.AES256;

                // Save the protected document with encryption settings.  
                try
                {
                    pdfDocumentProcessor.SaveDocument(dest, new PdfSaveOptions() { EncryptionOptions = encryptionOptions });
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static string GetUniqueFilePath(string folderPath, string fileName)
        {
            string fileExtension = Path.GetExtension(fileName);
            string baseFileName = Path.GetFileNameWithoutExtension(fileName);

            string filePath = Path.Combine(folderPath, fileName);
            int count = 1;

            while (File.Exists(filePath))
            {
                string tempFileName = $"{baseFileName} ({count++}){fileExtension}";
                filePath = Path.Combine(folderPath, tempFileName);
            }

            return filePath;
        }

        private void ItemEditInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            var baseEdit = dt306_BaseBUS.Instance.GetItemById(idBase);

            uc306_EditSignDoc ucEdit = new uc306_EditSignDoc();
            ucEdit.baseData = baseEdit;
            DialogResult result = XtraDialog.Show(this, ucEdit, "更新訊息", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
            {
                return;
            }

            string displayName = ucEdit.DisplayName;
            string code = ucEdit.Code;
            int idType = ucEdit.IdType;

            baseEdit.DisplayName = displayName;
            baseEdit.Code = string.IsNullOrWhiteSpace(code) ? baseEdit.Code : code;
            baseEdit.IdType = idType;

            dt306_BaseBUS.Instance.AddOrUpdate(baseEdit);
            LoadData();
        }

        private void ItemSaveAllFile_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));
            var allAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase).Where(r => r.IsCancel == false).ToList();

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select a folder";
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    string selectedPath = folderDialog.SelectedPath;
                    string destFolder = Path.Combine(selectedPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    foreach (var item in allAtts)
                    {
                        var att = dm_AttachmentBUS.Instance.GetItemById(item.IdAtt);
                        string encryptionName = att.EncryptionName;
                        string actualName = att.ActualName;

                        string sourcePath = Path.Combine(TPConfigs.Folder306, encryptionName);
                        string destPath = GetUniqueFilePath(destFolder, actualName);

                        SaveFileWithProtect(sourcePath, destPath);
                    }

                    string msg = "已儲存！";
                    MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=18>{msg}</font>");
                }
            }
        }

        private void ItemSaveFile_Click(object sender, EventArgs e)
        {
            GridView view = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));

            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);
            string filePath = att.EncryptionName;
            string actualName = att.ActualName;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = actualName;
            dialog.Filter = "Pdf Files|*.pdf";
            if (dialog.ShowDialog() != DialogResult.OK) return;

            string sourcePath = Path.Combine(TPConfigs.Folder306, filePath);
            string destPath = dialog.FileName;

            bool result = SaveFileWithProtect(sourcePath, destPath);

            string msg = result ? "已儲存！" : "<color=red>文件在開啟或有錯誤！</color>";
            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=18>{msg}</font>");
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            GridView view = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));

            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);
            string filePath = att.EncryptionName;
            string actualName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder306, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{actualName}_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath, isCanSave: false);
            fView.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;

            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f306_SignProgDetail fInfo = new f306_SignProgDetail();
            fInfo.idBase = idBase;
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
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = System.Drawing.Color.Blue }, $"[Remark] = \'{NAME_ISPROGRESS}\'");
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = System.Drawing.Color.Red }, $"[Remark] = \'{NAME_ISCANCEL}\'");
            gvDocs.FormatRules.AddExpressionRule(gColRemark2, new DevExpress.Utils.AppearanceDefault() { ForeColor = System.Drawing.Color.Red }, $"[Status] = \'{NAME_ISCANCEL}\'");
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var progs = dt306_ProgressBUS.Instance.GetListByIdUser(TPConfigs.LoginUser.Id);

                bases = dt306_BaseBUS.Instance.GetList();

                bases = (from data in bases
                         where data.Confidential == false || progs.Select(r => r.IdBase).Contains(data.Id) || data.UploadUsr == TPConfigs.LoginUser.Id
                         select data).ToList();

                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();
                var dmTypes = dt306_TypeBUS.Instance.GetList();

                var basesDisplay = (from data in bases
                                    join types in dmTypes on data.IdType equals types.Id
                                    join upUsr in users on data.UploadUsr equals upUsr.Id
                                    join urs in users on data.NextStepProg equals urs.Id into userGroup
                                    from urs in userGroup.DefaultIfEmpty()
                                    select new
                                    {
                                        data,
                                        types,
                                        urs,
                                        Dept = upUsr.IdDepartment,
                                        DisplayName = urs != null ? $"{urs.Id.Substring(urs.Id.Length - 5)} {urs.IdDepartment}/{urs.DisplayName}" : "",
                                        UploadUsr = $"{upUsr.Id.Substring(upUsr.Id.Length - 5)} {upUsr.DisplayName}",
                                        Code = data.Code
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc306_AllSignDocs_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvDocs.ReadOnlyGridView();

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gcData.ForceInitialize();
            gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
        }

        private void gvData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "Remark" && e.IsGetData)
            {
                bool isProcess = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.IsProcess"));
                bool isCancel = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.IsCancel"));

                if (isCancel)
                {
                    e.Value = NAME_ISCANCEL;
                }
                else if (isProcess)
                {
                    e.Value = NAME_ISPROGRESS;
                }
                else
                {
                    e.Value = NAME_ISCOMPLETE;
                }
            }
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

            string filePath = Path.Combine(documentsPath, $"電子核簽 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);
            bool isProgess = (bool)view.GetRowCellValue(e.RowHandle, gColIsProgess);
            bool isCancel = (bool)view.GetRowCellValue(e.RowHandle, gColIsCancel);

            var baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            var childList = (from data in baseAtts
                             join urs in users on data.UsrCancel equals urs.Id into userGroup
                             from urs in userGroup.DefaultIfEmpty()
                             select new
                             {
                                 data,
                                 DisplayName = urs != null ? $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}" : null,
                                 Status = data.IsCancel || isCancel ? "被退回" : isProgess ? "核簽中" : "核簽完畢"
                             }).ToList();

            e.ChildList = childList;
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "核簽文件";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isProcess = Convert.ToBoolean(view.GetRowCellValue(view.FocusedRowHandle, "data.IsProcess"));
                bool isOwner = view.GetRowCellValue(view.FocusedRowHandle, "data.UploadUsr").ToString() == TPConfigs.LoginUser.Id;

                e.Menu.Items.Add(itemViewInfo);
                if (!isProcess)
                {
                    e.Menu.Items.Add(itemSaveAllFile);

                    bool IsGrand = AppPermission.Instance.CheckAppPermission(AppPermission.EditInfo306);
                    if (IsGrand || isOwner)
                    {
                        itemEditInfo.BeginGroup = true;
                        e.Menu.Items.Add(itemEditInfo);
                    }
                }
            }
        }

        private void gvDocs_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                int rowHandle = e.HitInfo.RowHandle;
                view.FocusedRowHandle = rowHandle;

                GridView parent = view.ParentView as GridView;
                int rowHandleParent = parent.FocusedRowHandle;

                bool isProcess = Convert.ToBoolean(parent.GetRowCellValue(rowHandleParent, "data.IsProcess"));
                bool isCancel = Convert.ToBoolean(parent.GetRowCellValue(rowHandleParent, "data.IsCancel"));

                bool isCancel2 = Convert.ToBoolean(view.GetRowCellValue(rowHandle, "data.IsCancel"));

                if (isProcess == false && isCancel == false && isCancel2 == false)
                {
                    e.Menu.Items.Add(itemViewFile);
                    e.Menu.Items.Add(itemSaveFile);
                }
            }
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            gvData.ExpandMasterRow(gvData.FocusedRowHandle, 0);
        }
    }
}
