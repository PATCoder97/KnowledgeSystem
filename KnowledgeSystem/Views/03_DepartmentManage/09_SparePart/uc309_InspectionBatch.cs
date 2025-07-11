﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using System.Windows.Media.Media3D;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using DevExpress.XtraEditors.Controls;
using System.Web.Util;
using Font = System.Drawing.Font;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using Color = System.Drawing.Color;
using DataAccessLayer;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    /// <summary>
    /// Service sẽ tự động tạo đợt kiểm trả và trigger SQL sẽ tự bắt 20% lượng vật liệu trong kho để thêm vào bảng chi tiết
    /// </summary>
    public partial class uc309_InspectionBatch : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_InspectionBatch()
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
        string idDept2word = TPConfigs.idDept2word;

        DXMenuItem itemUpdateRemainDate;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            var ruleNotifyMain = new GridFormatRule
            {
                ApplyToRow = false,
                Column = gColStatus,
                Name = "RuleNotifyMain",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Status] == '待處理'",
                    Appearance =
                    {
                        ForeColor  = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical
                    }
                }
            };
            gvData.FormatRules.Add(ruleNotifyMain);

            var rule = new GridFormatRule
            {
                ApplyToRow = true,
                Name = $"RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[BatchMaterial.ActualQuantity] != [BatchMaterial.InitialQuantity]",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical }
                }
            };
            gvSparePart.FormatRules.Add(rule);
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

        private void InitializeMenuItems()
        {
            itemUpdateRemainDate = CreateMenuItem("延時提醒", ItemUpdateRemainDate_Click, TPSvgimages.DateAdd);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, TPSvgimages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, TPSvgimages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, TPSvgimages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, TPSvgimages.Num4);
            //itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, TPSvgimages.Num5);
        }

        private void ItemUpdateRemainDate_Click(object sender, EventArgs e)
        {
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            // Thiết lập mask để buộc nhập đúng định dạng
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "輸入時間",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = DateTime.Now.ToString("yyyy/MM/dd")
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            DateTime respTime;
            DateTime.TryParse(result.ToString(), out respTime);

            GridView view = gvData;
            var batch = (view.GetRow(view.FocusedRowHandle) as dynamic).Batch as dt309_InspectionBatch;

            if (respTime <= batch.CreatedDate || respTime < DateTime.Today)
            {
                XtraMessageBox.Show("日期不能早于创建日期或当前日期", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            batch.ExpiryDate = respTime;

            var resultUpdate = dt309_InspectionBatchBUS.Instance.AddOrUpdate(batch);

            if (resultUpdate)
            {
                LoadData();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var inspectionBatch = dt309_InspectionBatchBUS.Instance.GetList();
                var inspectionBatchMaterials = dt309_InspectionBatchMaterialBUS.Instance.GetList();
                var materials = dt309_MaterialsBUS.Instance.GetList();

                var depts = dm_DeptBUS.Instance.GetList();
                var users = dm_UserBUS.Instance.GetList();
                var units = dt309_UnitsBUS.Instance.GetList();

                var displayData = inspectionBatch.Select(batch =>
                {
                    var batchMaterialList = inspectionBatchMaterials
                        .Where(bm => bm.BatchId == batch.Id)
                        .Join(materials,
                              bm => bm.MaterialId,
                              m => m.Id,
                              (bm, m) => new
                              {
                                  Material = m,
                                  BatchMaterial = bm,
                                  Unit = units.FirstOrDefault(r => r.Id == m.IdUnit)?.DisplayName ?? "N/A",
                                  UserMngr = users.FirstOrDefault(r => r.Id == m.IdManager)?.DisplayName ?? "N/A",
                                  Dept = (depts.Where(r => r.Id == m.IdDept).Select(r => $"{r.Id} {r.DisplayName}").FirstOrDefault() ?? "N/A"),
                                  UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                                  IsComplete = bm.IsComplete
                              })
                        .ToList();

                    return new
                    {
                        Batch = batch,
                        Spare = batchMaterialList,
                        Status = batchMaterialList.Any(r => r.IsComplete != true) ? "待處理" : "已完成"
                    };
                }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();

                int rowHandle = gvData.FocusedRowHandle;
                if (gvData.IsMasterRow(rowHandle))
                {
                    gvData.ExpandMasterRow(rowHandle);
                }
            }
        }

        private void uc309_InspectionBatch_Load(object sender, EventArgs e)
        {
            gvSparePart.OptionsCustomization.AllowGroup = false;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "備品";
        }

        private void gvData_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemUpdateRemainDate);
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvSparePart_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == GridMenuType.Group)
            {
                GridViewGroupPanelMenu gridViewMenu = e.Menu as GridViewGroupPanelMenu;

                foreach (DXMenuItem menuItem in gridViewMenu.Items)
                {
                    if (menuItem.Caption.Equals(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelClearGrouping))
                        || menuItem.Caption.Equals(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelHide)))
                    {
                        menuItem.Visible = false;
                    }
                }
            }
        }

        //private int GetRowCountRecursive(GridView view, int rowHandle)
        //{
        //    int totalCount = 0;
        //    int childrenCount = view.GetChildRowCount(rowHandle);
        //    for (int i = 0; i < childrenCount; i++)
        //    {
        //        var childRowHandle = view.GetChildRowHandle(rowHandle, i);
        //        if (view.IsGroupRow(childRowHandle))
        //        {
        //            totalCount += GetRowCountRecursive(view, childRowHandle);
        //        }
        //        else
        //        {
        //            totalCount++;
        //        }
        //    }
        //    return totalCount;
        //}

        private int GetRowCountComplete(GridView view, int rowHandle)
        {
            int totalCount = 0;
            int childrenCount = view.GetChildRowCount(rowHandle);

            for (int i = 0; i < childrenCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(rowHandle, i);

                // Nếu là Group Row, đệ quy để lấy số lượng từ các nhóm con
                if (view.IsGroupRow(childRowHandle))
                {
                    totalCount += GetRowCountComplete(view, childRowHandle);
                }
                else
                {
                    // Nếu là Data Row, kiểm tra giá trị cột gColIsComplete
                    object cellValue = view.GetRowCellValue(childRowHandle, gColIsComplete);
                    if (cellValue != null && bool.TryParse(cellValue.ToString(), out bool isComplete) && isComplete)
                    {
                        totalCount++;
                    }
                }
            }

            return totalCount;
        }

        private void gvSparePart_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {

            var view = (GridView)sender;
            var info = (GridGroupRowInfo)e.Info;
            var caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
            {
                caption = info.Column.ToString();
            }

            var groupInfo = info.RowKey as GroupRowInfo;

            bool groupComplete = groupInfo?.ChildControllerRowCount == GetRowCountComplete(view, e.RowHandle);
            string colorName = groupComplete ? "Green" : "Red";
            string groupValue = groupComplete ? "已完成" : "處理中";

            info.GroupText = $" <color={colorName}>{groupValue}</color>：{info.GroupValueText}";
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "輸入名稱",
                Editor = editor,
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            string batchName = $"【經理室】{result}";

            if (XtraMessageBox.Show($"你確定新增批次名稱為 {batchName} 嗎?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            var newBatchId = dt309_InspectionBatchBUS.Instance.Add(new dt309_InspectionBatch
            {
                CreatedDate = DateTime.Now,
                BatchName = batchName,
                ExpiryDate = DateTime.Now.AddDays(7),
                NotifyNo = 0
            });

            if (newBatchId != -1)
            {
                LoadData();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }
    }
}
