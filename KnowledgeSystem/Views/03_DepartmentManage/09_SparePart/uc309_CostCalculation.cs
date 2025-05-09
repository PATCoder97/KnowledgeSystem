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
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Utils;
using KnowledgeSystem.Views._03_DepartmentManage._01_SafetyCertificate;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public partial class uc309_CostCalculation : DevExpress.XtraEditors.XtraUserControl
    {
        public uc309_CostCalculation()
        {
            InitializeComponent();
        }

        DateTime dateFrom, dateTo;

        private void LoadData()
        {
            uc309_ChooseDate ucInfo = new uc309_ChooseDate();
            if (XtraDialog.Show(ucInfo, "選時間", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            if (ucInfo.DateTo >= ucInfo.DateForm)
            {
                dateFrom = ucInfo.DateForm;
                dateTo = ucInfo.DateTo;
            }
            else
            {
                XtraMessageBox.Show("結束時間必須大於或等於開始時間。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var datas = dt309_TransactionsBUS.Instance.GetListByDate(dateFrom, dateTo).Where(r => r.TransactionType == "out");
            var materials = dt309_MaterialsBUS.Instance.GetList();

            //var displayData = (from data in datas
            //                   join material in materials on data.MaterialId equals material.Id
            //                   group data by new { data.StorageId, material } into materialGroup
            //                   select new
            //                   {
            //                       Dept = materialGroup.Key.material.IdDept,
            //                       StorageId = materialGroup.Key.StorageId,
            //                       MaterialName = materialGroup.Key.material,
            //                       Count = materialGroup.Count(),
            //                       TotalAmount = materialGroup.Sum(d => d.Quantity * materialGroup.Key.material.Price)
            //                   }).ToList();


            var displayData = (from data in datas
                               join material in materials on data.MaterialId equals material.Id
                               group new { data, material } by material.IdDept into deptGroup
                               select new
                               {
                                   Dept = deptGroup.Key,  // Phòng ban (Dept)
                                   Materials = deptGroup.GroupBy(x => x.data.StorageId).Select(storageGroup => new
                                   {
                                       StorageId = storageGroup.Key,                    // Kho lưu trữ
                                       Items = storageGroup.Select(item => new
                                       {
                                           MaterialName = item.material.DisplayName,           // Tên vật liệu
                                           Count = storageGroup.Count(),                // Số lần xuất hiện
                                           TotalQuantity = storageGroup.Sum(d => d.data.Quantity),  // Tổng số lượng
                                           TotalAmount = storageGroup.Sum(d => d.data.Quantity * item.material.Price) // Tổng tiền
                                       }).ToList()
                                   }).ToList()
                               }).ToList();

        }

        private void uc309_CostCalculation_Load(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
