﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_UpdateUsrReq : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_UpdateUsrReq()
        {
            InitializeComponent();
            InitializeIcon();
        }

        List<dt201_UpdateUsrReq> baseReq;
        List<dm_User> users;
        List<dm_Departments> depts;

        BindingSource sourceForm = new BindingSource();

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void LoadData()
        {
            baseReq = dt201_UpdateUsrReqBUS.Instance.GetList();
            users = dm_UserBUS.Instance.GetList();
            depts = dm_DeptBUS.Instance.GetList();

            var dataInfo = (from data in baseReq
                            join usr in users on data.IdUsr equals usr.Id
                            join dept in depts on data.IdDept equals dept.Id
                            select new
                            {
                                data,
                                usr,
                                dept
                            }).ToList();

            sourceForm.DataSource = dataInfo;

            gvData.BestFitColumns();
            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
        }

        private void uc201_UpdateUsrReq_Load(object sender, EventArgs e)
        {
            gcData.DataSource = sourceForm;
            LoadData();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int idReq = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColId));

            f201_UpdateUsrReq_Detail fDetail = new f201_UpdateUsrReq_Detail(idReq);
            fDetail.ShowDialog();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"人員異動 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }
    }
}
