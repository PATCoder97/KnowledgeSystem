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
using static DevExpress.XtraEditors.Mask.MaskSettings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KnowledgeSystem.Helpers
{
    public class BorrVehicleHelper
    {
        TPLogger logger;

        private static BorrVehicleHelper instance;
        private static readonly string[] ApiItemSeparator = new[] { "o|o" };

        static Uri baseUrl = new Uri("https://www.fhs.com.tw/ads/api/Furnace/rest/json/ve/");
        string idDept2word = SafeLeft(TPConfigs.LoginUser?.IdDepartment, 2);

        public static BorrVehicleHelper Instance
        {
            get { if (instance == null) instance = new BorrVehicleHelper(); return instance; }
            private set { instance = value; }
        }

        private BorrVehicleHelper()
        {
            logger = new TPLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName);
        }

        private static string SafeLeft(string value, int length)
        {
            if (string.IsNullOrEmpty(value) || length <= 0)
            {
                return string.Empty;
            }

            return value.Length <= length ? value : value.Substring(0, length);
        }

        private static void SplitUserInfo(string value, out string idUser, out string nameUser)
        {
            idUser = SafeLeft(value, 10);
            nameUser = string.IsNullOrEmpty(value) || value.Length <= idUser.Length
                ? string.Empty
                : value.Substring(idUser.Length);
        }

        private static string EncodeApiValue(string value)
        {
            return (HttpUtility.UrlEncode(value ?? string.Empty) ?? string.Empty).Replace("+", "%20").ToUpper();
        }

        private static int ParseFirstPipeNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            var firstPart = value.Split('|').FirstOrDefault();
            return int.TryParse(firstPart?.Trim(), out var number) ? number : 0;
        }

        private static void SplitPlaces(string value, out string fromPlace, out string toPlace)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                fromPlace = string.Empty;
                toPlace = string.Empty;
                return;
            }

            var parts = value.Split(new[] { '-' }, 2);
            fromPlace = parts.Length > 0 ? parts[0] : string.Empty;
            toPlace = parts.Length > 1 ? parts[1] : string.Empty;
        }

        public async Task<List<VehicleStatus>> GetListVehicle(string parameter)
        {
            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync(parameter);

                var statues = new List<VehicleStatus>();
                foreach (var subItem in content.Split(ApiItemSeparator, StringSplitOptions.RemoveEmptyEntries))
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
            if (status == null)
            {
                return new List<VehicleBorrInfo>();
            }

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync($"s34/{status.Dept ?? string.Empty}vkv{(status.Name ?? string.Empty).Replace(".", string.Empty)}");

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(ApiItemSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 10)
                    {
                        SplitUserInfo(parts[9], out var idUserBorr, out var nameUserBorr);

                        infos.Add(new VehicleBorrInfo
                        {
                            IdUserBorr = idUserBorr,
                            NameUserBorr = nameUserBorr,
                            BorrTime = DateTime.TryParseExact(parts[3], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var borrTime) ? borrTime : default,
                            BackTime = DateTime.TryParseExact(parts[2], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var backTime) ? backTime : default,
                            StartKm = int.TryParse(parts[0], out var startKm) ? startKm : 0,
                            EndKm = int.TryParse(parts[1], out var endKm) ? endKm : 0,
                            TotalKm = int.TryParse(parts[7], out var totalKm) ? totalKm : 0,
                            Place = parts[4],
                            Uses = parts[5],
                            VehicleName = status.Name
                        });
                    }
                }
                return infos;
            }
        }

        public async Task<List<VehicleBorrInfo>> GetBorrCarUser(VehicleStatus status)
        {
            if (status == null)
            {
                return new List<VehicleBorrInfo>();
            }

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string content = await webClient.DownloadStringTaskAsync($"s45/{status.Dept ?? string.Empty}vkv{status.Name ?? string.Empty}");

                var infos = new List<VehicleBorrInfo>();
                foreach (var subItem in content.Split(ApiItemSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = subItem.Split('|');
                    if (parts.Length >= 23)
                    {
                        SplitUserInfo(parts[3], out var idUserBorr, out var nameUserBorr);

                        infos.Add(new VehicleBorrInfo
                        {
                            IdUserBorr = idUserBorr,
                            NameUserBorr = nameUserBorr,
                            BorrTime = DateTime.TryParseExact(parts[1], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var borrTime) ? borrTime : default,
                            BackTime = DateTime.TryParseExact(parts[4], "yyyyMMddHHmm", null, System.Globalization.DateTimeStyles.None, out var backTime) ? backTime : default,
                            StartKm = int.TryParse(parts[8], out var startKm) ? startKm : 0,
                            EndKm = int.TryParse(parts[9], out var endKm) ? endKm : 0,
                            TotalKm = int.TryParse(parts[10], out var totalKm) ? totalKm : 0,
                            Place = $"{parts[5]}-{parts[6]}",
                            Uses = parts[7],
                            VehicleName = status.Name
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

                return content.Split(ApiItemSeparator, StringSplitOptions.RemoveEmptyEntries)
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
            //https://www.fhs.com.tw/ads/api/Furnace/rest/json/ve/s35/78vkv27513vkvvkvvkv202508221538vkvTtkt-vlvkvB.%E9%96%8B%E6%9C%83_%E4%B8%8A%E8%AA%B2%E8%A8%93%E7%B7%B4%20vkv1vkvvkv38LD-40103vkvVNW0014732vkvYvkvYvkvYvkvVNW0018983vkv202508221539
            if (borrUsr == null)
            {
                return false;
            }

            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = EncodeApiValue(purposes);
            var placeUrl = EncodeApiValue(place);
            int startKm = ParseFirstPipeNumber(await GetLastKmMotor(nameVehicle));

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s35/{idDept2word}vkv{startKm}vkvvkvvkv{borrTime}vkv{placeUrl}vkv{purposesUrl}vkv{numUser}vkvvkv{nameVehicle}vkv{borrUsr.Id}vkvYvkvYvkvYvkv{managerVehicle}vkv{DateTime.Now:yyyyMMddHHmm}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> BackMotor(dm_User borrUsr, string nameVehicle, int endKm, string borrTime, string backTime, int totalKm)
        {
            if (borrUsr == null)
            {
                return false;
            }

            var userBackUrl = EncodeApiValue($"{borrUsr.Id}{borrUsr.DisplayName}");

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s36/{nameVehicle}vkv{borrTime}vkv{endKm}vkv{backTime}vkv{totalKm}vkv{userBackUrl}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> BorrCar(dm_User borrUsr, string nameVehicle, string borrTime, string fromPlace, string toPlace, string purposes, string licExpDate, int startKm = 0)
        {
            if (borrUsr == null)
            {
                return false;
            }

            string managerVehicle = await GetManagerVehicle(nameVehicle);
            var purposesUrl = EncodeApiValue(purposes);
            var formPlaceUrl = EncodeApiValue(fromPlace);
            var toPlaceUrl = EncodeApiValue(toPlace);

            if (startKm == 0)
            {
                startKm = ParseFirstPipeNumber(await GetLastKmCar(nameVehicle));
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
            if (borrUsr == null)
            {
                return false;
            }

            var userBackUrl = EncodeApiValue($"{borrUsr.Id}{borrUsr.DisplayName}");

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s47/{nameVehicle}vkv{borrTime}vkv{userBackUrl}vkv{backTime}vkv{endKm}vkv{totalKm}vkvYvkv0vkvvkvvkvNvkv0";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> EditInfoMoto(VehicleBorrInfo info)
        {
            if (info == null)
            {
                return false;
            }

            var place = EncodeApiValue(info.Place);
            var purposes = EncodeApiValue(info.Uses);

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s53/{place}vkv{purposes}vkv{info.IdUserBorr}vkv{info.StartKm}vkv{info.EndKm}vkv{info.VehicleName}vkv{info.BorrTime:yyyyMMddHHmm}vkv{info.BackTime:yyyyMMddHHmm}vkv{info.EndKm - info.StartKm}";
                return (await webClient.DownloadStringTaskAsync(parameter)) == "ok";
            }
        }

        public async Task<bool> EditInfoCar(VehicleBorrInfo info)
        {
            if (info == null)
            {
                return false;
            }

            SplitPlaces(info.Place, out var fromPlaceRaw, out var toPlaceRaw);
            var fromPlace = EncodeApiValue(fromPlaceRaw);
            var toPlace = EncodeApiValue(toPlaceRaw);
            var purposes = EncodeApiValue(info.Uses);

            using (var webClient = new WebClient { BaseAddress = baseUrl.ToString() })
            {
                webClient.Encoding = Encoding.UTF8;
                string parameter = $"s54/{fromPlace}vkv{toPlace}vkv{purposes}vkv{info.StartKm}vkv{info.EndKm}vkv0vkvvkvvkvNvkv{info.VehicleName}vkv{info.IdUserBorr}vkv{info.BorrTime:yyyyMMddHHmm}vkv{info.EndKm - info.StartKm}vkv{info.BackTime:yyyyMMddHHmm}vkvY";
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
        public string VehicleName { get; set; }
    }
}
