using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_MaterialReplacementChain : XtraForm
    {
        private readonly dt309_Materials sourceMaterial;
        private readonly List<MaterialReplacementChainNode> chainNodes;

        public f309_MaterialReplacementChain(dt309_Materials sourceMaterial, List<MaterialReplacementChainNode> chainNodes)
        {
            this.sourceMaterial = sourceMaterial ?? throw new ArgumentNullException(nameof(sourceMaterial));
            this.chainNodes = chainNodes ?? new List<MaterialReplacementChainNode>();

            InitializeComponent();
            InitializeIcon();
            ConfigureGrid();
        }

        private void InitializeIcon()
        {
            btnClose.ImageOptions.SvgImage = TPSvgimages.Close;
        }

        private void ConfigureGrid()
        {
            gvData.Columns.Clear();
            gvData.ReadOnlyGridView();
            gvData.OptionsBehavior.Editable = false;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowGroupPanel = false;

            gvData.Columns.AddVisible(nameof(MaterialReplacementChainNode.StepNo), "\u9806\u5e8f").Width = 80;
            gvData.Columns.AddVisible(nameof(MaterialReplacementChainNode.Code), "\u7269\u6599\u7de8\u865f").Width = 140;
            gvData.Columns.AddVisible(nameof(MaterialReplacementChainNode.DisplayName), "\u54c1\u540d\u898f\u683c").Width = 320;

            var colReplacementDate = gvData.Columns.AddVisible(nameof(MaterialReplacementChainNode.ReplacementDate), "\u66ff\u4ee3\u65e5\u671f");
            colReplacementDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colReplacementDate.DisplayFormat.FormatString = "yyyy/MM/dd";
            colReplacementDate.Width = 120;

            gvData.Columns.AddVisible(nameof(MaterialReplacementChainNode.Status), "\u72c0\u614b").Width = 90;
        }

        private void LoadData()
        {
            var displayNodes = chainNodes.Count > 0
                ? chainNodes
                : new List<MaterialReplacementChainNode>
                {
                    new MaterialReplacementChainNode
                    {
                        StepNo = 1,
                        MaterialId = sourceMaterial.Id,
                        Code = sourceMaterial.Code,
                        DisplayName = sourceMaterial.DisplayName,
                        ReplacementDate = sourceMaterial.ReplacementDate,
                        Status = sourceMaterial.IsDisable == true ? "\u505c\u7528" : "\u555f\u7528"
                    }
                };

            lblSummary.Text = $"\u66ff\u4ee3\u93c8\uff1a{string.Join(" -> ", displayNodes.Select(r => r.Code))}";
            gcData.DataSource = displayNodes;
            gvData.BestFitColumns();
        }

        private void f309_MaterialReplacementChain_Load(object sender, EventArgs e)
        {
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            LoadData();
        }

        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }
    }
}
