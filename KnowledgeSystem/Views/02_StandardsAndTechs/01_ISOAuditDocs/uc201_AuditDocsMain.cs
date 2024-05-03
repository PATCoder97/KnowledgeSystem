using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
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
            InitializeMenuItems();
        }

        DXMenuItem itemAddNode;
        DXMenuItem itemAddAtt;
        DXMenuItem itemCopyNode;
        DXMenuItem itemDelNode;
        DXMenuItem itemEditNode;

        BindingSource sourceFunc = new BindingSource();

        private void InitializeMenuItems()
        {
            itemAddNode = CreateMenuItem("新增文件", ItemAddNote_Click, TPSvgimages.Add);
            itemAddAtt = CreateMenuItem("新增附件", ItemAddFinalNote_Click, TPSvgimages.Attach);
            itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            itemEditNode = CreateMenuItem("更新", ItemDeleteNote_Click, TPSvgimages.Edit);
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemDeleteNote_Click(object sender, EventArgs e)
        {

        }

        private void ItemCopyNote_Click(object sender, EventArgs e)
        {
            MessageBox.Show("3");
        }

        private void ItemAddFinalNote_Click(object sender, EventArgs e)
        {
            MessageBox.Show("2");
        }

        private void ItemAddNote_Click(object sender, EventArgs e)
        {
            f201_AddNode fAdd = new f201_AddNode();
            fAdd._eventInfo = EventFormInfo.Create;
            fAdd._formName = "文件";
            fAdd.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

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

        private void treeFolder_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            // Sửa tên các Item về tiếng trung
            TranslateMenuItemCaption("Full Expand", "完全展開", e.Menu.Items);
            TranslateMenuItemCaption("Expand", "展開", e.Menu.Items);
            TranslateMenuItemCaption("Full Collapse", "完全折疊", e.Menu.Items);
            TranslateMenuItemCaption("Collapse", "折疊", e.Menu.Items);

            void TranslateMenuItemCaption(string originalCaption, string translatedCaption, DXMenuItemCollection menuItems)
            {
                var menuItem = menuItems.FirstOrDefault(item => item.Caption == originalCaption);
                if (menuItem != null) menuItem.Caption = translatedCaption;
            }

            TreeList treeList = sender as TreeList;
            if (e.HitInfo.InRowCell && e.HitInfo.Node.Id >= 0)
            {
                dt201_Base rowData = treeList.GetRow(e.HitInfo.Node.Id) as dt201_Base;

                if (rowData.IsFinalNode == true)
                {
                    dt201_Base parentData = treeList.GetRow(e.HitInfo.Node.ParentNode.Id) as dt201_Base;
                    int newestVersion = dt201_BaseBUS.Instance.GetListByParentId(parentData.Id).Max(r => r.Id);

                    if (newestVersion == rowData.Id)
                    {
                        itemAddAtt.BeginGroup = true;
                        e.Menu.Items.Add(itemAddAtt);
                        e.Menu.Items.Add(itemCopyNode);
                        e.Menu.Items.Add(itemEditNode);
                        e.Menu.Items.Add(itemDelNode);
                    }
                }
                else
                {
                    itemAddNode.BeginGroup = true;
                    e.Menu.Items.Add(itemAddNode);
                    e.Menu.Items.Add(itemEditNode);
                }
            }
        }
    }
}
