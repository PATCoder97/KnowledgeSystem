﻿using DataAccessLayer;
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
    public class dt201_FormsBUS
    {
        TPLogger logger;

        private static dt201_FormsBUS instance;

        public static dt201_FormsBUS Instance
        {
            get { if (instance == null) instance = new dt201_FormsBUS(); return instance; }
            private set { instance = value; }
        }

        private dt201_FormsBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public int Add(dt201_Forms item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Forms.Add(item);
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

        public List<dt201_Forms> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Forms.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt201_Forms> GetListByIdBase(int idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Forms.Where(r =>  r.IdBase == idBase).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt201_Forms> GetListProcessing()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Forms.Where(r => r.IsProcessing == true).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt201_Forms GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt201_Forms.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool AddOrUpdate(dt201_Forms item)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt201_Forms.AddOrUpdate(item);
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

        public bool Remove(int Id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt201_Forms.FirstOrDefault(r => r.Id == Id);
                    _context.dt201_Forms.Remove(entity);
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