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
    public class dm_GroupUserBUS
    {
        TPLogger logger;

        private static dm_GroupUserBUS instance;

        public static dm_GroupUserBUS Instance
        {
            get { if (instance == null) instance = new dm_GroupUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_GroupUserBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_GroupUser> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_GroupUser.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dm_GroupUser> GetListByUID(string _UID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_GroupUser.Where(r => r.IdUser == _UID).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dm_GroupUser> GetListByIdGroup(int _idGroup)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_GroupUser.Where(r => r.IdGroup == _idGroup).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddRange(List<dm_GroupUser> lsGroupUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_GroupUser.AddRange(lsGroupUser);
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

        public bool Add(dm_GroupUser groupUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_GroupUser.Add(groupUser);
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

        public bool AddOrUpdate(dm_GroupUser groupUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_GroupUser.AddOrUpdate(groupUser);
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
                    var _lsItemDel = _context.dm_GroupUser.Where(r => r.IdUser == idUsr).ToList();
                    _context.dm_GroupUser.RemoveRange(_lsItemDel);

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

        public bool Remove(int groupUserId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var groupUser = _context.dm_GroupUser.FirstOrDefault(r => r.Id == groupUserId);
                    _context.dm_GroupUser.Remove(groupUser);

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

        public bool RemoveRangeByIdGroup(int _idGroup)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_GroupUser.RemoveRange(_context.dm_GroupUser.Where(r => r.IdGroup == _idGroup));
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
