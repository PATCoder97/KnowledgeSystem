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
    public class dm_GroupBUS
    {
        TPLogger logger;

        private static dm_GroupBUS instance;

        public static dm_GroupBUS Instance
        {
            get { if (instance == null) instance = new dm_GroupBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_GroupBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_Group> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Group.OrderBy(r => r.Prioritize).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách nhóm bằng tên nhóm. VD: ISO組
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <returns></returns>
        public List<dm_Group> GetListByName(string nameGroup)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Group.Where(r => r.DisplayName == nameGroup).OrderBy(r => r.Prioritize).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy nhóm người dùng bằng tên
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <returns></returns>
        public dm_Group GetItemByName(string nameGroup)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Group.FirstOrDefault(r => r.DisplayName == nameGroup);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dm_Group group)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Group.Add(group);
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

        public bool AddOrUpdate(dm_Group group)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Group.AddOrUpdate(group);
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

        public bool Remove(int groupId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var group = _context.dm_Group.FirstOrDefault(r => r.Id == groupId);
                    _context.dm_Group.Remove(group);

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
