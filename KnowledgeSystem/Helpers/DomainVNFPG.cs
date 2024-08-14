using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.InteropServices;
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
                        ContextOptions.Negotiate);
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
            string userDisplayName = null;

            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainVNFPG))
                {
                    var usr = UserPrincipal.FindByIdentity(pc, userID_);
                    if (usr != null)
                    {
                        userDisplayName = usr.DisplayName;
                    }
                }
            }
            catch (COMException ex) when (ex.HResult == unchecked((int)0x8007052E))
            {
                // The user name or password is incorrect
                Console.WriteLine("The user name or password is incorrect.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            return userDisplayName;
        }
    }
}
