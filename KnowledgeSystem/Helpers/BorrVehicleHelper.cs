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
        WebProxy proxy;

        private static BorrVehicleHelper instance;

        static string proxyHost = "10.198.4.150";
        static string proxyPort = "8080";
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

            proxy = new WebProxy
            {
                Address = new Uri($"http://{proxyHost}:{proxyPort}"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(TPConfigs.LoginUser.Id, EncryptionHelper.DecryptPass(TPConfigs.LoginUser.SecondaryPassword))
            };
        }

        public async Task<List<VehicleStatus>> GetListVehicle(string parameter)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync(parameter);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                var statues = new List<VehicleStatus>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 4)
                    {
                        var status = new VehicleStatus
                        {
                            Name = parts[0],
                            StatusRaw = parts[1],
                            Dept = $"{parts[2]}{parts[3]}",
                        };
                        statues.Add(status);
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
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s34/{status.Dept}vkv{status.Name}");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');

                    string userBorr = parts[9];
                    string idUserBorr = userBorr.Substring(0, 10);
                    string nameUserBorr = userBorr.Substring(10, userBorr.Length - 10);

                    DateTime borrTime;
                    DateTime.TryParseExact(parts[3], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out borrTime);
                    DateTime backTime;
                    DateTime.TryParseExact(parts[2], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out backTime);

                    int startKm;
                    int.TryParse(parts[0], out startKm);
                    int endKm;
                    int.TryParse(parts[1], out endKm);
                    int totalKm;
                    int.TryParse(parts[7], out totalKm);

                    if (parts.Length >= 9)
                    {
                        var info = new VehicleBorrInfo
                        {
                            IdUserBorr = idUserBorr,
                            NameUserBorr = nameUserBorr,
                            BorrTime = borrTime,
                            BackTime = backTime,
                            Place = parts[4],
                            Uses = parts[5],
                            StartKm = startKm,
                            EndKm = endKm,
                            TotalKm = totalKm
                        };
                        infos.Add(info);
                    }
                }

                return infos;
            }
        }

        public async Task<List<VehicleBorrInfo>> GetBorrCarUser(VehicleStatus status)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s45/{status.Dept}vkv{status.Name}");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');

                    string userBorr = parts[3];
                    string idUserBorr = userBorr.Substring(0, 10);
                    string nameUserBorr = userBorr.Substring(10, userBorr.Length - 10);

                    DateTime borrTime;
                    DateTime.TryParseExact(parts[1], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out borrTime);
                    DateTime backTime;
                    DateTime.TryParseExact(parts[4], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out backTime);

                    int startKm;
                    int.TryParse(parts[8], out startKm);
                    int endKm;
                    int.TryParse(parts[9], out endKm);
                    int totalKm;
                    int.TryParse(parts[10], out totalKm);

                    string place = $"{parts[5]}-{parts[6]}";
                    string uses = parts[7];

                    if (parts.Length >= 23)
                    {
                        var info = new VehicleBorrInfo
                        {
                            IdUserBorr = idUserBorr,
                            NameUserBorr = nameUserBorr,
                            BorrTime = borrTime,
                            BackTime = backTime,
                            Place = place,
                            Uses = uses,
                            StartKm = startKm,
                            EndKm = endKm,
                            TotalKm = totalKm
                        };
                        infos.Add(info);
                    }
                }

                return infos;
            }
        }

        public async Task<List<string>> GetListPurposess()
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s66/o");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var purposess = new List<string>();
                foreach (var subItem in content.Split(new[] { "o|o" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    purposess.Add(subItem.Trim());
                }

                return purposess.OrderBy(r => r).ToList();
            }
        }

        public async Task<string> GetManagerVehicle(string nameVehicle)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s62/{nameVehicle}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetLastKmMotor(string nameVehicle)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s52/{nameVehicle}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> GetLastKmCar(string nameVehicle)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                var response = await httpClient.GetAsync($"s51/{nameVehicle}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<bool> BorrMotor(dm_User borrUsr, string nameVehicle, string borrTime, string place, string purposes, string numUser)
        {
            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = HttpUtility.UrlEncode(purposes).Replace("+", "%20").ToUpper();
            var placeUrl = HttpUtility.UrlEncode(place).Replace("+", "%20").ToUpper();

            int startKm = 0;
            var lastKm = await GetLastKmMotor(nameVehicle);
            int.TryParse(lastKm.Split('|')[0].Trim(), out startKm);

            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;

                string parameter = $"s35/{idDept2word}vkv{startKm}vkvvkvvkv{borrTime}vkv{placeUrl}vkv{purposesUrl}vkv{numUser}vkvvkv{nameVehicle}vkv{borrUsr.Id}vkvYvkvYvkvYvkv{managerVehicle}vkv{DateTime.Now.ToString("yyyyMMddHHmm")}";

                var response = await httpClient.GetAsync(parameter);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content == "ok";
            }
        }

        public async Task<bool> BackMotor(dm_User borrUsr, string nameVehicle, int endKm, string borrTime, string backTime, int totalKm)
        {
            var userBackUrl = HttpUtility.UrlEncode($"{borrUsr.Id}{borrUsr.DisplayName}").Replace("+", "%20").ToUpper();

            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;
                //s36/38LD-40006vkv202404260112vkv36497vkv202404261113vkv9vkvVNW0017146%E9%BB%8E%E6%B0%8F%E5%9E%82%E7%8E%B2
                string parameter = $"s36/{nameVehicle}vkv{borrTime}vkv{endKm}vkv{backTime}vkv{totalKm}vkv{userBackUrl}";

                var response = await httpClient.GetAsync(parameter);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content == "ok";
            }
        }

        public async Task<bool> BorrCar(dm_User borrUsr, string nameVehicle, string borrTime, string fromPlace, string toPlace, string purposes, string licExpDate)
        {
            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = HttpUtility.UrlEncode(purposes).Replace("+", "%20").ToUpper();
            var formPlaceUrl = HttpUtility.UrlEncode(fromPlace).Replace("+", "%20").ToUpper();
            var toPlaceUrl = HttpUtility.UrlEncode(toPlace).Replace("+", "%20").ToUpper();

            int startKm = 0;
            var lastKm = await GetLastKmCar(nameVehicle);
            int.TryParse(lastKm.Split('|')[0].Trim(), out startKm);

            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;

                string parameter = $"s46/{nameVehicle}vkv{borrUsr.Id}vkv{idDept2word}vkv{startKm}vkvvkv{borrTime}vkvvkv{formPlaceUrl}vkv{toPlaceUrl}vkv{purposesUrl}vkvYvkvvkvvkv{licExpDate}vkvYvkvYvkvYvkvvkvvkvvkvvkv{DateTime.Now.ToString("yyyyMMddHHmm")}vkv{managerVehicle}";

                //return true;

                var response = await httpClient.GetAsync(parameter);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content == "ok";
            }
        }

        public async Task<bool> BackCar(dm_User borrUsr, string nameVehicle, int endKm, string borrTime, string backTime, int totalKm)
        {
            var userBackUrl = HttpUtility.UrlEncode($"{borrUsr.Id}{borrUsr.DisplayName}").Replace("+", "%20").ToUpper();

            using (var httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy }))
            {
                httpClient.BaseAddress = baseUrl;

                string parameter = $"s47/{nameVehicle}vkv{borrTime}vkv{userBackUrl}vkv{backTime}vkv{endKm}vkv{totalKm}vkvYvkv0vkvvkvvkvNvkv0";

                var response = await httpClient.GetAsync(parameter);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                return content == "ok";
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
