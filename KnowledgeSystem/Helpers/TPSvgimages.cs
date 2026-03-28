using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace KnowledgeSystem.Helpers
{
    public class TPSvgimages
    {
        public static readonly string StartupPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string ImagesPath = ResolveImagesPath();

        public static readonly Image Background = LoadImage("background.jpg");
        public static readonly Image NoImage = LoadImage("no-image.png");
        public static readonly Image SplashScreen = LoadImage("SplashScreen.png");

        public static readonly SvgImage CheckedRadio = LoadSvg("checked_radio_button.svg");
        public static readonly SvgImage UncheckedRadio = LoadSvg("unchecked_radio_button.svg");
        public static readonly SvgImage Add = LoadSvg("icons_add.svg");
        public static readonly SvgImage Add2 = LoadSvg("icons_plus.svg");
        public static readonly SvgImage Edit = LoadSvg("icons_edit.svg");
        public static readonly SvgImage Reload = LoadSvg("icons_reload.svg");
        public static readonly SvgImage Remove = LoadSvg("icons_remove.svg");
        public static readonly SvgImage Cancel = LoadSvg("icons_cancel.svg");
        public static readonly SvgImage Confirm = LoadSvg("icons_ok.svg");
        public static readonly SvgImage AddUserGroup = LoadSvg("icons_add_User_Group.svg");
        public static readonly SvgImage Approval = LoadSvg("icons_signature.svg");
        public static readonly SvgImage EmailSend = LoadSvg("icons_email_send.svg");
        public static readonly SvgImage Progress = LoadSvg("StepProcess.svg");
        public static readonly SvgImage Close = LoadSvg("icons_close.svg");
        public static readonly SvgImage Search = LoadSvg("Search_more.svg");
        public static readonly SvgImage UploadFile = LoadSvg("icons_upload_to_ftp.svg");
        public static readonly SvgImage Excel = LoadSvg("icons_excel.svg");
        public static readonly SvgImage Suspension = LoadSvg("icons_unfriend.svg");
        public static readonly SvgImage Transfer = LoadSvg("icons_exchange.svg");
        public static readonly SvgImage Resign = LoadSvg("icons_denied.svg");
        public static readonly SvgImage Conferred = LoadSvg("icons_change_user.svg");
        public static readonly SvgImage UpLevel = LoadSvg("icons_uplevel.svg");
        public static readonly SvgImage PersonnelChanges = LoadSvg("icons_personnel_changes.svg");
        public static readonly SvgImage Num1 = LoadSvg("icons_circled_1.svg");
        public static readonly SvgImage Num2 = LoadSvg("icons_circled_2.svg");
        public static readonly SvgImage Num3 = LoadSvg("icons_circled_3.svg");
        public static readonly SvgImage Num4 = LoadSvg("icons_circled_4.svg");
        public static readonly SvgImage Num5 = LoadSvg("icons_circled_5.svg");
        public static readonly SvgImage Num6 = LoadSvg("icons_circled_6.svg");
        public static readonly SvgImage Filter = LoadSvg("icons_filter.svg");
        public static readonly SvgImage View = LoadSvg("icons_view.svg");
        public static readonly SvgImage Info = LoadSvg("icons_info.svg");
        public static readonly SvgImage Stamp = LoadSvg("Approval.svg");
        public static readonly SvgImage BorrVehicle = LoadSvg("icons_borr_vehicle.svg");
        public static readonly SvgImage Attach = LoadSvg("icons_attach.svg");
        public static readonly SvgImage Copy = LoadSvg("icons_transfer.svg");
        public static readonly SvgImage Money = LoadSvg("icons8_stack_of_money.svg");
        public static readonly SvgImage Start = LoadSvg("icons8_time.svg");
        public static readonly SvgImage Finish = LoadSvg("icons8_finish_flag.svg");
        public static readonly SvgImage Print = LoadSvg("icons8_print.svg");
        public static readonly SvgImage Learn = LoadSvg("icons8_learn.svg");
        public static readonly SvgImage Disable = LoadSvg("icons8_disable.svg");
        public static readonly SvgImage GgSheet = LoadSvg("icons8_google_sheets.svg");
        public static readonly SvgImage GgForm = LoadSvg("icons8_google_forms_new_logo.svg");
        public static readonly SvgImage Word = LoadSvg("icons8_word.svg");
        public static readonly SvgImage Gears = LoadSvg("icons8_gears.svg");
        public static readonly SvgImage Robot = LoadSvg("icons8_robot.svg");
        public static readonly SvgImage Dept = LoadSvg("icons8_department.svg");
        public static readonly SvgImage DateAdd = LoadSvg("icons8_calendar_plus_1.svg");
        public static readonly SvgImage Schedule = LoadSvg("icons8_schedule.svg");
        public static readonly SvgImage GasStation = LoadSvg("icons8_gas_station.svg");
        public static readonly SvgImage Bot = LoadSvg("icons8_bot.svg");

        private static string ResolveImagesPath()
        {
            foreach (string root in GetCandidateRoots())
            {
                foreach (string candidate in GetCandidateImageFolders(root))
                {
                    if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "icons_add.svg")))
                    {
                        return candidate;
                    }
                }
            }

            return Path.Combine(StartupPath, "Images");
        }

        private static IEnumerable<string> GetCandidateRoots()
        {
            var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            AddRoot(roots, AppDomain.CurrentDomain.BaseDirectory);
            AddRoot(roots, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            AddRoot(roots, Environment.CurrentDirectory);
            return roots;
        }

        private static void AddRoot(HashSet<string> roots, string root)
        {
            if (!string.IsNullOrWhiteSpace(root) && Directory.Exists(root))
            {
                roots.Add(root);
            }
        }

        private static IEnumerable<string> GetCandidateImageFolders(string root)
        {
            string current = root;
            while (!string.IsNullOrWhiteSpace(current) && Directory.Exists(current))
            {
                yield return Path.Combine(current, "Images");
                yield return Path.Combine(current, "KnowledgeSystem", "Images");

                DirectoryInfo parent = Directory.GetParent(current);
                current = parent?.FullName;
            }
        }

        private static Image LoadImage(string fileName)
        {
            string filePath = Path.Combine(ImagesPath, fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var image = Image.FromStream(stream))
                    {
                        return (Image)image.Clone();
                    }
                }
            }
            catch
            {
            }

            return CreatePlaceholderImage();
        }

        private static SvgImage LoadSvg(string fileName)
        {
            string filePath = Path.Combine(ImagesPath, fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    return SvgImage.FromFile(filePath);
                }
            }
            catch
            {
            }

            return null;
        }

        private static Image CreatePlaceholderImage()
        {
            var bitmap = new Bitmap(1, 1);
            bitmap.MakeTransparent();
            return bitmap;
        }
    }
}
