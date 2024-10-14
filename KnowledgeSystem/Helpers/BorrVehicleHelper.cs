using BusinessLayer;
using DataAccessLayer;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.XtraCharts.Native;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Helpers
{
    public class BorrVehicleHelper
    {
        TPLogger logger;

        private static BorrVehicleHelper instance;

        static Uri baseUrl = new Uri("https://www.fhs.com.tw/ads/api/Furnace/rest/json/ve/");
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        public static BorrVehicleHelper Instance
        {
            get { if (instance == null) instance = new BorrVehicleHelper(); return instance; }
            private set { instance = value; }
        }

        private BorrVehicleHelper()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        public async Task<List<VehicleStatus>> GetListVehicle(string parameter)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync(parameter);

                var statues = new List<VehicleStatus>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 4)
                    {
                        statues.Add(new VehicleStatus
                        {
                            Name = parts[0],
                            StatusRaw = parts[1],
                            Dept = $"{parts[2]}{parts[3]}"
                        });
                    }
                }
                return statues;
            }
        }

        public async Task<List<VehicleStatus>> GetListMotor()
        {
            return await GetListVehicle($"s60/{idDept2word}");
        }

        public async Task<List<VehicleStatus>> GetListCar()
        {
            return await GetListVehicle($"s61/{idDept2word}");
        }

        public async Task<List<VehicleBorrInfo>> GetBorrMotorUser(VehicleStatus status)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync($"s34/{status.Dept}vkv{status.Name}");

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 9)
                    {
                        infos.Add(new VehicleBorrInfo
                        {
                            IdUserBorr = parts[9].Substring(0, 10),
                            NameUserBorr = parts[9].Substring(10),
                            BorrTime = DateTime.TryParseExact(parts[3], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var borrTime) ? borrTime : default,
                            BackTime = DateTime.TryParseExact(parts[2], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var backTime) ? backTime : default,
                            StartKm = int.TryParse(parts[0], out var startKm) ? startKm : 0,
                            EndKm = int.TryParse(parts[1], out var endKm) ? endKm : 0,
                            TotalKm = int.TryParse(parts[7], out var totalKm) ? totalKm : 0,
                            Place = parts[4],
                            Uses = parts[5]
                        });
                    }
                }
                return infos;
            }
        }

        public async Task<List<VehicleBorrInfo>> GetBorrCarUser(VehicleStatus status)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync($"s45/{status.Dept}vkv{status.Name}");

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 23)
                    {
                        infos.Add(new VehicleBorrInfo
                        {
                            IdUserBorr = parts[3].Substring(0, 10),
                            NameUserBorr = parts[3].Substring(10),
                            BorrTime = DateTime.TryParseExact(parts[1], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var borrTime) ? borrTime : default,
                            BackTime = DateTime.TryParseExact(parts[4], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var backTime) ? backTime : default,
                            StartKm = int.TryParse(parts[8], out var startKm) ? startKm : 0,
                            EndKm = int.TryParse(parts[9], out var endKm) ? endKm : 0,
                            TotalKm = int.TryParse(parts[10], out var totalKm) ? totalKm : 0,
                            Place = $"{parts[5]}-{parts[6]}",
                            Uses = parts[7]
                        });
                    }
                }
                return infos;
            }
        }

        public async Task<List<string>> GetListPurposess()
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync($"s66/o");

                return content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(p => p.Trim())
                              .OrderBy(r => r)
                              .ToList();
            }
        }

        public async Task<string> GetManagerVehicle(string nameVehicle)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                return await webClient.DownloadStringTaskAsync($"s62/{nameVehicle}");
            }
        }

        public async Task<string> GetLastKmMotor(string nameVehicle)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                return await webClient.DownloadStringTaskAsync($"s52/{nameVehicle}");
            }
        }

        public async Task<string> GetLastKmCar(string nameVehicle)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                return await webClient.DownloadStringTaskAsync($"s51/{nameVehicle}");
            }
        }

        public async Task<bool> BorrMotor(dm_User borrUsr, string nameVehicle, string borrTime, string place, string purposes, string numUser)
        {
            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = HttpUtility.UrlEncode(purposes).Replace("+", "%20").ToUpper();
            var placeUrl = HttpUtility.UrlEncode(place).Replace("+", "%20").ToUpper();

            int startKm = int.TryParse((await GetLastKmMotor(nameVehicle)).Split('|')[0].Trim(), out var lastKm) ? lastKm : 0;

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s35/{idDept2word}vkv{startKm}vkvvkvvkv{borrTime}vkv{placeUrl}vkv{purposesUrl}vkv{numUser}vkvvkv{nameVehicle}vkv{borrUsr.Id}vkvYvkvYvkvYvkv{managerVehicle}vkv{DateTime.Now:yyyyMMddHHmm}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> BackMotor(dm_User borrUsr, string nameVehicle, int endKm, string borrTime, string backTime, int totalKm)
        {
            var userBackUrl = HttpUtility.UrlEncode($"{borrUsr.Id}{borrUsr.DisplayName}").Replace("+", "%20").ToUpper();

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s36/{nameVehicle}vkv{borrTime}vkv{endKm}vkv{backTime}vkv{totalKm}vkv{userBackUrl}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> BorrCar(dm_User borrUsr, string nameVehicle, string borrTime, string fromPlace, string toPlace, string purposes, string licExpDate, int startKm = 0)
        {
            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = HttpUtility.UrlEncode(purposes).Replace("+", "%20").ToUpper();
            var formPlaceUrl = HttpUtility.UrlEncode(fromPlace).Replace("+", "%20").ToUpper();
            var toPlaceUrl = HttpUtility.UrlEncode(toPlace).Replace("+", "%20").ToUpper();

            if (startKm == 0)
            {
                int.TryParse((await GetLastKmCar(nameVehicle)).Split('|')[0].Trim(), out startKm);
            }

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s46/{nameVehicle}vkv{borrUsr.Id}vkv{idDept2word}vkv{startKm}vkvvkv{borrTime}vkvvkv{formPlaceUrl}vkv{toPlaceUrl}vkv{purposesUrl}vkvYvkvvkvvkv{licExpDate}vkvYvkvYvkvYvkvvkvvkvvkvvkv{DateTime.Now:yyyyMMddHHmm}vkv{managerVehicle}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> BackCar(dm_User borrUsr, string nameVehicle, int endKm, string borrTime, string backTime, int totalKm)
        {
            var userBackUrl = HttpUtility.UrlEncode($"{borrUsr.Id}{borrUsr.DisplayName}").Replace("+", "%20").ToUpper();

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s47/{nameVehicle}vkv{borrTime}vkv{userBackUrl}vkv{backTime}vkv{endKm}vkv{totalKm}vkvYvkv0vkvvkvvkvNvkv0";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }
    }

    public class VehicleStatus
    {
        public string Name { get; set; }
        public string Dept { get; set; }
        public string IdUserBorr { get; set; }
        public string NameUserBorr { get; set; }
        public string BorrTime { get; set; }
        private string _status;

        public string StatusRaw { get => _status; set => _status = value; }

        public string Status
        {
            get { return _status == "a" ? "Xe đã mượn" : "Có thể mượn"; }
            set { }
        }
        public bool CanBorr
        {
            get { return _status == "a" ? false : true; }
            set { }
        }
    }

    public class VehicleBorrInfo
    {
        public string IdUserBorr { get; set; }
        public string NameUserBorr { get; set; }
        public DateTime BorrTime { get; set; }
        public DateTime BackTime { get; set; }
        public string Place { get; set; }
        public string Uses { get; set; }
        public int StartKm { get; set; }
        public int? EndKm { get; set; }
        public int? TotalKm { get; set; }
    }
}
