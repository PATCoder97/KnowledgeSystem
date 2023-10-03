using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logger
{
    public class TPLogger
    {
        private readonly string _thread;
        private readonly string _user;

        public TPLogger(string _Thread, string _User = "")
        {
            _thread = _Thread;
            _user = _User;
        }

        public void Info(string _Logger, string _Message)
        {
            string _level = "Info";
            Log(_level, _Logger, _Message);
        }

        public void Warning(string _Logger, string _Message)
        {
            string _level = "Warning";
            Log(_level, _Logger, _Message);
        }

        public void Error(string _Logger, string _Exception)
        {
            string _level = "Error";
            Log(_level, _Logger, "", _Exception);
        }

        private async void Log(string _Level, string _Logger, string _Message, string _Exception = "")
        {
            sys_Log _log = new sys_Log()
            {
                DateCreate = DateTime.Now,
                Thread = _thread,
                Level = _Level,
                Logger = GetTextLogger(_Logger),
                Message = _Message,
                Exception = _Exception,
                UserId = _user
            };

            await CreateLog(_log);
        }

        public string GetTextLogger(string input)
        {
            Regex regex = new Regex("<(.*?)>");
            Match match = regex.Match(input);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return input;
            }
        }

        public async Task<bool> CreateLog(sys_Log _log)
        {
            using (var _context = new DBDocumentManagementSystemEntities())
            {
                try
                {
                    _context.sys_Log.Add(_log);
                    int affectedRecords = await _context.SaveChangesAsync();

                    return affectedRecords > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error create log: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
