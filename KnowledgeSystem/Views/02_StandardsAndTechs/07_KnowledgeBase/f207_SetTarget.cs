﻿using BusinessLayer;
using DataAccessLayer;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.X509;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class f207_SetTarget : DevExpress.XtraEditors.XtraForm
    {
        public f207_SetTarget()
        {
            InitializeComponent();
        }

        private class TargetKnowedge
        {
            public string Id { get; set; }
            public string Grade { get; set; }
            public string Class { get; set; }
            public int Targets { get; set; }
        }

        private void f207_SetTarget_Load(object sender, EventArgs e)
        {
            var lsTargets = dt207_TargetsBUS.Instance.GetList();
            var lsDepts = dm_DeptBUS.Instance.GetList();

            dm_Departments gradeDel = lsDepts.FirstOrDefault(r => r.Id == "7");

            var lsDeptTargets = (from data in lsDepts
                                 join names in lsDepts
                                 on data.IdParent equals names.IdChild into dgt
                                 from d in dgt.DefaultIfEmpty()
                                 select new TargetKnowedge
                                 {
                                     Id = data.Id,
                                     Grade = d?.DisplayName ?? gradeDel.DisplayName,
                                     Class = data.DisplayName,
                                 }
                                 into dtDept
                                 join targets in lsTargets on dtDept.Id equals targets.IdDept into dgt
                                 from g in dgt.DefaultIfEmpty()
                                 select new TargetKnowedge
                                 {
                                     Id = dtDept.Id,
                                     Grade = dtDept.Grade,
                                     Class = dtDept.Class,
                                     Targets = g?.Targets ?? 0
                                 }
                                 ).ToList();

            gcData.DataSource = lsDeptTargets;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gvData.FocusedRowHandle = GridControl.AutoFilterRowHandle;

            List<TargetKnowedge> lsSource = gcData.DataSource as List<TargetKnowedge>;

            List<dt207_Targets> lsTargetsUpdate = (from data in lsSource
                                                   select new dt207_Targets()
                                                   {
                                                       IdDept = data.Id,
                                                       Targets = data.Targets,
                                                   }).ToList();

            foreach (var item in lsTargetsUpdate)
            {
                dt207_TargetsBUS.Instance.AddOrUpdate(item);
            }

            XtraMessageBox.Show("更新成功", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
    }
}