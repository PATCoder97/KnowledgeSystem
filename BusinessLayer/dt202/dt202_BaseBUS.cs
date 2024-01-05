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
    public class dt202_BaseBUS
    {
        TPLogger logger;

        private static dt202_BaseBUS instance;

        public static dt202_BaseBUS Instance
        {
            get { if (instance == null) instance = new dt202_BaseBUS(); return instance; }
            private set { instance = value; }
        }

        private dt202_BaseBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public string GetNewBaseId(string _idDept, int _indexId = 1, string _startIdStr = "")
        {
            if (string.IsNullOrEmpty(_startIdStr))
            {
                _startIdStr = $"{_idDept.Substring(0, 3)}-{DateTime.Now.ToString("yyMMddHHmm")}-";
            }

            string tempId = $"{_startIdStr}{_indexId:d2}";
            using (var db = new DBDocumentManagementSystemEntities())
            {
                bool isExistsId = db.dt202_Base.Any(kb => kb.Id == tempId);
                if (!isExistsId)
                {
                    return tempId;
                }
            }

            return GetNewBaseId(_idDept, _indexId + 1, _startIdStr);
        }

        public List<dt202_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt202_Base.ToList();
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
        public dt202_Base GetItemById(string idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt202_Base.FirstOrDefault(r => r.Id == idBase);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt202_Base _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt202_Base.Add(_base);
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

        public bool AddOrUpdate(dt202_Base _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt202_Base.AddOrUpdate(_base);
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
