﻿using DataAccessLayer;
using Logger;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class dt301_CertReqSetBUS
    {
        TPLogger logger;

        private static dt301_CertReqSetBUS instance;

        public static dt301_CertReqSetBUS Instance
        {
            get { if (instance == null) instance = new dt301_CertReqSetBUS(); return instance; }
            private set { instance = value; }
        }

        private dt301_CertReqSetBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt301_CertReqSetting> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_CertReqSetting.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt301_CertReqSetting> GetListByDept(string _idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_CertReqSetting.Where(r => r.IdDept == _idDept).OrderByDescending(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Lấy danh sách quy định các chứng chỉ phải học theo chức vụ của Bộ phận
        /// </summary>
        /// <param name="_idJobTitle"></param>
        /// <param name="_idDept"></param>
        /// <returns></returns>
        public List<dt301_CertReqSetting> GetListByJobAndDept(string _idJobTitle, string _idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt301_CertReqSetting.Where(r => r.IdJobTitle == _idJobTitle && r.IdDept == _idDept).OrderByDescending(r => r.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dt301_CertReqSetting _certreq)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt301_CertReqSetting.Add(_certreq);
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

        public bool AddOrUpdate(dt301_CertReqSetting _certreq)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dt301_CertReqSetting.AddOrUpdate(_certreq);
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
                    var _itemDel = _context.dt301_CertReqSetting.FirstOrDefault(r => r.Id == _id);
                    _context.dt301_CertReqSetting.Remove(_itemDel);

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
