using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;
using KnowledgeSystem.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_UpdateSoftware : DevExpress.XtraEditors.XtraForm
    {
        public f00_UpdateSoftware(string urlSetupFile_)
        {
            InitializeComponent();
            urlSetupFile = urlSetupFile_;
        }

        WebClient webClient;
        string urlSetupFile = "";
        string pathSetup = "";

        private void f00_UpdateSoftware_Load(object sender, EventArgs e)
        {
            string pathDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pathFolderSoft = Path.Combine(pathDocuments, TPConfigs.SoftNameEN);
            if (!Directory.Exists(pathFolderSoft))
            {
                Directory.CreateDirectory(pathFolderSoft);
            }

            string setupFileName = $"{DateTime.Now:MMddhhMMss}-setup.msi";
            pathSetup = Path.Combine(pathFolderSoft, setupFileName);

            DownloadFile(urlSetupFile, pathSetup);
        }

        public void DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);

                try
                {
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update the progressbar percentage only when the value is not the same.
            progressBar.Value = e.ProgressPercentage;

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            labelDownloaded.Text = string.Format("{0} MB's / {1} MB's - {2:N0}%",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"), e.ProgressPercentage);
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Download has been canceled.");
            }
            else
            {
                Process.Start(pathSetup);
                Close();
            }
        }
    }
}