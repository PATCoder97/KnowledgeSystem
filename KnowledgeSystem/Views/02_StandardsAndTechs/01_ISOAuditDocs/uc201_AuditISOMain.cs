using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
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
    public partial class uc201_AuditISOMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_AuditISOMain()
        {
            InitializeComponent();
            InitializeMenuItems();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        List<dt201_Base> baseDatas;
        BindingSource sourceData = new BindingSource();
        dt201_Base nodeFocus;

        DXMenuItem itemAddNode;
        DXMenuItem itemAddAtt;
        DXMenuItem itemCopyNode;
        DXMenuItem itemDelNode;
        DXMenuItem itemEditNode;
        DXMenuItem itemAddVer;

        private void InitializeMenuItems()
        {
            itemAddNode = CreateMenuItem("新增表單", ItemAddNote_Click, TPSvgimages.Add);
            itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
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

        private void ItemEditNode_Click(object sender, EventArgs e)
        {
            f201_AddNode fAdd = new f201_AddNode();
            fAdd.eventInfo = EventFormInfo.Update;
            fAdd.formName = "文件";
            fAdd.baseData = nodeFocus;
            fAdd.ShowDialog();

            LoadData();
        }

        private void ItemAddVer_Click(object sender, EventArgs e)
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "輸入年版名稱",
                DefaultButtonIndex = 0,
                Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0) },
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string version = result?.ToString() ?? "";

            bool IsExist = baseDatas.Any(r => r.IdParent == nodeFocus.Id && r.DisplayName == version);
            if (IsExist)
            {
                XtraMessageBox.Show("Return");
                return;
            }

            dt201_Base baseVer = new dt201_Base()
            {
                IdParent = nodeFocus.Id,
                DocCode = "",
                DisplayName = version,
                IsFinalNode = true
            };

            dt201_BaseBUS.Instance.Add(baseVer);

            LoadData();
        }

        private void ItemDeleteNote_Click(object sender, EventArgs e)
        {

        }

        private void ItemCopyNote_Click(object sender, EventArgs e)
        {

        }

        private void ItemAddAtt_Click(object sender, EventArgs e)
        {
            f201_AddAttachment fAtt = new f201_AddAttachment();
            fAtt.eventInfo = EventFormInfo.Create;
            fAtt.formName = "表單";
            fAtt.idBase = nodeFocus.Id;
            fAtt.ShowDialog();

            LoadData();
        }

        private void ItemAddNote_Click(object sender, EventArgs e)
        {
            f201_AddNode fAdd = new f201_AddNode();
            fAdd.eventInfo = EventFormInfo.Create;
            fAdd.formName = "文件";
            fAdd.baseParent = nodeFocus;
            fAdd.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            baseDatas = dt201_BaseBUS.Instance.GetList().ToList();
            sourceData.DataSource = baseDatas;

            tlsData.BestFitColumns();

            tlsData.RefreshDataSource();
        }

        private void uc201_AuditISOMain_Load(object sender, EventArgs e)
        {
            LoadData();

            tlsData.DataSource = sourceData;
            tlsData.KeyFieldName = "Id";
            tlsData.ParentFieldName = "IdParent";
            tlsData.BestFitColumns();

            tlsData.ReadOnlyTreelist();
            tlsData.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;
        }

        private void tlsData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            if (e.HitInfo.InRowCell && e.HitInfo.Node.Id >= 0)
            {
                dt201_Base rowData = treeList.GetRow(e.HitInfo.Node.Id) as dt201_Base;
                nodeFocus = rowData;
                //idNodeParent = e.HitInfo.Node.ParentNode != null ? e.HitInfo.Node.ParentNode.Id : -1;
                bool HaveFinalNode = dt201_BaseBUS.Instance.GetListByParentId(rowData.Id).Any(r => r.IsFinalNode == true);

                if (rowData.IsFinalNode == true)
                {
                    dt201_Base parentData = treeList.GetRow(e.HitInfo.Node.ParentNode.Id) as dt201_Base;
                    var nodes = dt201_BaseBUS.Instance.GetListByParentId(parentData.Id);
                    int newestVersion = nodes.Max(r => r.Id);

                    if (newestVersion == rowData.Id)
                    {
                        itemAddAtt.BeginGroup = true;
                        e.Menu.Items.Add(itemAddAtt);
                        e.Menu.Items.Add(itemCopyNode);
                        e.Menu.Items.Add(itemEditNode);
                        e.Menu.Items.Add(itemDelNode);
                    }
                }
                else if (HaveFinalNode)
                {
                    itemAddVer.BeginGroup = true;
                    e.Menu.Items.Add(itemAddVer);
                    e.Menu.Items.Add(itemEditNode);
                    e.Menu.Items.Add(itemDelNode);
                }
                else
                {
                    itemAddVer.BeginGroup = true;
                    e.Menu.Items.Add(itemAddVer);
                    e.Menu.Items.Add(itemAddNode);
                    e.Menu.Items.Add(itemEditNode);
                    e.Menu.Items.Add(itemDelNode);
                }
            }
        }

        private void tlsData_DoubleClick(object sender, EventArgs e)
        {
            TreeList treeList = sender as TreeList;
            TreeListNode focusedNode = treeList.FocusedNode;
            TreeListHitInfo hitInfo = treeList.CalcHitInfo(treeList.PointToClient(MousePosition));

            if (focusedNode != null && focusedNode.Nodes != null && hitInfo.HitInfoType == HitInfoType.Cell)
            {
                dt201_Base rowData = treeList.GetRow(focusedNode.Id) as dt201_Base;

                if (rowData.IsFinalNode != true)
                {
                    treeList.FocusedNode.Expanded = !treeList.FocusedNode.Expanded;

                    //gcData.DataSource = null;
                    return;
                }

                var atts = dt201_FormsBUS.Instance.GetListByIdBase(rowData.Id);
                //gcData.DataSource = atts;
                //gvData.BestFitColumns();
            }
        }
    }
}
