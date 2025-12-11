using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.dt403
{
    public class dt403_05_StandardBUS
    {
        private static dt403_05_StandardBUS instance;

        public static dt403_05_StandardBUS Instance
        {
            get { if (instance == null) instance = new dt403_05_StandardBUS(); return instance; }
            private set { instance = value; }
        }

        private dt403_05_StandardBUS() { }

        public List<dt403_05_Standard> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt403_05_Standard.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public dt403_05_Standard GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt403_05_Standard.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Add(dt403_05_Standard item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_Standard.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddRange(List<dt403_05_Standard> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_Standard.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddOrUpdate(dt403_05_Standard item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_Standard.AddOrUpdate(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool RemoveById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var itemRemove = _context.dt403_05_Standard.FirstOrDefault(r => r.Id == id);
                    _context.dt403_05_Standard.Remove(itemRemove);

                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
