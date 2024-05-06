using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void f201_DocSignInfo_Load(object sender, EventArgs e)
        {
            var users = dm_UserBUS.Instance.GetList();
            var progress = dt201_ProgressBUS.Instance.GetListByIdForm(idBaseForm);

            var progressInfo = (from data in progress
                                join usr in users on data.IdUser equals usr.Id
                                select new { data, usr }).ToList();

            // Thêm danh sách các bước vào StepProgressBar
            foreach (var item in progressInfo)
                stepProgressDoc.Items.Add(new StepProgressBarItem(item.usr.DisplayName));
            stepProgressDoc.ItemOptions.Indicator.Width = 40;

            var progInfos = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBaseForm);
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();

            int stepNow = progNow != null ? progInfos.IndexOf(progNow) : -1;
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
            f00_PdfTools pdfTools = new f00_PdfTools(@"C:\Users\ANHTUAN\Desktop\New folder\Blank.pdf", TPConfigs.Folder201);
            pdfTools.ShowDialog();

            // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            string returnedString = pdfTools.OutFileName;
        }
    }
}