using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt313_InspectionBatchAssetBUS
    {
        TPLogger logger;

        private static dt313_InspectionBatchAssetBUS instance;

        public static dt313_InspectionBatchAssetBUS Instance
        {
            get { if (instance == null) instance = new dt313_InspectionBatchAssetBUS(); return instance; }
            private set { instance = value; }
        }

        private dt313_InspectionBatchAssetBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt313_InspectionBatchAsset> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_InspectionBatchAsset.OrderByDescending(r => r.CreatedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_InspectionBatchAsset> GetListByBatchId(int batchId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_InspectionBatchAsset.Where(r => r.BatchId == batchId).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_InspectionBatchAsset> GetAbnormalList(bool onlyOpen = false)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var query = _context.dt313_InspectionBatchAsset.Where(r => r.Result == "Abnormal");
                    if (onlyOpen)
                    {
                        query = query.Where(r => r.CorrectionStatus != "Closed");
                    }

                    return query.OrderByDescending(r => r.CreatedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_InspectionBatchAsset GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_InspectionBatchAsset.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_InspectionBatchAsset GetItemByBatchAndAsset(int batchId, int fixedAssetId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_InspectionBatchAsset.FirstOrDefault(r => r.BatchId == batchId && r.FixedAssetId == fixedAssetId);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt313_InspectionBatchAsset item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_InspectionBatchAsset.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0 ? item.Id : -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return -1;
            }
        }

        public bool AddRange(List<dt313_InspectionBatchAsset> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_InspectionBatchAsset.AddRange(items);
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

        public bool AddOrUpdate(dt313_InspectionBatchAsset item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_InspectionBatchAsset.AddOrUpdate(item);
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
    }
}
