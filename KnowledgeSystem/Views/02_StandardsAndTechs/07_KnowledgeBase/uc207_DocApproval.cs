using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_DocApproval : DevExpress.XtraEditors.XtraUserControl
    {
        public uc207_DocApproval()
        {
            InitializeComponent();
        }

        private void uc207_DocApproval_Load(object sender, EventArgs e)
        {
            lbInfo.Text= "※ 請審查下列資料。<br>※ 若同意上傳請按<color=red>「核准」</color>按鈕<br>※ 不同意上傳請按<color=red>「退回」</color>按鈕<br>※ 若要修改請按<color=red>「編輯」</color>按鈕";
            lbInfo.AllowHtmlString = true;
            lbInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            lbInfo.Appearance.Options.UseTextOptions = true;
            lbInfo.AutoSizeMode = LabelAutoSizeMode.Vertical;


        }
    }
}
