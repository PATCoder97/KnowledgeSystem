using BusinessLayer;
using DataAccessLayer;
using Logger;
using NotesMail;
using Scriban;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestService
{
    internal class Program
    {
        static TPLogger logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);

        static void Main(string[] args)
        {
            Task.Run(async () => { await Mail207Knowledge.Notify207DocProcessing(); });

            Console.ReadKey();
        }
    }
}
