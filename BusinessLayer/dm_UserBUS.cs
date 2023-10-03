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
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public List<User> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public User GetUserByUID(string _UID)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.Users.FirstOrDefault(r => r.Id == _UID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public User CheckLogin(string _UID, string _pass)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var user = _context.Users.FirstOrDefault(p => p.Id == _UID && p.SecondaryPassword == _pass);
                    return user;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public bool Create(User _user)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Users.Add(_user);
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

        public bool Update(User _user)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    _context.Users.AddOrUpdate(_user);
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

        public bool Delete(string _idUser)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var _userDel = _context.Users.FirstOrDefault(r => r.Id == _idUser);
                    _context.Users.Remove(_userDel);

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
