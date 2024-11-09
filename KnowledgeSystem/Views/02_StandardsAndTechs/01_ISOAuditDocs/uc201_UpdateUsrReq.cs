using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._03_Extension;
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
    public partial class uc201_UpdateUsrReq : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_UpdateUsrReq()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<dt201_UpdateUsrReq> baseReq;
        List<dm_User> users;
        List<dm_Departments> depts;

        BindingSource sourceForm = new BindingSource();

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnManualChange.ImageOptions.SvgImage = TPSvgimages.Add;
        }

        private void LoadData()
        {
            baseReq = dt201_UpdateUsrReqBUS.Instance.GetList();
            users = dm_UserBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetList();

            // Check quyền truy cập của tổ nào theo quyền của nhóm ISO đó
            var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var groups = dm_GroupBUS.Instance.GetListByName("ISO組");
            var idDeptGroups = (from data in groups
                                join grp in grpUsrs on data.Id equals grp.IdGroup
                                select data.IdDept).ToList();

            var dataInfo = (from data in baseReq.Where(r => idDeptGroups.Contains(r.IdDept))
                            join usr in users on data.IdUsr equals usr.Id
                            join dept in depts on data.IdDept equals dept.Id
                            select new
                            {
                                data,
                                usr,
                                dept
                            }).ToList();

            sourceForm.DataSource = dataInfo;

            gvData.BestFitColumns();
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void uc201_UpdateUsrReq_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceForm;
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idReq = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f201_UpdateUsrReq_Detail fDetail = new f201_UpdateUsrReq_Detail(idReq);
            fDetail.ShowDialog();
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

            string filePath = Path.Combine(documentsPath, $"人員異動 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void btnManualChange_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc201_UpdateUsrReq_Manual ucInfo = new uc201_UpdateUsrReq_Manual();
            if (XtraDialog.Show(ucInfo, "手動創建", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            string userId = ucInfo.UserId;
            string desscription = ucInfo.Description;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(desscription))
            {
                XtraMessageBox.Show("請填寫所以訊息！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var usr = dm_UserBUS.Instance.GetItemById(userId);

            if (XtraMessageBox.Show($"是否需要創建「{usr.DisplayName}」到ISO 17025「{(desscription == "新增" ? "新進" : "離職")}人員」之關係表單？", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            dt201_UpdateUsrReq updateReq = new dt201_UpdateUsrReq()
            {
                IdUsr = userId,
                IdDept = usr.IdDepartment,
                DateCreate = DateTime.Now,
                TypeChange = desscription == "新增" ? "新進" : "離職",
                Describe = $"手動創建ISO 17025「{(desscription == "新增" ? "新進" : "離職")}人員」之關係表單"
            };
            dt201_UpdateUsrReqBUS.Instance.Add(updateReq);

            LoadData();
        }
    }
}
