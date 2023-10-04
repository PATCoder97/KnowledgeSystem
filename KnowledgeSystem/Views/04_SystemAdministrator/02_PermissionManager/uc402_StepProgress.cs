using DataAccessLayer;
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

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class uc402_StepProgress : DevExpress.XtraEditors.XtraUserControl
    {
        public uc402_StepProgress()
        {
            InitializeComponent();
        }

        #region parameters

        Font fontIndicator = new Font("Times New Roman", 12.0f, FontStyle.Italic);

        BindingSource sourceProgress = new BindingSource();

        List<dm_Progress> lsProgresses = new List<dm_Progress>();
        List<dm_StepProgress> lsStepProgresses = new List<dm_StepProgress>();
        List<dm_Group> lsGroups = new List<dm_Group>();

        #endregion

        #region methods

        private void LoadData()
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsProgresses = db.dm_Progress.ToList();
                lsStepProgresses = db.dm_StepProgress.ToList();
                lsGroups = db.dm_Group.ToList();

                sourceProgress.DataSource = lsProgresses;
            }

            gcProgress.RefreshDataSource();
        }

        bool cal(Int32 _Width, GridView _View)
        {
            _View.IndicatorWidth = _View.IndicatorWidth < _Width ? _Width : _View.IndicatorWidth;
            return true;
        }

        void IndicatorDraw(RowIndicatorCustomDrawEventArgs e)
        {
            e.Info.Appearance.Font = fontIndicator;
            e.Info.Appearance.ForeColor = Color.FromArgb(16, 110, 190);
        }

        #endregion

        private void uc207_StepProgress_Load(object sender, EventArgs e)
        {
            gvProgress.ReadOnlyGridView();
            gvStepProgress.ReadOnlyGridView();

            gvProgress.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gcProgress.DataSource = sourceProgress;

            LoadData();
        }

        private void btnAddProgress_Click(object sender, EventArgs e)
        {
            lsProgresses.Add(new dm_Progress() { DisplayName = "New" });

            gcProgress.RefreshDataSource();
        }

        private void gvProgress_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idProgressSelect = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, "Id"));

            var lsStepProgressSelect = (from data in lsStepProgresses.Where(r => r.IdProgress == idProgressSelect)
                                        join groups in lsGroups on data.IdGroup equals groups.Id
                                        select new dm_GroupProgressM
                                        {
                                            IndexStep = data.IndexStep,
                                            DisplayName = groups.DisplayName
                                        }).ToList();

            if (lsStepProgressSelect.Count != 0)
            {
                e.ChildList = lsStepProgressSelect;
            }
        }

        private void gvProgress_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvProgress_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "審查流程";
        }

        private void gvProgress_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            GridView view = sender as GridView;

            int idProgressSelect = Convert.ToInt16(view.GetRowCellValue(e.RowHandle, "Id"));

            List<dm_StepProgress> lsStepProgressSelect = lsStepProgresses.Where(r => r.IdProgress == idProgressSelect).ToList();

            if (lsStepProgresses.Any(r => r.IdProgress == idProgressSelect) == false) e.IsEmpty = true;
        }

        private void gvProgress_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (!gvProgress.IsGroupRow(e.RowHandle))
            {
                if (e.Info.IsRowIndicator)
                {
                    if (e.RowHandle < 0)
                    {
                        e.Info.ImageIndex = 0;
                        e.Info.DisplayText = string.Empty;
                    }
                    else
                    {
                        e.Info.ImageIndex = -1;
                        e.Info.DisplayText = (e.RowHandle + 1).ToString();
                    }

                    IndicatorDraw(e);

                    SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                    int width = (int)size.Width + 20;

                    BeginInvoke(new MethodInvoker(() => cal(width, gvProgress)));
                }
            }
            else
            {
                e.Info.ImageIndex = -1;
                e.Info.DisplayText = $"[{e.RowHandle * -1}]";

                IndicatorDraw(e);

                SizeF size = e.Graphics.MeasureString(e.Info.DisplayText, fontIndicator);
                int width = (int)size.Width + 20;

                BeginInvoke(new MethodInvoker(() => cal(width, gvProgress)));
            }
        }

        private void btnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc402_StepProgress_Info ucInfo = new uc402_StepProgress_Info();
            if (XtraDialog.Show(ucInfo, "新增審查流程", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            var lsStepProgressSelect = (from data in ucInfo.lsGroupProgress
                                        join groups in lsGroups on data.Id equals groups.Id
                                        select new dm_GroupProgressM
                                        {
                                            IndexStep = data.IndexStep,
                                            Id = data.Id,
                                            DisplayName = groups.DisplayName,
                                        }).ToList();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                int idProgress = lsProgresses.Count != 0 ? lsProgresses.Max(r => r.Id) + 1 : 1;
                string nameProgress = "「經辦人」⇒" + string.Join("⇒", lsStepProgressSelect.Select(r =>$"「{r.DisplayName}」" ));

                db.dm_Progress.Add(new dm_Progress() { Id = idProgress, DisplayName = nameProgress });
                db.SaveChanges();

                foreach (var item in lsStepProgressSelect)
                {
                    db.dm_StepProgress.Add(new dm_StepProgress() { IdProgress = idProgress, IndexStep = item.IndexStep, IdGroup = item.Id });
                }
                db.SaveChanges();
            }

            LoadData();
        }
    }
}
