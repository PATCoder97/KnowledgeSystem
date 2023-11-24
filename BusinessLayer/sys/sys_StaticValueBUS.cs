using DataAccessLayer;
using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class sys_StaticValueBUS
    {
        TPLogger logger;

        private static sys_StaticValueBUS instance;

        public static sys_StaticValueBUS Instance
        {
            get { if (instance == null) instance = new sys_StaticValueBUS(); return instance; }
            private set { instance = value; }
        }

        private sys_StaticValueBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<sys_StaticValue> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.sys_StaticValue.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw new Exception(ex.ToString());
            }
        }
    }
}
