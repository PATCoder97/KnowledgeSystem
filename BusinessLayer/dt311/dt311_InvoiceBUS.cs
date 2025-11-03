using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt311_InvoiceBUS
    {
        TPLogger logger;

        private static dt311_InvoiceBUS instance;

        public static dt311_InvoiceBUS Instance
        {
            get { if (instance == null) instance = new dt311_InvoiceBUS(); return instance; }
            private set { instance = value; }
        }

        private dt311_InvoiceBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt311_Invoice> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListByStartDeptId(string deptId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.Where(r => r.IdDept.StartsWith(deptId)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListFuleByStartDeptId(string deptId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.Where(r => r.IdDept.StartsWith(deptId) && !string.IsNullOrEmpty(r.LicensePlate)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetListByStartDeptId(string deptId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice
                        .Where(r => r.IdDept.StartsWith(deptId)
                                    && r.IssueDate >= startDate
                                    && r.IssueDate <= endDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt311_Invoice> GetFuelLogWithPrevious(string deptId, DateTime startDate, DateTime endDate)
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                // 🔹 Lấy toàn bộ dữ liệu trước endDate để tìm lần đổ trước đó
                var allLogs = _context.dt311_Invoice
                    .Where(f => f.IdDept.StartsWith(deptId)
                             && f.IssueDate <= endDate
                             && !string.IsNullOrEmpty(f.LicensePlate))
                    .OrderBy(f => f.LicensePlate)
                    .ThenBy(f => f.IssueDate)
                    .ToList();

                // 🔹 Lọc ra các bản ghi nằm trong khoảng thời gian mong muốn
                var rangeLogs = allLogs
                    .Where(f => f.IssueDate >= startDate && f.IssueDate <= endDate)
                    .ToList();

                // 🔹 Tạo danh sách kết quả kèm theo bản ghi lần đổ trước đó (nếu có)
                var result = rangeLogs
                    .Select(f =>
                    {
                        // Lấy bản ghi trước đó theo xe, có thể nằm ngoài startDate
                        var prev = allLogs
                            .Where(p => p.LicensePlate == f.LicensePlate && p.IssueDate < f.IssueDate)
                            .OrderByDescending(p => p.IssueDate)
                            .FirstOrDefault();

                        return new dt311_Invoice
                        {
                            LicensePlate = f.LicensePlate,
                            // BuyerTax dùng để hiển thị ngày trước và sau (tuỳ bạn đặt tên cột)
                            BuyerTax = $"{prev?.IssueDate:yyyy/MM/dd} - {f.IssueDate:yyyy/MM/dd}",
                            // SellerTax hiển thị km trước và sau
                            SellerTax = $"{prev?.OdometerReading} - {f.OdometerReading}",
                            // OdometerReading hiển thị chênh lệch km
                            OdometerReading = prev != null ? f.OdometerReading - prev.OdometerReading : 0,
                            FuelFilledBy = f.FuelFilledBy,
                            InvoiceCode = f.InvoiceCode,
                            InvoiceNumber = f.InvoiceNumber,
                            TransactionID = f.TransactionID,
                            TotalAfterVAT = f.TotalAfterVAT,
                            TotalBeforeVAT = f.TotalBeforeVAT
                        };
                    })
                    .ToList();

                return result;
            }
        }

        public dt311_Invoice GetItemById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt311_Invoice GetItemBytransactionID(string transactionID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == transactionID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public string Add(dt311_Invoice item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                        return item.TransactionID;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        public bool AddRange(List<dt311_Invoice> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public string AddOrUpdate(dt311_Invoice item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt311_Invoice.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    if (affectedRecords > 0)
                        return item.TransactionID;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return null;
            }
        }

        public bool RemoveById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt311_Invoice.FirstOrDefault(r => r.TransactionID == id);
                    _context.dt311_Invoice.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
