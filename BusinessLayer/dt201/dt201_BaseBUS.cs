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
    public class dt201_BaseBUS
    {
        TPLogger logger;

        private static dt201_BaseBUS instance;

        public static dt201_BaseBUS Instance
        {
            get { if (instance == null) instance = new dt201_BaseBUS(); return instance; }
            private set { instance = value; }
        }

        private dt201_BaseBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public int Add(dt201_Base item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Base.Add(item);
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

        public List<dt201_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Base.Where(r => r.IsDel != true).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt201_Base> GetListByParentId(int idParent)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Base.Where(r => r.IsDel != true && r.IdParent == idParent).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt201_Base> GetAllChildByParentId(int parentId)
        {
            var allChildren = new List<dt201_Base>();
            var visited = new HashSet<int>(); // To track visited nodes and avoid circular references

            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Fetch all records in one query to reduce database calls
                    var allRecords = _context.dt201_Base.Where(r => r.IsDel != true).ToList();
                    var parent = allRecords.FirstOrDefault(r => r.Id == parentId);

                    if (parent == null)
                    {
                        return allChildren;
                    }

                    // Recursive local function
                    void FetchChildren(dt201_Base current)
                    {
                        if (current == null || visited.Contains(current.Id))
                            return;

                        visited.Add(current.Id);
                        allChildren.Add(current);

                        var children = allRecords.Where(r => r.IdParent == current.Id).ToList();
                        foreach (var child in children)
                        {
                            FetchChildren(child);
                        }
                    }

                    FetchChildren(parent);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }

            return allChildren; // Return the entire list of children
        }

        public dt201_Base GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Base.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt201_Base GetParentById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var child = _context.dt201_Base.FirstOrDefault(r => r.Id == id);
                    if (child == null)
                        return null;

                    return _context.dt201_Base.FirstOrDefault(r => r.Id == child.IdParent);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddOrUpdate(dt201_Base item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Base.AddOrUpdate(item);
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

        public bool UpdateRange(List<dt201_Base> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Thêm hoặc cập nhật các bản ghi trong vòng lặp
                    foreach (var item in items)
                    {
                        _context.dt201_Base.AddOrUpdate(item); // Dùng AddOrUpdate để thêm hoặc cập nhật
                    }

                    // Gọi SaveChanges một lần sau khi tất cả bản ghi đã được thêm/cập nhật
                    int affectedRecords = _context.SaveChanges();

                    // Kiểm tra nếu có bản ghi nào đã bị ảnh hưởng (thêm/cập nhật)
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về false nếu xảy ra lỗi
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                return false;
            }
        }

        public bool Remove(int Id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt201_Base.FirstOrDefault(r => r.Id == Id);
                    entity.IsDel = true;
                    _context.dt201_Base.AddOrUpdate(entity);
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
