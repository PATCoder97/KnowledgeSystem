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
    public class sys_NotesMailBUS
    {
        TPLogger logger;

        private static sys_NotesMailBUS instance;

        public static sys_NotesMailBUS Instance
        {
            get { if (instance == null) instance = new sys_NotesMailBUS(); return instance; }
            private set { instance = value; }
        }

        private sys_NotesMailBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<sys_NotesMail> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.sys_NotesMail.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách Mail chưa thông báo theo Thread
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public List<sys_NotesMail> GetListNotNotifyByThread(string thread)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.sys_NotesMail.Where(r => r.TimeNotify == null && r.Thread == thread).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(sys_NotesMail mail)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.sys_NotesMail.Add(mail);
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

        public bool AddOrUpdate(sys_NotesMail mail)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.sys_NotesMail.AddOrUpdate(mail);
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
