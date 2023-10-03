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

        public List<User> GetLists()
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

        public bool CreateUser(User _user)
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

        public bool UpdateUser(User _user)
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

        public bool DeleteUser(string _idUser)
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
