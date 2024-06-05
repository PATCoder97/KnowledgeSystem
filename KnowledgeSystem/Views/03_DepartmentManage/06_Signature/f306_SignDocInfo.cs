using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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

namespace KnowledgeSystem.Views._03_DepartmentManage._06_Signature
{
    public partial class f306_SignDocInfo : DevExpress.XtraEditors.XtraForm
    {
        public f306_SignDocInfo()
        {
            InitializeComponent();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        int idRoleConfirm = -1;

        List<dm_JobTitle> jobTitles;
        List<dt306_Role> roleConfirms;

        List<dt306_ProgInfo> progInfos;
        List<dt306_Progress> progress;
        dt201_Forms baseForm;

        BindingSource sourceAtts = new BindingSource();
        List<dt306_BaseAtts> baseAtts;

        bool IsLastStep = false;

        private void f306_SignDocInfo_Load(object sender, EventArgs e)
        {
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            baseForm = dt201_FormsBUS.Instance.GetItemById(idBase);
            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roleConfirms = dt306_RoleBUS.Instance.GetList();

            var users = dm_UserBUS.Instance.GetList();
            progress = dt306_ProgressBUS.Instance.GetListByIdBase(idBase);

            var progressInfo = (from data in progress
                                join usr in users on data.IdUsr equals usr.Id
                                select new { data, usr }).ToList();

            // Thêm danh sách các bước vào StepProgressBar
            foreach (var item in progressInfo)
            {
                var barItem = new StepProgressBarItem();
                barItem.ContentBlock1.Caption = $"{item.usr.IdDepartment} {item.usr.DisplayName}";
                barItem.ContentBlock1.Description = $"{item.usr.Id}\r\n{jobTitles.FirstOrDefault(r => r.Id == item.usr.JobCode).DisplayName}";
                barItem.ContentBlock2.Caption = roleConfirms.FirstOrDefault(r => r.Id == item.data.IdRole)?.DisplayName;
                stepProgressDoc.Items.Add(barItem);
            }
            stepProgressDoc.ItemOptions.Indicator.Width = 40;

            progInfos = dt306_ProgInfoBUS.Instance.GetListByIdBase(idBase);
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();

            int stepNow = progNow != null ? progress.IndexOf(progress.First(r => r.IdUsr == progNow.IdUsr)) : -1;
            stepProgressDoc.SelectedItemIndex = stepNow; // Focus đến bước hiện tại

            var nextStepUsr = progress[stepNow + 1].IdUsr;

            IsLastStep = stepNow == progress.Count() - 2;

            // Thêm lịch sử trình ký vào gridProcess
            var lsHistoryProcess = (from data in progInfos
                                    join usr in users on data.IdUsr equals usr.Id
                                    join job in jobTitles on usr.JobCode equals job.Id
                                    select new
                                    {
                                        data,
                                        usr,
                                        job,
                                        DisplayName = $"{usr.Id} LG{usr.IdDepartment}/{usr.DisplayName}"
                                    }).ToList();

            gcHistoryProcess.DataSource = lsHistoryProcess;

            gvHistoryProcess.ReadOnlyGridView();

            idRoleConfirm = progress.FirstOrDefault(r => r.IdUsr == TPConfigs.LoginUser.Id)?.IdRole ?? -1;

            btnSign.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            // Thêm các file vào gvDocs
            baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase);
            sourceAtts.DataSource = baseAtts;
            gcDocs.DataSource = sourceAtts;
            gvDocs.ReadOnlyGridView();

            if (nextStepUsr == TPConfigs.LoginUser.Id)
            {
                switch (idRoleConfirm)
                {
                    case 1:
                        btnSign.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;
                    case 2:
                        btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;

                }
            }
        }

        private void gvDocs_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            int idAtt = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdAtt));
            var attProgress = dm_AttachmentBUS.Instance.GetItemById(idAtt);
            string fileName = attProgress?.EncryptionName ?? "";

            string sourcePath = Path.Combine(TPConfigs.Folder201, idAtt.ToString(), fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_PdfTools pdfTools = new f00_PdfTools(destPath, Path.Combine(TPConfigs.Folder201, idAtt.ToString()));
            pdfTools.ShowDialog();
        }
    }
}