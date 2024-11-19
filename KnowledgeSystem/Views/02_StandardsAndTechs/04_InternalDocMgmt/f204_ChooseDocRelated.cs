using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._04_InternalDocMgmt
{
    public partial class f204_ChooseDocRelated : DevExpress.XtraEditors.XtraForm
    {
        public f204_ChooseDocRelated()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idBaseDoc = -1;
        public List<dt204_InternalDocMgmt> DocsInput { get; set; }
        public List<dt204_InternalDocMgmt> DocsOutput { get; set; }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void LoadData()
        {
            var idsDoc = DocsInput.Select(r => r.Id).ToList();

            // Lọc danh sách người dùng theo điều kiện
            var docs = dt204_InternalDocMgmtBUS.Instance.GetList()
                .Where(r => !idsDoc.Contains(r.Id) && r.Id != idBaseDoc)
                .ToList();

            gcData.DataSource = docs;
            gvData.BestFitColumns();
        }

        private void f204_ChooseDocRelated_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var rows = gvData.GetSelectedRows();

            List<dt204_InternalDocMgmt> docsOutput = new List<dt204_InternalDocMgmt>();
            foreach (var item in rows)
            {
                var data = gvData.GetRow(item) as dt204_InternalDocMgmt;

                if (data != null)
                {
                    //// Extract the original dm_User object from the anonymous type
                    //var usr = ((dynamic)data).usr as dm_User;
                    docsOutput.Add(data);
                }
            }

            DocsOutput = docsOutput;
            Close();
        }
    }
}