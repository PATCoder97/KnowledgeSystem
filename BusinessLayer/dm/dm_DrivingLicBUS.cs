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
    public class dm_DrivingLicBUS
    {
        TPLogger logger;

        private static dm_DrivingLicBUS instance;

        public static dm_DrivingLicBUS Instance
        {
            get { if (instance == null) instance = new dm_DrivingLicBUS(); return instance; }
            private set { instance = value; }
        }

        private dm_DrivingLicBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dm_DrivingLic> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dm_DrivingLic.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().ReflectedType.Name, ex.ToString());
                throw;
            }
        }
    }
}
