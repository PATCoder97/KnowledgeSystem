using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Org.BouncyCastle.Crypto;

namespace KnowledgeSystem.Views._00_Generals
{
    public partial class f00_SendNoteSystem : DevExpress.XtraEditors.XtraForm
    {
        public f00_SendNoteSystem()
        {
            InitializeComponent();
        }

        public string To, Cc, Title, Subject, Atts, ThreadId;

        private void f00_SendNoteSystem_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(To))
                txbTo.EditValue = To;
            if (!string.IsNullOrEmpty(Cc))
                txbCc.EditValue = Cc;
            if (!string.IsNullOrEmpty(Title))
                txbTitle.EditValue = Title;
            if (!string.IsNullOrEmpty(Subject))
                txbSubject.EditValue = Subject;
        }

        private void btnSend_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrEmpty(txbTo.EditValue?.ToString()))
            {
                XtraMessageBox.Show("請輸入收件者", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            if (string.IsNullOrEmpty(txbTitle.EditValue?.ToString()))
            {
                XtraMessageBox.Show("請輸入主旨", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            if (string.IsNullOrEmpty(txbSubject.EditValue?.ToString()))
            {
                XtraMessageBox.Show("請輸入內容", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }

            string toUsers = string.Join(",", txbTo.EditValue.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => $"{r.ToUpper()}@VNFPG"));

            string ccUsers = string.IsNullOrEmpty(txbSubject.EditValue?.ToString()) ? "" :
                string.Join(",", txbCc.EditValue.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => $"{r.ToUpper()}@VNFPG"));

            sys_NotesMail mail = new sys_NotesMail()
            {
                ToUsers = toUsers,
                CCUsers = ccUsers,
                Subjects = txbTitle.Text.Trim(),
                Content = txbSubject.Text.Trim(),
                Thread = ThreadId,
                Attachments = Atts,
            };

            bool result = sys_NotesMailBUS.Instance.Add(mail);
            if (result)
            {
                XtraMessageBox.Show("訊息已創建，請等待系統發送。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MsgTP.MsgErrorDB();
            }
        }

        private void f00_SendNoteSystem_Load(object sender, EventArgs e)
        {
#if DEBUG

#else
            if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG)
            {
                string msg = "請使用公司電腦！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                Close();
                return;
            }
#endif
        }

        private void ValidateToken(object sender, TokenEditValidateTokenEventArgs e)
        {
            string name = DomainVNFPG.Instance.GetAccountName(e.Description);
            e.IsValid = !string.IsNullOrEmpty(name);

            e.Value = e.Description;
            e.Description = $"{e.Description} {name}";
        }
    }
}