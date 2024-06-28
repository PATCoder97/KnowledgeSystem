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
    public class dm_FunctionBUS
    {
        TPLogger logger;

        private static dm_FunctionBUS instance;

        public static dm_FunctionBUS Instance
        {
            get { if (instance == null) instance = new dm_FunctionBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_FunctionBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_FunctionM> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Function.Select(r => new dm_FunctionM
                    {
                        Id = r.Id,
                        IdParent = r.IdParent,
                        DisplayName = r.DisplayName,
                        ControlName = r.ControlName,
                        Prioritize = r.Prioritize,
                        Status = r.Status,
                        Images = r.Images,
                    }).OrderBy(r => r.Prioritize).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public List<dm_FunctionM> GetListByIdParent(int _idParent)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Function
                        .Where(r => r.IdParent == _idParent)
                        .Select(r => new dm_FunctionM
                        {
                            Id = r.Id,
                            IdParent = r.IdParent,
                            DisplayName = r.DisplayName,
                            ControlName = r.ControlName,
                            Prioritize = r.Prioritize,
                            Status = r.Status,
                            Images = r.Images,
                        }).OrderBy(r => r.Prioritize).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public dm_Function GetItemByControl(string controlName)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Function.FirstOrDefault(r => r.ControlName == controlName);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public int GetNewId()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    int currentId = _context.dm_Function
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.Id)
                        .FirstOrDefault();

                    if (currentId == default(int))
                    {
                        return 1;
                    }
                    else
                    {
                        return currentId + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public bool Add(dm_FunctionM _functionM)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    dm_Function _function = new dm_Function()
                    {
                        Id = _functionM.Id,
                        IdParent = _functionM.IdParent,
                        DisplayName = _functionM.DisplayName,
                        ControlName = _functionM.ControlName,
                        Prioritize = _functionM.Prioritize,
                        Status = _functionM.Status,
                        Images = _functionM.Images,
                    };

                    _context.dm_Function.Add(_function);
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

        public bool AddOrUpdate(dm_FunctionM _function)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Function.AddOrUpdate(_function);
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

        public bool Remove(int _idFunction)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _itemDel = _context.dm_Function.FirstOrDefault(r => r.Id == _idFunction);
                    _context.dm_Function.Remove(_itemDel);

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
