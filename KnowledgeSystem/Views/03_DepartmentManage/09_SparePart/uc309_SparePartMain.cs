using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck;
using Org.BouncyCastle.Math;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_SparePartMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_SparePartMain()
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

        List<dm_User> users = new List<dm_User>();

        List<dt309_Materials> materials;
        List<dt309_Storages> storages;

        Dictionary<string, string> events = new Dictionary<string, string>()
        {
            {"IN","入庫" },
            {"OUT","出庫" },
            {"CHECK","盤點" },
        };

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            // Quy tắc định dạng khi TransactionType = 'C'
            var ruleCHECK = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleCheck",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'CHECK'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvTransactions.FormatRules.Add(ruleCHECK);

            // Quy tắc định dạng khi TransactionType = 'I'
            var ruleIN = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleIn",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'IN'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question, }
                }
            };
            gvTransactions.FormatRules.Add(ruleIN);

            // Quy tắc cảnh báo khi số lượng trong kho + máy < min
            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.QuantityInStorage] + [data.QuantityInMachine] < [data.MinQuantity]",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };
            gvData.FormatRules.Add(ruleNotify);

            // Quy tắc hiển thị biểu tượng tăng/giảm trong lịch sử giao dịch
            var ruleIconSet = new GridFormatRule
            {
                Name = "RuleTransactionTrend",
                Column = gColQuantity,
                Rule = new FormatConditionRuleIconSet
                {
                    IconSet = new FormatConditionIconSet
                    {
                        ValueType = FormatConditionValueType.Automatic,
                        Icons =
                        {
                            new FormatConditionIconSetIcon { PredefinedName = "Arrows3_1.png", Value = 0, ValueComparison = FormatConditionComparisonType.Greater },
                            new FormatConditionIconSetIcon { PredefinedName = "Triangles3_2.png", Value = 0, ValueComparison = FormatConditionComparisonType.GreaterOrEqual },
                            new FormatConditionIconSetIcon { PredefinedName = "Arrows3_3.png", Value = decimal.MinValue, ValueComparison = FormatConditionComparisonType.GreaterOrEqual }
                        }
                    }
                }
            };
            gvTransactions.FormatRules.Add(ruleIconSet);
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
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, TPSvgimages.View);
            itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, TPSvgimages.Money);

            itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, TPSvgimages.Num1);
            itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, TPSvgimages.Num2);
            itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, TPSvgimages.Num3);
            itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, TPSvgimages.Num4);
        }

        private void HandleMaterialTransaction(string eventInfo)
        {
            var idMaterial = Convert.ToInt16(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdMaterial));

            var transactionForm = new f309_Transaction_Info
            {
                eventInfo = eventInfo,
                idMaterial = idMaterial
            };

            transactionForm.ShowDialog();
            LoadData();
        }

        private void ItemMaterialCheck_Click(object sender, EventArgs e) => HandleMaterialTransaction("盤點");

        private void ItemMaterialTransfer_Click(object sender, EventArgs e) => HandleMaterialTransaction("轉庫");

        private void ItemMaterialOut_Click(object sender, EventArgs e) => HandleMaterialTransaction("領用");

        private void ItemMaterialIn_Click(object sender, EventArgs e) => HandleMaterialTransaction("收貨");

        private void ItemUpdatePrice_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idMaterial = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMaterial));

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入新單價",
                DefaultButtonIndex = 0,
                Editor = new TextEdit
                {
                    Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                    Properties = { Mask = { MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric, EditMask = "N0", UseMaskAsDisplayFormat = true } }
                },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result)) return;

            var newPrice = Convert.ToInt32(result);
            if (newPrice < 0) return;

            var resultUpdate = dt309_PricesBUS.Instance.Add(new dt309_Prices()
            {
                MaterialId = idMaterial,
                Price = newPrice,
                ChangedAt = DateTime.Now,
                ChangedBy = TPConfigs.LoginUser.Id
            });

            LoadData();
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            int idMaterial = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMaterial));
            f309_Material_Info fInfo = new f309_Material_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "備品",
                idBase = idMaterial
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                storages = dt309_StoragesBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                var units = dt309_UnitsBUS.Instance.GetList();

                materials = dt309_MaterialsBUS.Instance.GetListByIdDept(TPConfigs.LoginUser.IdDepartment);

                var displayData = materials.Select(x => new
                {
                    data = x,
                    Unit = units.FirstOrDefault(u => u.Id == x.IdUnit).DisplayName,
                    UserMngr = users.FirstOrDefault(u => u.Id == x.IdManager).DisplayName,
                }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void uc309_SparePartMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvTransactions.ReadOnlyGridView();
            gvTransactions.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvPrices.ReadOnlyGridView();
            gvPrices.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvMachine.ReadOnlyGridView();
            gvMachine.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f309_Material_Info finfo = new f309_Material_Info();
            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemUpdatePrice);


                DXSubMenuItem dXSubMenuReports = new DXSubMenuItem("庫存作業") { SvgImage = TPSvgimages.PersonnelChanges };
                dXSubMenuReports.ImageOptions.SvgImageSize = new Size(24, 24);

                dXSubMenuReports.Items.Add(itemMaterialIn);
                dXSubMenuReports.Items.Add(itemMaterialOut);
                dXSubMenuReports.Items.Add(itemMaterialTransfer);
                dXSubMenuReports.Items.Add(itemMaterialCheck);
                dXSubMenuReports.BeginGroup = true;

                e.Menu.Items.Add(dXSubMenuReports);

                //e.Menu.Items.Add(itemMaterialIn);
                //e.Menu.Items.Add(itemMaterialOut);
                //e.Menu.Items.Add(itemMaterialTransfer);
                //e.Menu.Items.Add(itemMaterialCheck);
            }
        }

        private void gvData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "TotalPrice")
            {
                dt309_Materials currentRow = ((dynamic)e.Row).data as dt309_Materials;
                if (currentRow == null) return;

                double sumQuantity = Convert.ToDouble(currentRow.QuantityInStorage + currentRow.QuantityInMachine);

                // Tạo một mảng các chuỗi và chỉ lấy các chuỗi không rỗng
                e.Value = sumQuantity * currentRow.Price;
            }
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 3;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            switch (e.RelationIndex)
            {
                case 0:
                    e.RelationName = "出入庫記錄";
                    break;
                case 1:
                    e.RelationName = "單價管理";
                    break;
                case 2:
                    e.RelationName = "用於設備";
                    break;
                    //case 3:
                    //    e.RelationName = "單價管理";
                    //    break;
            }
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                GridView view = sender as GridView;
                dt309_Materials material = (view.GetRow(e.RowHandle) as dynamic).data as dt309_Materials;
                int idMaterial = material.Id;

                if (material != null)
                {
                    switch (e.RelationIndex)
                    {
                        case 0:

                            var transactions = dt309_TransactionsBUS.Instance.GetListByidMaterial(idMaterial).Select(r => new
                            {
                                data = r,
                                Event = events[r.TransactionType],
                                UserDo = users.FirstOrDefault(u => u.Id == r.UserDo)?.DisplayName,
                                Starage = storages.FirstOrDefault(u => u.Id == r.StorageId).DisplayName,
                            }).OrderByDescending(r => r.data.Id).ToList();
                            e.ChildList = transactions;

                            break;
                        case 1:

                            var prices = dt309_PricesBUS.Instance.GetListByIdMaterial(idMaterial).Select(r => new
                            {
                                r.Price,
                                r.ChangedAt,
                                ChangedBy = users.FirstOrDefault(x => x.Id == r.ChangedBy).DisplayName
                            }).ToList();

                            e.ChildList = prices;

                            break;
                        case 2:

                            var ids = dt309_MachineMaterialsBUS.Instance.GetListByIdMaterial(idMaterial).Select(r => r.MachineId).ToList();
                            var machines = dt309_MachinesBUS.Instance.GetListByIds(ids);

                            e.ChildList = machines;

                            break;
                            //case 3:
                            //    List<Prices> lsPrices = PricesDAO.Instance.GetListByIdSpare(material.IdSparePart);
                            //    e.ChildList = lsPrices.OrderByDescending(r => r.PriceTime).ToList();
                            //    break;
                    }
                }
            }
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }
    }
}
