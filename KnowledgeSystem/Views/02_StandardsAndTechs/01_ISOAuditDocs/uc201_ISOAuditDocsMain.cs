using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_ISOAuditDocsMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_ISOAuditDocsMain()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<TestData> testData;

        class TestData
        {
            public string Id { get; set; }
            public string TypeDoc { get; set; }
            public string DisplayName { get; set; }

            public static List<TestData> GenerateData()
            {
                return new List<TestData>()
                {
                    new TestData(){Id = "GA-700-B001",TypeDoc ="....",DisplayName = "實驗室資訊保密管理規定"},
                    new TestData(){Id = "GA-701-A01",TypeDoc ="....",DisplayName = "Quy trinh XYZ"},
                    new TestData(){Id = "GA-702-C01",TypeDoc ="....",DisplayName = "Quy trinh 123"},
                };
            }
        }

        public class FileObject
        {
            public string Version { get; set; }
            public string FileName { get; set; }
            public string DisplayName { get; set; }
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            testData = TestData.GenerateData();
            gcData.DataSource = testData;
        }

        private void uc201_ISOAuditDocsMain_Load(object sender, EventArgs e)
        {
            LoadData();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            gvVersion.ReadOnlyGridView();
            gvVersion.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;

            //gvAttachment.ReadOnlyGridView();
        }

        // Master-Detail : gvData
        private void gvData_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            //GridView view = sender as GridView;
            //string idBase = view.GetRowCellValue(e.RowHandle, gColId)?.ToString();
            //var reports = attachments.Where(r => r.IdBase == idBase).Select(r => r.Id).ToList();

            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            GridView view = sender as GridView;
            string idBase = view.GetRowCellValue(e.RowHandle, gColId).ToString();

            List<FileObject> fileList = new List<FileObject> { new FileObject { Version = "01", FileName = "file123.pdf" }, new FileObject { Version = "02", FileName = "document456.txt" }, new FileObject { Version = "03", FileName = "image789.jpg" } };

            e.ChildList = fileList;
        }

        private void gvView_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvView_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            GridView view = sender as GridView;
            switch (view.Name)
            {
                case "gvData":
                    e.RelationName = "發佈历史";
                    break;
                case "gvVersion":
                    e.RelationName = "表單";
                    break;
            }
        }

        private void gvVersion_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvVersion_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            List<FileObject> fileList = new List<FileObject> { new FileObject { DisplayName = "gasdghas.pdf" }, new FileObject { DisplayName = "31asdasd.pdf" } };
            e.ChildList = fileList;
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f00_PdfTools frm = new f00_PdfTools(@"C:\Users\ANHTUAN\Desktop\New folder\Blank.pdf");
            frm.ShowDialog();
                
        }
    }
}
