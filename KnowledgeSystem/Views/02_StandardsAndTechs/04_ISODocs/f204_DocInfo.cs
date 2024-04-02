using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_ISODocs
{
    public partial class f204_DocInfo : DevExpress.XtraEditors.XtraForm
    {
        public f204_DocInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<Attachments> attachments = new List<Attachments>();
        BindingSource sourceAtt = new BindingSource();

        private class Attachments
        {
            public string ActualName { get; set; }
            public string EncryptionName { get; set; }
            public string FullPath { get; set; }
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f204_DocInfo_Load(object sender, EventArgs e)
        {
            gcFiles.DataSource = sourceAtt;
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = TPConfigs.FilterFile,
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            int index = 0;
            foreach (string fileName in openFileDialog.FileNames)
            {
                string encryptionName = EncryptionHelper.EncryptionFileName(fileName);
                Attachments attachment = new Attachments
                {
                    FullPath = fileName,
                    ActualName = Path.GetFileName(fileName),
                    EncryptionName = $"{encryptionName}{index++}"
                };
                attachments.Add(attachment);
                Thread.Sleep(5);
            }

            sourceAtt.DataSource = attachments;
            lbCountFile.Text = $"共{attachments.Count}個表單";
            gvFiles.RefreshData();
        }

        private void btnDelFile_Click(object sender, EventArgs e)
        {
            Attachments attachment = gvFiles.GetRow(gvFiles.FocusedRowHandle) as Attachments;

            string msg = $"您想要刪除表單：\r\n{attachment.ActualName}?";
            if (MsgTP.MsgYesNoQuestion(msg) == DialogResult.No)
            {
                return;
            }

            attachments.Remove(attachment);
            lbCountFile.Text = $"共{attachments.Count}個表單";

            int rowIndex = gvFiles.FocusedRowHandle;
            gvFiles.RefreshData();
            gvFiles.FocusedRowHandle = rowIndex;
        }
    }
}