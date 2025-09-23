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
    public class dt307_InterviewReportBUS
    {
        TPLogger logger;

        private static dt307_InterviewReportBUS instance;

        public static dt307_InterviewReportBUS Instance
        {
            get { if (instance == null) instance = new dt307_InterviewReportBUS(); return instance; }
            private set { instance = value; }
        }

        private dt307_InterviewReportBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt307_InterviewReport> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt307_InterviewReport.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt307_InterviewReport GetItemById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt307_InterviewReport.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt307_InterviewReport item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    // Tạo prefix theo năm + tháng (yyyyMM)
                    string prefix = DateTime.Now.ToString("yyyyMM");

                    // Lấy Id lớn nhất trong tháng này
                    var maxId = _context.dt307_InterviewReport
                        .Where(x => x.Id.StartsWith(prefix))
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.Id)
                        .FirstOrDefault();

                    string newId;
                    if (string.IsNullOrEmpty(maxId))
                    {
                        // Nếu chưa có bản ghi nào trong tháng → bắt đầu từ 01
                        newId = prefix + "01";
                    }
                    else
                    {
                        // Lấy phần số cuối cùng rồi +1
                        int lastNumber = int.Parse(maxId.Substring(6, 2));
                        newId = prefix + (lastNumber + 1).ToString("D2");
                    }

                    // Gán Id cho item
                    item.Id = newId;

                    // Thêm vào DbContext
                    _context.dt307_InterviewReport.Add(item);
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


        public bool AddRange(List<dt307_InterviewReport> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt307_InterviewReport.AddRange(items);
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

        public bool AddOrUpdate(dt307_InterviewReport item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt307_InterviewReport.AddOrUpdate(item);
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

        public bool RemoveById(string id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt307_InterviewReport.FirstOrDefault(r => r.Id == id);
                    _context.dt307_InterviewReport.Remove(itemRemove);

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
