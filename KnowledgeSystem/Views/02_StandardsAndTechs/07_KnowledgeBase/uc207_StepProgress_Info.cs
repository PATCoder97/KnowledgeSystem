﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
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
    public partial class uc207_StepProgress_Info : DevExpress.XtraEditors.XtraUserControl
    {
        public List<GroupProgress> lsGroupProgress { get; set; }

        public uc207_StepProgress_Info()
        {
            InitializeComponent();
        }

        BindingSource sourceStep = new BindingSource();

        private void uc207_StepProgress_Info_Load(object sender, EventArgs e)
        {
            sourceStep.DataSource = lsGroupProgress;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                var query = db.Groups.ToList();
                cbbGroup.DataSource = query;
                cbbGroup.ValueMember = "Id";
                cbbGroup.DisplayMember = "DisplayName";
                cbbGroup.Columns.AddRange(new[] { new LookUpColumnInfo { FieldName = "DisplayName", Caption = "名稱" } });
            }

            gcStep.DataSource = sourceStep;
        }

        private void btnNewStep_Click(object sender, EventArgs e)
        {
            if (lsGroupProgress == null)
            {
                lsGroupProgress = new List<GroupProgress>();
                lsGroupProgress.Add(new GroupProgress() { IndexStep = 1 });
                sourceStep.DataSource = lsGroupProgress;
            }
            else
            {
                lsGroupProgress.Add(new GroupProgress() { IndexStep = lsGroupProgress.Max(r => r.IndexStep + 1) });
            }

            gcStep.RefreshDataSource();
        }
    }
}