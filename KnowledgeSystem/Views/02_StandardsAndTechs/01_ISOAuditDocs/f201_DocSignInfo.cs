using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_DocSignInfo : DevExpress.XtraEditors.XtraForm
    {
        public f201_DocSignInfo()
        {
            InitializeComponent();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBaseForm = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        List<dt201_ProgInfo> progInfos;
        List<dt201_Progress> progress;
        dt201_Forms baseForm;

        private void f201_DocSignInfo_Load(object sender, EventArgs e)
        {
            baseForm = dt201_FormsBUS.Instance.GetItemById(idBaseForm);
            var users = dm_UserBUS.Instance.GetList();
            progress = dt201_ProgressBUS.Instance.GetListByIdForm(idBaseForm);

            var progressInfo = (from data in progress
                                join usr in users on data.IdUser equals usr.Id
                                select new { data, usr }).ToList();

            // Thêm danh sách các bước vào StepProgressBar
            foreach (var item in progressInfo)
                stepProgressDoc.Items.Add(new StepProgressBarItem(item.usr.DisplayName));
            stepProgressDoc.ItemOptions.Indicator.Width = 40;

            progInfos = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBaseForm);
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();

            int stepNow = progNow != null ? progress.IndexOf(progress.First(r => r.IdUser == progNow.IdUser)) : -1;
            stepProgressDoc.SelectedItemIndex = stepNow; // Focus đến bước hiện tại

            // Thêm lịch sử trình ký vào gridProcess
            var lsHistoryProcess = (from data in progInfos
                                    join usr in users on data.IdUser equals usr.Id
                                    select new
                                    {
                                        data,
                                        usr
                                    }).ToList();

            gcHistoryProcess.DataSource = lsHistoryProcess;

            gvHistoryProcess.ReadOnlyGridView();
        }

        private void btnSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int idAtt = baseForm.AttId ?? -1;
            string fileName = dm_AttachmentBUS.Instance.GetItemById(idAtt)?.EncryptionName ?? "";

            string sourcePath = Path.Combine(TPConfigs.Folder201, fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_PdfTools pdfTools = new f00_PdfTools(destPath, Path.Combine(TPConfigs.Folder201, idAtt.ToString()));
            pdfTools.ShowDialog();

            // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            string encrytFileName = pdfTools.OutFileName;
            if (encrytFileName == null) return;

            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdAtt = idAtt,
                IdForm = idBaseForm,
                IdUser = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Note = "已簽名"
            };

            dt201_ProgInfoBUS.Instance.Add(info);
        }
    }
}