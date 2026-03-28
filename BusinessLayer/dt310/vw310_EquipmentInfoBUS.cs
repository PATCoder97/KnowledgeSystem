using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLayer
{
    public class vw310_EquipmentInfoBUS
    {
        TPLogger logger;

        private static vw310_EquipmentInfoBUS instance;

        public static vw310_EquipmentInfoBUS Instance
        {
            get { if (instance == null) instance = new vw310_EquipmentInfoBUS(); return instance; }
            private set { instance = value; }
        }

        private vw310_EquipmentInfoBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<vw310_EquipmentInfo> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<vw310_EquipmentInfo> GetListByDeptId(string deptId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo
                        .Where(r => r.DeptId == deptId)
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<vw310_EquipmentInfo> GetListByDeptPrefix(string deptPrefix)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo
                        .Where(r => r.DeptId != null && r.DeptId.StartsWith(deptPrefix))
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<vw310_EquipmentInfo> GetListByManagerId(string managerId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo
                        .Where(r => r.ManagerId == managerId)
                        .OrderBy(r => r.Code)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public vw310_EquipmentInfo GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public vw310_EquipmentInfo GetItemByCode(string code)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.vw310_EquipmentInfo.FirstOrDefault(r => r.Code == code);
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
