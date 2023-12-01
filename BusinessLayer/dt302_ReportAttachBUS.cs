using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt302_ReportAttachBUS
    {
        TPLogger logger;

        private static dt302_ReportAttachBUS instance;

        public static dt302_ReportAttachBUS Instance
        {
            get { if (instance == null) instance = new dt302_ReportAttachBUS(); return instance; }
            private set { instance = value; }
        }

        private dt302_ReportAttachBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public int Add(dt302_ReportAttach reportAtt)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt302_ReportAttach.Add(reportAtt);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return reportAtt.Id;
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

        public List<dt302_ReportAttach> GetListByReport(int idReport)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt302_ReportAttach.Where(r => r.IdReport == idReport).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Dùng danh sách IdReport để lấy danh sách liên kết report và phụ kiện
        /// </summary>
        /// <param name="List IdReport"></param>
        /// <returns></returns>
        public List<dt302_ReportAttach> GetListByListReport(List<int> idsReport)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt302_ReportAttach.Where(r => idsReport.Contains(r.IdReport)).ToList();
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
