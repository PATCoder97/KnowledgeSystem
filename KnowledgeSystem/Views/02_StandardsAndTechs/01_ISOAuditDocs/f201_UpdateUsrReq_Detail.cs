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
using System.Security.Cryptography;
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
        DXMenuItem itemConfirmDoc;
        DXMenuItem itemRejectDoc;

        bool IsHasPermission = false;

        private void InitializeMenuItems()
        {
            itemCompleteDoc = CreateMenuItem("項目已完成", ItemCompleteDoc_Click, TPSvgimages.EmailSend);
            itemConfirmDoc = CreateMenuItem("確認已完成", ItemConfirmDoc_Click, TPSvgimages.Confirm);
            itemRejectDoc = CreateMenuItem("退回", ItemRejectDoc_Click, TPSvgimages.Cancel);
        }

        private void ItemRejectDoc_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("您確定退回這項目嗎?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            GridView view = gvData;
            int idDetail = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var detail = dt201_UpdateUsrReq_DetailBUS.Instance.GetItemById(idDetail);
            detail.CompleteDate = null;
            detail.UsrComplete = null;

            dt201_UpdateUsrReq_DetailBUS.Instance.AddOrUpdate(detail);

            LoadData();
        }

        private void ItemConfirmDoc_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("您確定結案這項目嗎?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            GridView view = gvData;
            int idDetail = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var detail = dt201_UpdateUsrReq_DetailBUS.Instance.GetItemById(idDetail);
            detail.UsrConfirm = TPConfigs.LoginUser.Id;

            dt201_UpdateUsrReq_DetailBUS.Instance.AddOrUpdate(detail);

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

        private void ItemCompleteDoc_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("您確定已完成文件上傳嗎?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            GridView view = gvData;
            int idDetail = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            var detail = dt201_UpdateUsrReq_DetailBUS.Instance.GetItemById(idDetail);
            detail.CompleteDate = DateTime.Now;
            detail.UsrComplete = TPConfigs.LoginUser.Id;

            dt201_UpdateUsrReq_DetailBUS.Instance.AddOrUpdate(detail);

            LoadData();
        }

        private void LoadData()
        {
            reqUpdateDocs = dt201_ReqUpdateDocsBUS.Instance.GetList();
            baseReqDetail = dt201_UpdateUsrReq_DetailBUS.Instance.GetListByIdUpdateReq(IDUpdateReq);
            users = dm_UserBUS.Instance.GetList();
            var records = dt201_RecordCodeBUS.Instance.GetList();

            var dataInfo = (from data in baseReqDetail
                            join doc in reqUpdateDocs on data.IdReq equals doc.Id
                            let UserComplete = users.FirstOrDefault(r => r.Id == data.UsrComplete)
                            let UserConfirm = users.FirstOrDefault(r => r.Id == data.UsrConfirm)
                            let record = records.FirstOrDefault(r => r.Id == doc.IdRecordCode)
                            select new
                            {
                                data,
                                doc,
                                record,
                                UsrComplete = UserComplete != null && UserComplete.Id.Length > 5
                                    ? $"{UserComplete.Id.Substring(5)} LG{UserComplete.IdDepartment}/{UserComplete.DisplayName}" : "",
                                UsrConfirm = UserConfirm != null && UserConfirm.Id.Length > 5
                                    ? $"{UserConfirm.Id.Substring(5)} LG{UserConfirm.IdDepartment}/{UserConfirm.DisplayName}" : ""
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

            var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).Select(r => r.IdGroup).ToList();
            var groups = dm_GroupBUS.Instance.GetListByName($"二級{TPConfigs.LoginUser.IdDepartment}").Select(r => r.Id).ToList();

            IsHasPermission = groups.Any(r => grpUsrs.Contains(r));
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.Column.OptionsColumn.AllowMerge == DevExpress.Utils.DefaultBoolean.False)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                bool isComplete = string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "data.CompleteDate")?.ToString());
                bool isConfirm = string.IsNullOrEmpty(view.GetRowCellValue(view.FocusedRowHandle, "UsrConfirm")?.ToString());

                if (isComplete)
                {
                    e.Menu.Items.Add(itemCompleteDoc);
                }
                else if (isConfirm && IsHasPermission)
                {
                    e.Menu.Items.Add(itemConfirmDoc);
                    e.Menu.Items.Add(itemRejectDoc);
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