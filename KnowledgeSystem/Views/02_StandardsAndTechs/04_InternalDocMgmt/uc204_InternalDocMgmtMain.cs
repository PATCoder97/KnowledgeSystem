using BusinessLayer;
using DataAccessLayer;
using DevExpress.Pdf;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
using KnowledgeSystem.Views._03_DepartmentManage._06_Signature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt
{
    public partial class uc204_InternalDocMgmtMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc204_InternalDocMgmtMain()
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

        List<dt204_InternalDocMgmt> dt204Bases;
        List<dm_Attachment> attachments;
        List<dt204_Form> forms = new List<dt204_Form>();

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdateVer;
        DXMenuItem itemGetFile;
        DXMenuItem itemPauseNotify;
        DXMenuItem itemViewHistory;

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemUpdateVer = CreateMenuItem("更新版本", ItemUpdateVer_Click, TPSvgimages.UpLevel);
            itemGetFile = CreateMenuItem("下載檔案", ItemGetFile_Click, TPSvgimages.Attach);
            itemPauseNotify = CreateMenuItem("暫停通知", ItemPauseNotify_Click, TPSvgimages.Suspension);
            itemViewHistory = CreateMenuItem("版本歷史", ItemViewHistory_Click, TPSvgimages.Progress);
        }

        private void ItemViewHistory_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f204_OldVersions oldVersions = new f204_OldVersions() { idBase = idBase };
            oldVersions.ShowDialog();
        }

        private void ItemPauseNotify_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入需要停止通知的月數",
                DefaultButtonIndex = 0,
                Editor = new TextEdit
                {
                    Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                    Properties = { Mask = { MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric, EditMask = "d", UseMaskAsDisplayFormat = true } }
                },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result)) return;

            int pauseMonth = Convert.ToInt16(result);
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var dataPause = dt204_InternalDocMgmtBUS.Instance.GetItemById(idBase);
            dataPause.PauseNotify = pauseMonth;
            dt204_InternalDocMgmtBUS.Instance.AddOrUpdate(dataPause);

            LoadData();
        }

        private void ItemGetFile_Click(object sender, EventArgs e)
        {
            int idAttGet = -1;
            if (sender is DXMenuItem menuItem && menuItem.Tag is string nameGridView)
            {
                switch (nameGridView)
                {
                    case "gvData":

                        int focusedRowHandle = gvData.FocusedRowHandle;
                        var rowData = gvData.GetRow(focusedRowHandle);
                        dt204_InternalDocMgmt data = ((dynamic)gvData.GetRow(focusedRowHandle)).data as dt204_InternalDocMgmt;
                        idAttGet = data.IdAtt;

                        break;
                    case "gvForm":

                        GridView view = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
                        idAttGet = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAttForm));

                        break;
                }

                var att = dm_AttachmentBUS.Instance.GetItemById(idAttGet);
                string filePath = att.EncryptionName;
                string actualName = $"{Path.GetFileNameWithoutExtension(att.ActualName)}-{DateTime.Now:HHmmss}{Path.GetExtension(att.ActualName)}";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = actualName;
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string sourcePath = Path.Combine(TPConfigs.Folder204, filePath);
                string destPath = dialog.FileName;

                File.Copy(sourcePath, destPath, true);
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>下載完成!</font>");
            }
        }

        private void ItemUpdateVer_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f204_UpdateVerDoc fUpdate = new f204_UpdateVerDoc()
            {
                eventInfo = EventFormInfo.View,
                idBase = idBase,
                Text = "更新版本"
            };
            fUpdate.ShowDialog();

            LoadData();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f204_DocMgmt_Info fInfo = new f204_DocMgmt_Info()
            {
                eventInfo = EventFormInfo.View,
                idBase = idBase,
                formName = "文件"
            };
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
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            var rule = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "AddMonths(Today(),3) >= AddMonths([data.DeployDate], [data.PeriodNotify] + Iif(IsNullOrEmpty([data.PauseNotify]), 0, [data.PauseNotify]))",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };

            // Thêm quy tắc vào GridView
            gvData.FormatRules.Add(rule);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

                var depts = dm_DeptBUS.Instance.GetList();
                var groups = dm_GroupBUS.Instance.GetListByName("ISO組");

                var deptAccess = (from data in groups
                                  join grp in grpUsrs on data.Id equals grp.IdGroup
                                  select data.IdDept).ToList();

                dt204Bases = dt204_InternalDocMgmtBUS.Instance.GetList();

                // Lấy các văn kiện khác 三階, hoặc của tổ mình đưa lên theo quyền trong nhóm ISO
                dt204Bases = (from data in dt204Bases
                              where data.DocLevel != "三階" || deptAccess.Contains(data.IdDept)
                              select data).ToList();

                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();
                var docCatoraries = dt204_DocCatoraryBUS.Instance.GetList();
                var funcCatoraries = dt204_FuncCatoraryBUS.Instance.GetList();
                forms = dt204_FormBUS.Instance.GetList();

                var basesDisplay = (from data in dt204Bases
                                    join docCato in docCatoraries on data.IdDocCatorary equals docCato.Id
                                    join funcCato in funcCatoraries on data.IdFuncCatorary equals funcCato.Id
                                    join urs in users on data.IdFounder equals urs.Id
                                    join ursUp in users on data.IdUsrUpload equals ursUp.Id
                                    let DocCatorary = docCato.DisplayName
                                    let FuncCatorary = funcCato.DisplayName
                                    select new
                                    {
                                        data,
                                        DocCatorary,
                                        FuncCatorary,
                                        urs,
                                        Founder = urs != null
                                            ? $"{urs.Id.Substring(5)} LG{urs.IdDepartment}/{urs.DisplayName}" : "",
                                        UserUpload = ursUp != null
                                            ? $"{ursUp.Id.Substring(5)} LG{ursUp.IdDepartment}/{ursUp.DisplayName}" : ""
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        private void uc204_InternalDocMgmtMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvForm.ReadOnlyGridView();
            gvForm.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f204_DocMgmt_Info fAdd = new f204_DocMgmt_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "文件"
            };
            fAdd.ShowDialog();

            LoadData();
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
            GridView view = sender as GridView;
            int idBase = view.GetRowCellValue(e.RowHandle, gColId) != null ? (int)view.GetRowCellValue(e.RowHandle, gColId) : -1;

            e.IsEmpty = !forms.Any(r => r.IdBase == idBase);
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);

            e.ChildList = forms.Where(r => r.IdBase == idBase).ToList();
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

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                itemGetFile.Tag = "gvData";

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemUpdateVer);
                e.Menu.Items.Add(itemGetFile);
                itemPauseNotify.BeginGroup = true;
                e.Menu.Items.Add(itemPauseNotify);
                e.Menu.Items.Add(itemViewHistory);
            }
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder204, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath);
            fView.ShowDialog();
        }

        private void gvForm_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAttForm));
            var att = dm_AttachmentBUS.Instance.GetItemById(idAtt);

            string filePath = att.EncryptionName;
            string fileName = att.ActualName;

            string sourcePath = Path.Combine(TPConfigs.Folder204, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_VIewFile fView = new f00_VIewFile(destPath);
            fView.ShowDialog();
        }

        private void gvForm_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                itemGetFile.Tag = "gvForm";
                e.Menu.Items.Add(itemGetFile);
            }
        }
    }
}
