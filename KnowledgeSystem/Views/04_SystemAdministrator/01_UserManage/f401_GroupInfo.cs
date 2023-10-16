using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage
{
    public partial class f401_GroupInfo : DevExpress.XtraEditors.XtraForm
    {
        public f401_GroupInfo()
        {
            InitializeComponent();
            LockControl(false);
            InitializeIcon();
        }

        public f401_GroupInfo(int idGroup_)
        {
            InitializeComponent();
            LockControl();
            InitializeIcon();

            idGroup = idGroup_;
        }

        #region parameters

        int idGroup = -1;

        List<dm_User> lsUserData = new List<dm_User>();
        List<dm_User> lsUserChoose = new List<dm_User>();

        BindingSource sourceData = new BindingSource();
        BindingSource sourceChoose = new BindingSource();

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDel.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void LoadData()
        {
            var lsDepts = dm_DeptBUS.Instance.GetList().Select(r => new dm_Departments { Id = r.Id, DisplayName = $"{r.Id,-5} {r.DisplayName}" }).ToList();
            cbbDept.Properties.DataSource = lsDepts;
            cbbDept.Properties.DisplayMember = "DisplayName";
            cbbDept.Properties.ValueMember = "Id";

            using (var db = new DBDocumentManagementSystemEntities())
            {
                lsUserData = db.dm_User.ToList();

                var lsGroupUsers = db.dm_GroupUser.Where(r => r.IdGroup == idGroup).ToList();
                foreach (var item in lsGroupUsers)
                {
                    var userHave = lsUserData.FirstOrDefault(r => r.Id == item.IdUser);
                    if (userHave != null)
                    {
                        lsUserChoose.Add(userHave);
                        lsUserData.Remove(userHave);
                    }
                }

                sourceData.DataSource = lsUserData;
                sourceChoose.DataSource = lsUserChoose;
            }
        }

        private void LockControl(bool isFormView = true)
        {
            txbName.ReadOnly = isFormView;
            txbDescribe.ReadOnly = isFormView;
            txbPrioritize.ReadOnly = isFormView;
            cbbDept.ReadOnly = isFormView;

            btnEdit.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDel.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnConfirm.Visibility = isFormView ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;

            Text = isFormView ? "群組信息" : "新增、修改群組";
        }

        #endregion

        private void f401_GroupManage_Info_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvChoose.ReadOnlyGridView();

            gcData.DataSource = sourceData;
            gcChoose.DataSource = sourceChoose;

            LoadData();

            if (idGroup > 0)
            {
                using (var db = new DBDocumentManagementSystemEntities())
                {
                    var groups = db.dm_Group.Where(r => r.Id == idGroup).FirstOrDefault();

                    txbName.EditValue = groups.DisplayName;
                    txbDescribe.EditValue = groups.Describe;
                    txbPrioritize.EditValue = groups.Prioritize;
                    cbbDept.EditValue = groups.IdDept;
                }
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string name = txbName.Text.Trim();
            string moTa = txbDescribe.Text.Trim();
            int prioritize = Convert.ToInt16(txbPrioritize.Text);

            if (string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("請填寫所有信息", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var groups = new dm_Group()
            {
                DisplayName = name,
                Describe = moTa,
                Prioritize = prioritize
            };

            using (var db = new DBDocumentManagementSystemEntities())
            {
                if (idGroup == 0)
                {
                    db.dm_Group.Add(groups);
                }
                else
                {
                    groups.Id = idGroup;
                    db.dm_Group.AddOrUpdate(groups);

                    db.dm_GroupUser.RemoveRange(db.dm_GroupUser.Where(r => r.IdGroup == idGroup));
                }

                var newGroupUserList = lsUserChoose.Select(item => new dm_GroupUser
                {
                    IdGroup = groups.Id,
                    IdUser = item.Id
                });

                db.dm_GroupUser.AddRange(newGroupUserList);

                db.SaveChanges();
            }

            Close();
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LockControl(false);
        }

        private void btnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var db = new DBDocumentManagementSystemEntities())
            {
                db.dm_Group.RemoveRange(db.dm_Group.Where(r => r.Id == idGroup));
                db.dm_GroupUser.RemoveRange(db.dm_GroupUser.Where(r => r.IdGroup == idGroup));
                db.SaveChanges();
            }

            Close();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvData.FocusedRowHandle;
            if (forcusRow < 0) return;

            dm_User dataRow = gvData.GetRow(forcusRow) as dm_User;

            lsUserChoose.Add(dataRow);
            lsUserData.Remove(dataRow);

            gcData.RefreshDataSource();
            gcChoose.RefreshDataSource();
        }

        private void gvChoose_DoubleClick(object sender, EventArgs e)
        {
            int forcusRow = gvChoose.FocusedRowHandle;
            if (forcusRow < 0) return;

            dm_User dataRow = gvChoose.GetRow(forcusRow) as dm_User;

            lsUserChoose.Remove(dataRow);
            lsUserData.Add(dataRow);

            gcData.RefreshDataSource();
            gcChoose.RefreshDataSource();
        }

        private void cbbDept_AutoSearch(object sender, LookUpEditAutoSearchEventArgs e)
        {
            e.ClearHighlight();
        }
    }
}