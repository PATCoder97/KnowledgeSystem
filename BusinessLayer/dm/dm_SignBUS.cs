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
    public class dm_SignBUS
    {
        TPLogger logger;

        private static dm_SignBUS instance;

        public static dm_SignBUS Instance
        {
            get { if (instance == null) instance = new dm_SignBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_SignBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_Sign> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Sign.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dm_Sign _sign)
        {
            try
            {
                //_base.IsDelete = false;
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Sign.Add(_sign);
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

        public bool AddOrUpdate(dm_Sign _sign)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Sign.AddOrUpdate(_sign);
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
        public bool Remove(int _id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _itemDel = _context.dm_Sign.FirstOrDefault(r => r.Id == _id);
                    _context.dm_Sign.Remove(_itemDel);

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
