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
    public class dt207_BaseBUS
    {
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public List<dt207_Base> GetListWithoutKeyword()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Base
                        .Where(r => !r.IsDelete)
                        .Select(r => new
                        {
                            r.Id,
                            r.DisplayName,
                            r.IdTypes,
                            r.UserUpload,
                            r.UserProcess,
                            r.UploadDate,
                            r.IsDelete
                        }).ToList()
                        .Select(r => new dt207_Base
                        {
                            Id = r.Id,
                            DisplayName = r.DisplayName,
                            IdTypes = r.IdTypes,
                            Keyword = "",
                            UserUpload = r.UserUpload,
                            UserProcess = r.UserProcess,
                            UploadDate = r.UploadDate,
                            IsDelete = r.IsDelete
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt207_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Base.Where(r => !r.IsDelete).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public string GetNewBaseId(string _idDept, int _indexId = 1, string _startIdStr = "")
        {
            if (string.IsNullOrEmpty(_startIdStr))
            {
                _startIdStr = $"{_idDept.Substring(0, 3)}-{DateTime.Now.ToString("yyMMddHHmm")}-";
            }

            string tempId = $"{_startIdStr}{_indexId:d2}";
            using (var db = new DBDocumentManagementSystemEntities())
            {
                bool isExistsId = db.dt207_Base.Any(kb => kb.Id == tempId);
                if (!isExistsId)
                {
                    return tempId;
                }
            }

            return GetNewBaseId(_idDept, _indexId + 1, _startIdStr);
        }

        public dt207_Base GetItemById(string _id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Base.Where(r => !r.IsDelete && r.Id == _id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Create(dt207_Base entity)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_Base.Add(entity);
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

        public bool AddOrUpdate(dt207_Base entity)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_Base.AddOrUpdate(entity);
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

        public bool Delete(string entityId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt207_Base.FirstOrDefault(r => r.Id == entityId);
                    entity.IsDelete = true;
                    _context.SaveChanges();

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
