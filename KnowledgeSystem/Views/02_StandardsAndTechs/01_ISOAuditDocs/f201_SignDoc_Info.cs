﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.text.pdf;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_SignDoc_Info : DevExpress.XtraEditors.XtraForm
    {
        public f201_SignDoc_Info()
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
        List<dt201_Role> roleConfirms;

        List<dt201_ProgInfo> progInfos;
        List<dt201_Progress> progress;

        BindingSource sourceAtts = new BindingSource();
        Attachment attachment;

        DateTime minTimeRespValue = default;

        private class Attachment : dm_Attachment
        {
            public bool IsCancel { get; set; }
            public string Desc { get; set; }
            public dm_Attachment BaseAtt { get; set; }
        }

        bool IsLastStep = false;

        private void InitializeIcon()
        {
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
            btnApproval.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.EmailSend;
        }

        private void f201_SignDoc_Info_Load(object sender, EventArgs e)
        {
            var baseData = dt201_FormsBUS.Instance.GetItemById(idBase);
            minTimeRespValue = baseData.UploadTime; // Lấy thời gian đưa lên làm mốc để ký

            Text = $"待簽文件 | {baseData.Code} | {baseData.DisplayName}";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            jobTitles = dm_JobTitleBUS.Instance.GetList();
            roleConfirms = dt201_RoleBUS.Instance.GetList();

            var users = dm_UserBUS.Instance.GetList();
            progress = dt201_ProgressBUS.Instance.GetListByIdBase(idBase);

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

            progInfos = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBase).Where(r => r.IdUsr != "VNW0000000").ToList();
            var progNow = progInfos.OrderByDescending(r => r.RespTime).FirstOrDefault();
            minTimeRespValue = progNow?.RespTime ?? minTimeRespValue;

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

            idRoleConfirm = progress.Where(r => r.IdRole != 0).FirstOrDefault(r => r.IdUsr == TPConfigs.LoginUser.Id)?.IdRole ?? -1;

            btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            // Thêm các file vào gvDocs
            attachment = new Attachment() { BaseAtt = dm_AttachmentBUS.Instance.GetItemById(baseData.AttId ?? -1) };
            sourceAtts.DataSource = attachment;
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
                        btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                        break;
                }
            }
        }

        private void gvDocs_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo hitInfo = view.CalcHitInfo(pt);
            if (!hitInfo.InRowCell) return;

            string fileName = view.GetRowCellValue(view.FocusedRowHandle, gColEncryptName).ToString();

            string sourceFolder = Path.Combine(TPConfigs.Folder201, attachment.BaseAtt.Id.ToString());
            string sourcePath = Path.Combine(sourceFolder, fileName);
            string destPath = Path.Combine(TPConfigs.TempFolderData, $"sign_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!Directory.Exists(TPConfigs.TempFolderData))
                Directory.CreateDirectory(TPConfigs.TempFolderData);

            File.Copy(sourcePath, destPath, true);

            switch (idRoleConfirm)
            {
                case 1:
                    f00_PdfTools pdfTools = new f00_PdfTools(destPath, sourceFolder);
                    pdfTools.ShowDialog();

                    // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
                    string encrytFileName = pdfTools.OutFileName;
                    string describe = pdfTools.Describe;

                    if (string.IsNullOrEmpty(encrytFileName))
                    {
                        attachment.IsCancel = true;
                        attachment.Desc = describe;
                        attachment.BaseAtt.EncryptionName = null;
                    }
                    else
                    {
                        attachment.Desc = "已簽名";
                        attachment.BaseAtt.EncryptionName = encrytFileName;
                    }

                    gvDocs.RefreshData();

                    // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
                    bool CanConfirm = attachment.BaseAtt.EncryptionName != null;
                    btnApproval.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
                case 2:
                    f00_VIewFile fView = new f00_VIewFile(destPath, false, true);
                    fView.ShowDialog();

                    // Truy cập giá trị chuỗi trả về sau khi Form đã đóng
                    bool IsConfirm = fView.IsConfirm;
                    describe = fView.Describe;

                    if (IsConfirm != true) return;// Kiểm tra xem đã xác nhận hay chưa

                    if (string.IsNullOrEmpty(describe))
                    {
                        attachment.Desc = "已確認";
                    }
                    else
                    {
                        attachment.Desc = describe;
                        attachment.IsCancel = true;
                        attachment.BaseAtt.EncryptionName = null;
                    }

                    gvDocs.RefreshData();

                    // Kiểm tra xem có văn kiện nào được đi tiếp không, Nếu có mới hiện nút Approval
                    CanConfirm = attachment.IsCancel != true;
                    btnConfirm.Visibility = CanConfirm ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                    break;
            }
        }

        private void btnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 1. Kiểm tra mô tả đính kèm
            if (string.IsNullOrEmpty(attachment.Desc))
            {
                ShowMessage("請處理所有文件！");
                return;
            }

            // 2. Tạo TextEdit với định dạng giờ
            var editor = new TextEdit
            {
                Font = new Font("Microsoft JhengHei UI", 14F)
            };
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm:ss");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);

            // 3. Hiển thị hộp nhập giờ phản hồi
            string defaultTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>輸入核准時間</font>",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = defaultTime
            });

            if (string.IsNullOrWhiteSpace(result?.ToString())) return;

            // 4. Xử lý kết quả nhập
            if (!DateTime.TryParseExact(result.ToString(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime respTime))
            {
                ShowMessage("時間格式不正確，請重新輸入！");
                return;
            }

            if (!DateTimeHelper.IsWithinWorkingHours(respTime))
            {
                ShowMessage("請選擇工作時間內的時間！");
                return;
            }

            if (minTimeRespValue == DateTime.MinValue || respTime < minTimeRespValue)
            {
                ShowMessage("簽署時間無效！");
                return;
            }

            if (MsgTP.MsgYesNoQuestion($"文件將於<color=red>{respTime:yyyy/MM/dd HH:mm}</color>簽署") != DialogResult.Yes)
                return;

            // 5. Nếu là bước cuối → cập nhật SendNoteTime
            if (IsLastStep)
            {
                var progInfoSendNote = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                    .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString()))
                    .ToList();

                foreach (var item in progInfoSendNote)
                {
                    item.SendNoteTime = DateTime.Now;
                    dt201_ProgInfoBUS.Instance.AddOrUpdate(item);
                }
            }

            // 6. Tạo dòng phản hồi
            var progInfo = new dt201_ProgInfo
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = respTime,
                Desc = "簽名"
            };
            dt201_ProgInfoBUS.Instance.Add(progInfo);

            // 7. Cập nhật form chính
            var form = dt201_FormsBUS.Instance.GetItemById(idBase);
            form.NextStepProg = nextStepProg;

            // 8. Cập nhật file đính kèm
            var baseAtt = attachment.BaseAtt;
            dm_AttachmentBUS.Instance.AddOrUpdate(baseAtt);

            // 9. Nếu là bước cuối → ngưng xử lý và copy file
            if (IsLastStep)
            {
                form.IsProcessing = false;

                string sourceFile = Path.Combine(TPConfigs.Folder201, baseAtt.Id.ToString(), baseAtt.EncryptionName);
                string destFile = Path.Combine(TPConfigs.Folder201, baseAtt.EncryptionName);
                File.Copy(sourceFile, destFile, true);
            }

            // 10. Lưu form
            dt201_FormsBUS.Instance.AddOrUpdate(form);

            // 11. Đóng form
            Close();
        }

        void ShowMessage(string message)
        {
            MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{message}</font>");
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

            var baseData = dt201_FormsBUS.Instance.GetItemById(idBase);
            baseData.IsProcessing = false;
            baseData.NextStepProg = "";
            baseData.IsCancel = true;
            baseData.Descript = $"已由{TPConfigs.LoginUser.DisplayName}退回，退回說明：{describe}";

            dt201_FormsBUS.Instance.AddOrUpdate(baseData);

            // Các bước trước nếu chưa gửi note thì khỏi gửi luôn
            var progInfoSendNote = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString())).ToList();
            foreach (var item in progInfoSendNote)
            {
                item.SendNoteTime = DateTime.Now;
                dt201_ProgInfoBUS.Instance.AddOrUpdate(item);
            }

            // Gửi bước cuối cùng
            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = DateTime.Now,
                Desc = "退回"
            };

            dt201_ProgInfoBUS.Instance.Add(info);

            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // kiểm tra xem đã xử lý hết các file chưa
            bool validate = string.IsNullOrEmpty(attachment.Desc);
            if (validate)
            {
                string msg = "請處理所有文件！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                return;
            }

            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            // Thiết lập mask để buộc nhập đúng định dạng
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd HH:mm:ss");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>輸入核准時間</font>",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") // Định dạng mặc định
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            DateTime respTime;
            DateTime.TryParse(result.ToString(), out respTime);

            if (!DateTimeHelper.IsWithinWorkingHours(respTime))
            {
                string msg = "請選擇工作時間內的時間！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
            }

            if (respTime < minTimeRespValue)
            {
                string msg = "簽署時間無效！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                return;
            }

            if (MsgTP.MsgYesNoQuestion($"文件將於<color=red>{respTime:yyyy/MM/dd HH:mm}</color>簽署") != DialogResult.Yes)
                return;

            // Các bước trước nếu chưa gửi note thì khỏi gửi luôn
            if (IsLastStep)
            {
                var progInfoSendNote = dt201_ProgInfoBUS.Instance.GetListByIdForm(idBase)
                    .Where(r => string.IsNullOrEmpty(r.SendNoteTime.ToString())).ToList();
                foreach (var item in progInfoSendNote)
                {
                    item.SendNoteTime = DateTime.Now;
                    dt201_ProgInfoBUS.Instance.AddOrUpdate(item);
                }
            }

            dt201_ProgInfo info = new dt201_ProgInfo()
            {
                IdForm = idBase,
                IdUsr = TPConfigs.LoginUser.Id,
                RespTime = respTime,
                Desc = "確認"
            };

            dt201_ProgInfoBUS.Instance.Add(info);

            var baseData = dt201_FormsBUS.Instance.GetItemById(idBase);
            baseData.NextStepProg = nextStepProg;

            if (IsLastStep)
            {
                baseData.IsProcessing = false;

                var att = attachment.BaseAtt;
                string sourceFile = Path.Combine(TPConfigs.Folder201, att.Id.ToString(), att.EncryptionName);
                string destFile = Path.Combine(TPConfigs.Folder201, att.EncryptionName);

                File.Copy(sourceFile, destFile, true);
            }

            dt201_FormsBUS.Instance.AddOrUpdate(baseData);

            Close();
        }
    }
}