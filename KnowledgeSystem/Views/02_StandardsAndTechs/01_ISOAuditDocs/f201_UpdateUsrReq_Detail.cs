using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DocumentFormat.OpenXml.Spreadsheet;
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

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class f201_UpdateUsrReq_Detail : DevExpress.XtraEditors.XtraForm
    {
        public f201_UpdateUsrReq_Detail(int idUpdateReq)
        {
            IDUpdateReq = idUpdateReq;
            InitializeComponent();
        }

        int IDUpdateReq = 0;

        BindingSource sourceForm = new BindingSource();

        List<dt201_UpdateUsrReq_Detail> baseReqDetail;
        List<dt201_ReqUpdateDocs> reqUpdateDocs;
        List<dm_User> users;

        private void LoadData()
        {
            reqUpdateDocs = dt201_ReqUpdateDocsBUS.Instance.GetList();
            baseReqDetail = dt201_UpdateUsrReq_DetailBUS.Instance.GetListByIdUpdateReq(IDUpdateReq);
            users = dm_UserBUS.Instance.GetList();

            var dataInfo = (from data in baseReqDetail
                            join doc in reqUpdateDocs on data.IdReq equals doc.Id
                            join usr in users on data.UsrComplete equals usr.Id into dtg
                            from g in dtg.DefaultIfEmpty()
                            select new
                            {
                                data,
                                doc,
                                UserComplete = g != null ? g.DisplayName : ""
                            }).ToList();

            sourceForm.DataSource = dataInfo;

            gvData.BestFitColumns();
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void f201_UpdateUsrReq_Detail_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceForm;
            LoadData();
        }
    }
}