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
    public class dt207_Security_BAKBUS
    {
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public List<dt207_Security_BAK> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Security_BAK.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt207_Security> GetListByIdBase(string _idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == _idBase).ToList()
                        .Select(r => new dt207_Security
                        {
                            Id = r.Id,
                            IdKnowledgeBase = r.IdKnowledgeBase,
                            IdGroup = r.IdGroup,
                            IdUser = r.IdUser,
                            ReadInfo = r.ReadInfo,
                            UpdateInfo = r.UpdateInfo,
                            DeleteInfo = r.DeleteInfo,
                            SearchInfo = r.SearchInfo,
                            ReadFile = r.ReadFile,
                            SaveFile = r.SaveFile,
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Create(dt207_Security_BAK security)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_Security_BAK.Add(security);
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

        public bool CreateRange(List<dt207_Security> lsSecurities)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    List<dt207_Security_BAK> lsCreate = lsSecurities.Select(r => new dt207_Security_BAK
                    {
                        Id = r.Id,
                        IdKnowledgeBase = r.IdKnowledgeBase,
                        IdGroup = r.IdGroup,
                        IdUser = r.IdUser,
                        ReadInfo = r.ReadInfo,
                        UpdateInfo = r.UpdateInfo,
                        DeleteInfo = r.DeleteInfo,
                        SearchInfo = r.SearchInfo,
                        ReadFile = r.ReadFile,
                        SaveFile = r.SaveFile,
                    }).ToList();

                    _context.dt207_Security_BAK.AddRange(lsCreate);
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

        public bool Update(dt207_Security_BAK security)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt207_Security_BAK.AddOrUpdate(security);
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

        public bool Delete(int securityId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var security = _context.dt207_Security_BAK.FirstOrDefault(r => r.Id == securityId);
                    _context.dt207_Security_BAK.Remove(security);

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

        public bool RemoveRangeByIdBase(string _idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var security = _context.dt207_Security_BAK.Where(r => r.IdKnowledgeBase == _idBase);
                    _context.dt207_Security_BAK.RemoveRange(security);

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
