using BusinessLayer;
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
using System.Web.Security;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_AuditDocsMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_AuditDocsMain()
        {
            InitializeComponent();
        }

        BindingSource sourceFunc = new BindingSource();

        private void LoadData()
        {
            var bases = dt201_BaseBUS.Instance.GetList().ToList();

            //lsFunctions = (from data in db.dm_Function.OrderBy(r => r.Prioritize).ToList()
            //               join funcs in lsFuncRole on data.Id equals funcs.IdFunction into dtg
            //               from p in dtg.DefaultIfEmpty()
            //               select new dm_FunctionM
            //               {
            //                   Id = data.Id,
            //                   IdParent = data.IdParent,
            //                   DisplayName = data.DisplayName,
            //                   ControlName = data.ControlName,
            //                   Prioritize = data.Prioritize,
            //                   Status = p != null,
            //                   Images = data.Images,
            //               }).ToList();

            sourceFunc.DataSource = bases;

            //lsRoles = dm_RoleBUS.Instance.GetList();
            //sourceRole.DataSource = lsRoles;
            //gcRoles.DataSource = sourceRole;
            //gvRoles.BestFitColumns();

            treeFolder.RefreshDataSource();
            //gcRoles.RefreshDataSource();
        }

        private void uc201_AuditDocsMain_Load(object sender, EventArgs e)
        {
            LoadData();

            //sourceFunc.DataSource = lsFunctions;
            treeFolder.DataSource = sourceFunc;
            treeFolder.KeyFieldName = "Id";
            treeFolder.ParentFieldName = "IdParent";
            //treeFolder.CheckBoxFieldName = "Status";
            treeFolder.BestFitColumns();

            treeFolder.ReadOnlyTreelist();
        }
    }
}
