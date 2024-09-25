using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using iTextSharp.text;
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
using Font = System.Drawing.Font;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs
{
    public partial class uc201_AuditISOMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc201_AuditISOMain()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
            CreateRuleGV();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        List<dt201_Base> baseDatas;
        BindingSource sourceData = new BindingSource();
        dt201_Base nodeFocus;
        List<dm_Group> groups;
        List<dm_Departments> depts;

        DXMenuItem itemAddNode;
        DXMenuItem itemAddAtt;
        DXMenuItem itemCopyNode;
        DXMenuItem itemDelNode;
        DXMenuItem itemEditNode;
        DXMenuItem itemAddVer;
        DXMenuItem itemDisable;
        DXMenuItem itemEnable;

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

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            tlsData.FormatRules.AddExpressionRule(treeListColumn2, new DevExpress.Utils.AppearanceDefault()
            {
                ForeColor = Color.Blue
            }, "[data.IsFinalNode] = True");

            tlsData.FormatRules.Add(new TreeListFormatRule
            {
                Column = treeListColumn2,
                ApplyToRow = true,
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.IsDisable] = True",
                    Appearance =
                    {
                        ForeColor = Color.DarkGray,
                        Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Italic)
                    }
                }
            });

            tlsData.FormatRules.AddExpressionRule(treeListColumn3, new DevExpress.Utils.AppearanceDefault()
            {
                ForeColor = Color.Red,
                Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Regular)
            }, "[IsPaperType] != ''");
        }

        private void InitializeMenuItems()
        {
            itemAddNode = CreateMenuItem("新增下級", ItemAddNote_Click, TPSvgimages.Add);
            itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
            itemAddAtt = CreateMenuItem("新增檔案", ItemAddAtt_Click, TPSvgimages.Attach);
            itemCopyNode = CreateMenuItem("複製年版", ItemCopyNote_Click, TPSvgimages.Copy);
            itemDelNode = CreateMenuItem("刪除", ItemDeleteNote_Click, TPSvgimages.Close);
            itemEditNode = CreateMenuItem("更新", ItemEditNode_Click, TPSvgimages.Edit);
            itemDisable = CreateMenuItem("停用", ItemDisable_Click, TPSvgimages.Suspension);
            itemEnable = CreateMenuItem("啟用", ItemEnable_Click, TPSvgimages.Suspension);
        }

        private void ItemEnable_Click(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                TreeListNode focusedNode = tlsData.FocusedNode;
                dt201_Base rowData = (tlsData.GetRow(focusedNode.Id) as dynamic).data as dt201_Base;

                // Lấy tất cả các node con
                List<dt201_Base> allChildren = dt201_BaseBUS.Instance.GetAllChildByParentId(rowData.Id);

                // Cập nhật thuộc tính IsDisable của tất cả các phần tử trong danh sách
                allChildren.ForEach(item => item.IsDisable = false);

                dt201_BaseBUS.Instance.UpdateRange(allChildren);
            }
            LoadData();
        }

        private void ItemDisable_Click(object sender, EventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                TreeListNode focusedNode = tlsData.FocusedNode;
                dt201_Base rowData = (tlsData.GetRow(focusedNode.Id) as dynamic).data as dt201_Base;

                // Lấy tất cả các node con
                List<dt201_Base> allChildren = dt201_BaseBUS.Instance.GetAllChildByParentId(rowData.Id);

                // Cập nhật thuộc tính IsDisable của tất cả các phần tử trong danh sách
                allChildren.ForEach(item => item.IsDisable = true);

                dt201_BaseBUS.Instance.UpdateRange(allChildren);
            }
            LoadData();
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
                XtraMessageBox.Show("年版已存在！");
                return;
            }

            dt201_Base baseVer = new dt201_Base()
            {
                IdParent = nodeFocus.Id,
                DocCode = "",
                DisplayName = version,
                IsFinalNode = true,
                IdDept = nodeFocus.IdDept,
            };

            dt201_BaseBUS.Instance.Add(baseVer);

            LoadData();
        }

        private void ItemDeleteNote_Click(object sender, EventArgs e)
        {

        }

        private void ItemCopyNote_Click(object sender, EventArgs e)
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
                MsgTP.MsgError("年版已存在！");
                return;
            }

            TreeListNode focusedNode = tlsData.FocusedNode.ParentNode;
            dt201_Base rowData = (tlsData.GetRow(focusedNode.Id) as dynamic).data as dt201_Base;

            dt201_Base baseVer = new dt201_Base()
            {
                IdParent = rowData.Id,
                DocCode = "",
                DisplayName = version,
                IsFinalNode = true,
                IdDept = rowData.IdDept,
            };

            int idBaseNew = dt201_BaseBUS.Instance.Add(baseVer);

            var formsByBase = dt201_FormsBUS.Instance.GetListByIdBase(nodeFocus.Id).Select(r => new dt201_Forms()
            {
                IdBase = idBaseNew,
                Code = r.Code,
                DisplayName = r.DisplayName,
                UploadTime = DateTime.Now,
                UploadUser = TPConfigs.LoginUser.Id,

            }).ToList();

            dt201_FormsBUS.Instance.AddRange(formsByBase);

            LoadData();
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
            var deptsChecked = cbbDepts.Items.Where(r => r.CheckState == CheckState.Checked).Select(r => r.Value).ToList();

            var result = (from data in baseDatas
                          join dept in depts on data.IdDept equals dept.Id
                          where deptsChecked.Contains(data.IdDept)
                          select new { data, dept }).ToList();

            sourceData.DataSource = result;

            tlsData.BestFitColumns();
            tlsData.RefreshDataSource();

            tlsColDept.Visible = deptsChecked.Count > 1;
        }

        private void uc201_AuditISOMain_Load(object sender, EventArgs e)
        {
            tlsData.DataSource = sourceData;
            tlsData.KeyFieldName = "data.Id";
            tlsData.ParentFieldName = "data.IdParent";
            tlsData.BestFitColumns();

            tlsData.ReadOnlyTreelist();
            tlsData.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;

            depts = dm_DeptBUS.Instance.GetList();
            groups = dm_GroupBUS.Instance.GetListByName("ISO組");

            var deptsCbb = (from data in depts
                            join grp in groups on data.Id equals grp.IdDept
                            select new CheckedListBoxItem() { Description = data.DisplayName, Value = data.Id, CheckState = CheckState.Checked }).ToArray();

            cbbDepts.Items.AddRange(deptsCbb);

            LoadData();

            var aa = this.Text;
        }

        private void tlsData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            if (e.HitInfo != null && e.HitInfo.InRowCell && e.HitInfo.Node.Id >= 0)
            {
                var nodeSelect = treeList.GetRow(e.HitInfo.Node.Id);
                if (nodeSelect == null) return;

                dt201_Base rowData = ((dynamic)nodeSelect).data as dt201_Base;
                if (rowData == null) return;

                nodeFocus = rowData;
                //idNodeParent = e.HitInfo.Node.ParentNode != null ? e.HitInfo.Node.ParentNode.Id : -1;
                bool HaveFinalNode = dt201_BaseBUS.Instance.GetListByParentId(rowData.Id).Any(r => r.IsFinalNode == true);

                // Tạo danh sách chứa các node cha
                List<dt201_Base> parentNodes = new List<dt201_Base>();
                TreeListNode currentNode = tlsData.FocusedNode;
                while (currentNode != null)
                {
                    dt201_Base node = (tlsData.GetRow(currentNode.Id) as dynamic).data as dt201_Base;

                    parentNodes.Add(node);
                    currentNode = currentNode.ParentNode;
                }
                var IsDisable = parentNodes.Any(p => p.IsDisable == true);

                if (IsDisable)
                {
                    dt201_Base nodeParent = null;
                    var nodeParentId = tlsData.GetRow(tlsData.FocusedNode.ParentNode?.Id ?? -1);
                    if (nodeParentId != null)
                    {
                        nodeParent = (tlsData.GetRow(tlsData.FocusedNode.ParentNode?.Id ?? -1) as dynamic).data as dt201_Base;
                    }

                    if (nodeParent == null || nodeParent.IsDisable != true)
                    {
                        e.Menu.Items.Add(itemEnable);
                    }
                    return;
                }

                if (rowData.IsFinalNode == true)
                {
                    dt201_Base parentData = ((dynamic)treeList.GetRow(e.HitInfo.Node.ParentNode.Id)).data as dt201_Base;
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

                e.Menu.Items.Add(itemDisable);
            }
        }

        private void tlsData_DoubleClick(object sender, EventArgs e)
        {
            TreeList treeList = sender as TreeList;
            TreeListNode focusedNode = treeList.FocusedNode;
            TreeListHitInfo hitInfo = treeList.CalcHitInfo(treeList.PointToClient(MousePosition));

            if (focusedNode != null && focusedNode.Nodes != null && hitInfo.HitInfoType == HitInfoType.Cell)
            {
                dt201_Base rowData = (treeList.GetRow(focusedNode.Id) as dynamic).data as dt201_Base;

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

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f201_AddNode fAdd = new f201_AddNode();
            fAdd.eventInfo = EventFormInfo.Create;
            fAdd.formName = "文件";
            fAdd.ShowDialog();

            LoadData();
        }

        private void cbbDepts_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        private void tlsData_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "IsPaperType")
            {
                dt201_Base currentRow = ((dynamic)e.Row).data as dt201_Base;
                if (currentRow == null) return;

                string des1 = currentRow.IsPaperType == true ? "紙本" : "";
                string des2 = currentRow.IsDisable == true ? "停用" : "";

                // Tạo một mảng các chuỗi và chỉ lấy các chuỗi không rỗng
                e.Value = string.Join("、", new[] { des1, des2 }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }
    }
}
