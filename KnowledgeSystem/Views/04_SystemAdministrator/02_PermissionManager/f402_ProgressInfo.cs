using BusinessLayer;
using DataAccessLayer;
using DevExpress.Charts.Native;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._02_PermissionManager
{
    public partial class f402_ProgressInfo : DevExpress.XtraEditors.XtraForm
    {
        public f402_ProgressInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public EventFormInfo _eventInfo = EventFormInfo.Create;
        public int _idProgress = -1;
        private dm_Progress _progress;

        BindingSource _sourceStep = new BindingSource();

        List<dm_Departments> lsDepts;
        List<dm_StepProgress> lsSteps = new List<dm_StepProgress>();
        List<dm_Group> lsGroups;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
        }

        private void LockControl()
        {
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            cbbDept.Enabled = false;
            gvStep.OptionsBehavior.ReadOnly = false;
            gColRemove.Visible = true;
            gvStep.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;
            txbPrioritize.Enabled = false;
            txbPrioritize.EditValue = 1;

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    cbbDept.Enabled = true;
                    txbPrioritize.Enabled = true;
                    break;
                case EventFormInfo.View:
                    btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    gvStep.OptionsBehavior.ReadOnly = true;
                    gColRemove.Visible = false;
                    gvStep.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
                    break;
                case EventFormInfo.Update:
                    btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    txbPrioritize.Enabled = true;
                    break;
            }
        }

        private void f402_ProgressInfo_Load(object sender, EventArgs e)
        {
            _sourceStep.DataSource = lsSteps;
            gcStep.DataSource = _sourceStep;

            lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id} {r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            lsGroups = dm_GroupBUS.Instance.GetList();
            cbbGroup.DataSource = lsGroups;
            cbbGroup.ValueMember = "Id";
            cbbGroup.DisplayMember = "DisplayName";
            cbbGroup.Columns.AddRange(new[] { new LookUpColumnInfo { FieldName = "DisplayName", Caption = "名稱" } });

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    cbbDept.EditValue = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
                    cbbDept.EditValue = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
                    break;
                case EventFormInfo.View:
                    _progress = dm_ProgressBUS.Instance.GetItemById(_idProgress);
                    cbbDept.EditValue = _progress.IdDept;
                    cbbDept.EditValue = _progress.IdDept;

                    lsSteps = dm_StepProgressBUS.Instance.GetListByIdProgress(_idProgress);
                    _sourceStep.DataSource = lsSteps;

                    txbPrioritize.EditValue = _progress.Prioritize;
                    break;
            }

            LockControl();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (lsSteps.Count == 0) return;

            int index = 0;
            lsSteps.OrderBy(r => r.IndexStep).ForEach(item => { item.IndexStep = ++index; });

            var lsStepSelect = (from steps in lsSteps
                                join groups in lsGroups on steps.IdGroup equals groups.Id
                                select new
                                {
                                    steps,
                                    groups
                                }).ToList();

            _idProgress = _idProgress == -1 ? dm_ProgressBUS.Instance.GetMaxId() + 1 : _progress.Id;
            string _nameProgress = "「經辦人」⇒" + string.Join("⇒", lsStepSelect.Select(r => $"「{r.groups.DisplayName}」"));

            switch (_eventInfo)
            {
                case EventFormInfo.Create:
                    _progress = new dm_Progress()
                    {
                        Id = _idProgress,
                        DisplayName = _nameProgress,
                        IdDept = cbbDept.EditValue?.ToString(),
                        Prioritize = Convert.ToInt16(txbPrioritize.EditValue)
                    };
                    dm_ProgressBUS.Instance.Add(_progress);
                    break;
                case EventFormInfo.View:
                    break;
                case EventFormInfo.Update:
                    _progress.DisplayName = _nameProgress;
                    _progress.Prioritize = Convert.ToInt16(txbPrioritize.EditValue);

                    dm_ProgressBUS.Instance.AddOrUpdate(_progress);
                    dm_StepProgressBUS.Instance.RemoveByIdProgress(_idProgress);
                    break;
            }

            foreach (var item in lsStepSelect)
            {
                dm_StepProgressBUS.Instance.Add(new dm_StepProgress()
                {
                    IdProgress = _idProgress,
                    IndexStep = item.steps.IndexStep,
                    IdGroup = item.steps.IdGroup
                });
            }

            Close();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _eventInfo = EventFormInfo.Update;
            LockControl();
        }

        private void gvStep_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            GridView view = sender as GridView;
            view.SetRowCellValue(e.RowHandle, "IndexStep", lsSteps.Count());
        }

        private void btnRemoveStep_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            GridView view = gvStep;
            dm_StepProgress steps = view.GetRow(view.FocusedRowHandle) as dm_StepProgress;
            DialogResult dialogResult = XtraMessageBox.Show($"您想要刪除步驟「{steps.IndexStep}」?", TPConfigs.SoftNameTW, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            lsSteps.Remove(steps);
            int index = 0;
            lsSteps.ForEach(item => { item.IndexStep = ++index; });

            int rowIndex = view.FocusedRowHandle;
            view.RefreshData();
            view.FocusedRowHandle = rowIndex;
        }
    }
}