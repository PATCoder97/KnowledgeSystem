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
    public class dm_SignUsersBUS
    {
        TPLogger logger;

        private static dm_SignUsersBUS instance;

        public static dm_SignUsersBUS Instance
        {
            get { if (instance == null) instance = new dm_SignUsersBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_SignUsersBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_SignUsers> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_SignUsers.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public List<dm_SignUsers> GetListByUID(string _idUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_SignUsers.Where(r => r.IdUser == _idUser).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public List<dm_SignUsers> GetListBySign(int idSign)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_SignUsers.Where(r => r.IdSign == idSign).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public bool AddRange(List<dm_SignUsers> signUsers)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_SignUsers.AddRange(signUsers);
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

        public bool RemoveRangeByUID(string idUsr)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _lsItemDel = _context.dm_SignUsers.Where(r => r.IdUser == idUsr).ToList();
                    _context.dm_SignUsers.RemoveRange(_lsItemDel);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords >= 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool RemoveRangeBySign(int idSign)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _lsItemDel = _context.dm_SignUsers.Where(r => r.IdSign == idSign).ToList();
                    _context.dm_SignUsers.RemoveRange(_lsItemDel);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords >= 0;
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
