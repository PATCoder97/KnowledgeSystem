using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt313_FixedAssetPhotoBUS
    {
        TPLogger logger;

        private static dt313_FixedAssetPhotoBUS instance;

        public static dt313_FixedAssetPhotoBUS Instance
        {
            get { if (instance == null) instance = new dt313_FixedAssetPhotoBUS(); return instance; }
            private set { instance = value; }
        }

        private dt313_FixedAssetPhotoBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt313_FixedAssetPhoto> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAssetPhoto.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_FixedAssetPhoto> GetListByAssetId(int fixedAssetId, bool activeOnly = false)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var query = _context.dt313_FixedAssetPhoto.Where(r => r.FixedAssetId == fixedAssetId);
                    if (activeOnly)
                    {
                        query = query.Where(r => r.IsActive);
                    }

                    return query.OrderByDescending(r => r.UploadedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_FixedAssetPhoto GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAssetPhoto.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_FixedAssetPhoto GetActivePhoto(int fixedAssetId, string photoType)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAssetPhoto.FirstOrDefault(r => r.FixedAssetId == fixedAssetId
                        && r.PhotoType == photoType
                        && r.IsActive);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt313_FixedAssetPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_FixedAssetPhoto.Add(item);
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

        public bool AddOrUpdate(dt313_FixedAssetPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_FixedAssetPhoto.AddOrUpdate(item);
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

        public int AddOrReplace(dt313_FixedAssetPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var oldItems = _context.dt313_FixedAssetPhoto.Where(r => r.FixedAssetId == item.FixedAssetId
                        && r.PhotoType == item.PhotoType
                        && r.IsActive).ToList();

                    foreach (var oldItem in oldItems)
                    {
                        oldItem.IsActive = false;
                    }

                    item.IsActive = true;
                    _context.dt313_FixedAssetPhoto.Add(item);

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

        public bool DeactivateById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var item = _context.dt313_FixedAssetPhoto.FirstOrDefault(r => r.Id == id);
                    if (item == null) return false;

                    item.IsActive = false;
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
