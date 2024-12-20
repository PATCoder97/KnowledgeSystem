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
    public class dt301_BaseBUS
    {
        TPLogger logger;

        private static dt301_BaseBUS instance;

        public static dt301_BaseBUS Instance
        {
            get { if (instance == null) instance = new dt301_BaseBUS(); return instance; }
            private set { instance = value; }
        }

        private dt301_BaseBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt301_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách bằng mã bộ phận
        /// </summary>
        /// <returns></returns>
        public List<dt301_Base> GetListByDept(string idDept2Word)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.Where(r => r.IdDept == idDept2Word).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách bằng theo mã nhân viên và đang còn hạn
        /// </summary>
        /// <returns></returns>
        public List<dt301_Base> GetListByUIDAndValidCert(string UID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.Where(r => r.IdUser == UID && r.ValidLicense).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách chứng chỉ đang đình chỉ theo mã nhân viên
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public List<dt301_Base> GetListByUIDAndCertSuspended(string UID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.Where(r => r.IdUser == UID && r.CertSuspended).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy ra danh sách chứng chỉ còn hạn bằng UID và mã chức vụ
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="jobTitle"></param>
        /// <returns></returns>
        public List<dt301_Base> GetListValidCertByUIDAndJobTitle(string UID, string jobTitle)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.Where(r => r.IdUser == UID && r.CertSuspended).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public dt301_Base GetItemById(int id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_Base.FirstOrDefault(r => r.Id == id);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt301_Base _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt301_Base.Add(_base);
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

        public bool AddOrUpdate(dt301_Base _base)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt301_Base.AddOrUpdate(_base);
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

        public bool Remove(int _id)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _itemDel = _context.dt301_Base.FirstOrDefault(r => r.Id == _id);
                    _context.dt301_Base.Remove(_itemDel);

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
