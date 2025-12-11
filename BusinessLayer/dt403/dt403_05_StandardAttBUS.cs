using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.dt403
{
    public class dt403_05_StandardAttBUS
    {
        private static dt403_05_StandardAttBUS instance;

        public static dt403_05_StandardAttBUS Instance
        {
            get { if (instance == null) instance = new dt403_05_StandardAttBUS(); return instance; }
            private set { instance = value; }
        }

        private dt403_05_StandardAttBUS() { }

        public List<dt403_05_StandardAtt> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt403_05_StandardAtt.ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public dt403_05_StandardAtt GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt403_05_StandardAtt.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Add(dt403_05_StandardAtt item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_StandardAtt.Add(item);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddRange(List<dt403_05_StandardAtt> items)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_StandardAtt.AddRange(items);
                    int affectedRecords = _context.SaveChanges();
                    return affectedRecords > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddOrUpdate(dt403_05_StandardAtt item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt403_05_StandardAtt.AddOrUpdate(item);
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
                    var itemRemove = _context.dt403_05_StandardAtt.FirstOrDefault(r => r.Id == id);
                    _context.dt403_05_StandardAtt.Remove(itemRemove);

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
