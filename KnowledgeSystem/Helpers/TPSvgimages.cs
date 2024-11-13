using DevExpress.Utils.Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class TPSvgimages
    {
        public static string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ImagesPath = Path.Combine(StartupPath, "Images");

        public static Image Background = Image.FromFile(Path.Combine(ImagesPath, "background.jpg"));
        public static Image NoImage = Image.FromFile(Path.Combine(ImagesPath, "no-image.png"));
        public static Image SplashScreen = Image.FromFile(Path.Combine(ImagesPath, "SplashScreen.png"));

        public static SvgImage CheckedRadio = SvgImage.FromFile(Path.Combine(ImagesPath, "checked_radio_button.svg"));
        public static SvgImage UncheckedRadio = SvgImage.FromFile(Path.Combine(ImagesPath, "unchecked_radio_button.svg"));
        public static SvgImage Add = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_add.svg"));
        public static SvgImage Add2 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_plus.svg"));
        public static SvgImage Edit = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_edit.svg"));
        public static SvgImage Reload = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_reload.svg"));
        public static SvgImage Remove = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_remove.svg"));
        public static SvgImage Cancel = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_cancel.svg"));
        public static SvgImage Confirm = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_ok.svg"));
        public static SvgImage AddUserGroup = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_add_User_Group.svg"));
        public static SvgImage Approval = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_signature.svg"));
        public static SvgImage EmailSend = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_email_send.svg"));
        public static SvgImage Progress = SvgImage.FromFile(Path.Combine(ImagesPath, "StepProcess.svg"));
        public static SvgImage Close = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_close.svg"));
        public static SvgImage Search = SvgImage.FromFile(Path.Combine(ImagesPath, "Search_more.svg"));
        public static SvgImage UploadFile = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_upload_to_ftp.svg"));
        public static SvgImage Excel = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_excel.svg"));
        public static SvgImage Suspension = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_unfriend.svg"));
        public static SvgImage Transfer = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_exchange.svg"));
        public static SvgImage Resign = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_denied.svg"));
        public static SvgImage Conferred = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_change_user.svg"));
        public static SvgImage UpLevel = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_uplevel.svg"));
        public static SvgImage PersonnelChanges = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_personnel_changes.svg"));
        public static SvgImage Num1 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_1.svg"));
        public static SvgImage Num2 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_2.svg"));
        public static SvgImage Num3 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_3.svg"));
        public static SvgImage Num4 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_4.svg"));
        public static SvgImage Num5 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_5.svg"));
        public static SvgImage Num6 = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_circled_6.svg"));
        public static SvgImage Filter = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_filter.svg"));
        public static SvgImage View = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_view.svg"));
        public static SvgImage Info = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_info.svg"));
        public static SvgImage Stamp = SvgImage.FromFile(Path.Combine(ImagesPath, "Approval.svg"));
        public static SvgImage BorrVehicle = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_borr_vehicle.svg"));
        public static SvgImage Attach = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_attach.svg"));
        public static SvgImage Copy = SvgImage.FromFile(Path.Combine(ImagesPath, "icons_transfer.svg"));
        public static SvgImage Money = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_stack_of_money.svg"));
        public static SvgImage Start = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_time.svg"));
        public static SvgImage Finish = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_finish_flag.svg"));
        public static SvgImage Print = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_print.svg"));
        public static SvgImage Learn = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_learn.svg"));
        public static SvgImage Disable = SvgImage.FromFile(Path.Combine(ImagesPath, "icons8_disable.svg"));
    }
}
