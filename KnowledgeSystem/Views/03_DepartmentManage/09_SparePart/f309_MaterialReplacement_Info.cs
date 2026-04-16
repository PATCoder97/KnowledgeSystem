using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_MaterialReplacement_Info : XtraForm
    {
        private readonly dt309_Materials sourceMaterial;
        private readonly List<dt309_Materials> candidateMaterials;

        public int? SelectedReplacementMaterialId { get; private set; }
        public DateTime? SelectedReplacementDate { get; private set; }

        public f309_MaterialReplacement_Info(dt309_Materials sourceMaterial, List<dt309_Materials> candidateMaterials)
        {
            this.sourceMaterial = sourceMaterial ?? throw new ArgumentNullException(nameof(sourceMaterial));
            this.candidateMaterials = candidateMaterials ?? new List<dt309_Materials>();

            InitializeComponent();
            InitializeIcon();
            ConfigureReplacementLookup();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
        }

        private void ConfigureReplacementLookup()
        {
            gvReplacement.Columns.Clear();
            gvReplacement.OptionsBehavior.Editable = false;
            gvReplacement.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvReplacement.OptionsView.ColumnAutoWidth = false;
            gvReplacement.OptionsView.EnableAppearanceOddRow = true;
            gvReplacement.OptionsView.ShowAutoFilterRow = true;
            gvReplacement.OptionsView.ShowGroupPanel = false;
            gvReplacement.FocusRectStyle = DrawFocusRectStyle.RowFocus;

            gvReplacement.Columns.AddVisible(nameof(dt309_Materials.Code), "\u7269\u6599\u7de8\u865f").Width = 130;
            gvReplacement.Columns.AddVisible(nameof(dt309_Materials.DisplayName), "\u54c1\u540d\u898f\u683c").Width = 280;
            gvReplacement.Columns.AddVisible(nameof(dt309_Materials.Location), "\u6599\u4f4d").Width = 110;
            gvReplacement.Columns.AddVisible(nameof(dt309_Materials.TypeUse), "\u7528\u9014").Width = 110;
        }

        private void LoadData()
        {
            txtSourceCode.EditValue = sourceMaterial.Code ?? string.Empty;
            txtSourceName.EditValue = sourceMaterial.DisplayName ?? string.Empty;

            sleReplacement.Properties.DataSource = candidateMaterials
                .Select(r => new
                {
                    r.Id,
                    r.Code,
                    r.DisplayName,
                    r.Location,
                    r.TypeUse
                })
                .ToList();

            if (sourceMaterial.ReplacementMaterialId != null)
            {
                sleReplacement.EditValue = sourceMaterial.ReplacementMaterialId;
            }

            deReplacementDate.EditValue = sourceMaterial.ReplacementDate?.Date ?? DateTime.Today;
        }

        private bool ValidateInput()
        {
            if (sleReplacement.EditValue == null)
            {
                MsgTP.MsgError("\u8acb\u9078\u64c7\u66ff\u4ee3\u6599\u3002");
                return false;
            }

            if (deReplacementDate.EditValue == null)
            {
                MsgTP.MsgError("\u8acb\u9078\u64c7\u66ff\u4ee3\u65e5\u671f\u3002");
                return false;
            }

            return true;
        }

        private void f309_MaterialReplacement_Info_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            SelectedReplacementMaterialId = Convert.ToInt32(sleReplacement.EditValue);
            SelectedReplacementDate = Convert.ToDateTime(deReplacementDate.EditValue).Date;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
