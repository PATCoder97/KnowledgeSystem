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
using System.Windows.Forms;

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

                    string idUserBorr = parts[9].Substring(0, 10);
                    string nameUserBorr = parts[9].Substring(10, parts[9].Length - 10);

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
