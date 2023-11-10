using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using System.Data.Entity.Migrations;

namespace BusinessLayer
{
    public class dm_UserBUS
    {
        TPLogger logger;

        private static dm_UserBUS instance;

        public static dm_UserBUS Instance
        {
            get { if (instance == null) instance = new dm_UserBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_UserBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_User> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_User.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public List<dm_User> GetListByDept(string _idDept)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_User.Where(r => r.IdDepartment.StartsWith(_idDept)).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public dm_User GetItemById(string _UID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_User.FirstOrDefault(r => r.Id == _UID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public dm_User CheckLogin(string _UID, string _pass)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var user = _context.dm_User.FirstOrDefault(p => p.Id == _UID && p.SecondaryPassword == _pass);
                    return user;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Add(dm_User _user)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_User.Add(_user);
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

        public bool AddOrUpdate(dm_User _user)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.dm_User.AddOrUpdate(_user);
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

        public bool Remove(string _idUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _userDel = _context.dm_User.FirstOrDefault(r => r.Id == _idUser);
                    _context.dm_User.Remove(_userDel);

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

        public bool ChangePass(string _idUser, string _newPass)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _userUpdate = _context.dm_User.FirstOrDefault(r => r.Id == _idUser);
                    _userUpdate.SecondaryPassword = _newPass;

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
