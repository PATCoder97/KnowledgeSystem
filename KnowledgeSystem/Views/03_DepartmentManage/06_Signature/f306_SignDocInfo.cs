﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Spreadsheet;
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
            InitializeIcon();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public int idBase = -1;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);

        int idRoleConfirm = -1;
        string nextStepProg = "";

        List<dm_JobTitle> jobTitles;
        List<dt306_Role> roleConfirms;

        List<dt306_ProgInfo> progInfos;
        List<dt306_Progress> progress;
        dt201_Forms baseForm;

        BindingSource sourceAtts = new BindingSource();
        List<Attachment> baseAtts;

        bool IsLastStep = false;

        private class Attachment : dt306_BaseAtts
        {
            public string EncryptName { get; set; }
            public dt306_BaseAtts BaseAtt { get; set; }
        }

        private void InitializeIcon()
        {
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
            btnApproval.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f306_SignDocInfo_Load(object sender, EventArgs e)
        {
            Text = $"核簽{formName}";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

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

            progInfos = dt306_ProgInfoBUS.Instance.GetListByIdBase(idBase).Where(r => r.IdUsr != "VNW0000000").ToList();
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();

            int stepNow = progNow != null ? progress.IndexOf(progress.First(r => r.IdUsr == progNow.IdUsr)) : -1;
            stepProgressDoc.SelectedItemIndex = stepNow; // Focus đến bước hiện tại

            var nextStepUsr = progress[stepNow + 1].IdUsr;
            nextStepProg = stepNow + 2 >= progress.Count ? "" : progress[stepNow + 2].IdUsr;

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

            btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            // Thêm các file vào gvDocs
            baseAtts = dt306_BaseAttsBUS.Instance.GetListByIdBase(idBase).Where(r => r.IsCancel != true).Select(r => new Attachment() { BaseAtt = r }).ToList();
            sourceAtts.DataSource = baseAtts;
            gcDocs.DataSource = sourceAtts;
            gvDocs.ReadOnlyGridView();

            if (nextStepUsr == TPConfigs.LoginUser.Id)
            {
                switch (idRoleConfirm)
                {
                    case 1:
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

            string sourceFolder = Path.Combine(TPConfigs.Folder306, idBase.ToString());
            string sourcePath = Path.Combine(sourceFolder, fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            f00_PdfTools pdfTools = new f00_PdfTools(destPath, sourceFolder);
            pdfTools.ShowDialog();

            // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
            string encrytFileName = pdfTools.OutFileName;
            string describe = pdfTools.Describe;

            Attachment itemToUpdate = baseAtts.SingleOrDefault(item => item.BaseAtt.IdAtt == idAtt);
            if (string.IsNullOrEmpty(encrytFileName))
            {
                itemToUpdate.BaseAtt.Desc = describe;
                itemToUpdate.BaseAtt.UsrCancel = TPConfigs.LoginUser.Id;
                itemToUpdate.BaseAtt.IsCancel = true;
                itemToUpdate.EncryptName = null;
            }
            else
            {
                itemToUpdate.BaseAtt.Desc = "已簽名";
                itemToUpdate.EncryptName = encrytFileName;
            }

            gvDocs.RefreshData();

            // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
            bool CanConfirm = baseAtts.Any(r => r.EncryptName != null);
            btnApproval.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void btnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dt306_ProgInfo info = new dt306_ProgInfo()
            {
                IdBase = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Desc = "簽名"
            };

            dt306_ProgInfoBUS.Instance.Add(info);

            var baseData = dt306_BaseBUS.Instance.GetItemById(idBase);
            baseData.NextStepProg = nextStepProg;

            foreach (var item in baseAtts)
            {
                if (!string.IsNullOrEmpty(item.EncryptName))
                {
                    var att = dm_AttachmentBUS.Instance.GetItemById(item.BaseAtt.IdAtt);
                    att.EncryptionName = item.EncryptName;
                    dm_AttachmentBUS.Instance.AddOrUpdate(att);
                }
                else
                {
                    dt306_BaseAttsBUS.Instance.AddOrUpdate(item.BaseAtt);
                }
            }

            if (IsLastStep)
            {
                baseData.IsProcess = false;

                foreach (var item in baseAtts)
                {
                    if (!string.IsNullOrEmpty(item.EncryptName))
                    {
                        string sourceFile = Path.Combine(TPConfigs.Folder306, idBase.ToString(), item.EncryptName);
                        string destFile = Path.Combine(TPConfigs.Folder306, item.EncryptName);

                        File.Copy(sourceFile, destFile, true);
                    }
                }
            }

            dt306_BaseBUS.Instance.AddOrUpdate(baseData);

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;
            string describe = result?.ToString() ?? "";

            var baseData = dt306_BaseBUS.Instance.GetItemById(idBase);
            baseData.NextStepProg = "";
            baseData.IsProcess = false;
            baseData.IsCancel = true;
            baseData.Desc = describe;

            dt306_BaseBUS.Instance.AddOrUpdate(baseData);

            dt306_ProgInfo info = new dt306_ProgInfo()
            {
                IdBase = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Desc = "退回"
            };

            dt306_ProgInfoBUS.Instance.Add(info);

            Close();
        }
    }
}