using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Configs
{
    public class PCInfoHelper
    {
        private static PCInfoHelper instance;

        public static PCInfoHelper Instance
        {
            get { if (instance == null) instance = new PCInfoHelper(); return instance; }
            private set { instance = value; }
        }

        private PCInfoHelper()
        {
        }

        public string GetPCName()
        {
            return Environment.MachineName;
        }

        public string GetIPAddress()
        {
            // Get the IP addresses associated with the host name
            IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());

            // Find and display the first IPv4 address
            IPAddress firstIPv4Address = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return firstIPv4Address?.ToString() ?? "";
        }
    }
}
