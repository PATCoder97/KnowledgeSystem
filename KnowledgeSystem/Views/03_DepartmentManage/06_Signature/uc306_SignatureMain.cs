using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Wizards;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class uc306_SignatureMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc306_SignatureMain()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dm_User> users = new List<dm_User>();
        List<dm_JobTitle> jobs;

        List<dt306_Base> bases;
        List<dm_Attachment> attachments;

        DXMenuItem itemViewInfo;
        DXMenuItem itemViewFile;
        DXMenuItem itemAddPlanFile;
        DXMenuItem itemCloseReport;
        DXMenuItem itemAddAttach;
        DXMenuItem itemDelAttach;

        const string NAME_ISPROGRESS = "核簽中";
        const string NAME_ISCANCEL = "被退回";
        const string NAME_ISCOMPLETE = "核簽完畢";

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("看信息", ItemViewInfo_Click, TPSvgimages.View);
            //itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            //itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            //itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            //itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            //itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;

            int idBase = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f306_SignDocInfo fInfo = new f306_SignDocInfo();
            fInfo.idBase = idBase;
            fInfo.ShowDialog();

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
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Blue }, $"[Remark] = \'{NAME_ISPROGRESS}\'");
            gvData.FormatRules.AddExpressionRule(gColRemark, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, $"[Remark] = \'{NAME_ISCANCEL}\'");
            gvDocs.FormatRules.AddExpressionRule(gColRemark2, new DevExpress.Utils.AppearanceDefault() { ForeColor = Color.Red }, $"[Status] = \'{NAME_ISCANCEL}\'");
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                bases = dt306_BaseBUS.Instance.GetListByUploadUsr(TPConfigs.LoginUser.Id);
                users = dm_UserBUS.Instance.GetList();
                jobs = dm_JobTitleBUS.Instance.GetList();

                var basesDisplay = (from data in bases
                                    join urs in users on data.UploadUsr equals urs.Id
                                    //join job in jobs on urs.JobCode equals job.Id
                                    //where urs.IdDepartment.StartsWith(idDept2word)
                                    select new
                                    {
                                        data,
                                        urs,
                                        DisplayName = $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}"
                                    }).ToList();

                sourceBases.DataSource = basesDisplay;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
            }
        }

        private void uc306_SignatureMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvDocs.ReadOnlyGridView();

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gcData.ForceInitialize();
            gvData.CustomUnboundColumnData += gvData_CustomUnboundColumnData;
        }

        private void gvData_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            GridView view = sender as GridView;

            if (e.Column.FieldName == "Remark" && e.IsGetData)
            {
                bool isProcess = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.IsProcess"));
                bool isCacel = Convert.ToBoolean(view.GetListSourceRowCellValue(e.ListSourceRowIndex, "data.IsCancel"));

                if (isCacel)
                {
                    e.Value = NAME_ISCANCEL;
                }
                else if (isProcess)
                {
                    e.Value = NAME_ISPROGRESS;
                }
                else
                {
                    e.Value = NAME_ISCOMPLETE;
                }
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f306_NewSignDoc fAdd = new f306_NewSignDoc();
            fAdd.ShowDialog();

            LoadData();
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            int idBase = (int)view.GetRowCellValue(e.RowHandle, gColId);
            bool isProgess = (bool)view.GetRowCellValue(e.RowHandle, gColIsProgess);
            bool isCancel = (bool)view.GetRowCellValue(e.RowHandle, gColIsCancel);

            var baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            var childList = (from data in baseAtts
                             join urs in users on data.UsrCancel equals urs.Id into userGroup
                             from urs in userGroup.DefaultIfEmpty()
                             select new
                             {
                                 data,
                                 DisplayName = urs != null ? $"{urs.Id} {urs.IdDepartment}/{urs.DisplayName}" : null,
                                 Status = data.IsCancel || isCancel ? "被退回" : isProgess ? "核簽中" : "核簽完畢"
                             }).ToList();

            e.ChildList = childList;
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "核簽文件";
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell)
            {
                GridView view = sender as GridView;
                view.FocusedRowHandle = e.HitInfo.RowHandle;

                e.Menu.Items.Add(itemViewInfo);

                //int idReport = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdReport));
                //var closeRpDate = view.GetRowCellValue(view.FocusedRowHandle, gColCloseRp);

                //var atts = dt302_ReportAttachBUS.Instance.GetListByReport(idReport);

                //if (closeRpDate == null)
                //{
                //    e.Menu.Items.Add(itemAddAttach);
                //    if (atts.Count != 0) e.Menu.Items.Add(itemCloseReport);
                //}
            }
        }
    }
}
