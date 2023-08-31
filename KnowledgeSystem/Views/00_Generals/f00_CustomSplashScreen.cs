using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_CustomSplashScreen : SplashScreen
    {
        const string startDate = "2023.06.08";

        public f00_CustomSplashScreen()
        {
            InitializeComponent();
            labelCopyright.Text = "Copyright ©  2023 - " + DateTime.Now.Year.ToString();

            lbVersion.Text = $":{AppCopyRight.version}";
            lbOwner.Text = $":{AppCopyRight.ownerSoft}";
            lbSupporter.Text = $":{AppCopyRight.supporter}";
            lbStartDate.Text = $":{startDate}";
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }
    }
}