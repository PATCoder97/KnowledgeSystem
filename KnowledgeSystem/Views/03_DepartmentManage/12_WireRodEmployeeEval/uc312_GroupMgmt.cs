using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312_GroupMgmt : DevExpress.XtraEditors.XtraUserControl
    {
        public uc312_GroupMgmt()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
            CreateRuleGV();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI14 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();       

        //List<dm_User> lsUser = new List<dm_User>();
        //List<dt202_Type> types;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            //gvData.FormatRules.AddExpressionRule(gColQuesCount, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, "[Count] < [QuizData.QuesCount]");
        }

        private void InitializeMenuItems()
        {
            
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

        private void ItemEditInfo_Click(object sender, EventArgs e)
        {
            //string idJob = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId).ToString();

            //f307_JobSet_Info fInfo = new f307_JobSet_Info();
            //fInfo.idJob = idJob;
            //fInfo.ShowDialog();

            //LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var baseData = dt312_GroupsBUS.Instance.GetList();
                var ques = dt312_QuestionsBUS.Instance.GetList();


                var quesCount = from data in ques
                                group data by data.GroupId into dtg
                                select new
                                {
                                    IdGroup = dtg.Key,
                                    QuestionCount = dtg.Count(),
                                };

                var dataDisplays = (from data in baseData
                                    join count in quesCount on data.Id equals count.IdGroup into joinedCounts
                                    from subCount in joinedCounts.DefaultIfEmpty()
                                    select new
                                    {
                                        Id = data.Id,
                                        DisplayName = data.DisplayName,
                                        QuestionCount = subCount != null ? subCount.QuestionCount : 0
                                    }).ToList();

                sourceBases.DataSource = dataDisplays;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc312_GroupMgmt_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TextEdit textEdit = new TextEdit
            {
                Font = new Font("Tahoma", 14F)
            };

            var args = new XtraInputBoxArgs
            {
                Caption = "輸入問題組名稱",
                Prompt = "問題組名稱:",
                Editor = textEdit,
                DefaultResponse = ""
            };

            var groupName = XtraInputBox.Show(args);
            if (groupName == null) return;

            dt312_Groups group = new dt312_Groups()
            {
                DisplayName = groupName.ToString().Trim(),
                CreateAt = DateTime.Now,
                CreateBy = TPConfigs.LoginUser.Id
            };

            var result = dt312_GroupsBUS.Instance.Add(group);
            if (result)
            {
                XtraMessageBox.Show("新增問題組成功！", "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idGroup = (int)view.GetRowCellValue(view.FocusedRowHandle, gColId);
            string groupName = view.GetRowCellValue(view.FocusedRowHandle, gColName).ToString();

            if (idGroup < 0) return;

            f312_QuesMgmt quesMgmt = new f312_QuesMgmt(idGroup) { Text = $"問題組：{groupName}" };
            quesMgmt.ShowDialog();

            LoadData();
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "HaveImg" && e.IsGetData)
            {
                string imgName = view.GetListSourceRowCellValue(e.ListSourceRowIndex, "ImageName")?.ToString();

                e.Value = !string.IsNullOrWhiteSpace(imgName);
            }
        }
    }
}
