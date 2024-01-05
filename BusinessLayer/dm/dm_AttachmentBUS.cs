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
    public class dm_AttachmentBUS
    {
        TPLogger logger;

        private static dm_AttachmentBUS instance;

        public static dm_AttachmentBUS Instance
        {
            get { if (instance == null) instance = new dm_AttachmentBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_AttachmentBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        /// <summary>
        /// Thêm phụ kiện và trả về Id của phụ kiện vừa thêm
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>IdAttachment</returns>
        public int Add(dm_Attachment attachment)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_Attachment.Add(attachment);
                    int affectedRecords = _context.SaveChanges();

                    if (affectedRecords > 0)
                    {
                        return attachment.Id;
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

        /// <summary>
        /// Lấy các phụ kiện theo thread: 301, 302,...
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public List<dm_Attachment> GetListByThread(string thread)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Attachment.Where(r => r.Thread == thread).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy được các phụ kiện bằng danh sách id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<dm_Attachment> GetListById(List<int> ids)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Attachment.Where(r => ids.Contains(r.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dm_Attachment GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_Attachment.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemsRemove = _context.dm_Attachment.FirstOrDefault(r => r.Id == id);
                    _context.dm_Attachment.Remove(itemsRemove);

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
