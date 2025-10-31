using BusinessLayer;
using DataAccessLayer;
using DevExpress.CodeParser.VB;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DocumentFormat.OpenXml.Spreadsheet;
using KAutoHelper;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949;
using Newtonsoft.Json.Linq;
using Spire.Presentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement.f311_AutoERP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class uc311_ExpenseMain : XtraUserControl
    {
        private static readonly string URL = "https://www.meinvoice.vn/tra-cuu/GetInvoiceDataByTransactionID";

        public uc311_ExpenseMain()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();

            helper = new RefreshHelper(gvData, "Id");
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        RefreshHelper helper;
        List<dt311_Invoice> invoiceDatas;
        BindingSource sourceData = new BindingSource();
        List<dm_Group> groups;
        List<dm_Departments> depts;
        List<dt311_SellerBuyer> sellerBuyers;
        private static string idDept2Word = TPConfigs.idDept2word;

        DXMenuItem itemERP01;
        DXMenuItem itemERP02;
        DXMenuItem itemERP03;
        DXMenuItem itemUpdateAddFuel;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            //btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnGetManagerVehicle.ImageOptions.SvgImage = TPSvgimages.Gears;
            btnGetFillFuel.ImageOptions.SvgImage = TPSvgimages.GasStation;
        }

        private void InitializeMenuItems()
        {
            itemERP01 = CreateMenuItem("ERP：一般費用發票輸入", ItemERP01_Click, TPSvgimages.Num1);
            itemERP02 = CreateMenuItem("ERP：車輛稅費繳納管理", ItemERP02_Click, TPSvgimages.Num2);
            itemERP03 = CreateMenuItem("ERP：一般費用報銷輸入", ItemERP03_Click, TPSvgimages.Num3);
            itemUpdateAddFuel = CreateMenuItem("車輛：加油資料", ItemUpdateAddFuel_Click, TPSvgimages.UpLevel);
        }

        private void ItemUpdateAddFuel_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            //string typeOfSeller = view.GetRowCellValue(view.FocusedRowHandle, "seller.Type")?.ToString();
            string idBase = view.GetRowCellValue(view.FocusedRowHandle, "data.TransactionID")?.ToString();

            //if (typeOfSeller != "xang_dau")
            //    return;

            using (f311_AddFuel_Info fuel_Info = new f311_AddFuel_Info(idBase))
            {
                fuel_Info.ShowDialog();
            }

            LoadData();
        }

        private void ItemERP03_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            if (view == null || view.FocusedRowHandle < 0)
                return;

            if (!KeyboardHelper.IsEnglishAndCapsOff())
                return;

            // 🔹 Lấy danh sách hóa đơn được chọn / 取得所選發票
            var selectedInvoices = view.GetSelectedRows()
                .Select(rowHandle => view.GetRow(rowHandle) as dynamic)
                .Select(row => row != null ? row.data as dt311_Invoice : null)
                .Where(invoice => invoice != null)
                .ToList();

            if (selectedInvoices.Count == 0)
            {
                XtraMessageBox.Show(
                    "Vui lòng tích chọn các dòng cần điền。\n請選取要輸入的資料列。",
                    "Thông báo / 提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 🔹 Kiểm tra cùng nhà thầu & phương tiện / 檢查是否相同承包商與車輛
            bool isSameContractorGroup = selectedInvoices
                .Select(r => new { r.SellerTax, r.BuyerTax })
                .Distinct()
                .Count() == 1;

            if (!isSameContractorGroup)
            {
                XtraMessageBox.Show(
                    "Không cùng nhà thầu!\n非同一承包商！",
                    "Lỗi / 錯誤",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            //====== 輸入說明 / Input Description ======
            XtraInputBoxArgs inputArgs = new XtraInputBoxArgs();
            inputArgs.Caption = TPConfigs.SoftNameTW + " - 輸入說明 / Input Description";
            inputArgs.Prompt = "請輸入說明內容：";
            inputArgs.DefaultButtonIndex = 0;
            inputArgs.Editor = new TextEdit
            {
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F)
            };
            inputArgs.DefaultResponse = "";

            string description = (XtraInputBox.Show(inputArgs) as string)?.Trim();
            if (string.IsNullOrEmpty(description))
                return;

            //====== 組合前綴資料 / Build Prefix Key ======
            const string TAB = "{Tab}";

            dt311_Invoice firstInvoice = selectedInvoices.First();
            string deptMain = TPConfigs.LoginUser.IdDepartment;
            string sellerTax = firstInvoice.SellerTax;
            string buyerTax = firstInvoice.BuyerTax;

            string prefixKey = string.Join(TAB, new string[]
            {
                "LG", "",
                deptMain, "",
                "2",
                "W", "", "", "", "", "", "",
                sellerTax,
                buyerTax,
                "A1",
                description,
            });

            //====== 組合發票資料字串 / Build Invoice Data String ======
            IEnumerable<string> invoiceDataStrings = selectedInvoices.Select(invoice =>
            {
                var vehicleInfo = dt311_VehicleManagementBUS.Instance.GetItemById(invoice.LicensePlate);
                string deptSub = vehicleInfo != null ? vehicleInfo.IdDept : TPConfigs.LoginUser.IdDepartment;

                return string.Join(TAB, new string[]
                {
                    deptSub, "",
                    "NN",
                    "E" + invoice.SellerTax,
                    invoice.InvoiceCode,
                    invoice.InvoiceNumber, "", "", "", ""
                });
            });
            // 🔹 Ghép toàn bộ chuỗi hóa đơn / 合併所有發票資料字串
            string invoiceDataCombined = string.Join(TAB, invoiceDataStrings);

            invoiceDataStrings = selectedInvoices.Select(invoice =>
            {
                return string.Join(TAB, new string[]
                {
                    "", "",
                    invoice.IssueDate?.ToString("yyyyMMdd"),
                    description,
                });
            });
            string invoiceSubDataCombined = string.Join(TAB, invoiceDataStrings);

            // 🔹 Gom dữ liệu gửi đi ERP / 整合發送至ERP資料
            List<ErpAction> erpDataPayload = new List<ErpAction>();
            erpDataPayload.Add(new ErpAction() { Text = prefixKey });

            var subBitmap = ImageScanOpenCV.GetImage(Path.Combine(TPConfigs.Folder311, "tempSubmitBtn.png"));
            erpDataPayload.Add(new ErpAction() { IsClick = true, TempImage = subBitmap });
            erpDataPayload.Add(new ErpAction() { Text = invoiceDataCombined });
            erpDataPayload.Add(new ErpAction() { Text = "%a" });
            erpDataPayload.Add(new ErpAction() { Text = "s" });
            erpDataPayload.Add(new ErpAction() { Text = "{ENTER}" });
            erpDataPayload.Add(new ErpAction() { Text = invoiceSubDataCombined });

            // Hiển thị cảnh báo trước khi mở form ERP / 顯示警告訊息
            MsgBoxAlert();

            // 🔹 Mở form AutoERP / 開啟 AutoERP 視窗
            using (f311_AutoERP formAutoERP = new f311_AutoERP(erpDataPayload))
            {
                formAutoERP.ShowDialog();
            }
        }

        private void ItemERP02_Click(object sender, EventArgs e)
        {
            GridView gridView = gvData;
            if (gridView == null || gridView.FocusedRowHandle < 0)
                return;

            if (!KeyboardHelper.IsEnglishAndCapsOff())
                return;

            // 🔹 Lấy danh sách hóa đơn được chọn
            var selectedInvoices = gridView.GetSelectedRows()
                .Select(rowHandle => gvData.GetRow(rowHandle) as dynamic)
                .Select(row => row?.data as dt311_Invoice)
                .Where(invoice => invoice != null)
                .ToList();

            if (selectedInvoices.Count == 0)
            {
                XtraMessageBox.Show(
                    "Vui lòng tích chọn các dòng cần điền。\n請選取要輸入的資料列。",
                    "Thông báo / 提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 🔹 Kiểm tra cùng nhà thầu & phương tiện
            bool sameGroup = selectedInvoices
                .Select(r => new { r.SellerTax, r.BuyerTax, r.LicensePlate })
                .Distinct()
                .Count() == 1;

            if (!sameGroup)
            {
                XtraMessageBox.Show("Không cùng nhà thầu hoặc phương tiện!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Dữ liệu đầu tiên để lấy thông tin xe
            var firstItem = selectedInvoices.First();
            var vehicle = dt311_VehicleManagementBUS.Instance.GetItemById(firstItem.LicensePlate);

            string desc = string.Format("冶金技術部{0}加{2}油費用報銷。Thanh toán phí đổ {3} {1} BP Luyện Kim",
                vehicle.VehicleType.Split('/')[1],
                vehicle.VehicleType.Split('/')[0],
                vehicle.FuelType.Split('/')[1],
                vehicle.FuelType.Split('/')[0]);

            // 🔹 Cấu hình và chuỗi prefix
            const string tabDelimiter = "{Tab}";

            string prefixKey = string.Join(tabDelimiter, new[]
            {
                "LG",
                vehicle?.IdDept ?? "", "",
                "0", "",
                "W", "",
                "2",
                "LG",
                firstItem.SellerTax ?? "",
                firstItem.BuyerTax ?? "", "",
                desc ?? "", ""
            });

            // 🔹 Ghép chuỗi dữ liệu hóa đơn
            var invoiceStrings = selectedInvoices.Select(invoice =>
            {
                // Lấy dòng đầu tiên trong InvoiceItem (nếu có)
                var quantity = dt311_InvoiceItemBUS.Instance
                    .GetListByInvoiceId(invoice.TransactionID)
                    .FirstOrDefault()?.Quantity ?? 0;

                string vehicleTypeCode = (vehicle?.VehicleType == "xe máy/摩托車") ? "X" : "Y";

                return string.Join(tabDelimiter, new[]
                {
                    invoice.LicensePlate ?? "",
                    "",
                    vehicleTypeCode,
                    "E",
                    invoice.SellerTax ?? "", "",
                    invoice.InvoiceCode ?? "",
                    invoice.InvoiceNumber ?? "",
                    quantity.ToString("0.###"),
                    invoice.TotalBeforeVAT?.ToString("0") ?? "0",
                    invoice.VATAmount?.ToString("0") ?? "0"
                });
            });

            // 🔹 Nối tất cả hóa đơn (tab giữa các item)
            string invoiceDataString = string.Join(tabDelimiter, invoiceStrings);

            // 🔹 Gom dữ liệu thành danh sách gửi đi
            List<ErpAction> erpDataList = new List<ErpAction> { new ErpAction() { Text = prefixKey }, new ErpAction() { Text = invoiceDataString } };

            MsgBoxAlert();

            // Mở form AutoERP
            using (var autoErpForm = new f311_AutoERP(erpDataList))
            {
                autoErpForm.ShowDialog();
            }
        }

        private void ItemERP01_Click(object sender, EventArgs e)
        {
            GridView gridView = gvData;
            if (gridView == null || gridView.FocusedRowHandle < 0)
                return;

            if (!KeyboardHelper.IsEnglishAndCapsOff())
                return;

            // Các giá trị khởi tạo
            const string tabDelimiter = "{Tab}";
            const string prefixKey = "LG";

            // Lấy danh sách hóa đơn được chọn
            var selectedInvoices = gridView.GetSelectedRows()
                .Select(rowHandle => gvData.GetRow(rowHandle) as dynamic)
                .Select(row => row?.data as dt311_Invoice)
                .Where(invoice => invoice != null)
                .ToList();

            if (selectedInvoices.Count == 0)
            {
                XtraMessageBox.Show(
                    "Vui lòng tích chọn các dòng cần điền。\n請選取要輸入的資料列。",
                    "Thông báo / 提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Ghép chuỗi dữ liệu hóa đơn
            var invoiceStrings = selectedInvoices.Select(invoice => string.Join(tabDelimiter, new string[]
            {
                invoice.InvoiceCode ?? "",
                invoice.InvoiceNumber ?? "",
                invoice.IssueDate?.ToString("yyyyMMdd") ?? "",
                invoice.SellerTax?.ToString()?? "",
                invoice.TotalBeforeVAT?.ToString("0")?? "",
                invoice.VATAmount?.ToString("0")?? "",
                TPConfigs.LoginUser.Id ?? "",
                ""
            }));

            // Ghép tất cả hóa đơn (tab giữa các hóa đơn)
            string invoiceDataString = string.Join(tabDelimiter, invoiceStrings);

            // Gom dữ liệu thành danh sách
            List<ErpAction> erpDataList = new List<ErpAction> { new ErpAction() { Text = prefixKey }, new ErpAction() { Text = invoiceDataString } };

            MsgBoxAlert();

            // Mở form AutoERP
            using (var autoErpForm = new f311_AutoERP(erpDataList))
            {
                autoErpForm.ShowDialog();
            }
        }

        private void MsgBoxAlert()
        {
            string msg =
                "1️. Vui lòng không thao tác khi chương trình đang tự động。\n 請勿在程式自動輸入時操作鍵盤或滑鼠。";

            if (KeyboardHelper.IsUnikeyRunning())
            {
                msg += "\n\n" +
                       "2️. Phát hiện Unikey đang chạy, vui lòng tắt hoặc chuyển sang English (ENG)。\n 偵測到 UniKey 正在執行，請關閉或切換至英文輸入模式 (ENG)。";
            }

            XtraMessageBox.Show(
                msg,
                "⚠️ Cảnh báo / 警告",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                invoiceDatas = dt311_InvoiceBUS.Instance.GetList();
                sellerBuyers = dt311_SellerBuyerBUS.Instance.GetList();

                var displayDatas = (from data in invoiceDatas
                                    join seller in sellerBuyers on data.SellerTax equals seller.Tax
                                    select new
                                    {
                                        data,
                                        seller,
                                        SellerName = seller.DisplayName,
                                    }).ToList();

                sourceData.DataSource = displayDatas;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        // ========== MAIN FUNCTION ==========
        private static async Task<bool> ParseInvoiceAsync(string transactionId = null, string xmlFilePath = null)
        {
            try
            {
                string xmlString = "";

                if (!string.IsNullOrEmpty(xmlFilePath))
                {
                    xmlString = File.ReadAllText(xmlFilePath);
                }
                else
                {
                    using (var client = new HttpClient())
                    {
                        var formData = new FormUrlEncodedContent(new Dictionary<string, string> {
                        { "transactionID", transactionId }
                    });

                        var response = await client.PostAsync(URL, formData);
                        if (!response.IsSuccessStatusCode)
                        {
                            XtraMessageBox.Show($"Không thể truy cập MISA API. Mã lỗi: {response.StatusCode}",
                                "Lỗi truy cập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                        var jsonText = await response.Content.ReadAsStringAsync();
                        var json = JObject.Parse(jsonText);
                        xmlString = json.Value<string>("data");
                    }
                }

                if (string.IsNullOrEmpty(xmlString))
                {
                    XtraMessageBox.Show("Không có dữ liệu XML trong phản hồi!", "Lỗi dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var root = XElement.Parse(xmlString);
                var formats = JObject.Parse(File.ReadAllText(Path.Combine(TPConfigs.Folder311, "invoice_formats.json")));

                string fmtName = DetectFormat(root, formats);
                if (fmtName == null)
                {
                    XtraMessageBox.Show("Không nhận dạng được loại hóa đơn!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var fmt = (JObject)formats[fmtName];

                // --- Khởi tạo đối tượng ---
                var invoiceData = new dt311_Invoice
                {
                    SourceType = fmtName,
                    TransactionID = transactionId,
                    IdDept = idDept2Word
                };

                var itemsList = new List<dt311_InvoiceItem>();

                // --- Parse fields ---
                var fields = (JObject)fmt["fields"];
                foreach (var field in fields)
                {
                    string key = field.Key;
                    var path = field.Value;
                    string value = ExtractField(root, path);

                    switch (key)
                    {
                        case "invoice_id": if (string.IsNullOrEmpty(invoiceData.TransactionID)) invoiceData.TransactionID = value; break;
                        case "invoice_code": invoiceData.InvoiceCode = value; break;
                        case "invoice_number": invoiceData.InvoiceNumber = value; break;
                        case "issue_date":
                            if (DateTime.TryParse(value, out var d)) invoiceData.IssueDate = d;
                            break;
                        case "seller_tax": invoiceData.SellerTax = value; break;
                        case "buyer_tax": invoiceData.BuyerTax = value; break;
                        case "total_before_vat":
                            invoiceData.TotalBeforeVAT = ParseDecimal(value); break;
                        case "vat_amount":
                            invoiceData.VATAmount = ParseDecimal(value); break;
                        case "total_after_vat":
                            invoiceData.TotalAfterVAT = ParseDecimal(value); break;
                    }
                }

                // --- Seller & Buyer ---
                var seller = new dt311_SellerBuyer
                {
                    Tax = invoiceData.SellerTax,
                    DisplayName = ExtractField(root, fmt["fields"]["seller_name"])
                };
                var buyer = new dt311_SellerBuyer
                {
                    Tax = invoiceData.BuyerTax,
                    DisplayName = ExtractField(root, fmt["fields"]["buyer_name"])
                };

                // --- Lưu dữ liệu ---
                string invoiceId = dt311_InvoiceBUS.Instance.AddOrUpdate(invoiceData);
                dt311_SellerBuyerBUS.Instance.AddOrUpdate(seller);
                dt311_SellerBuyerBUS.Instance.AddOrUpdate(buyer);

                // --- Parse danh sách hàng ---
                string listTag = fmt["list_items"].ToString();
                var itemFields = (JObject)fmt["item_fields"];

                foreach (var el in root.Descendants().Where(e => e.Name.LocalName == listTag))
                {
                    var item = new dt311_InvoiceItem { IdInvoice = invoiceId };

                    foreach (var kv in itemFields)
                    {
                        string val = ExtractField(el, kv.Value);
                        switch (kv.Key)
                        {
                            case "line_number": item.LineNumber = val; break;
                            case "code": item.Code = val; break;
                            case "name": item.DisplayName = val; break;
                            case "unit": item.Unit = val; break;
                            case "quantity": item.Quantity = ParseDecimal(val); break;
                            case "unit_price": item.UnitPrice = ParseDecimal(val); break;
                            case "amount": item.Amount = ParseDecimal(val); break;
                            case "vat_rate": item.VATRate = ExtractNumber(val); break;
                            case "vat_amount": item.VATAmount = ParseDecimal(val); break;
                            case "note": item.Note = val; break;
                        }
                    }

                    itemsList.Add(item);
                }

                // --- Lưu danh sách hàng ---
                dt311_InvoiceItemBUS.Instance.RemoveRangeByIdInvoice(invoiceId);
                dt311_InvoiceItemBUS.Instance.AddRange(itemsList);

                XtraMessageBox.Show(
                    $"Đã thêm hóa đơn số {invoiceData.InvoiceNumber} ({invoiceData.InvoiceCode})\n" +
                    $"Người bán: {seller.DisplayName}\nNgười mua: {buyer.DisplayName}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi xử lý hóa đơn:\n{ex.Message}",
                    "Lỗi xử lý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ========== HÀM HỖ TRỢ ==========
        private static string DetectFormat(XElement root, JObject formats)
        {
            // Lấy toàn bộ tên tag, nội dung text, và thuộc tính
            var tagNames = root.Descendants().Select(e => e.Name.LocalName).Distinct().ToList();
            var texts = root.Descendants()
                            .Select(e => e.Value)
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                            .Distinct()
                            .ToList();
            var attrs = root.Descendants()
                            .SelectMany(e => e.Attributes())
                            .Select(a => a.Value)
                            .Where(v => !string.IsNullOrWhiteSpace(v))
                            .Distinct()
                            .ToList();

            string bestMatch = null;
            int bestScore = 0;

            foreach (var prop in formats)
            {
                var detectList = prop.Value["detect"].ToObject<List<string>>();
                int score = 0;

                foreach (var key in detectList)
                {
                    // so khớp theo tag, text, hoặc thuộc tính
                    bool match = tagNames.Any(t => t.Equals(key, StringComparison.OrdinalIgnoreCase)) ||
                                 texts.Any(t => t.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                 attrs.Any(a => a.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);

                    if (match)
                        score++;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = prop.Key;
                }
            }

            // Ngưỡng: ít nhất 2 keyword trùng mới chấp nhận
            return bestScore >= 2 ? bestMatch : null;
        }

        private static string ExtractField(XElement root, JToken path)
        {
            try
            {
                if (path.Type == JTokenType.Array)
                {
                    var vals = new List<string>();
                    foreach (var p in path)
                    {
                        string val = ExtractField(root, p);
                        if (!string.IsNullOrEmpty(val))
                            vals.Add(val);
                    }
                    return string.Join("", vals);
                }

                string pathStr = path.ToString();

                if (pathStr.Contains("|"))
                {
                    foreach (var p in pathStr.Split('|'))
                    {
                        string val = ExtractField(root, p.Trim());
                        if (!string.IsNullOrEmpty(val))
                            return val;
                    }
                    return "";
                }

                if (pathStr.StartsWith("@"))
                {
                    var parts = pathStr.Substring(1).Split(':');
                    if (parts.Length == 2)
                    {
                        var parent = root.Descendants().FirstOrDefault(e => e.Name.LocalName == parts[1]);
                        return parent?.Attribute(parts[0])?.Value ?? "";
                    }
                }

                if (pathStr.Contains(":"))
                {
                    var parts = pathStr.Split(':');
                    var parent = root.Descendants().FirstOrDefault(e => e.Name.LocalName == parts[1]);
                    var child = parent?.Descendants().FirstOrDefault(e => e.Name.LocalName == parts[0]);
                    return child?.Value?.Trim() ?? "";
                }

                var el = root.Descendants().FirstOrDefault(e => e.Name.LocalName == pathStr);
                return el?.Value?.Trim() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static string ExtractNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            var digits = new string(input.Where(char.IsDigit).ToArray());
            return digits;
        }

        private static decimal? ParseDecimal(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = new string(input.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            if (decimal.TryParse(input.Replace(",", ""), out var num))
                return num;
            return null;
        }

        private async void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var inputArgs = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入交易代碼 (TransactionID)",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            };

            string transactionId = XtraInputBox.Show(inputArgs) as string;
            transactionId = transactionId?.Trim();
            if (string.IsNullOrEmpty(transactionId))
                return;

            btnAdd.Enabled = false;
            try
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(this))
                {
                    var ok = await ParseInvoiceAsync(transactionId);
                }
            }
            finally
            {
                btnAdd.Enabled = true;
            }

            LoadData();
        }

        private async void btnAddXmlFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn file XML";
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string filePath = openFileDialog.FileName;

                btnAdd.Enabled = false;
                try
                {
                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        var ok = await ParseInvoiceAsync(xmlFilePath: filePath);
                    }
                }
                finally
                {
                    btnAdd.Enabled = true;
                }

                LoadData();
            }
        }

        private void uc311_ExpenseMain_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvDetail.ReadOnlyGridView();
            gvDetail.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            LoadData();
            gcData.DataSource = sourceData;

            bool IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            if (!IsSysAdmin)
            {
                btnGetManagerVehicle.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }

            gvData.BestFitColumns();
        }

        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            //GridView view = sender as GridView;
            //int idBase = view.GetRowCellValue(e.RowHandle, gColId) != null ? (int)view.GetRowCellValue(e.RowHandle, gColId) : -1;

            //e.IsEmpty = !forms.Any(r => r.IdBase == idBase);
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Detail";
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            string idBase = view.GetRowCellValue(e.RowHandle, "data.TransactionID").ToString();

            e.ChildList = dt311_InvoiceItemBUS.Instance.GetListByInvoiceId(idBase);
        }

        private void gvData_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                e.Menu.Items.Add(itemERP01);
                e.Menu.Items.Add(itemERP02);
                e.Menu.Items.Add(itemERP03);

                GridView view = gvData;
                string typeOfSeller = view.GetRowCellValue(view.FocusedRowHandle, "seller.Type")?.ToString();
                //string idBase = view.GetRowCellValue(view.FocusedRowHandle, "data.TransactionID")?.ToString();

                if (typeOfSeller == "xang_dau")
                {
                    itemUpdateAddFuel.BeginGroup = true;
                    e.Menu.Items.Add(itemUpdateAddFuel);
                }
            }
        }

        private void btnGetManagerVehicle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Hiển thị overlay loading
            var handle = SplashScreenManager.ShowOverlayForm(this);

            // Chạy công việc nặng trong Task riêng để không khóa UI
            Task.Run(async () =>
            {
                try
                {
                    var vehicles = dt311_VehicleManagementBUS.Instance.GetList();

                    foreach (var item in vehicles)
                    {
                        string managerID = await BorrVehicleHelper.Instance.GetManagerVehicle(item.LicensePlate.Replace(".", ""));
                        item.ManagerId = managerID;

                        dt311_VehicleManagementBUS.Instance.AddOrUpdate(item);
                    }

                    // Có thể hiển thị thông báo sau khi hoàn tất
                    XtraMessageBox.Show("Cập nhật thông tin người quản lý xe hoàn tất!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Lỗi khi lấy thông tin: {ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Đóng overlay form an toàn (phải gọi từ UI thread)
                    if (handle != null)
                    {
                        this.Invoke(new Action(() => SplashScreenManager.CloseOverlayForm(handle)));
                    }
                }
            });
        }

        private async void btnGetFillFuel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 🔹 Hiển thị overlay loading
            var overlayHandle = SplashScreenManager.ShowOverlayForm(this);

            try
            {
                await Task.Run(async () =>
                {
                    // 🔸 Lấy danh sách xe theo bộ phận
                    var allVehicles = dt311_VehicleManagementBUS.Instance
                        .GetList()
                        .Where(v => v.IdDept.StartsWith(idDept2Word))
                        .ToList();

                    // 🔸 Lấy danh sách hóa đơn từ Grid, lọc theo loại "xăng dầu"
                    var invoiceList = (
                        from inv in (sourceData.DataSource as IEnumerable<dynamic>)?.Select(o => (dt311_Invoice)o.data)
                        join seller in sellerBuyers on inv.SellerTax equals seller.Tax
                        where seller.Type == "xang_dau"
                        select inv
                    ).ToList();

                    if (invoiceList == null || invoiceList.Count == 0)
                        return;

                    // 🔸 Chuẩn bị danh sách xe cần kiểm tra
                    var vehicleInfos = allVehicles.Select(v => new VehicleStatus
                    {
                        Dept = v.IdDept.Substring(0, 2),
                        Name = v.LicensePlate
                    }).ToList();

                    // 🔸 Thu thập thông tin mượn xe (F.加油)
                    var fuelBorrowInfos = new List<VehicleBorrInfo>();

                    foreach (var vehicleInfo in vehicleInfos)
                    {
                        var borrowRecords = new List<VehicleBorrInfo>();

                        borrowRecords.AddRange(await BorrVehicleHelper.Instance.GetBorrMotorUser(vehicleInfo));
                        borrowRecords.AddRange(await BorrVehicleHelper.Instance.GetBorrCarUser(vehicleInfo));

                        // Lọc "F.加油" và lấy bản ghi mới nhất mỗi ngày
                        var latestFuelRecords = borrowRecords
                            .Where(r => !string.IsNullOrEmpty(r.Uses) && r.Uses.StartsWith("F.加油"))
                            .GroupBy(r => r.BorrTime.Date)
                            .Select(g => g.OrderByDescending(r => r.BorrTime).First())
                            .ToList();

                        fuelBorrowInfos.AddRange(latestFuelRecords);
                    }

                    // 🔸 Ghép hóa đơn với thông tin mượn xe theo ngày
                    var combinedData = (
                        from invoice in invoiceList
                        join fuel in fuelBorrowInfos
                            on (invoice.IssueDate?.Date ?? DateTime.MinValue) equals fuel.BorrTime.Date
                        select new { Invoice = invoice, Fuel = fuel }
                    ).ToList();

                    // 🔸 Cập nhật dữ liệu hóa đơn
                    foreach (var item in combinedData)
                    {
                        var invoice = item.Invoice;
                        var fuel = item.Fuel;

                        invoice.LicensePlate = fuel.VehicleName;
                        invoice.OdometerReading = fuel.EndKm;
                        invoice.FuelFilledBy = fuel.IdUserBorr;

                        dt311_InvoiceBUS.Instance.AddOrUpdate(invoice);
                    }
                });

                // 🔸 Sau khi Task hoàn tất → tải lại dữ liệu
                LoadData();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Đã xảy ra lỗi khi đồng bộ dữ liệu:\n{ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 🔹 Đảm bảo đóng overlay dù có lỗi hay không
                SplashScreenManager.CloseOverlayForm(overlayHandle);
            }
        }
    }
}
