using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt202_AttachBUS
    {
        TPLogger logger;

        private static dt202_AttachBUS instance;

        public static dt202_AttachBUS Instance
        {
            get { if (instance == null) instance = new dt202_AttachBUS(); return instance; }
            private set { instance = value; }
        }

        private dt202_AttachBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public int Add(dt202_Attach reportAtt)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt202_Attach.Add(reportAtt);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return reportAtt.Id;
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

        public List<dt202_Attach> GetListByBase(string idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt202_Attach.Where(r => r.IdBase == idBase).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Dùng danh sách IdReport để lấy danh sách liên kết report và phụ kiện
        /// </summary>
        /// <param name="List IdReport"></param>
        /// <returns></returns>
        public List<dt202_Attach> GetListByListBases(List<string> idsBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt202_Attach.Where(r => idsBase.Contains(r.IdBase)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool RemoveByIdAtt(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemsRemove = _context.dt202_Attach.Where(r => r.IdAttach == id).ToList();
                    _context.dt202_Attach.RemoveRange(itemsRemove);

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

        public bool RemoveRangeByIdBase(string _idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var lsAttachments = _context.dt202_Attach.Where(r => r.IdBase == _idBase);
                    _context.dt202_Attach.RemoveRange(lsAttachments);

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
