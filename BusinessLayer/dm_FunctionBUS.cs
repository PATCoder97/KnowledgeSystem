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
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

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
                    }).ToList();
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
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public bool Create(dm_FunctionM _function)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
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

        public bool Update(dm_FunctionM _function)
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

        public bool Delete(int _idFunction)
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
