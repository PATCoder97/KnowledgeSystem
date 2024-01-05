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
    public class dt202_TypeBUS
    {
        TPLogger logger;

        private static dt202_TypeBUS instance;

        public static dt202_TypeBUS Instance
        {
            get { if (instance == null) instance = new dt202_TypeBUS(); return instance; }
            private set { instance = value; }
        }

        private dt202_TypeBUS() { logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName); }

        public List<dt202_Type> GetList()
        {
            try
            {
                using (var _context = new DBDocumentManagementSystemEntities())
                {
                    return _context.dt202_Type.ToList();
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
