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
    public class dt207_DocProcessingInfoBUS
    {
        TPLogger logger;

        private static dt207_DocProcessingInfoBUS instance;

        public static dt207_DocProcessingInfoBUS Instance
        {
            get { if (instance == null) instance = new dt207_DocProcessingInfoBUS(); return instance; }
            private set { instance = value; }
        }

        private dt207_DocProcessingInfoBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt207_DocProcessingInfo> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_DocProcessingInfo.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt207_DocProcessingInfo> GetListNotNotify()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_DocProcessingInfo.Where(r => string.IsNullOrEmpty(r.TimeNotifyNotes.ToString())).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt207_DocProcessingInfo> GetListByIdDocProcess(int _idDocProcess)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_DocProcessingInfo.Where(r => r.IdDocProgress == _idDocProcess).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt207_DocProcessingInfo GetItemById(int _id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_DocProcessingInfo.FirstOrDefault(r => r.Id == _id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt207_DocProcessingInfo docProgressInfo)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_DocProcessingInfo.Add(docProgressInfo);
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

        public bool AddOrUpdate(dt207_DocProcessingInfo docProgressInfo)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_DocProcessingInfo.AddOrUpdate(docProgressInfo);
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

        public bool Remove(int docProgressInfoId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var docProgressInfo = _context.dt207_DocProcessingInfo.FirstOrDefault(r => r.Id == docProgressInfoId);
                    _context.dt207_DocProcessingInfo.Remove(docProgressInfo);

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
