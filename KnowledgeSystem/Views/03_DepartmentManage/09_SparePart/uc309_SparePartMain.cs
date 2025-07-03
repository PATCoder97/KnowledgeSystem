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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Math;
using static KnowledgeSystem.Views._04_SystemAdministrator._03_Extension._02_MaterialTrends.uc403_02_MaterialTrends;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;
using Tuple = System.Tuple;

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

            barCbbDept.EditValueChanged += CbbDept_EditValueChanged;
        }

        RefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = TPConfigs.idDept2word;
        string deptGetData = "";

        List<dm_User> users = new List<dm_User>();
        List<dm_Group> groups;
        List<dm_Departments> depts;
        List<dt309_Units> units;

        List<dt309_Materials> materials;
        List<dt309_Storages> storages;

        public static Dictionary<string, string> events = new Dictionary<string, string>()
        {
            {"in","入庫" },
            {"out","出庫" },
            {"check","盤點" },
            {"transfer","轉庫" },
        };

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;
        DXMenuItem itemMaterialGetFromOther;
        DXMenuItem itemMultiselect;
        DXMenuItem itemPrintStamp;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            barCbbDept.ImageOptions.SvgImage = TPSvgimages.Dept;

            btnExcelBySpare.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnExcelByMachine.ImageOptions.SvgImage = TPSvgimages.Num2;
            btnExcelByNotify.ImageOptions.SvgImage = TPSvgimages.Num3;
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
                    Expression = "[data.TransactionType] = 'check'",
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
                    Expression = "[data.TransactionType] = 'in'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question, }
                }
            };
            gvTransactions.FormatRules.Add(ruleIN);

            // Quy tắc định dạng khi TransactionType = 'I'
            var ruleTransfer = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleTransfer",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'transfer'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information, }
                }
            };
            gvTransactions.FormatRules.Add(ruleTransfer);

            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = false, // Áp dụng cho ô, không phải cả hàng
                Column = gColDisplayName, // Chỉ áp dụng cho cột gColDisplayName
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

            var ruleUserError = new GridFormatRule
            {
                Column = gColUserMngr,
                Name = "RuleUserError",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[UserError] == true",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };
            gvData.FormatRules.Add(ruleUserError);
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
            itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, TPSvgimages.Num5);

            itemMultiselect = CreateMenuItem("啟用多選", ItemMultiselect_Click, TPSvgimages.CheckedRadio);
            itemPrintStamp = CreateMenuItem("執行列印", ItemPrintStamp_Click, TPSvgimages.Print);
        }

        private void ItemPrintStamp_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> stampFormats = new Dictionary<string, string>()
            {
                { "10×5.21 cm", "rp309-10x5,21.repx" },
                { "10×8 cm", "rp309-10x8.repx" },
            };

            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.Caption = "標籤格式";
            args.Prompt = $"請選擇標籤格式";
            args.DefaultButtonIndex = 0;
            ComboBoxEdit editor = new ComboBoxEdit();

            editor.Properties.Items.AddRange(stampFormats.Select(r => r.Key).ToList());
            args.Editor = editor;

            editor.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            editor.Properties.Appearance.Font = TPConfigs.fontUI14;
            editor.Properties.AppearanceDropDown.Font = TPConfigs.fontUI14;
            editor.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            editor.Properties.NullText = "";

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string stampName = stampFormats.First(r => r.Key == result.ToString()).Value;

            var selectedItems = gvData.GetSelectedRows()
                .Select(rowHandle => (gvData.GetRow(rowHandle) as dynamic).data as dt309_Materials)
                .Where(item => item != null)
                .ToList();

            var labels = selectedItems.Select(r => new
            {
                code = r.Code,
                unit = units.FirstOrDefault(x => x.Id == r.IdUnit).DisplayName,
                productname = r.DisplayName,
                place = r.Location
            });
            var report = new XtraReport();
            string reportFormatpath = Path.Combine(TPConfigs.FolderReportFormat, stampName);
            report.LoadLayout(reportFormatpath);
            report.ShowPrintMarginsWarning = false;

            // Gán danh sách làm nguồn dữ liệu
            report.DataSource = labels;

            // Không cần set Parameter nữa
            report.CreateDocument();

            f00_PrintReport printReport = new f00_PrintReport();
            printReport.ViewerReport.DocumentSource = report;

            printReport.ShowDialog();
        }

        private void ItemMultiselect_Click(object sender, EventArgs e)
        {
            bool multiSelect = gvData.OptionsSelection.MultiSelect;
            gvData.OptionsSelection.MultiSelect = !multiSelect;
            gvData.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
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

        private void ItemMaterialGetFromOther_Click(object sender, EventArgs e) => HandleMaterialTransaction("調撥");

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
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看物料", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GridView view = gvData;
            int idMaterial = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMaterial));
            f309_Material_Info fInfo = new f309_Material_Info()
            {
                eventInfo = EventFormInfo.View,
                formName = "物料",
                idBase = idMaterial,
                idDeptGetData = deptGetData
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = (barCbbDept.EditValue?.ToString().Split(' ')[0]) ?? string.Empty;
                storages = dt309_StoragesBUS.Instance.GetList();
                users = dm_UserBUS.Instance.GetList();
                units = dt309_UnitsBUS.Instance.GetList();

                materials = dt309_MaterialsBUS.Instance.GetListByStartIdDept(deptGetData);
                var materialsRechecking = dt309_InspectionBatchMaterialBUS.Instance.GetListRechecking().Select(r => r.MaterialId).ToList();

                var displayData = materials
                    .Where(r => !materialsRechecking.Contains(r.Id))
                    .Select(x =>
                    {
                        var userMngr = users.FirstOrDefault(u => u.Id == x.IdManager);
                        return new
                        {
                            data = x,
                            Unit = units.FirstOrDefault(u => u.Id == x.IdUnit)?.DisplayName,
                            UserMngr = userMngr != null ? $"{userMngr.IdDepartment}/{userMngr.DisplayName}" : "",
                            UserError = userMngr != null ? userMngr.Status != 0 || userMngr.IdDepartment != x.IdDept : false
                        };
                    })
                    .ToList();

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

            gColDisplayName.AppearanceCell.Options.HighPriority = true;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
            var departments = dm_DeptBUS.Instance.GetList();
            var groups = dm_GroupBUS.Instance.GetListByName("機邊庫");

            var accessibleGroups = groups
                .Where(group => userGroups.Any(userGroup => userGroup.IdGroup == group.Id))
                .ToList();

            var departmentItems = departments
                .Where(dept => accessibleGroups.Any(group => group.IdDept == dept.Id))
                .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
                .ToArray();

            cbbDept.Items.AddRange(departmentItems);
            barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void CbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來新增物料", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            f309_Material_Info finfo = new f309_Material_Info()
            {
                eventInfo = EventFormInfo.Create,
                formName = "物料",
                idDeptGetData = deptGetData
            };

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
                bool multiSelect = gvData.OptionsSelection.MultiSelect;
                itemMultiselect.Caption = multiSelect ? "啟用單選" : "啟用多選";

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(itemUpdatePrice);

                DXSubMenuItem dXSubMenuReports = new DXSubMenuItem("庫存作業") { SvgImage = TPSvgimages.PersonnelChanges };
                dXSubMenuReports.ImageOptions.SvgImageSize = new Size(24, 24);

                dXSubMenuReports.Items.Add(itemMaterialIn);
                dXSubMenuReports.Items.Add(itemMaterialOut);
                dXSubMenuReports.Items.Add(itemMaterialTransfer);
                dXSubMenuReports.Items.Add(itemMaterialCheck);
                dXSubMenuReports.Items.Add(itemMaterialGetFromOther);
                dXSubMenuReports.BeginGroup = true;

                e.Menu.Items.Add(dXSubMenuReports);

                //DXSubMenuItem dXSubMenuPrint = new DXSubMenuItem("列印標籤") { SvgImage = TPSvgimages.Print };
                //dXSubMenuPrint.ImageOptions.SvgImageSize = new Size(24, 24);

                itemMultiselect.BeginGroup = true;

                e.Menu.Items.Add(itemMultiselect);
                e.Menu.Items.Add(itemPrintStamp);
                //dXSubMenuPrint.Items.Add(itemPrintStamp);
                //dXSubMenuPrint.Items.Add(itemMultiselect);

                //e.Menu.Items.Add(dXSubMenuPrint);
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
                                Starage = storages.FirstOrDefault(u => u.Id == r.StorageId)?.DisplayName,
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

        private void ExportExcelBySpare(bool _isNotify = false)
        {
            var materialsRechecking = dt309_InspectionBatchMaterialBUS.Instance.GetListRechecking().Select(r => r.MaterialId).ToList();

            var excelDatas = materials.Where(r => !materialsRechecking.Contains(r.Id) && (_isNotify ? (r.QuantityInStorage + r.QuantityInMachine) < r.MinQuantity : true))
                .OrderBy(x => x.IdDept)
                .Select(x => new
                {
                    部門 = x.IdDept,
                    材料編號 = x.Code,
                    品名規格 = x.DisplayName,
                    備品用途 = x.TypeUse,
                    料位 = x.Location,
                    單位 = units.FirstOrDefault(u => u.Id == x.IdUnit).DisplayName,
                    安全數量 = x.MinQuantity,
                    課庫數量 = x.QuantityInStorage,
                    機邊庫數量 = x.QuantityInMachine,
                    單價 = x.Price,
                    總金額 = x.Price * (x.QuantityInMachine + x.QuantityInStorage),
                    管理員 = users.FirstOrDefault(u => u.Id == x.IdManager).DisplayName,
                }).ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"備品管理-{(_isNotify ? "提醒備品" : "按照備品")} - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Thiết lập độ rộng các cột
                ws.Column(1).Width = 30;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 35;
                ws.Column(5).Width = 35;
                ws.Column(6).Width = 35;
                ws.Column(7).Width = 35;
                ws.Column(8).Width = 30;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true, OfficeOpenXml.Table.TableStyles.Medium2);
                // Bật WrapText cho tất cả các ô
                ws.Column(3).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(3).Width = 35;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }

        private void btnExcelBySpare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportExcelBySpare();
        }

        private void btnExcelByNotify_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportExcelBySpare(true);
        }

        private void btnExcelByMachine_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var machines = dt309_MachinesBUS.Instance.GetListByStartIdDept(deptGetData);
            var machineMaterials = dt309_MachineMaterialsBUS.Instance.GetList();

            // Tạo danh sách dạng phẳng: mỗi dòng là 1 vật liệu gắn với máy
            var excelDatas = machines.SelectMany(machine =>
            {
                var machineMaterialList = machineMaterials
                    .Where(mm => mm.MachineId == machine.Id)
                    .Join(materials,
                          mm => mm.MaterialId,
                          m => m.Id,
                          (mm, m) => m)
                    .ToList();

                double totalPrice = machineMaterialList.Sum(m =>
                    Convert.ToDouble(m.Price) *
                    (Convert.ToDouble(m.QuantityInStorage) + Convert.ToDouble(m.QuantityInMachine)));

                return machineMaterialList.Select(m => new
                {
                    部門 = machine.IdDept,
                    設備名稱 = machine.DisplayName,
                    設備位置 = machine.Location,
                    所屬機台總價值 = totalPrice,
                    材料編號 = m.Code,
                    品名規格 = m.DisplayName,
                    備品用途 = m.TypeUse,
                    料位 = m.Location,
                    單位 = units.FirstOrDefault(u => u.Id == m.IdUnit)?.DisplayName ?? "",
                    安全數量 = m.MinQuantity,
                    課庫數量 = m.QuantityInStorage,
                    機邊庫數量 = m.QuantityInMachine,
                    單價 = m.Price,
                    總金額 = m.Price * (m.QuantityInMachine + m.QuantityInStorage),
                    管理員 = users.FirstOrDefault(u => u.Id == m.IdManager)?.DisplayName ?? "",
                });
            }).ToList();

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"備品管理-按照設備 - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true);

                int startRow = 2, endRow = excelDatas.Count + 1;

                // Merge các cột A-D nếu giá trị giống nhau
                for (int col = 1; col <= 4; col++)
                {
                    int mergeStart = startRow;
                    for (int row = startRow + 1; row <= endRow + 1; row++)
                    {
                        var curr = ws.Cells[row, col].Text;
                        var prev = ws.Cells[mergeStart, col].Text;

                        if (curr != prev || row == endRow + 1)
                        {
                            if (row - 1 > mergeStart)
                                ws.Cells[mergeStart, col, row - 1, col].Merge = true;
                            mergeStart = row;
                        }
                    }
                }

                // Bật WrapText cho tất cả các ô
                ws.Column(6).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(6).Width = 35;

                // Định dạng cột D (giả sử D là cột số lượng chẳng hạn)
                ws.Column(4).Style.Numberformat.Format = "#,##0"; // D = 4
                ws.Column(13).Style.Numberformat.Format = "#,##0"; // M = 13
                ws.Column(14).Style.Numberformat.Format = "#,##0"; // N = 14

                // Căn giữa và kẻ ô toàn bảng
                var fullRange = ws.Cells[ws.Dimension.Address];
                fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullRange.Style.Border.Top.Style = fullRange.Style.Border.Bottom.Style =
                    fullRange.Style.Border.Left.Style = fullRange.Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }
    }
}
