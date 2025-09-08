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
    public class dm_DeptBUS
    {
        TPLogger logger;

        private static dm_DeptBUS instance;

        public static dm_DeptBUS Instance
        {
            get { if (instance == null) instance = new dm_DeptBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_DeptBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_Departments> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Departments.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dm_Departments> GetListByParent(string idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Departments.Where(d => d.IdParent == _context.dm_Departments
                                      .Where(p => p.Id == idDept)
                                      .Select(p => p.IdChild)
                                      .FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dm_Departments> GetAllChildren(int idChildDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var result = new List<dm_Departments>();

                    // Hàm đệ quy nội bộ
                    void CollectChildren(int parentId)
                    {
                        var children = _context.dm_Departments
                                               .Where(d => d.IdParent == parentId)
                                               .ToList();

                        foreach (var child in children)
                        {
                            result.Add(child);
                            // Gọi tiếp cho cấp con của child
                            if (child.IdChild != null)
                                CollectChildren((int)child.IdChild);
                        }
                    }

                    // Thêm chính nó vào trước
                    var root = _context.dm_Departments
                                       .FirstOrDefault(d => d.IdChild == idChildDept);
                    if (root != null)
                    {
                        result.Add(root);
                        CollectChildren(idChildDept);
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }


        public dm_Departments GetItemById(string _idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Departments.FirstOrDefault(r => r.Id == _idDept);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dm_Departments GetItemByParentId(int _idParent)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Departments.FirstOrDefault(r => r.IdParent == _idParent);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dm_Departments _dept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Departments.Add(_dept);
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

        public bool AddOrUpdate(dm_Departments _dept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Departments.AddOrUpdate(_dept);
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

        public bool Remove(string _idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _itemDel = _context.dm_Departments.FirstOrDefault(r => r.Id == _idDept);
                    _context.dm_Departments.Remove(_itemDel);

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
