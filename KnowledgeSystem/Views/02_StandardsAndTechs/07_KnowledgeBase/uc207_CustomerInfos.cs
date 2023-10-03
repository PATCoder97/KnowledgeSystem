using DataAccessLayer;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Configs;
using KnowledgeSystem.Views._04_SystemAdministrator._01_UserManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_CustomerInfos : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc207_CustomerInfos()
        {
            InitializeComponent();
        }

        Font fontDFKaiSB12 = new Font("DFKai-SB", 12.0f, FontStyle.Regular);
        BindingSource sourceCustomers = new BindingSource();

        private void LoadCustomers()
        {

        }

        private void uc207_CustomerInfos_Load(object sender, EventArgs e)
        {
            gvData.OptionsEditForm.ShowOnDoubleClick = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsEditForm.ShowOnF2Key = DevExpress.Utils.DefaultBoolean.False;
            gvData.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;

            gcData.DataSource = sourceCustomers;
            helper = new RefreshHelper(gvData, "Id");
            LoadCustomers();

            gvData.OptionsEditForm.CustomEditFormLayout = new uc401_UserManage_Info();

            gvData.RowUpdated += GridView_RowUpdated;

            // Thêm 2 nút Sửa, Xóa vào row để mở EditForms hoặc xóa quyền hạn
            RepositoryItemButtonEdit commandsEvent = new RepositoryItemButtonEdit { AutoHeight = false, Name = "CommandsEdit", TextEditStyle = TextEditStyles.HideTextEditor };
            commandsEvent.Buttons.Clear();

            Image imgEdit = Properties.Resources.pen_16px;
            Color colorEdit = DXSkinColors.FillColors.Question;

            DevExpress.Utils.AppearanceObject appearanceEdit = new DevExpress.Utils.AppearanceObject();
            appearanceEdit.Font = fontDFKaiSB12;
            appearanceEdit.ForeColor = colorEdit;

            DevExpress.Utils.AppearanceObject appearanceDelete = new DevExpress.Utils.AppearanceObject();
            appearanceDelete.Font = fontDFKaiSB12;

            EditorButton btnEdit = new EditorButton(ButtonPredefines.Glyph, "修改", -1, true, true, false, ImageLocation.Default, imgEdit, null, appearanceEdit);

            commandsEvent.Buttons.Add(btnEdit);

            int widthCol = 70;
            GridColumn _commandsColumn = gvData.Columns.AddField("事件");
            _commandsColumn.Visible = true;
            _commandsColumn.Width = widthCol;
            _commandsColumn.MaxWidth = widthCol;
            _commandsColumn.MinWidth = widthCol;
            _commandsColumn.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;

            gvData.CustomRowCellEdit += (s, ee) =>
            {
                if (ee.RowHandle == gvData.FocusedRowHandle && ee.Column == _commandsColumn)
                    ee.RepositoryItem = commandsEvent;
            };

            gvData.CustomRowCellEditForEditing += (s, ee) =>
            {
                if (ee.RowHandle == gvData.FocusedRowHandle && ee.Column == _commandsColumn)
                    ee.RepositoryItem = commandsEvent;
            };

            gvData.ShowingEditor += (s, ee) =>
            {
                ee.Cancel = gvData.FocusedColumn != _commandsColumn;
            };

            // Event nút nhấn Sửa, Xóa
            commandsEvent.ButtonClick += CommandsEvent_ButtonClick;
        }

        private void CommandsEvent_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            switch (e.Button.Caption)
            {
                case "修改":
                    gvData.CloseEditor();
                    gvData.ShowEditForm();
                    break;
            }
        }

        private void GridView_RowUpdated(object sender, RowObjectEventArgs e)
        {
            User rowUser = e.Row as User;

            using (var db = new DBDocumentManagementSystemEntities())
            {
                db.Users.AddOrUpdate(rowUser);
                db.SaveChanges();
            }

            LoadCustomers();

            XtraMessageBox.Show("Thao tác sửa thành công!", TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            uc207_CustomerInfos_Info ucInfo = new uc207_CustomerInfos_Info();
            if (XtraDialog.Show(ucInfo, "新增客戶信息", MessageBoxButtons.OKCancel) != DialogResult.OK) return;

            //var userNew = new User()
            //{
            //    Id = ucInfo.Id,
            //    DisplayName = ucInfo.DisplayName,
            //    IdDepartment = ucInfo.IdDept.ToString(),
            //    IdRole = ucInfo.IdRole,
            //    DateCreate = DateTime.Now
            //};

            //string msg = "Thao tác sửa thành công!";
            //using (var db = new DBDocumentManagementSystemEntities())
            //{
            //    db.Users.Add(userNew);
            //    try
            //    {
            //        db.SaveChanges();
            //    }
            //    catch (DbEntityValidationException ex)
            //    {
            //        msg = string.Empty;
            //        foreach (var entityValidationErrors in ex.EntityValidationErrors)
            //        {
            //            foreach (var validationError in entityValidationErrors.ValidationErrors)
            //            {
            //                msg += "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
            //            }
            //        }
            //    }
            //}

            //LoadCustomers();
            //XtraMessageBox.Show(msg, TempDatas.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadCustomers();
        }
    }
}
