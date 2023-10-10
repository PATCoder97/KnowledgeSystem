using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_StepProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_StepProgress()
        {
            InitializeComponent();
            InitializeIcon();
        }

        #region parameters

        dm_ProgressBUS _dm_ProgressBUS = new dm_ProgressBUS();
        dm_GroupBUS _dm_GroupBUS = new dm_GroupBUS();

        BindingSource sourceProgress = new BindingSource();

        List<dm_Progress> lsProgresses;
        List<dm_Group> lsGroups;
        List<dm_Departments> lsDepts;

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnNew.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
        }

        private void LoadData()
        {
            lsProgresses = _dm_ProgressBUS.GetList();
            lsGroups = _dm_GroupBUS.GetList();
            lsDepts = dm_DeptBUS.Instance.GetList();

            sourceProgress.DataSource = (from data in lsProgresses
                                         join dept in lsDepts on data.IdDept equals dept.Id
                                         select new dm_Progress
                                         {
                                             Id = data.Id,
                                             DisplayName = data.DisplayName,
                                             IdDept = $"{dept.Id} {dept.DisplayName}",
                                         }).ToList();

            gcData.RefreshDataSource();
            gvData.BestFitColumns();
        }

        #endregion

        private void uc207_StepProgress_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceProgress;
            LoadData();
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f402_ProgressInfo fInfo = new f402_ProgressInfo();
            fInfo.ShowDialog();

            //uc402_StepProgress_Info ucInfo = new uc402_StepProgress_Info();
            //if (XtraDialog.Show(ucInfo, "新增審查流程", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            //var lsStepProgressSelect = (from data in ucInfo.lsGroupProgress
            //                            join groups in lsGroups on data.Id equals groups.Id
            //                            select new dm_GroupProgressM
            //                            {
            //                                IndexStep = data.IndexStep,
            //                                Id = data.Id,
            //                                DisplayName = groups.DisplayName,
            //                            }).ToList();

            //using (var db = new DBDocumentManagementSystemEntities())
            //{
            //    int idProgress = lsProgresses.Count != 0 ? lsProgresses.Max(r => r.Id) + 1 : 1;
            //    string nameProgress = "「經辦人」⇒" + string.Join("⇒", lsStepProgressSelect.Select(r => $"「{r.DisplayName}」"));
            //    string idDept = ucInfo._idDept;

            //    db.dm_Progress.Add(new dm_Progress() { Id = idProgress, DisplayName = nameProgress, IdDept = idDept });
            //    db.SaveChanges();

            //    foreach (var item in lsStepProgressSelect)
            //    {
            //        db.dm_StepProgress.Add(new dm_StepProgress() { IdProgress = idProgress, IndexStep = item.IndexStep, IdGroup = item.Id });
            //    }
            //    db.SaveChanges();
            //}

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            f402_ProgressInfo fInfo = new f402_ProgressInfo();
            fInfo._eventInfo = EventFormInfo.View;
            fInfo.ShowDialog();
        }
    }
}
