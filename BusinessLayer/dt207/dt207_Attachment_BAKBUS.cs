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
    public class dt207_Attachment_BAKBUS
    {
        TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public List<dt207_Attachment> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Attachment_BAK.ToList()
                        .Select(r => new dt207_Attachment
                        {
                            Id = r.Id,
                            IdKnowledgeBase = r.IdKnowledgeBase,
                            EncryptionName = r.EncryptionName,
                            FileName = r.FileName,
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public List<dt207_Attachment> GetListByIdBase(string _idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == _idBase).ToList()
                        .Select(r => new dt207_Attachment
                        {
                            Id = r.Id,
                            IdKnowledgeBase = r.IdKnowledgeBase,
                            EncryptionName = r.EncryptionName,
                            FileName = r.FileName,
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }

        public bool Create(dt207_Attachment attachment)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    dt207_Attachment_BAK _newItem = new dt207_Attachment_BAK()
                    {
                        Id = attachment.Id,
                        IdKnowledgeBase = attachment.IdKnowledgeBase,
                        EncryptionName = attachment.EncryptionName,
                        FileName = attachment.FileName,
                    };

                    _context.dt207_Attachment_BAK.Add(_newItem);
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

        public bool CreateRange(List<dt207_Attachment> lsAttachments)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    List<dt207_Attachment_BAK> _lsNewItem = lsAttachments.Select(r => new dt207_Attachment_BAK
                    {
                        Id = r.Id,
                        IdKnowledgeBase = r.IdKnowledgeBase,
                        EncryptionName = r.EncryptionName,
                        FileName = r.FileName,
                    }).ToList();

                    _context.dt207_Attachment_BAK.AddRange(_lsNewItem);
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

        public bool Update(dt207_Attachment_BAK attachment)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    dt207_Attachment_BAK _itemUpdate = new dt207_Attachment_BAK()
                    {
                        Id = attachment.Id,
                        IdKnowledgeBase = attachment.IdKnowledgeBase,
                        EncryptionName = attachment.EncryptionName,
                        FileName = attachment.FileName,
                    };

                    _context.dt207_Attachment_BAK.AddOrUpdate(_itemUpdate);
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

        public bool Delete(int attachmentId)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var attachment = _context.dt207_Attachment_BAK.FirstOrDefault(r => r.Id == attachmentId);
                    _context.dt207_Attachment_BAK.Remove(attachment);

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

        public bool RemoveRangeByIdBase(string _idBase)
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    var attachment = _context.dt207_Attachment_BAK.Where(r => r.IdKnowledgeBase == _idBase);
                    _context.dt207_Attachment_BAK.RemoveRange(attachment);

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
