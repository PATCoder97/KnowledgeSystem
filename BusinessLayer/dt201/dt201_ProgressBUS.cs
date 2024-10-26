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
    public class dt201_ProgressBUS
    {
        TPLogger logger;

        private static dt201_ProgressBUS instance;

        public static dt201_ProgressBUS Instance
        {
            get { if (instance == null) instance = new dt201_ProgressBUS(); return instance; }
            private set { instance = value; }
        }

        private dt201_ProgressBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt201_Progress> GetListByIdBase(int idForm)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Progress.Where(r => r.IdForm == idForm).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public int Add(dt201_Progress item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Progress.Add(item);
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

        public bool AddRange(List<dt201_Progress> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Progress.AddRange(items);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public List<dt201_Progress> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Progress.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt201_Progress> GetListByIdForm(int idForm)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Progress.Where(r => r.IdForm == idForm).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddOrUpdate(dt201_Progress item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Progress.AddOrUpdate(item);
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

        public bool Remove(int IdForm)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt201_Progress.FirstOrDefault(r => r.Id == IdForm);
                    _context.dt201_Progress.Remove(entity);
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

        public bool RemoveByIdForm(int IdForm)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt201_Progress.Where(r => r.IdForm == IdForm);
                    _context.dt201_Progress.RemoveRange(entity);
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
