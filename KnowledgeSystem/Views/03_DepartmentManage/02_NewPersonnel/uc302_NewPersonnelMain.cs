using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._02_NewPersonnel
{
    public partial class uc302_NewPersonnelMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc302_NewPersonnelMain()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> lsUser = new List<dm_User>();
        List<dm_JobTitle> lsJobs;

        List<dt302_ReportInfo> reportsInfo;
        List<dt302_ReportAttach> reportAttaches;
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;
        DXMenuItem itemAddPlanFile;
        DXMenuItem itemCloseReport;
        DXMenuItem itemAddAttach;
        DXMenuItem itemDelAttach;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = new DXMenuItem("顯示信息", ItemViewInfo_Click, TPSvgimages.Info, DXMenuItemPriority.Normal);
            itemViewFile = new DXMenuItem("讀取檔案", ItemViewFile_Click, TPSvgimages.View, DXMenuItemPriority.Normal);
            itemAddPlanFile = new DXMenuItem("上傳計劃表", ItemAddPlanFile_Click, TPSvgimages.UpLevel, DXMenuItemPriority.Normal);
            itemAddAttach = new DXMenuItem("上傳報告", ItemAddAttach_Click, TPSvgimages.UploadFile, DXMenuItemPriority.Normal);
            itemCloseReport = new DXMenuItem("結案", ItemCloseReport_Click, TPSvgimages.Confirm, DXMenuItemPriority.Normal);
            itemDelAttach = new DXMenuItem("刪除附件", ItemDelAttach_Click, TPSvgimages.Remove, DXMenuItemPriority.Normal);

            itemViewInfo.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewInfo.AppearanceHovered.ForeColor = Color.Blue;

            itemViewFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemViewFile.AppearanceHovered.ForeColor = Color.Blue;

            itemAddPlanFile.ImageOptions.SvgImageSize = new Size(24, 24);
            itemAddPlanFile.AppearanceHovered.ForeColor = Color.Blue;

            itemCloseReport.ImageOptions.SvgImageSize = new Size(24, 24);
            itemCloseReport.AppearanceHovered.ForeColor = Color.Blue;

            itemAddAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            itemAddAttach.AppearanceHovered.ForeColor = Color.Blue;

            itemDelAttach.ImageOptions.SvgImageSize = new Size(24, 24);
            itemDelAttach.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemDelAttach_Click(object sender, EventArgs e)
        {
            GridView detailGridView = gcData.FocusedView as GridView;
            if (detailGridView != null)
            {
                int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
                if (detailFocusedRowHandle < 0) return;

                var idAttach = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdAttach));
                var attachName = detailGridView.GetRowCellValue(detailFocusedRowHandle, gColActualName);

                string msg = $"您確定要刪除附件：\r\n{attachName}";
                if (MsgTP.MsgYesNoQuestion(msg) != DialogResult.Yes) return;

                dt302_ReportAttachBUS.Instance.RemoveByIdAtt(idAttach);
                dm_AttachmentBUS.Instance.RemoveById(idAttach);

                LoadData();
            }
        }

        private void ItemCloseReport_Click(object sender, EventArgs e)
        {
            GridView detailGridView = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            if (detailGridView != null)
            {
                int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
                if (detailFocusedRowHandle < 0) return;

                var idReport = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdReport));
                dt302_ReportInfo report = dt302_ReportInfoBUS.Instance.GetItemById(idReport);

                report.UploadDate = DateTime.Now;

                dt302_ReportInfoBUS.Instance.AddOrUpdate(report);

                LoadData();
            }
        }

        private void ItemAddPlanFile_Click(object sender, EventArgs e)
        {
            string msg = "<font='Microsoft JhengHei UI' size=14>請上培養訓練計劃包含兩個檔案：\r\n  <color=red>Word檔案</color>：傳大學新進人員培養訓練計劃表\r\n  <color=red>PDF檔案</color>：OA部門內便簽</font>";
            MsgTP.MsgShowInfomation(msg);

            int idBase = Convert.ToInt16(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            DateTime enterDate = Convert.ToDateTime(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColEnterDate));
            var base302 = dt302_BaseBUS.Instance.GetItemById(idBase);

            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "培養訓練計劃(*.docx;*.pdf)|*.docx;*.pdf";

                if (dlg.ShowDialog() != DialogResult.OK) return;
                var fileNames = dlg.FileNames;

                // Kiểm tra xem đã chọn đúng 2 tệp tin hay không
                if (fileNames.Length != 2)
                {
                    msg = "<font='Microsoft JhengHei UI' size=14>請選擇正確的兩個檔案\r\nVui lòng chọn đúng 2 tệp tin</font>";
                    MsgTP.MsgShowInfomation(msg);
                    return;
                }

                // Kiểm tra xem có đúng một tệp DOCX và một tệp PDF hay không
                bool hasDocx = fileNames.Any(file => file.EndsWith(".docx", StringComparison.OrdinalIgnoreCase));
                bool hasPdf = fileNames.Any(file => file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

                if (!hasDocx || !hasPdf)
                {
                    MsgTP.MsgShowInfomation(msg);
                    return;
                }

                var fileDocx = fileNames.First(file => file.EndsWith(".docx", StringComparison.OrdinalIgnoreCase));
                var filePdf = fileNames.First(file => file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));
                DataTable data = WordHelper.Instance.ReadTableFromMSWord(fileDocx);

                var userName = data.Columns[2].ColumnName;

                var rowHeader = data.Rows[1];

                var indexColumns = rowHeader.ItemArray.Select((value, index) => new { Value = value, Index = index })
                                           .Where(x => x.Value.Equals("Nội dung công việc đào tạo訓練工作內容及目標"))
                                           .Select(x => x.Index).ToList();

                if (indexColumns.Count != 2)
                {
                    return;
                }

                Dictionary<DateTime, string> contents = new Dictionary<DateTime, string>();
                DateTime expectedDate = enterDate.AddDays(-1);
                for (int i = 0; i < 3; i++)
                {
                    expectedDate = expectedDate.AddMonths(1);
                    contents.Add(expectedDate, data.Rows[i + 2][indexColumns[0]].ToString());
                }

                for (int i = 0; i < 4; i++)
                {
                    expectedDate = expectedDate.AddMonths(3);
                    contents.Add(expectedDate, data.Rows[i + 2][indexColumns[1]].ToString());
                }

                List<string> mergedList = contents.Select(entry => $"    {entry.Key:yyyy/MM/dd} - {entry.Value}").ToList();
                var msgContents = string.Join("\r\n", mergedList);
                string msgValidCert = $"<font='Microsoft JhengHei UI' size=14>新進人員<color=blue>「{userName}」</color>有培養訓練計劃如下：</br>{msgContents}</font>";
                var resultDlg = MsgTP.MsgHtmlOKCancelQuestion(msgValidCert);

                // return;

                foreach (var item in contents)
                {
                    dt302_ReportInfo reportInfo = new dt302_ReportInfo()
                    {
                        IdBase = idBase,
                        Content = item.Value,
                        ExpectedDate = item.Key,
                    };

                    dt302_ReportInfoBUS.Instance.Add(reportInfo);
                }

                string nameFilePdf = Path.GetFileName(filePdf);
                dm_Attachment att = new dm_Attachment()
                {
                    Thread = "302",
                    EncryptionName = EncryptionHelper.EncryptionFileName(nameFilePdf),
                    ActualName = nameFilePdf
                };
                var idAtt = dm_AttachmentBUS.Instance.Add(att);

                base302.TrainingPlan = idAtt;
                dt302_BaseBUS.Instance.AddOrUpdate(base302);

                File.Copy(filePdf, Path.Combine(TPConfigs.Folder302, att.EncryptionName), true);
            }

            LoadData();
        }

        private void ItemAddAttach_Click(object sender, EventArgs e)
        {
            GridView detailGridView = gvData.GetDetailView(gvData.FocusedRowHandle, 0) as GridView;
            if (detailGridView != null)
            {
                int detailFocusedRowHandle = detailGridView.FocusedRowHandle;
                if (detailFocusedRowHandle < 0) return;

                int idReport = Convert.ToInt16(detailGridView.GetRowCellValue(detailFocusedRowHandle, gColIdReport));

                using (OpenFileDialog openFile = new OpenFileDialog())
                {
                    openFile.Filter = "pdf|*pdf";
                    openFile.Multiselect = true;

                    if (openFile.ShowDialog() != DialogResult.OK) return;

                    using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
                    {
                        foreach (var item in openFile.FileNames)
                        {
                            string filename = Path.GetFileName(item);

                            dm_Attachment att = new dm_Attachment()
                            {
                                Thread = "302",
                                EncryptionName = EncryptionHelper.EncryptionFileName(filename),
                                ActualName = filename
                            };
                            var idAtt = dm_AttachmentBUS.Instance.Add(att);

                            dt302_ReportAttach reportAttach = new dt302_ReportAttach()
                            {
                                IdReport = idReport,
                                IdAttach = idAtt
                            };

                            dt302_ReportAttachBUS.Instance.Add(reportAttach);

                            File.Copy(item, Path.Combine(TPConfigs.Folder302, att.EncryptionName), true);
                        }
                    }
                }

                LoadData();
            }
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            int idBase = Convert.ToInt16(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            var base302 = dt302_BaseBUS.Instance.GetItemById(idBase);

            var attachment = dm_AttachmentBUS.Instance.GetItemById(base302.TrainingPlan ?? -1);

            if (attachment == null)
            {
                return;
            }

            string source = Path.Combine(TPConfigs.Folder302, attachment.EncryptionName);
            string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {attachment.ActualName}");
            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(source, dest, true);

            f00_VIewFile viewFile = new f00_VIewFile(dest);
            viewFile.ShowDialog();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            //GridView view = sender as GridView;
            int idBase = Convert.ToInt16(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId));
            //if (_base == null) return;

            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "證照";
            fInfo.idBase302 = idBase;
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var lsBases = dt302_BaseBUS.Instance.GetList();
                lsUser = dm_UserBUS.Instance.GetList();
                lsJobs = dm_JobTitleBUS.Instance.GetList();
                reportsInfo = dt302_ReportInfoBUS.Instance.GetList();
                attachments = dm_AttachmentBUS.Instance.GetListByThread("302");

                var lsBasesDisplay = (from data in lsBases
                                      join urs in lsUser on data.IdUser equals urs.Id
                                      join supvr in lsUser on data.Supervisor equals supvr.Id
                                      join job in lsJobs on urs.JobCode equals job.Id
                                      where urs.IdDepartment.StartsWith(idDept2word)
                                      select new
                                      {
                                          Id = data.Id,
                                          IdDept = urs.IdDepartment,
                                          IdUser = data.IdUser,
                                          IdJobTitle = urs.JobCode,
                                          EnterDate = urs.DateCreate,
                                          Describe = data.Describe,
                                          UserName = $"{urs.DisplayName} {urs.DisplayNameVN}",
                                          JobName = $"{job.Id} {job.DisplayName}",
                                          Supervisor = $"{supvr.DisplayName} {supvr.DisplayNameVN}",
                                          School = data.School,
                                          Major = data.Major
                                      }).ToList();

                sourceBases.DataSource = lsBasesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
                gvData.ExpandMasterRow(gvData.FocusedRowHandle);
            }
        }

        private void uc302_NewPersonnelMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvReport.ReadOnlyGridView();
            gvReport.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvAttachment.ReadOnlyGridView();

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;

            // gvData.OptionsDetail.DetailMode = DetailMode.Embedded;
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f302_NewUserInfo fInfo = new f302_NewUserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "大學新進人員";
            fInfo.ShowDialog();

            LoadData();
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));
            var reports = reportsInfo.Where(r => r.IdBase == idBase).Select(r => r.Id).ToList();
            reportAttaches = dt302_ReportAttachBUS.Instance.GetListByListReport(reports);

            e.IsEmpty = reports.Count == 0;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColId));

            int index = 1;
            e.ChildList = reportsInfo.Where(r => r.IdBase == idBase).Select(r => new
            {
                Index = $"第{index++}次",
                r.Id,
                r.Content,
                r.ExpectedDate,
                r.UploadDate
            }).ToList();
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "報告進度";
        }

        private void gridView_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        // Master-Detail : gvReport
        private void gvReport_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;
            int idReport = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColIdReport));
            e.IsEmpty = !reportAttaches.Any(r => r.IdReport == idReport);
        }

        private void gvReport_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idReport = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, gColIdReport));

            var reportsAtt = dt302_ReportAttachBUS.Instance.GetListByReport(idReport);

            e.ChildList = (from data in reportsAtt
                           join att in attachments on data.IdAttach equals att.Id
                           select new
                           {
                               Id = att.Id,
                               EncryptionName = att.EncryptionName,
                               ActualName = att.ActualName
                           }).ToList();

        }

        private void gvReport_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvReport_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "附件";
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                int idBase = Convert.ToInt16(view.GetRowCellValue(e.HitInfo.RowHandle, gColId));
                bool HaveReport = reportsInfo.Any(r => r.IdBase == idBase);

                e.Menu.Items.Add(itemViewInfo);

                if (HaveReport)
                {
                    e.Menu.Items.Add(itemViewFile);
                }
                else
                {
                    e.Menu.Items.Add(itemAddPlanFile);
                }
            }
        }

        private void gvReport_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                int idReport = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdReport));
                var closeRpDate = view.GetRowCellValue(view.FocusedRowHandle, gColCloseRp);

                var atts = dt302_ReportAttachBUS.Instance.GetListByReport(idReport);

                if (closeRpDate == null)
                {
                    e.Menu.Items.Add(itemAddAttach);
                    if (atts.Count != 0) e.Menu.Items.Add(itemCloseReport);
                }
            }
        }

        private void gvAttachment_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                GridView masterView = view.ParentView as GridView;

                int idReport = Convert.ToInt16(masterView.GetRowCellValue(masterView.FocusedRowHandle, gColIdReport));
                var closeRpDate = masterView.GetRowCellValue(masterView.FocusedRowHandle, gColCloseRp);

                //var atts = dt302_ReportAttachBUS.Instance.GetListByReport(idReport);

                if (closeRpDate == null)
                {
                    e.Menu.Items.Add(itemDelAttach);
                    //    if (atts.Count != 0) e.Menu.Items.Add(itemCloseReport);
                }
            }
        }

        private void gridView_ExpandMasterRow(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            view.ExpandMasterRow(view.FocusedRowHandle);
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

        private void gvAttachment_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            string actualName = view.GetRowCellValue(view.FocusedRowHandle, gColActualName).ToString();
            string encryptName = view.GetRowCellValue(view.FocusedRowHandle, gColEncryptName).ToString();

            string source = Path.Combine(TPConfigs.Folder302, encryptName);
            string dest = Path.Combine(TPConfigs.TempFolderData, $"{DateTime.Now:yyMMddhhmmss} {actualName}");
            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(source, dest, true);

            f00_VIewFile viewFile = new f00_VIewFile(dest);
            viewFile.ShowDialog();
        }
    }
}
