using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._06_InternationalStd
{
    public partial class uc206_InternationalStdMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc206_InternationalStdMain()
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
        bool isManager206 = false;

        List<dm_User> users = new List<dm_User>();

        List<dt206_Documents> dt206Bases;
        List<dt206_DocVersions> docOldVersion;

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdateVer;
        DXMenuItem itemGetFile;
        DXMenuItem itemPauseNotify;
        DXMenuItem itemViewHistory;

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemUpdateVer = CreateMenuItem("更新版本", ItemUpdateVer_Click, TPSvgimages.UpLevel);
            //itemGetFile = CreateMenuItem("下載檔案", ItemGetFile_Click, TPSvgimages.Attach);
            //itemPauseNotify = CreateMenuItem("暫停通知", ItemPauseNotify_Click, TPSvgimages.Suspension);
            //itemViewHistory = CreateMenuItem("版本歷史", ItemViewHistory_Click, TPSvgimages.Progress);
        }

        private void ItemUpdateVer_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f206_UpdateDocVersion fUpdate = new f206_UpdateDocVersion()
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

            f206_Doc_Info fInfo = new f206_Doc_Info()
            {
                eventInfo = EventFormInfo.View,
                idBase = idBase,
                formName = "規範"
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

                //var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

                //var depts = dm_DeptBUS.Instance.GetList();
                //var groups = dm_GroupBUS.Instance.GetListByName("ISO組");

                //var deptAccess = (from data in groups
                //                  join grp in grpUsrs on data.Id equals grp.IdGroup
                //                  select data.IdDept).ToList();


                //// Lấy các văn kiện khác 三階, hoặc của tổ mình đưa lên theo quyền trong nhóm ISO
                //dt206Bases = (from data in dt206Bases
                //              where data.DocLevel != "三階" || deptAccess.Contains(data.IdDept)
                //              select data).ToList();

                dt206Bases = dt206_DocumentsBUS.Instance.GetList();
                docOldVersion = dt206_DocVersionsBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                var docCatoraries = dt206_DocCategoriesBUS.Instance.GetList();

                var basesDisplay = (from data in dt206Bases
                                    join docCato in docCatoraries on data.IdCategory equals docCato.Id
                                    join urs in users on data.CreateBy equals urs.Id
                                    let DocCatorary = docCato.DisplayName
                                    let UserUpload = $"LG{urs.IdDepartment}/{urs.DisplayName}"
                                    select new
                                    {
                                        data,
                                        DocCatorary,
                                        UserUpload
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        private void uc206_InternationalStdMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvForm.ReadOnlyGridView();
            gvForm.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var departments = dm_DeptBUS.Instance.GetList();
            var manager206grps = dm_GroupBUS.Instance.GetListByName("國際規範");
            isManager206 = manager206grps.Any(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id));

            if (!isManager206)
            {
                btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f206_Doc_Info fInfo = new f206_Doc_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "規範"
            };
            fInfo.ShowDialog();

            LoadData();
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

            string sourcePath = Path.Combine(TPConfigs.Folder206, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            var mainForm = f00_ViewMultiFile.Instance;
            if (!mainForm.Visible)
                mainForm.Show();

            mainForm.OpenFormInDocumentManager(destPath);
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

            e.IsEmpty = !docOldVersion.Any(r => r.IdDocument == idBase);
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);

            e.ChildList = docOldVersion.Where(r => r.IdDocument == idBase).ToList();
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
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && isManager206)
            {
                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemUpdateVer);
            }
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

            string sourcePath = Path.Combine(TPConfigs.Folder206, filePath);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"{Regex.Replace(fileName, @"[\\/:*?""<>|]", "")}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileName)}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            var mainForm = f00_ViewMultiFile.Instance;
            if (!mainForm.Visible)
                mainForm.Show();

            mainForm.OpenFormInDocumentManager(destPath);
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

            string filePath = Path.Combine(documentsPath, $"國際規範 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
