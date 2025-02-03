using DataAccessLayer;
using Logger;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using System.Data.Entity.Migrations;

public class dm_WatermarkBUS
{
    TPLogger logger;

    private static dm_WatermarkBUS instance;

    public static dm_WatermarkBUS Instance
    {
        get { if (instance == null) instance = new dm_WatermarkBUS(); return instance; }
        private set { instance = value; }
    }

    private dm_WatermarkBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

    public List<dm_Watermark> GetList()
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                return _context.dm_Watermark.ToList();
            }
        }
        catch (Exception ex)
        {
            logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
            throw;
        }
    }

    public dm_Watermark GetItemById(int id)
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                return _context.dm_Watermark.FirstOrDefault(r => r.ID == id);
            }
        }
        catch (Exception ex)
        {
            logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
            throw;
        }
    }

    public bool Add(dm_Watermark item)
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                _context.dm_Watermark.Add(item);
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

    public bool AddRange(List<dm_Watermark> items)
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                _context.dm_Watermark.AddRange(items);
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

    public bool AddOrUpdate(dm_Watermark item)
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                _context.dm_Watermark.AddOrUpdate(item);
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

    public bool RemoveById(int id)
    {
        try
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                var itemRemove = _context.dm_Watermark.FirstOrDefault(r => r.ID == id);
                _context.dm_Watermark.Remove(itemRemove);

                int affectedRecords = _context.SaveChanges();
                return affectedRecords > 0;
            }
        }
        catch (Exception ex)
        {
            logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
            throw;
        }
    }
}