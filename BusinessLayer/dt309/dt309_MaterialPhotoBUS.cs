using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt309_MaterialPhotoBUS
    {
        TPLogger logger;

        private static dt309_MaterialPhotoBUS instance;

        public static dt309_MaterialPhotoBUS Instance
        {
            get { if (instance == null) instance = new dt309_MaterialPhotoBUS(); return instance; }
            private set { instance = value; }
        }

        private dt309_MaterialPhotoBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt309_MaterialPhoto> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt309_MaterialPhoto.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt309_MaterialPhoto> GetListByMaterialId(int materialId, bool activeOnly = false)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var query = _context.dt309_MaterialPhoto.Where(r => r.MaterialId == materialId);
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

        public dt309_MaterialPhoto GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt309_MaterialPhoto.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt309_MaterialPhoto GetActivePhoto(int materialId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt309_MaterialPhoto.FirstOrDefault(r => r.MaterialId == materialId && r.IsActive);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt309_MaterialPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt309_MaterialPhoto.Add(item);
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

        public bool AddOrUpdate(dt309_MaterialPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt309_MaterialPhoto.AddOrUpdate(item);
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

        public int AddOrReplace(dt309_MaterialPhoto item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var oldItems = _context.dt309_MaterialPhoto.Where(r => r.MaterialId == item.MaterialId && r.IsActive).ToList();

                    foreach (var oldItem in oldItems)
                    {
                        oldItem.IsActive = false;
                    }

                    item.IsActive = true;
                    _context.dt309_MaterialPhoto.Add(item);

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
                    var item = _context.dt309_MaterialPhoto.FirstOrDefault(r => r.Id == id);
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
