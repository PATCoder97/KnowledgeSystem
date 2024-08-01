using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_UpdateUsrReq_Detail : DevExpress.XtraEditors.XtraForm
    {
        public f201_UpdateUsrReq_Detail(int idUpdateReq)
        {
            IDUpdateReq = idUpdateReq;
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        int IDUpdateReq = 0;

        BindingSource sourceForm = new BindingSource();

        List<dt201_UpdateUsrReq_Detail> baseReqDetail;
        List<dt201_ReqUpdateDocs> reqUpdateDocs;
        List<dm_User> users;

        DXMenuItem itemCompleteDoc;

        private void InitializeMenuItems()
        {
            itemCompleteDoc = CreateMenuItem("項目已完成", ItemCompleteDoc_Click, TPSvgimages.Confirm);
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

        private void ItemCompleteDoc_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("請上傳文件確認完成！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);

            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() != DialogResult.OK) return;

            string filePath = openFile.FileName;
            string fileName = Path.GetFileName(filePath);

            GridView view = gvData;
            int idDetail = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var detail = dt201_UpdateUsrReq_DetailBUS.Instance.GetItemById(idDetail);
            detail.CompleteDate = DateTime.Now;
            detail.UsrComplete = TPConfigs.LoginUser.Id;
            detail.AttActualName = fileName;
            detail.AttEncryptName = EncryptionHelper.EncryptionFileName(fileName);

            dt201_UpdateUsrReq_DetailBUS.Instance.AddOrUpdate(detail);

            string folderDest = TPConfigs.Folder201EmpChange;
            if (!Directory.Exists(folderDest)) Directory.CreateDirectory(folderDest);

            File.Copy(filePath, Path.Combine(folderDest, detail.AttEncryptName), true);

            LoadData();
        }

        private void LoadData()
        {
            reqUpdateDocs = dt201_ReqUpdateDocsBUS.Instance.GetList();
            baseReqDetail = dt201_UpdateUsrReq_DetailBUS.Instance.GetListByIdUpdateReq(IDUpdateReq);
            users = dm_UserBUS.Instance.GetList();

            var dataInfo = (from data in baseReqDetail
                            join doc in reqUpdateDocs on data.IdReq equals doc.Id
                            join usr in users on data.UsrComplete equals usr.Id into dtg
                            from g in dtg.DefaultIfEmpty()
                            select new
                            {
                                data,
                                doc,
                                UserComplete = g != null ? g.DisplayName : ""
                            }).ToList();

            sourceForm.DataSource = dataInfo;

            gvData.BestFitColumns();
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void f201_UpdateUsrReq_Detail_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceForm;
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isComplete = string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "data.CompleteDate")?.ToString());

                if (isComplete)
                {
                    e.Menu.Items.Add(itemCompleteDoc);
                }
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"人員異動 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}