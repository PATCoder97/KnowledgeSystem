using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Helpers
{
    public class KeyboardHelper
    {
        /// <summary>
        /// Kiểm tra xem có đang ở kiểu gõ English (ENG) và CapsLock đã tắt chưa
        /// </summary>
        /// <returns>
        /// true = đúng ENG và CapsLock tắt
        /// false = sai (chưa đúng kiểu gõ hoặc CapsLock đang bật)
        /// </returns>
        public static bool IsEnglishAndCapsOff()
        {
            // 🔹 Kiểm tra CapsLock
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                XtraMessageBox.Show("⚠️ CapsLock đang bật. Vui lòng tắt trước khi tiếp tục!",
                                "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // 🔹 Kiểm tra ngôn ngữ bàn phím hiện tại
            var currentLang = InputLanguage.CurrentInputLanguage;
            var culture = currentLang.Culture;

            if (culture.TwoLetterISOLanguageName != "en")
            {
                XtraMessageBox.Show($"⚠️ Hiện tại đang ở kiểu gõ: {currentLang.LayoutName}.\n" +
                                "👉 Vui lòng chuyển sang English (ENG) để tránh lỗi nhập liệu.",
                                "Cảnh báo kiểu gõ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // ✅ Đúng điều kiện
            return true;
        }

        public static bool IsUnikeyRunning()
        {
            return Process.GetProcesses().Any(r => r.ProcessName.ToLower().Contains("unikey"));
        }
    }
}
