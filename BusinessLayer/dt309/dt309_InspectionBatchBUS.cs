using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using Logger;

namespace BusinessLayer
{
    public class dt309_InspectionBatchBUS
    {
        TPLogger logger;

        private static dt309_InspectionBatchBUS instance;

        public static dt309_InspectionBatchBUS Instance
        {
            get { if (instance == null) instance = new dt309_InspectionBatchBUS(); return instance; }
            private set { instance = value; }
        }

        private dt309_InspectionBatchBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt309_InspectionBatch> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt309_InspectionBatch.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt309_InspectionBatch GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt309_InspectionBatch.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt309_InspectionBatch item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt309_InspectionBatch.Add(item);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return item.Id;
                    }

                    return -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt309_InspectionBatch> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt309_InspectionBatch.AddRange(items);
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

        public bool AddOrUpdate(dt309_InspectionBatch item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt309_InspectionBatch.AddOrUpdate(item);
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

        public bool CancelBatch(int batchId, string currentUserId, string cancelReason, out string message)
        {
            message = string.Empty;

            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var batch = _context.dt309_InspectionBatch.FirstOrDefault(item => item.Id == batchId);
                    if (batch == null)
                    {
                        message = "找不到對應盤點批次。";
                        return false;
                    }

                    if (batch.IsCancelled)
                    {
                        message = "此批次已取消。";
                        return false;
                    }

                    bool isCompleted = !_context.dt309_InspectionBatchMaterial
                        .Any(item => item.BatchId == batchId && item.IsComplete != true);
                    if (isCompleted)
                    {
                        message = "已完成批次不可取消。";
                        return false;
                    }

                    batch.IsCancelled = true;
                    batch.CancelledBy = currentUserId;
                    batch.CancelledDate = DateTime.Now;
                    batch.CancelReason = string.IsNullOrWhiteSpace(cancelReason)
                        ? null
                        : cancelReason.Trim();

                    return _context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(nameof(CancelBatch), ex.ToString());
                message = ex.Message;
                return false;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt309_InspectionBatch.FirstOrDefault(r => r.Id == id);
                    _context.dt309_InspectionBatch.Remove(itemRemove);

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
