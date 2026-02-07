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
    public class dt310_UpdateLeaveUserBUS
    {
        TPLogger logger;

        private static dt310_UpdateLeaveUserBUS instance;

        public static dt310_UpdateLeaveUserBUS Instance
        {
            get { if (instance == null) instance = new dt310_UpdateLeaveUserBUS(); return instance; }
            private set { instance = value; }
        }

        private dt310_UpdateLeaveUserBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt310_UpdateLeaveUser> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_UpdateLeaveUser.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt310_UpdateLeaveUser GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt310_UpdateLeaveUser.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }


        public int Add(dt310_UpdateLeaveUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt310_UpdateLeaveUser.Add(item);
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

        public bool AddRange(List<dt310_UpdateLeaveUser> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt310_UpdateLeaveUser.AddRange(items);
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

        public bool AddOrUpdate(dt310_UpdateLeaveUser item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt310_UpdateLeaveUser.AddOrUpdate(item);
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

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt310_UpdateLeaveUser.FirstOrDefault(r => r.Id == id);
                    _context.dt310_UpdateLeaveUser.Remove(itemRemove);

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
