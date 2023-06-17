using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
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
    public partial class f207_fAdvancedSearch : DevExpress.XtraEditors.XtraForm
    {
        RefreshHelper helper;
        public f207_fAdvancedSearch()
        {
            InitializeComponent();
            helper = new RefreshHelper(gvData, "Id");
        }

        List<User> lsUsers = new List<User>();
        List<KnowledgeBase> lsKnowledgeBase = new List<KnowledgeBase>();
        List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();

        int idType = 0;

        BindingSource source = new BindingSource();

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

        private void LoadData()
        {
            helper.SaveViewInfo();
            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                var knowledgeType = lsKnowledgeTypes.Where(r => r.DisplayName == Text).Select(r => r).FirstOrDefault();
                if (knowledgeType != null) { idType = knowledgeType.Id; }

                lsUsers = db.Users.ToList();
                lsKnowledgeBase = db.KnowledgeBases.Select(r => new { r.Id, r.DisplayName, r.Keyword, r.UserRequest, r.IdTypes, r.UserUpload, r.UploadDate })
                    .ToList().Select(r => new KnowledgeBase
                    {
                        Id = r.Id,
                        DisplayName = r.DisplayName,
                        Keyword = r.Keyword,
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

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
            gvData.RefreshData();
        }

        private void f207_fAdvancedSearch_Load(object sender, EventArgs e)
        {
            gcData.DataSource = source;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gvData.FindFilterText = txbKeywords.Text.Trim();
        }

        private void txbKeywords_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txbKeywords.Text = string.Empty;
            gvData.FindFilterText = txbKeywords.Text.Trim();
        }
    }
}