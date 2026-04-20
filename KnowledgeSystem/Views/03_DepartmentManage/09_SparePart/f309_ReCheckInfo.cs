using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class f309_ReCheckInfo : DevExpress.XtraEditors.XtraForm
    {
        public f309_ReCheckInfo()
        {
            InitializeComponent();
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Cancel;
        }

        public bool _IsUploadAbnormal = false;
        public List<dt309_InspectionBatchMaterial> InspectionBatchMaterials { get; set; } = new List<dt309_InspectionBatchMaterial>();
        public Dictionary<int, string> InspectionPhotoNames { get; set; } = new Dictionary<int, string>();
        public bool _isChecked { get; set; } = false;

        private void f309_ReCheckInfo_Load(object sender, EventArgs e)
        {
            var materials = dt309_MaterialsBUS.Instance.GetList();

            var depts = dm_DeptBUS.Instance.GetList();
            var users = dm_UserBUS.Instance.GetList();
            var units = dt309_UnitsBUS.Instance.GetList();
            Dictionary<string, dm_Attachment> photoAttachmentsByThread = dm_AttachmentBUS.Instance
                .GetListByThreads(InspectionBatchMaterials.Select(x => Material309CheckPhotoHelper.GetThread(x.Id)).ToList())
                .GroupBy(x => x.Thread)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Id).First());

            var batchMaterialList = InspectionBatchMaterials
                .Join(materials,
                      bm => bm.MaterialId,
                      m => m.Id,
                      (bm, m) =>
                      {
                          string thread = Material309CheckPhotoHelper.GetThread(bm.Id);
                          photoAttachmentsByThread.TryGetValue(thread, out dm_Attachment photoAttachment);

                          return new
                          {
                              Material = m,
                              BatchMaterial = bm,
                              Unit = units.FirstOrDefault(r => r.Id == m.IdUnit)?.DisplayName ?? "N/A",
                              UserMngr = users.FirstOrDefault(r => r.Id == m.IdManager)?.DisplayName ?? "N/A",
                              Dept = (depts.Where(r => r.Id == m.IdDept).Select(r => $"{r.Id} {r.DisplayName}").FirstOrDefault() ?? "N/A") + (bm.IsComplete != true ? " - 處理中" : " - 已完成"),
                              UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                              IniQuantity = _IsUploadAbnormal ? bm.InitialQuantity : -1,
                              Desc = bm.Description,
                              PhotoName = InspectionPhotoNames.TryGetValue(bm.Id, out string photoName)
                                  ? photoName
                                  : photoAttachment?.ActualName ?? string.Empty
                          };
                      })
                .ToList();

            gcData.DataSource = batchMaterialList;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvSparePart.BestFitColumns();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_IsUploadAbnormal)
            {
                if (InspectionBatchMaterials.Any(r => string.IsNullOrEmpty(r.Description)))
                {
                    XtraMessageBox.Show("請確認所有物料的異常說明是否已填寫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (InspectionBatchMaterials.Any(r => r.ActualQuantity == null))
                {
                    XtraMessageBox.Show("請先確認所有物料的實際數量！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> missingPhotoMaterials = InspectionBatchMaterials
                    .Where(r => !InspectionPhotoNames.ContainsKey(r.Id) || string.IsNullOrWhiteSpace(InspectionPhotoNames[r.Id]))
                    .Select(r => r.MaterialId)
                    .ToList();

                if (missingPhotoMaterials.Count > 0)
                {
                    string preview = string.Join("、", missingPhotoMaterials.Take(8));
                    if (missingPhotoMaterials.Count > 8)
                    {
                        preview += " ...";
                    }

                    XtraMessageBox.Show($"請先確認所有物料的盤點圖片！缺少圖片的編碼：{preview}", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _isChecked = true;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _isChecked = false;
            Close();
        }
    }
}
