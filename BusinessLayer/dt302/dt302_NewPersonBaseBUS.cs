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
    public class dt302_NewPersonBaseBUS
    {
        TPLogger logger;

        private static dt302_NewPersonBaseBUS instance;

        public static dt302_NewPersonBaseBUS Instance
        {
            get { if (instance == null) instance = new dt302_NewPersonBaseBUS(); return instance; }
            private set { instance = value; }
        }

        private dt302_NewPersonBaseBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt302_NewPersonBase> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt302_NewPersonBase.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy 1 Nhân viên mới bằng Id 302
        /// </summary>
        /// <param name="idBase"></param>
        /// <returns></returns>
        public dt302_NewPersonBase GetItemById(int idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt302_NewPersonBase.FirstOrDefault(r => r.Id == idBase);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt302_NewPersonBase _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt302_NewPersonBase.Add(_base);
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

        public bool AddOrUpdate(dt302_NewPersonBase _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt302_NewPersonBase.AddOrUpdate(_base);
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
