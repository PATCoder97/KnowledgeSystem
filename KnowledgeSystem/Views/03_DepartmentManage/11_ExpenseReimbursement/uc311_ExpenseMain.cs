using BusinessLayer;
using DataAccessLayer;
using DevExpress.CodeParser.VB;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.ExpressionEditor;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using KAutoHelper;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949;
using MiniSoftware;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Presentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement.f311_AutoERP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;
using Path = System.IO.Path;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class uc311_ExpenseMain : XtraUserControl
    {
        private static readonly string URL = "https://www.meinvoice.vn/tra-cuu/GetInvoiceDataByTransactionID";
        private static readonly string URL_INVOICE_PDF = "https://www.meinvoice.vn/tra-cuu/DownloadHandler.ashx?Type=pdf&Viewer=1&ext=XZ28Q9E6&Code=";
        private const string InvoiceAttachmentThread = "311";
        private const string FuelPhotoAttachmentThread = "311_FUEL_PHOTO";
        private const string PdfCjkFontName = "DFKai-SB";
        private const string PdfLatinFontName = "Times New Roman";
        private const float FuelPhotoPdfPageMargin = 24f;
        private const float FuelPhotoPdfEntrySpacing = 18f;
        private const float FuelPhotoPdfImageHeight = 220f;

        private sealed class FuelPhotoReportRow
        {
            public string LicensePlate { get; set; }
            public string FuelTypeDisplay { get; set; }
            public DateTime IssueDate { get; set; }
            public decimal Quantity { get; set; }
            public int? FuelPhotoAttId { get; set; }
        }

        private sealed class FuelUsageStatisticRow
        {
            public string FuelCategory { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal Quantity { get; set; }
        }

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
        List<dt311_SellerBuyer> sellerBuyers;
        private static string idDept2Word = TPConfigs.idDept2word;

        DXMenuItem itemERP01;
        DXMenuItem itemERP02;
        DXMenuItem itemERP03;
        DXMenuItem itemUpdateAddFuel;
        DXMenuItem itemViewFile;
        DXMenuItem itemAddFile;
        DXMenuItem itemViewFuelPhoto;
        DXMenuItem itemAddFuelPhoto;

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
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            barSubExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnGetManagerVehicle.ImageOptions.SvgImage = TPSvgimages.Gears;
            btnGetFillFuel.ImageOptions.SvgImage = TPSvgimages.GasStation;

            btnFillFuelTable.ImageOptions.SvgImage = TPSvgimages.Num1;
            btnFuelUsageStatistics.ImageOptions.SvgImage = TPSvgimages.Num2;
        }

        private void InitializeMenuItems()
        {
            itemERP01 = CreateMenuItem("ERP：一般費用發票輸入", ItemERP01_Click, TPSvgimages.Num1);
            itemERP02 = CreateMenuItem("ERP：車輛稅費繳納管理", ItemERP02_Click, TPSvgimages.Num2);
            itemERP03 = CreateMenuItem("ERP：一般費用報銷輸入", ItemERP03_Click, TPSvgimages.Num3);
            itemUpdateAddFuel = CreateMenuItem("車輛：加油資料", ItemUpdateAddFuel_Click, TPSvgimages.UpLevel);

            itemAddFile = CreateMenuItem("上傳發票PDF", ItemAddFile_Click, TPSvgimages.UploadFile);
            itemViewFile = CreateMenuItem("查看發票PDF", ItemViewFile_Click, TPSvgimages.View);
            itemAddFuelPhoto = CreateMenuItem("上傳加油照片", ItemAddFuelPhoto_Click, TPSvgimages.UploadFile);
            itemViewFuelPhoto = CreateMenuItem("查看加油照片", ItemViewFuelPhoto_Click, TPSvgimages.View);
        }

        private void ItemViewFile_Click(object sender, EventArgs e)
        {
            OpenAttachmentFromGrid(gColAttId, "未上傳發票PDF。");
        }

        private void ItemViewFuelPhoto_Click(object sender, EventArgs e)
        {
            OpenAttachmentFromGrid(gColFuelPhotoAttId, "未上傳加油照片。");
        }

        private void ItemAddFile_Click(object sender, EventArgs e)
        {
            UploadAttachmentForFocusedInvoice(
                "PDF files (*.pdf)|*.pdf",
                InvoiceAttachmentThread,
                invoice => invoice.AttId,
                (invoice, attachmentId) => invoice.AttId = attachmentId,
                "已上傳發票PDF。");
        }

        private void ItemAddFuelPhoto_Click(object sender, EventArgs e)
        {
            UploadAttachmentForFocusedInvoice(
                "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                FuelPhotoAttachmentThread,
                invoice => invoice.FuelPhotoAttId,
                (invoice, attachmentId) => invoice.FuelPhotoAttId = attachmentId,
                "已上傳加油照片。");
        }

        private void OpenAttachmentFromGrid(GridColumn attachmentColumn, string emptyMessage)
        {
            int attachmentId = GetFocusedIntValue(attachmentColumn);
            if (attachmentId <= 0)
            {
                XtraMessageBox.Show(emptyMessage, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            OpenAttachment(attachmentId);
        }

        private void OpenAttachment(int attachmentId)
        {
            var attachment = dm_AttachmentBUS.Instance.GetItemById(attachmentId);
            if (attachment == null)
            {
                XtraMessageBox.Show("找不到附件資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sourcePath = BuildAttachmentPath(attachment);
            if (!File.Exists(sourcePath))
            {
                XtraMessageBox.Show("找不到附件檔案。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(TPConfigs.TempFolderData))
            {
                Directory.CreateDirectory(TPConfigs.TempFolderData);
            }

            string safeFileName = Regex.Replace(attachment.ActualName ?? "file", @"[\\/:*?""<>|]", "");
            string destPath = Path.Combine(
                TPConfigs.TempFolderData,
                $"{safeFileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(attachment.ActualName)}");

            File.Copy(sourcePath, destPath, true);

            var mainForm = f00_ViewMultiFile.Instance;
            if (!mainForm.Visible)
            {
                mainForm.Show();
            }

            mainForm.OpenFormInDocumentManager(destPath);
        }

        private void UploadAttachmentForFocusedInvoice(
            string dialogFilter,
            string attachmentThread,
            Func<dt311_Invoice, int?> getAttachmentId,
            Action<dt311_Invoice, int?> setAttachmentId,
            string successMessage)
        {
            dt311_Invoice invoice = GetFocusedInvoice();
            if (invoice == null)
            {
                XtraMessageBox.Show("找不到選取的發票資料。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var openFileDialog = new OpenFileDialog { Filter = dialogFilter })
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string sourceFilePath = openFileDialog.FileName;
                string actualName = Path.GetFileName(sourceFilePath);
                string encryptionName = EncryptionHelper.EncryptionFileName(actualName);
                string destPath = Path.Combine(TPConfigs.Folder311, encryptionName);

                DirectoryInfo parentDir = Directory.GetParent(destPath);
                if (parentDir != null && !Directory.Exists(parentDir.FullName))
                {
                    Directory.CreateDirectory(parentDir.FullName);
                }

                File.Copy(sourceFilePath, destPath, true);

                var attachment = new dm_Attachment
                {
                    Thread = attachmentThread,
                    EncryptionName = encryptionName,
                    ActualName = actualName
                };

                int newAttachmentId = dm_AttachmentBUS.Instance.Add(attachment);
                if (newAttachmentId <= 0)
                {
                    if (File.Exists(destPath))
                    {
                        File.Delete(destPath);
                    }

                    XtraMessageBox.Show("附件上傳失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int? oldAttachmentId = getAttachmentId(invoice);
                setAttachmentId(invoice, newAttachmentId);
                string invoiceId = dt311_InvoiceBUS.Instance.AddOrUpdate(invoice);
                if (string.IsNullOrWhiteSpace(invoiceId))
                {
                    RemoveAttachment(newAttachmentId);
                    XtraMessageBox.Show("發票附件關聯失敗。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (oldAttachmentId.HasValue && oldAttachmentId.Value > 0 && oldAttachmentId.Value != newAttachmentId)
                {
                    RemoveAttachment(oldAttachmentId.Value);
                }

                LoadData();
                XtraMessageBox.Show(successMessage, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RemoveAttachment(int attachmentId)
        {
            var attachment = dm_AttachmentBUS.Instance.GetItemById(attachmentId);
            if (attachment == null)
            {
                return;
            }

            string sourcePath = BuildAttachmentPath(attachment);
            if (File.Exists(sourcePath))
            {
                File.Delete(sourcePath);
            }

            dm_AttachmentBUS.Instance.RemoveById(attachmentId);
        }

        private string BuildAttachmentPath(dm_Attachment attachment)
        {
            return Path.Combine(TPConfigs.Folder311, attachment.EncryptionName);
        }

        private dt311_Invoice GetFocusedInvoice()
        {
            GridView view = gvData;
            string invoiceId = view.GetRowCellValue(view.FocusedRowHandle, gColId)?.ToString();
            return string.IsNullOrWhiteSpace(invoiceId)
                ? null
                : dt311_InvoiceBUS.Instance.GetItemById(invoiceId);
        }

        private int GetFocusedIntValue(GridColumn column)
        {
            object value = gvData.GetRowCellValue(gvData.FocusedRowHandle, column);
            return value == null || value == DBNull.Value ? -1 : Convert.ToInt32(value);
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
            uc311_MoreInfo ucInfo = new uc311_MoreInfo();

            // Hiển thị dialog
            if (XtraDialog.Show(ucInfo, "輸入資訊", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            // Lấy dữ liệu từ control uc311_MoreInfo
            string description = ucInfo.Desc;
            string code = ucInfo.Code ?? "";
            string costDept = ucInfo.Dept ?? TPConfigs.LoginUser.IdDepartment;

            // Kiểm tra dữ liệu hợp lệ (ví dụ)
            if (string.IsNullOrWhiteSpace(description))
            {
                XtraMessageBox.Show("請輸入說明內容。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //====== 組合前綴資料 / Build Prefix Key ======
            const string TAB = "{Tab}";

            dt311_Invoice firstInvoice = selectedInvoices.First();
            string sellerTax = firstInvoice.SellerTax;
            string buyerTax = firstInvoice.BuyerTax;

            string prefixKey = string.Join(TAB, new string[]
            {
                "LG", "",
                costDept, "",
                "2",
                $"W{code}", "", "", "", "", "",
                sellerTax,
                buyerTax,
                "A1", "",
                description,
            });

            //====== 組合發票資料字串 / Build Invoice Data String ======
            IEnumerable<string> invoiceDataStrings = selectedInvoices.Select(invoice =>
            {
                //var vehicleInfo = dt311_VehicleManagementBUS.Instance.GetItemById(invoice.LicensePlate);
                //string deptSub = vehicleInfo != null ? vehicleInfo.IdDept : TPConfigs.LoginUser.IdDepartment;

                return string.Join(TAB, new string[]
                {
                    costDept, "",
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

            var subBitmap = ImageScanOpenCV.GetImage(Path.Combine(TPConfigs.Folder311_Template, "tempSubmitBtn.png"));
            erpDataPayload.Add(new ErpAction() { IsClick = true, TempImage = subBitmap });
            erpDataPayload.Add(new ErpAction() { Text = invoiceDataCombined });
            erpDataPayload.Add(new ErpAction() { Step = 2, Text = "%a" });
            erpDataPayload.Add(new ErpAction() { Step = 2, Text = "s" });
            erpDataPayload.Add(new ErpAction() { Step = 2, Text = "{ENTER}" });
            erpDataPayload.Add(new ErpAction() { Step = 2, Text = invoiceSubDataCombined });

            // Hiển thị cảnh báo trước khi mở form ERP / 顯示警告訊息
            MsgBoxAlert();

            // 🔹 Mở form AutoERP / 開啟 AutoERP 視窗
            using (f311_AutoERP formAutoERP = new f311_AutoERP(erpDataPayload))
            {
                formAutoERP.ShowDialog();
            }
        }

        private string GetCostDept()
        {
            var editor = new TextEdit
            {
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
            };

            var args = new XtraInputBoxArgs
            {
                Caption = "輸入成本部門",
                Prompt = "請輸入成本部門 (範例：7820)",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = editor.EditValue?.ToString() ?? ""
            };

            object result = XtraInputBox.Show(args);

            // Nếu người dùng nhấn Cancel
            if (result == null)
                return TPConfigs.LoginUser.IdDepartment;

            return result.ToString().Trim();
        }

        private void ItemERP02_Click(object sender, EventArgs e)
        {
            string costDept = GetCostDept();

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

            if (vehicle == null)
            {
                XtraMessageBox.Show("Không có thông tin phương tiện!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
                costDept, "",
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

                string vehicleTypeCode = (vehicle?.FuelType == "xăng/汽") ? "O" : "B";

                return string.Join(tabDelimiter, new[]
                {
                    invoice.LicensePlate ?? "",
                    "",
                    vehicleTypeCode,
                    "8",
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

                invoiceDatas = dt311_InvoiceBUS.Instance.GetListByStartDeptId(idDept2Word);
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
                int attId = -1;

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

                    attId = await DownloadInvoice(transactionId);
                }

                if (string.IsNullOrEmpty(xmlString))
                {
                    XtraMessageBox.Show("Không có dữ liệu XML trong phản hồi!", "Lỗi dữ liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var root = XElement.Parse(xmlString);
                var formats = JObject.Parse(File.ReadAllText(Path.Combine(TPConfigs.Folder311_Template, "invoice_formats.json")));

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
                    IdDept = idDept2Word,
                    CreateBy = TPConfigs.LoginUser.Id
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

                string sellerName = Regex.Replace(ExtractField(root, fmt["fields"]["seller_name"]), @"(?i)\s*Công\s*ty\s*TNHH(\s*MTV)?(\s*Thương\s*Mại)?\s*", "", RegexOptions.IgnoreCase).Trim();
                string buyerName = Regex.Replace(ExtractField(root, fmt["fields"]["buyer_name"]), @"(?i)\s*Công\s*ty\s*TNHH(\s*MTV)?(\s*Thương\s*Mại)?\s*", "", RegexOptions.IgnoreCase).Trim();


                // --- Seller & Buyer ---
                var seller = new dt311_SellerBuyer
                {
                    Tax = invoiceData.SellerTax,
                    DisplayName = sellerName
                };
                var buyer = new dt311_SellerBuyer
                {
                    Tax = invoiceData.BuyerTax,
                    DisplayName = buyerName
                };

                // --- Lưu dữ liệu ---
                if (attId != -1) invoiceData.AttId = attId;
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

                if (attId == -1 && !string.IsNullOrEmpty(transactionId))
                {
                    XtraMessageBox.Show("Lỗi không thể tải hóa đơn PDF!\nVui lòng đưa lên thủ công hoặc thử lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Lỗi khi xử lý hóa đơn:\n{ex.Message}",
                    "Lỗi xử lý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static async Task<int> DownloadInvoice(string invoice_id)
        {
            string url = $"{URL_INVOICE_PDF}{invoice_id}";
            string folderPath = TPConfigs.Folder311;
            string actualName = $"{invoice_id}.pdf";

            string encryptionName = EncryptionHelper.EncryptionFileName(actualName);

            var attachment = new dm_Attachment()
            {
                Thread = InvoiceAttachmentThread,
                EncryptionName = encryptionName,
                ActualName = actualName,
            };

            string filePath = System.IO.Path.Combine(folderPath, encryptionName);

            try
            {
                // Đảm bảo thư mục tồn tại
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                using (var client = new System.Net.Http.HttpClient())
                {
                    // Gửi yêu cầu GET tải file
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    // Đọc stream nội dung file
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }

                var attId = dm_AttachmentBUS.Instance.Add(attachment);
                return attId;
            }
            catch (Exception ex)
            {
                return -1;
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
                string typeOfSeller = view.GetRowCellValue(view.FocusedRowHandle, gColSellerType)?.ToString();
                int attId = GetFocusedIntValue(gColAttId);
                int fuelPhotoAttId = GetFocusedIntValue(gColFuelPhotoAttId);

                itemAddFile.BeginGroup = true;
                e.Menu.Items.Add(itemAddFile);
                if (attId != -1)
                {
                    e.Menu.Items.Add(itemViewFile);
                }

                if (typeOfSeller == "xang_dau")
                {
                    itemUpdateAddFuel.BeginGroup = true;
                    e.Menu.Items.Add(itemUpdateAddFuel);
                    e.Menu.Items.Add(itemAddFuelPhoto);
                    if (fuelPhotoAttId != -1)
                    {
                        e.Menu.Items.Add(itemViewFuelPhoto);
                    }
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
                        where seller.Type == "xang_dau" && inv.LicensePlate == null
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

        private void btnFillFuelTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!TryGetDateRange(out DateTime startDate, out DateTime endDate))
                return;

            try
            {
                var reportRows = BuildFuelPhotoReportRows(startDate, endDate);
                if (reportRows.Count == 0)
                {
                    XtraMessageBox.Show("所選日期區間沒有加油資料。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "PDF Files|*.pdf";
                    saveFileDialog.RestoreDirectory = true;
                    saveFileDialog.FileName = $"公務車加油記錄表-{startDate:yyyyMMdd}-{endDate:yyyyMMdd}-{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    if (saveFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        ExportFuelPhotoReportPdf(reportRows, saveFileDialog.FileName);
                    }

                    Process.Start(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"導出 PDF 失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFuelUsageStatistics_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!TryGetYearRange(out int startYear, out int endYear))
                return;

            try
            {
                using (SaveFileDialog saveFile = new SaveFileDialog())
                {
                    saveFile.RestoreDirectory = true;
                    saveFile.FileName = $"廠處使用燃料量統計表-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    saveFile.Filter = "Excel| *.xlsx";

                    if (saveFile.ShowDialog() != DialogResult.OK)
                        return;

                    using (var handle = SplashScreenManager.ShowOverlayForm(this))
                    {
                        ExportFuelUsageStatisticsExcel(saveFile.FileName, startYear, endYear);
                    }

                    Process.Start(saveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"導出 Excel 失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryGetDateRange(out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.MinValue;
            endDate = DateTime.MinValue;

            var editor = new TextEdit
            {
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                EditValue = $"{DateTime.Today.AddDays(-7):yyyy/MM/dd} - {DateTime.Today:yyyy/MM/dd}"
            };

            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegExpMaskManager));
            editor.Properties.MaskSettings.Set("mask", @"\d{4}/\d{2}/\d{2}\s*-\s*\d{4}/\d{2}/\d{2}");
            editor.Properties.MaskSettings.Set("isAutoComplete", true);
            editor.Properties.Mask.UseMaskAsDisplayFormat = true;
            editor.Properties.NullValuePrompt = "yyyy/MM/dd - yyyy/MM/dd";
            editor.Properties.NullValuePromptShowForEmptyValue = true;

            var args = new XtraInputBoxArgs
            {
                Caption = "選擇日期區間",
                Prompt = "請輸入日期區間 (格式 yyyy/MM/dd - yyyy/MM/dd)",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = editor.EditValue.ToString()
            };

            object result = XtraInputBox.Show(args);
            if (result == null)
                return false;

            string rangeText = result.ToString().Trim();
            string pattern = @"^\d{4}/\d{2}/\d{2}\s*-\s*\d{4}/\d{2}/\d{2}$";
            if (!Regex.IsMatch(rangeText, pattern))
            {
                XtraMessageBox.Show("格式錯誤！請輸入正確格式：yyyy/MM/dd - yyyy/MM/dd", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var parts = rangeText.Split('-');
            startDate = DateTime.ParseExact(parts[0].Trim(), "yyyy/MM/dd", CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact(parts[1].Trim(), "yyyy/MM/dd", CultureInfo.InvariantCulture);

            if (startDate > endDate)
            {
                XtraMessageBox.Show("起始日期不能大於結束日期！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool TryGetYearRange(out int startYear, out int endYear)
        {
            startYear = 0;
            endYear = 0;

            var editor = new TextEdit
            {
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                EditValue = $"{DateTime.Today.Year - 1} - {DateTime.Today.Year}"
            };

            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegExpMaskManager));
            editor.Properties.MaskSettings.Set("mask", @"\d{4}\s*-\s*\d{4}");
            editor.Properties.MaskSettings.Set("isAutoComplete", true);
            editor.Properties.Mask.UseMaskAsDisplayFormat = true;
            editor.Properties.NullValuePrompt = "yyyy - yyyy";
            editor.Properties.NullValuePromptShowForEmptyValue = true;

            var args = new XtraInputBoxArgs
            {
                Caption = "選擇年度區間",
                Prompt = "請輸入年度區間 (格式 yyyy - yyyy)",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = editor.EditValue.ToString()
            };

            object result = XtraInputBox.Show(args);
            if (result == null)
                return false;

            string rangeText = result.ToString().Trim();
            string pattern = @"^\d{4}\s*-\s*\d{4}$";
            if (!Regex.IsMatch(rangeText, pattern))
            {
                XtraMessageBox.Show("格式錯誤！請輸入正確格式：yyyy - yyyy", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string[] parts = rangeText.Split('-');
            startYear = Convert.ToInt32(parts[0].Trim());
            endYear = Convert.ToInt32(parts[1].Trim());

            if (startYear > endYear)
            {
                XtraMessageBox.Show("起始年份不能大於結束年份！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private List<FuelPhotoReportRow> BuildFuelPhotoReportRows(DateTime startDate, DateTime endDate)
        {
            DateTime queryEnd = endDate.Date.AddDays(1).AddTicks(-1);
            var sellerTypeByTax = (sellerBuyers ?? dt311_SellerBuyerBUS.Instance.GetList())
                .Where(r => !string.IsNullOrWhiteSpace(r.Tax))
                .GroupBy(r => r.Tax)
                .ToDictionary(g => g.Key, g => g.First().Type);

            var fuelInvoices = dt311_InvoiceBUS.Instance.GetListByStartDeptId(idDept2Word, startDate.Date, queryEnd)
                .Where(r => r.IssueDate.HasValue
                    && !string.IsNullOrWhiteSpace(r.LicensePlate)
                    && sellerTypeByTax.TryGetValue(r.SellerTax ?? string.Empty, out string sellerType)
                    && sellerType == "xang_dau")
                .ToList();

            if (fuelInvoices.Count == 0)
                return new List<FuelPhotoReportRow>();

            var invoiceIds = new HashSet<string>(fuelInvoices.Select(r => r.TransactionID));
            var quantityByInvoice = dt311_InvoiceItemBUS.Instance.GetList()
                .Where(r => invoiceIds.Contains(r.IdInvoice))
                .GroupBy(r => r.IdInvoice)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Quantity ?? 0m));

            var vehicles = dt311_VehicleManagementBUS.Instance.GetList()
                .Where(r => !string.IsNullOrWhiteSpace(r.LicensePlate))
                .GroupBy(r => r.LicensePlate)
                .ToDictionary(g => g.Key, g => g.First());

            return fuelInvoices
                .Select(invoice =>
                {
                    vehicles.TryGetValue(invoice.LicensePlate, out dt311_VehicleManagement vehicle);
                    quantityByInvoice.TryGetValue(invoice.TransactionID, out decimal quantity);

                    return new FuelPhotoReportRow
                    {
                        LicensePlate = invoice.LicensePlate,
                        FuelTypeDisplay = GetFuelCategoryName(vehicle?.FuelType),
                        IssueDate = invoice.IssueDate.Value,
                        Quantity = quantity,
                        FuelPhotoAttId = invoice.FuelPhotoAttId
                    };
                })
                .OrderBy(r => r.LicensePlate)
                .ThenBy(r => r.IssueDate)
                .ToList();
        }

        private void ExportFuelPhotoReportPdf(List<FuelPhotoReportRow> reportRows, string outputPath)
        {
            Spire.License.LicenseProvider.SetLicenseKey(TPConfigs.KeySpirePPT);

            var attachmentIds = reportRows
                .Where(r => r.FuelPhotoAttId.HasValue && r.FuelPhotoAttId.Value > 0)
                .Select(r => r.FuelPhotoAttId.Value)
                .Distinct()
                .ToList();

            var attachmentPathById = dm_AttachmentBUS.Instance.GetListById(attachmentIds)
                .Where(r => !string.IsNullOrWhiteSpace(r.EncryptionName))
                .ToDictionary(r => r.Id, BuildAttachmentPath);

            using (PdfDocument document = new PdfDocument())
            using (System.Drawing.Font titleFontGdi = new System.Drawing.Font(PdfCjkFontName, 16f, FontStyle.Bold))
            using (System.Drawing.Font sectionFontGdi = new System.Drawing.Font(PdfCjkFontName, 12f, FontStyle.Bold))
            using (System.Drawing.Font contentFontGdi = new System.Drawing.Font(PdfCjkFontName, 10.5f, FontStyle.Regular))
            using (System.Drawing.Font latinFontGdi = new System.Drawing.Font(PdfLatinFontName, 9f, FontStyle.Regular))
            using (PdfTrueTypeFont titleFont = new PdfTrueTypeFont(titleFontGdi, true))
            using (PdfTrueTypeFont sectionFont = new PdfTrueTypeFont(sectionFontGdi, true))
            using (PdfTrueTypeFont contentFont = new PdfTrueTypeFont(contentFontGdi, true))
            using (PdfTrueTypeFont latinFont = new PdfTrueTypeFont(latinFontGdi, true))
            {
                document.PageSettings.Size = PdfPageSize.A4;
                document.DocumentInformation.Title = "公務車加油記錄表";
                document.DocumentInformation.Creator = TPConfigs.SoftNameEN ?? "KnowledgeSystem";
                document.DocumentInformation.Author = TPConfigs.LoginUser?.DisplayName ?? TPConfigs.LoginUser?.Id ?? "KnowledgeSystem";

                foreach (var vehicleGroup in reportRows.GroupBy(r => r.LicensePlate))
                {
                    PdfNewPage page = null;
                    float y = 0f;
                    int vehiclePageIndex = 0;

                    foreach (FuelPhotoReportRow row in vehicleGroup)
                    {
                        if (page == null)
                        {
                            vehiclePageIndex++;
                            page = (PdfNewPage)document.Pages.Add();
                            y = DrawFuelReportPageHeader(page, vehicleGroup.Key, titleFont, sectionFont, vehiclePageIndex);
                        }

                        float entryHeight = GetFuelReportEntryHeight();
                        float pageBottom = page.Canvas.ClientSize.Height - FuelPhotoPdfPageMargin;
                        if (y + entryHeight > pageBottom)
                        {
                            vehiclePageIndex++;
                            page = (PdfNewPage)document.Pages.Add();
                            y = DrawFuelReportPageHeader(page, vehicleGroup.Key, titleFont, sectionFont, vehiclePageIndex);
                        }

                        y = DrawFuelReportEntry(page, row, contentFont, latinFont, attachmentPathById, y);
                    }
                }

                document.SaveToFile(outputPath);
            }
        }

        private float DrawFuelReportPageHeader(PdfNewPage page, string licensePlate, PdfTrueTypeFont titleFont, PdfTrueTypeFont sectionFont, int pageIndex)
        {
            float pageWidth = page.Canvas.ClientSize.Width;
            float contentWidth = pageWidth - (FuelPhotoPdfPageMargin * 2);
            float y = FuelPhotoPdfPageMargin;

            page.Canvas.DrawString(
                "公務車加油記錄表",
                titleFont,
                PdfBrushes.Black,
                new RectangleF(FuelPhotoPdfPageMargin, y, contentWidth, 24f),
                new PdfStringFormat
                {
                    Alignment = PdfTextAlignment.Center,
                    LineAlignment = PdfVerticalAlignment.Middle
                });

            y += 30f;
            string vehicleTitle = pageIndex == 1 ? $"車號：{licensePlate}" : $"車號：{licensePlate}（續）";
            page.Canvas.DrawString(vehicleTitle, sectionFont, PdfBrushes.Black, FuelPhotoPdfPageMargin, y);
            y += 18f;
            page.Canvas.DrawLine(PdfPens.DarkGray, FuelPhotoPdfPageMargin, y, pageWidth - FuelPhotoPdfPageMargin, y);
            return y + 12f;
        }

        private float DrawFuelReportEntry(
            PdfNewPage page,
            FuelPhotoReportRow row,
            PdfTrueTypeFont contentFont,
            PdfTrueTypeFont latinFont,
            Dictionary<int, string> attachmentPathById,
            float y)
        {
            float contentWidth = page.Canvas.ClientSize.Width - (FuelPhotoPdfPageMargin * 2);
            string fuelDisplay = string.IsNullOrWhiteSpace(row.FuelTypeDisplay) ? "燃料" : row.FuelTypeDisplay;

            page.Canvas.DrawString(
                $"{row.LicensePlate} 於 {row.IssueDate:yyyy/MM/dd} 報車加{fuelDisplay}記錄",
                contentFont,
                PdfBrushes.Black,
                new RectangleF(FuelPhotoPdfPageMargin, y, contentWidth, 18f));

            y += 22f;
            page.Canvas.DrawString(
                $"加{fuelDisplay}表：{row.Quantity:0.###} Lit",
                latinFont,
                PdfBrushes.Black,
                new RectangleF(FuelPhotoPdfPageMargin, y, contentWidth, 16f));

            y += 24f;
            RectangleF imageBounds = new RectangleF(FuelPhotoPdfPageMargin, y, contentWidth, FuelPhotoPdfImageHeight);
            if (TryLoadFuelPhoto(row.FuelPhotoAttId, attachmentPathById, out Image fuelPhoto))
            {
                using (fuelPhoto)
                {
                    DrawImageInsideBounds(page, fuelPhoto, imageBounds);
                }
            }
            else
            {
                page.Canvas.DrawRectangle(PdfPens.Gray, imageBounds);
                page.Canvas.DrawString(
                    "未上傳加油照片",
                    contentFont,
                    PdfBrushes.DarkGray,
                    imageBounds,
                    new PdfStringFormat
                    {
                        Alignment = PdfTextAlignment.Center,
                        LineAlignment = PdfVerticalAlignment.Middle
                    });
            }

            return imageBounds.Bottom + FuelPhotoPdfEntrySpacing;
        }

        private float GetFuelReportEntryHeight()
        {
            return 22f + 24f + FuelPhotoPdfImageHeight + FuelPhotoPdfEntrySpacing;
        }

        private bool TryLoadFuelPhoto(int? attachmentId, Dictionary<int, string> attachmentPathById, out Image image)
        {
            image = null;
            if (!attachmentId.HasValue || attachmentId.Value <= 0)
                return false;

            if (!attachmentPathById.TryGetValue(attachmentId.Value, out string imagePath) || !File.Exists(imagePath))
                return false;

            using (Image original = Image.FromFile(imagePath))
            {
                image = new Bitmap(original);
            }

            return true;
        }

        private void DrawImageInsideBounds(PdfNewPage page, Image image, RectangleF bounds)
        {
            PdfImage pdfImage = PdfImage.FromImage(image);
            float widthRatio = bounds.Width / image.Width;
            float heightRatio = bounds.Height / image.Height;
            float scale = Math.Min(widthRatio, heightRatio);
            float drawWidth = image.Width * scale;
            float drawHeight = image.Height * scale;
            float drawX = bounds.X + ((bounds.Width - drawWidth) / 2f);
            float drawY = bounds.Y + ((bounds.Height - drawHeight) / 2f);

            page.Canvas.DrawRectangle(PdfPens.LightGray, bounds);
            page.Canvas.DrawImage(pdfImage, drawX, drawY, drawWidth, drawHeight);
        }

        private void ExportFuelUsageStatisticsExcel(string outputPath, int startYear, int endYear)
        {
            DateTime startDate = new DateTime(startYear, 1, 1);
            DateTime endDate = new DateTime(endYear, 12, 31).AddDays(1).AddTicks(-1);

            var sellerTypeByTax = (sellerBuyers ?? dt311_SellerBuyerBUS.Instance.GetList())
                .Where(r => !string.IsNullOrWhiteSpace(r.Tax))
                .GroupBy(r => r.Tax)
                .ToDictionary(g => g.Key, g => g.First().Type);

            var invoices = dt311_InvoiceBUS.Instance.GetListByStartDeptId(idDept2Word, startDate, endDate)
                .Where(r => r.IssueDate.HasValue
                    && !string.IsNullOrWhiteSpace(r.LicensePlate)
                    && sellerTypeByTax.TryGetValue(r.SellerTax ?? string.Empty, out string sellerType)
                    && sellerType == "xang_dau")
                .ToList();

            if (invoices.Count == 0)
                throw new InvalidOperationException("所選年度區間沒有加油資料。");

            var invoiceIds = new HashSet<string>(invoices.Select(r => r.TransactionID));
            var quantityByInvoice = dt311_InvoiceItemBUS.Instance.GetList()
                .Where(r => invoiceIds.Contains(r.IdInvoice))
                .GroupBy(r => r.IdInvoice)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Quantity ?? 0m));

            var vehicles = dt311_VehicleManagementBUS.Instance.GetList()
                .Where(r => !string.IsNullOrWhiteSpace(r.LicensePlate))
                .GroupBy(r => r.LicensePlate)
                .ToDictionary(g => g.Key, g => g.First());

            var sourceRows = invoices
                .Select(invoice =>
                {
                    vehicles.TryGetValue(invoice.LicensePlate, out dt311_VehicleManagement vehicle);
                    quantityByInvoice.TryGetValue(invoice.TransactionID, out decimal quantity);

                    return new FuelUsageStatisticRow
                    {
                        FuelCategory = GetFuelCategoryName(vehicle?.FuelType),
                        Year = invoice.IssueDate.Value.Year,
                        Month = invoice.IssueDate.Value.Month,
                        Quantity = quantity
                    };
                })
                .ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                BuildFuelUsageWorksheet(package, "汽油", sourceRows.Where(r => r.FuelCategory == "汽油").ToList(), startYear, endYear);
                BuildFuelUsageWorksheet(package, "柴油", sourceRows.Where(r => r.FuelCategory == "柴油").ToList(), startYear, endYear);
                package.SaveAs(new FileInfo(outputPath));
            }
        }

        private void BuildFuelUsageWorksheet(ExcelPackage package, string sheetName, List<FuelUsageStatisticRow> rows, int startYear, int endYear)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1:M1"].Merge = true;
            worksheet.Cells["A1"].Value = $"{sheetName}每月加油量年度比較";
            worksheet.Cells["A1"].Style.Font.Bold = true;
            worksheet.Cells["A1"].Style.Font.Size = 16;
            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[3, 1].Value = "年份";
            for (int month = 1; month <= 12; month++)
            {
                worksheet.Cells[3, month + 1].Value = $"{month}月";
            }

            int currentRow = 4;
            for (int year = startYear; year <= endYear; year++)
            {
                worksheet.Cells[currentRow, 1].Value = $"{year}年";
                for (int month = 1; month <= 12; month++)
                {
                    decimal quantity = rows
                        .Where(r => r.Year == year && r.Month == month)
                        .Sum(r => (decimal)r.Quantity);

                    worksheet.Cells[currentRow, month + 1].Value = quantity;
                    worksheet.Cells[currentRow, month + 1].Style.Numberformat.Format = "0.###";
                }

                currentRow++;
            }

            using (var headerRange = worksheet.Cells[3, 1, 3, 13])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(217, 225, 242));
            }

            using (var dataRange = worksheet.Cells[3, 1, currentRow - 1, 13])
            {
                dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                dataRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            worksheet.Column(1).Width = 12;
            for (int col = 2; col <= 13; col++)
            {
                worksheet.Column(col).Width = 10;
            }

            var chart = worksheet.Drawings.AddChart($"chart_{sheetName}", eChartType.ColumnClustered);
            chart.Title.Text = $"{sheetName}每月加油量年度比較";
            chart.SetPosition(1, 0, 14, 0);
            chart.SetSize(900, 360);
            chart.YAxis.Title.Text = "Lit";
            chart.XAxis.Title.Text = "月份";
            chart.Legend.Position = eLegendPosition.Top;

            var xRange = worksheet.Cells[3, 2, 3, 13];
            for (int row = 4; row < currentRow; row++)
            {
                var series = (ExcelBarChartSerie)chart.Series.Add(worksheet.Cells[row, 2, row, 13], xRange);
                series.Header = worksheet.Cells[row, 1].Value?.ToString();
                series.DataLabel.ShowValue = true;
                series.DataLabel.Position = eLabelPosition.OutEnd;
            }

            worksheet.View.FreezePanes(4, 2);
        }

        private string GetFuelCategoryName(string fuelType)
        {
            return IsGasolineFuelType(fuelType) ? "汽油" : "柴油";
        }

        private bool IsGasolineFuelType(string fuelType)
        {
            if (string.IsNullOrWhiteSpace(fuelType))
                return false;

            string normalized = fuelType.Trim().ToLowerInvariant();
            return normalized.Contains("xăng") || normalized.Contains("汽");
        }
    }
}
