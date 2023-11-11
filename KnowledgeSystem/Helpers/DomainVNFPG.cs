using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    internal class DomainVNFPG
    {
        private static DomainVNFPG instance;
        public static string domainVNFPG = "vn.fpg.com";

        public static DomainVNFPG Instance
        {
            get { if (instance == null) instance = new DomainVNFPG(); return instance; }
            private set { instance = value; }
        }

        private DomainVNFPG()
        {
        }

        public bool CheckLoginDomain(string userID_, string password_)
        {
            bool isLoginSuccess = false;

            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainVNFPG))
                {
                    isLoginSuccess = pc.ValidateCredentials(userID_, password_,
                        ContextOptions.Negotiate | ContextOptions.SecureSocketLayer |
                        ContextOptions.SimpleBind | ContextOptions.ServerBind);
                }
            }
            catch
            {
                isLoginSuccess = false;
            }
            return isLoginSuccess;
        }

        public string GetAccountName(string userID_)
        {
            string userDisplayName = string.Empty;

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainVNFPG))
            {
                var usr = UserPrincipal.FindByIdentity(pc, userID_);
                if (usr != null)
                    userDisplayName = usr.DisplayName;
            }

            return userDisplayName;
        }
    }
}
