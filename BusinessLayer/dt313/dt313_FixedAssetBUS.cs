using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class dt313_FixedAssetBUS
    {
        TPLogger logger;

        private static dt313_FixedAssetBUS instance;

        public static dt313_FixedAssetBUS Instance
        {
            get { if (instance == null) instance = new dt313_FixedAssetBUS(); return instance; }
            private set { instance = value; }
        }

        private dt313_FixedAssetBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt313_FixedAsset> GetAll()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_FixedAsset> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset.Where(r => r.IsDeleted != true).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_FixedAsset> GetListByDept(string idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset
                        .Where(r => r.IsDeleted != true && r.IdDept.StartsWith(idDept))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt313_FixedAsset> GetListByManager(string idManager)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset
                        .Where(r => r.IsDeleted != true && r.IdManager == idManager)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_FixedAsset GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt313_FixedAsset GetItemByCode(string assetCode)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt313_FixedAsset.FirstOrDefault(r => r.AssetCode == assetCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt313_FixedAsset item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_FixedAsset.Add(item);
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

        public bool AddRange(List<dt313_FixedAsset> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_FixedAsset.AddRange(items);
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

        public bool AddOrUpdate(dt313_FixedAsset item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt313_FixedAsset.AddOrUpdate(item);
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

        public int AddOrUpdateByCode(dt313_FixedAsset item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var currentItem = _context.dt313_FixedAsset.FirstOrDefault(r => r.AssetCode == item.AssetCode);
                    if (currentItem != null)
                    {
                        item.Id = currentItem.Id;
                        item.CreatedBy = currentItem.CreatedBy;
                        item.CreatedDate = currentItem.CreatedDate;
                        item.IsDeleted = currentItem.IsDeleted;
                        item.DeletedBy = currentItem.DeletedBy;
                        item.DeletedDate = currentItem.DeletedDate;
                    }

                    _context.dt313_FixedAsset.AddOrUpdate(item);
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

        public bool RemoveById(int id, string userDel)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt313_FixedAsset.FirstOrDefault(r => r.Id == id);
                    if (itemRemove == null) return false;

                    itemRemove.IsDeleted = true;
                    itemRemove.DeletedBy = userDel;
                    itemRemove.DeletedDate = DateTime.Now;
                    itemRemove.UpdatedBy = userDel;
                    itemRemove.UpdatedDate = DateTime.Now;

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
