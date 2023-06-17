using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_Main : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;
        public f207_Main()
        {
            InitializeComponent();
            helper = new RefreshHelper(gvData, "Id");
        }

        private class DataDisplay
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string UserRequest { get; set; }
            public string UserRequestName { get; set; }
            public string TypeName { get; set; }
            public string Keyword { get; set; }
            public string UserUploadName { get; set; }
            public DateTime? UploadDate { get; set; }
        }

        List<User> lsUsers = new List<User>();
        List<KnowledgeBase> lsKnowledgeBase = new List<KnowledgeBase>();
        List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();

        int idType = 0;

        BindingSource source = new BindingSource();

        private void LoadData()
        {
            helper.SaveViewInfo();
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                var knowledgeType = lsKnowledgeTypes.Where(r => r.DisplayName == Text).Select(r => r).FirstOrDefault();
                if (knowledgeType != null) { idType = knowledgeType.Id; }

                lsUsers = db.Users.ToList();
                lsKnowledgeBase = db.KnowledgeBases.Select(r => new { r.Id, r.DisplayName, r.UserRequest, r.IdTypes, r.UserUpload, r.UploadDate })
                    .Where(r => r.IdTypes == idType)
                    .ToList().Select(r => new KnowledgeBase
                    {
                        Id = r.Id,
                        DisplayName = r.DisplayName,
                        UserRequest = r.UserRequest,
                        IdTypes = r.IdTypes,
                        UserUpload = r.UserUpload,
                        UploadDate = r.UploadDate
                    }).ToList();
            }

            var lsDataDisplays = (from data in lsKnowledgeBase
                                  join userUpload_ in lsUsers on data.UserUpload equals userUpload_.Id
                                  join userRequest_ in lsUsers on data.UserRequest equals userRequest_.Id
                                  join type_ in lsKnowledgeTypes on data.IdTypes equals type_.Id
                                  select new DataDisplay
                                  {
                                      Id = data.Id,
                                      DisplayName = data.DisplayName,
                                      UserRequest = data.UserRequest,
                                      UserRequestName = userRequest_.DisplayName,
                                      TypeName = type_.DisplayName,
                                      Keyword = data.Keyword,
                                      UserUploadName = userUpload_.DisplayName,
                                      UploadDate = data.UploadDate
                                  }).ToList();

            source.DataSource = lsDataDisplays;
            helper.LoadViewInfo();
        }

        private void f207_Main_Load(object sender, EventArgs e)
        {
            LoadData();
            gcData.DataSource = source;

            gvData.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            f207_DocumentInfo fAddDocument = new f207_DocumentInfo(idType);
            fAddDocument.ShowDialog();

            LoadData();
            gcData.RefreshDataSource();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
            gcData.RefreshDataSource();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvData.FocusedRowHandle;
            if (forcusRow < 0) return;

            DataDisplay dataRow = gvData.GetRow(forcusRow) as DataDisplay;
            string idDocuments = dataRow.Id;

            f207_DocumentInfo fDocumentInfo = new f207_DocumentInfo(idDocuments);
            fDocumentInfo.ShowDialog();

            LoadData();
            gcData.RefreshDataSource();
        }
    }
}