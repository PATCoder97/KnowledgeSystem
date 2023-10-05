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
    public class dt207_Base_BAKBUS
    {
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public List<dt207_Base> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Base_BAK.ToList().Select(r => new dt207_Base
                    {
                        Id = r.Id,
                        DisplayName = r.DisplayName,
                        IdTypes = r.IdTypes,
                        Keyword = r.Keyword,
                        UserUpload = r.UserUpload,
                        UserProcess = r.UserProcess,
                        UploadDate = r.UploadDate,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Create(dt207_Base baseEntity)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    dt207_Base_BAK _newItem = new dt207_Base_BAK()
                    {
                        Id = baseEntity.Id,
                        DisplayName = baseEntity.DisplayName,
                        IdTypes = baseEntity.IdTypes,
                        Keyword = baseEntity.Keyword,
                        UserUpload = baseEntity.UserUpload,
                        UserProcess = baseEntity.UserProcess,
                        UploadDate = baseEntity.UploadDate
                    };
                    _context.dt207_Base_BAK.Add(_newItem);
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

        public bool Update(dt207_Base baseEntity)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    dt207_Base_BAK _itemUpdate = new dt207_Base_BAK()
                    {
                        Id = baseEntity.Id,
                        DisplayName = baseEntity.DisplayName,
                        IdTypes = baseEntity.IdTypes,
                        Keyword = baseEntity.Keyword,
                        UserUpload = baseEntity.UserUpload,
                        UserProcess = baseEntity.UserProcess,
                        UploadDate = baseEntity.UploadDate
                    };
                    _context.dt207_Base_BAK.AddOrUpdate(_itemUpdate);
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

        public bool Delete(string entityId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var entity = _context.dt207_Base_BAK.FirstOrDefault(r => r.Id == entityId);
                    _context.dt207_Base_BAK.Remove(entity);

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
