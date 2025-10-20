using BusinessLayer;
using DataAccessLayer;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class uc311_ExpenseMain : XtraUserControl
    {
        private static readonly string FORMAT_FILE = @"C:\Users\Dell Alpha\Desktop\TestPython\Formats.json";
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

        DXMenuItem itemERP01;
        DXMenuItem itemERP02;
        DXMenuItem itemERP03;

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
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            //btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void InitializeMenuItems()
        {
            itemERP01 = CreateMenuItem("ERP：一般費用報銷", ItemERP01_Click, TPSvgimages.Num1);
            itemERP02 = CreateMenuItem("ERP：車輛稅費報銷", ItemERP02_Click, TPSvgimages.Num2);
            itemERP03 = CreateMenuItem("ERP：一般費用報銷(02)", ItemERP03_Click, TPSvgimages.Num3);
        }

        private void ItemERP03_Click(object sender, EventArgs e)
        {

        }

        private void ItemERP02_Click(object sender, EventArgs e)
        {

        }

        private void ItemERP01_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            string idBase = view.GetRowCellValue(view.FocusedRowHandle, gColId).ToString();

            string keyTab = "{Tab}";
            string keyData1 = $"LG";

            var selectedItems = view.GetSelectedRows()
               .Select(rowHandle => (gvData.GetRow(rowHandle) as dynamic).data as dt311_Invoice)
               .Where(item => item != null)
               .ToList();

            string keyData2 = "";
            foreach (var item in selectedItems)
            {
                keyData2 += $"{keyTab}{item.InvoiceCode}{keyTab}'{item.InvoiceNumber}{keyTab}{item.IssueDate:yyyyMMdd}{keyTab}{item.TotalBeforeVAT:0}{keyTab}{item.VATAmount:0}{keyTab}{TPConfigs.LoginUser.Id}{keyTab}";
            }

            List<string> keyDatas = new List<string>() { keyData1, keyData2 };
            f311_AutoERP autoERP = new f311_AutoERP(keyDatas);
            autoERP.ShowDialog();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                invoiceDatas = dt311_InvoiceBUS.Instance.GetList();
                var sellers = dt311_SellerBuyerBUS.Instance.GetList();

                var displayDatas = (from data in invoiceDatas
                                    join seller in sellers on data.SellerTax equals seller.Tax
                                    select new
                                    {
                                        data,
                                        SellerName = seller.DisplayName
                                    }).ToList();

                sourceData.DataSource = displayDatas;
                helper.LoadViewInfo();

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;
            }
        }

        // ========== MAIN FUNCTION ==========
        private static async Task<bool> ParseInvoiceAsync(string transactionId)
        {
            try
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
                    var xmlString = json.Value<string>("data");

                    if (string.IsNullOrEmpty(xmlString))
                    {
                        XtraMessageBox.Show("Không có dữ liệu XML trong phản hồi!", "Lỗi dữ liệu",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    var root = XElement.Parse(xmlString);
                    var formats = JObject.Parse(File.ReadAllText(FORMAT_FILE));

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
                        TransactionID = transactionId
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
            var tags = root.Descendants().Select(e => e.Name.LocalName).Distinct().ToList();
            foreach (var prop in formats)
            {
                var detectList = prop.Value["detect"].ToObject<List<string>>();
                if (detectList.Any(tags.Contains))
                    return prop.Key;
            }
            return null;
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

                    if (ok)
                        XtraMessageBox.Show("Hóa đơn đã được thêm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                btnAdd.Enabled = true;
            }

            LoadData();
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
            }
        }
    }
}
