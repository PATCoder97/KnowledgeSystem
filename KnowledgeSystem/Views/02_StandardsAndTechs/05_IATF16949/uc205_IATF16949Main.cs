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
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._02_StandardsAndTechs._01_ISOAuditDocs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._05_IATF16949
{
    public partial class uc205_IATF16949Main : DevExpress.XtraEditors.XtraUserControl
    {
        public uc205_IATF16949Main()
        {
            InitializeComponent();
            InitializeMenuItems();
            InitializeIcon();
            CreateRuleGV();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        List<dt205_Base> baseDatas;
        BindingSource sourceData = new BindingSource();
        List<dm_Group> groups;
        List<dm_Departments> depts;

        DXMenuItem itemAddNode;
        DXMenuItem itemAddAtt;
        DXMenuItem itemDelNode;
        DXMenuItem itemViewNode;
        DXMenuItem itemAddVer;
        DXMenuItem itemDisable;
        DXMenuItem itemEnable;
        DXMenuItem itemClone;
        DXMenuItem itemSendNote;

        TreeListNode currentNode;
        TreeListNode parentNode;

        dt205_Base currentData;
        dt205_Base parentData;
        List<dt205_Base> childrenDatas;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = System.Drawing.Color.Blue;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
        }

        private void CreateRuleGV()
        {
            tlsData.FormatRules.Add(new TreeListFormatRule
            {
                Column = treeListColumn2,
                ApplyToRow = true,
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.IsFinalNode] = True",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Hyperlink }
                }
            });

            tlsData.FormatRules.Add(new TreeListFormatRule
            {
                Column = treeListColumn2,
                ApplyToRow = true,
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.IsDisable] = True",
                    Appearance =
                    {
                        ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.DisabledText,
                        Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Italic)
                    }
                }
            });

            tlsData.FormatRules.AddExpressionRule(treeListColumn3, new DevExpress.Utils.AppearanceDefault()
            {
                ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Regular)
            }, "[Desc] != ''");
        }

        private void InitializeMenuItems()
        {
            itemAddNode = CreateMenuItem("新增下級節點", ItemAddNote_Click, TPSvgimages.Add);
            itemViewNode = CreateMenuItem("編輯節點", ItemEditNode_Click, TPSvgimages.Edit);
            //itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
            //itemAddAtt = CreateMenuItem("新增表單", ItemAddAtt_Click, TPSvgimages.Attach);
            //itemDelNode = CreateMenuItem("刪除節點", ItemDeleteNote_Click, TPSvgimages.Close);
            //itemDisable = CreateMenuItem("停用節點", ItemDisable_Click, TPSvgimages.Disable);
            //itemEnable = CreateMenuItem("啟用節點", ItemEnable_Click, TPSvgimages.Confirm);
            //itemClone = CreateMenuItem("複製節點", ItemClone_Click, TPSvgimages.Copy);
            //itemSendNote = CreateMenuItem("通知Note", ItemSendNote_Click, TPSvgimages.EmailSend);
        }

        private void GetFocusData()
        {
            TreeList treeList = tlsData;
            currentNode = treeList.FocusedNode;
            parentNode = currentNode.ParentNode;

            currentData = treeList.GetRow(currentNode.Id) as dt205_Base;
            parentData = parentNode != null ? treeList.GetRow(parentNode.Id) as dt205_Base : null;
        }

        // Sửa Node cho treelist
        private void ItemEditNode_Click(object sender, EventArgs e)
        {
            GetFocusData();

            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = "Name",
                idBase = currentData.Id,
                eventInfo = EventFormInfo.View
            };

            node_Info.ShowDialog();
            LoadData();

            //if (currentData.IsFinalNode == true)
            //{
            //    var result = XtraInputBox.Show(new XtraInputBoxArgs
            //    {
            //        Caption = TPConfigs.SoftNameTW,
            //        AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
            //        Prompt = "<font='Microsoft JhengHei UI' size=14>中文與越南文之間用「/」分隔</font>",
            //        DefaultButtonIndex = 0,
            //        Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
            //        DefaultResponse = ""
            //    });

            //    if (string.IsNullOrEmpty(result?.ToString())) return;
            //    string[] version = result.ToString().Split('/');

            //    string verTW = version[0];
            //    string verVN = version.Count() > 1 ? version[1] : verTW;

            //    TreeListNode parentNode = tlsData.FocusedNode.ParentNode;
            //    dt201_Base parentData = (tlsData.GetRow(parentNode.Id) as dynamic).data as dt201_Base;

            //    bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
            //    if (IsExist)
            //    {
            //        MsgTP.MsgError("年版已存在！");
            //        return;
            //    }

            //    currentData.DisplayName = verTW;
            //    currentData.DisplayNameVN = verVN;
            //    dt201_BaseBUS.Instance.AddOrUpdate(currentData);
            //    LoadData();
            //    return;
            //}

            //f201_AddNode fAdd = new f201_AddNode()
            //{
            //    eventInfo = EventFormInfo.Update,
            //    formName = "文件",
            //    currentData = currentData,
            //    parentData = parentData
            //};

            //fAdd.ShowDialog();

            //LoadData();
        }

        // Thêm phiên bản theo năm
        private void ItemAddVer_Click(object sender, EventArgs e)
        {
            //GetFocusData();

            //var result = XtraInputBox.Show(new XtraInputBoxArgs
            //{
            //    Caption = TPConfigs.SoftNameTW,
            //    AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
            //    Prompt = "<font='Microsoft JhengHei UI' size=14>中文與越南文之間用「/」分隔</font>",
            //    DefaultButtonIndex = 0,
            //    Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
            //    DefaultResponse = ""
            //});

            //if (string.IsNullOrEmpty(result?.ToString())) return;
            //string[] version = result.ToString().Split('/');

            //string verTW = version[0];
            //string verVN = version.Count() > 1 ? version[1] : verTW;

            //bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
            //if (IsExist)
            //{
            //    MsgTP.MsgError("年版已存在！");
            //    return;
            //}

            //dt201_Base baseVer = new dt201_Base()
            //{
            //    IdParent = currentData.Id,
            //    DocCode = currentData.DocCode,
            //    DisplayName = verTW,
            //    DisplayNameVN = verVN,
            //    IsFinalNode = true,
            //    IdDept = currentData.IdDept,
            //};

            //dt201_BaseBUS.Instance.Add(baseVer);

            //LoadData();
        }

        // Thêm Node
        private void ItemAddNote_Click(object sender, EventArgs e)
        {
            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = "Name",
                idParent = currentData.Id,
            };

            node_Info.ShowDialog();
            LoadData();
        }

        public static void SetIdRecordCodeForFinalNodes(List<dt201_Base> documents)
        {
            // Create a dictionary to map Id to IdRecordCode for quick lookup
            Dictionary<int, int> idToRecordCodeMap = documents.ToDictionary(d => d.Id, d => d.IdRecordCode);

            foreach (var doc in documents)
            {
                if (doc.IsFinalNode == true && idToRecordCodeMap.ContainsKey(doc.IdParent))
                {
                    doc.IdRecordCode = idToRecordCodeMap[doc.IdParent];
                }
            }
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(tlsData))
            {
                baseDatas = dt205_BaseBUS.Instance.GetList();
                //SetIdRecordCodeForFinalNodes(baseDatas);

                //var deptsChecked = cbbDepts.Items.Where(r => r.CheckState == CheckState.Checked).Select(r => r.Value).ToList();
                //var records = dt201_RecordCodeBUS.Instance.GetList();

                //var result = (from data in baseDatas
                //              join dept in depts on data.IdDept equals dept.Id
                //              join record in records on data.IdRecordCode equals record.Id into recordGroup
                //              from record in recordGroup.DefaultIfEmpty()
                //              where deptsChecked.Contains(data.IdDept)
                //              select new { data, dept, record }).ToList();

                sourceData.DataSource = baseDatas;

                tlsData.RefreshDataSource();
                tlsData.Refresh();
                tlsData.BestFitColumns();

                //tlsColDept.Visible = deptsChecked.Count > 1;
            }
        }


        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = "Name",
            };

            node_Info.ShowDialog(this);
            LoadData();
        }

        private void uc205_IATF16949Main_Load(object sender, EventArgs e)
        {
            tlsData.DataSource = sourceData;
            tlsData.KeyFieldName = "Id";
            tlsData.ParentFieldName = "IdParent";
            tlsData.BestFitColumns();

            tlsData.ReadOnlyTreelist();
            tlsData.KeyDown += GridControlHelper.TreeViewCopyCellData_KeyDown;

            var grpUsrs = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            depts = dm_DeptBUS.Instance.GetList();
            groups = dm_GroupBUS.Instance.GetListByName("ISO組");

            groups = (from data in groups
                      join grp in grpUsrs on data.Id equals grp.IdGroup
                      select data).ToList();

            var deptsCbb = (from data in depts
                            join grp in groups on data.Id equals grp.IdDept
                            select new CheckedListBoxItem() { Description = data.Id, Value = data.Id, CheckState = CheckState.Checked }).ToArray();

            cbbDepts.Items.AddRange(deptsCbb);

            LoadData();
        }

        private void tlsData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            if (e.HitInfo?.InRowCell != true || e.HitInfo.Node.Id < 0) return;

            GetFocusData();

            // Get current node and data
            if (currentData == null) return;

            // Gather parent node and parent data
            List<dt201_Base> parentDatas = new List<dt201_Base>();

            while (parentNode != null)
            {
                var parentNodeRow = treeList.GetRow(parentNode.Id);
                dt201_Base parentNodeData = (parentNodeRow as dynamic)?.data as dt201_Base;
                if (parentNodeData != null) parentDatas.Add(parentNodeData);
                parentNode = parentNode.ParentNode;
            }

            dt201_Base parentData = parentDatas.FirstOrDefault();

            e.Menu.Items.Add(itemAddNode);
            e.Menu.Items.Add(itemViewNode);
        }

        private void tlsData_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "ConfidentialType")
            {
                dt205_Base currentRow = e.Row as dt205_Base;
                if (currentRow == null) return;

                e.Value = currentRow.Confidential? "機密" : "一般";
            }
        }
    }
}
