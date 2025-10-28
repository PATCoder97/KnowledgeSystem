using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt205_BaseBUS
    {
        TPLogger logger;

        private static dt205_BaseBUS instance;

        public static dt205_BaseBUS Instance
        {
            get { if (instance == null) instance = new dt205_BaseBUS(); return instance; }
            private set { instance = value; }
        }

        private dt205_BaseBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt205_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt205_Base.Where(r => string.IsNullOrEmpty(r.RemoveBy)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt205_Base> GetListByKeyword(string keyword)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Nếu không nhập keyword → trả về tất cả chưa xóa
                    if (string.IsNullOrWhiteSpace(keyword))
                    {
                        return _context.dt205_Base
                            .Where(r => string.IsNullOrEmpty(r.RemoveBy))
                            .ToList();
                    }

                    // 🔍 Tìm bằng FULL-TEXT SQL
                    string sql = @"
                SELECT *
                FROM dt205_Base
                WHERE RemoveBy IS NULL AND CreateDate IS NOT NULL AND
                (
                    FREETEXT(DisplayName, @p0)
                    OR FREETEXT(DisplayNameVN, @p0)
                    OR FREETEXT(DisplayNameEN, @p0)
                    OR FREETEXT(Keyword, @p0)
                )";

                    var result = _context.Database.SqlQuery<dt205_Base>(sql, keyword).ToList();

                    // 🔁 Fallback nếu chưa index xong hoặc không ra kết quả
                    if (result == null || result.Count == 0)
                    {
                        result = _context.dt205_Base
                            .Where(r =>
                                string.IsNullOrEmpty(r.RemoveBy) && r.CreateDate != null &&
                                (
                                    r.DisplayName.Contains(keyword) ||
                                    r.DisplayNameVN.Contains(keyword) ||
                                    r.DisplayNameEN.Contains(keyword) ||
                                    r.Keyword.Contains(keyword)
                                )
                            )
                            .ToList();
                    }

                    // ⚙️ Cache toàn bộ bảng để đệ quy nhanh
                    var allItems = _context.dt205_Base
                        .Where(r => string.IsNullOrEmpty(r.RemoveBy))
                        .Select(r => new { r.Id, r.IdParent, r.IsFinalNode })
                        .ToList();

                    // 🧩 Thu thập cha
                    HashSet<int> collectedParentIds = new HashSet<int>();
                    Action<int> CollectParents = null;

                    CollectParents = parentId =>
                    {
                        if (parentId <= 0 || collectedParentIds.Contains(parentId))
                            return;

                        collectedParentIds.Add(parentId);

                        var parent = allItems.FirstOrDefault(x => x.Id == parentId);
                        if (parent != null && parent.IdParent > 0)
                            CollectParents(parent.IdParent);
                    };

                    foreach (var item in result.Where(r => r.IdParent > 0))
                        CollectParents(item.IdParent);

                    // 🧩 Thu thập con (chỉ của các node có IsFinalNode = true)
                    HashSet<int> collectedChildIds = new HashSet<int>();
                    Action<int> CollectChildren = null;

                    CollectChildren = parentId =>
                    {
                        var children = allItems.Where(x => x.IdParent == parentId && x.IsFinalNode == true).ToList();
                        foreach (var child in children)
                        {
                            if (!collectedChildIds.Contains(child.Id))
                            {
                                collectedChildIds.Add(child.Id);
                                CollectChildren(child.Id); // đệ quy con
                            }
                        }
                    };

                    foreach (var item in result)
                        CollectChildren(item.Id);

                    // 🧩 Lấy bản ghi cha + con từ DB
                    List<dt205_Base> allParents = new List<dt205_Base>();
                    if (collectedParentIds.Any())
                    {
                        allParents = _context.dt205_Base
                            .Where(r => collectedParentIds.Contains(r.Id))
                            .ToList();
                    }

                    List<dt205_Base> allChildren = new List<dt205_Base>();
                    if (collectedChildIds.Any())
                    {
                        allChildren = _context.dt205_Base
                            .Where(r => collectedChildIds.Contains(r.Id))
                            .ToList();
                    }

                    // 🔗 Hợp tất cả kết quả (từ khóa + cha + con), loại trùng
                    result = result
                        .Concat(allParents)
                        .Concat(allChildren)
                        .GroupBy(r => r.Id)
                        .Select(g => g.First())
                        .ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt205_Base> GetAllChildByParentId(int parentId)
        {
            var allChildren = new List<dt205_Base>();
            var visited = new HashSet<int>(); // To track visited nodes and avoid circular references

            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Fetch all records in one query to reduce database calls
                    var allRecords = GetList();
                    var parent = allRecords.FirstOrDefault(r => r.Id == parentId);

                    if (parent == null)
                    {
                        return allChildren;
                    }

                    // Recursive local function
                    void FetchChildren(dt205_Base current)
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

        public dt205_Base GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt205_Base.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt205_Base item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt205_Base.Add(item);
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

        public bool AddRange(List<dt205_Base> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt205_Base.AddRange(items);
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

        public bool AddOrUpdate(dt205_Base item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt205_Base.AddOrUpdate(item);
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

        public bool UpdateRange(List<dt205_Base> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Thêm hoặc cập nhật các bản ghi trong vòng lặp
                    foreach (var item in items)
                    {
                        _context.dt205_Base.AddOrUpdate(item); // Dùng AddOrUpdate để thêm hoặc cập nhật
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

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt205_Base.FirstOrDefault(r => r.Id == id);
                    _context.dt205_Base.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
