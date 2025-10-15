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
        DXMenuItem itemEditNode;
        DXMenuItem itemAddVer;
        DXMenuItem itemAddDocs;
        DXMenuItem itemDelNode;
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
            //tlsData.FormatRules.Add(new TreeListFormatRule
            //{
            //    Column = treeListColumn2,
            //    ApplyToRow = true,
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "[IsFinalNode] = True",
            //        Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information }
            //    }
            //});

            tlsData.FormatRules.Add(new TreeListFormatRule
            {
                Column = treeListColumn2,
                ApplyToRow = true,
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[CreateDate] Is Not Null",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question }
                }
            });



            //tlsData.FormatRules.Add(new TreeListFormatRule
            //{
            //    Column = treeListColumn2,
            //    ApplyToRow = true,
            //    Rule = new FormatConditionRuleExpression
            //    {
            //        Expression = "[data.IsDisable] = True",
            //        Appearance =
            //        {
            //            ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.DisabledText,
            //            Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Italic)
            //        }
            //    }
            //});

            //tlsData.FormatRules.AddExpressionRule(treeListColumn3, new DevExpress.Utils.AppearanceDefault()
            //{
            //    ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
            //    Font = new Font(tlsData.Appearance.Row.Font, FontStyle.Regular)
            //}, "[Desc] != ''");
        }

        private void GetFocusData()
        {
            TreeList treeList = tlsData;
            currentNode = treeList.FocusedNode;
            parentNode = currentNode.ParentNode;

            currentData = treeList.GetRow(currentNode.Id) as dt205_Base;
            parentData = parentNode != null ? treeList.GetRow(parentNode.Id) as dt205_Base : null;
        }

        private void InitializeMenuItems()
        {
            itemAddNode = CreateMenuItem("新增下級節點", ItemAddNote_Click, TPSvgimages.Add);
            itemEditNode = CreateMenuItem("編輯節點、文件", ItemEditNode_Click, TPSvgimages.Edit);
            itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
            itemAddDocs = CreateMenuItem("新增文件", ItemAddDocs_Click, TPSvgimages.Attach);
            itemDelNode = CreateMenuItem("刪除節點、文件", ItemDeleteNote_Click, TPSvgimages.Close);
        }

        private void ItemDeleteNote_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入您的工號以確認刪除",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

            GetFocusData();
            childrenDatas = dt205_BaseBUS.Instance.GetAllChildByParentId(currentData.Id);

            // Cập nhật thuộc tính IsDisable của tất cả các phần tử trong danh sách
            childrenDatas.ForEach(item =>
            {
                item.RemoveBy = TPConfigs.LoginUser.Id;
                item.RemoveAt = DateTime.Now;
            });

            dt205_BaseBUS.Instance.UpdateRange(childrenDatas);

            tlsData.ClearNodes();

            LoadData();
        }

        private void ItemAddNote_Click(object sender, EventArgs e)
        {
            GetFocusData();

            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = "節點",
                idParent = currentData.Id,
                parentData = currentData
            };

            node_Info.ShowDialog();
            LoadData();
        }

        private void ItemAddDocs_Click(object sender, EventArgs e)
        {
            GetFocusData();

            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = "文件",
                idParent = currentData.Id,
                parentData = currentData
            };

            node_Info.ShowDialog();
            LoadData();
        }

        // Sửa Node cho treelist
        private void ItemEditNode_Click(object sender, EventArgs e)
        {
            GetFocusData();

            if (currentData.IsFinalNode == true)
            {
                var result = XtraInputBox.Show(new XtraInputBoxArgs
                {
                    Caption = TPConfigs.SoftNameTW,
                    Prompt = "請輸入版本【格式：中文/越文/英文】",
                    DefaultButtonIndex = 0,
                    Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                    DefaultResponse = ""
                });

                if (string.IsNullOrEmpty(result?.ToString())) return;
                string[] version = result.ToString().Split('/');

                string verTW = version[0];
                string verVN = version.Count() > 1 ? version[1] : verTW;
                string verEN = version.Count() > 2 ? version[2] : verTW;

                TreeListNode parentNode = tlsData.FocusedNode.ParentNode;
                dt205_Base parentData = tlsData.GetRow(parentNode.Id) as dt205_Base;

                bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
                if (IsExist)
                {
                    MsgTP.MsgError("年版已存在！");
                    return;
                }

                currentData.DisplayName = verTW;
                currentData.DisplayNameVN = verVN;
                currentData.DisplayNameEN = verEN;
                dt205_BaseBUS.Instance.AddOrUpdate(currentData);
                LoadData();
                return;
            }

            f205_Node_Info node_Info = new f205_Node_Info()
            {
                formName = currentData.CreateDate == null ? "節點" : "文件",
                idBase = currentData.Id,
                eventInfo = EventFormInfo.Update,
                parentData = parentData
            };

            node_Info.ShowDialog();
            LoadData();
        }

        // Thêm phiên bản theo năm
        private void ItemAddVer_Click(object sender, EventArgs e)
        {
            GetFocusData();

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入版本【格式：中文/越文/英文】",
                DefaultButtonIndex = 0,
                Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;
            string[] version = result.ToString().Split('/');

            string verTW = version[0];
            string verVN = version.Count() > 1 ? version[1] : verTW;
            string verEN = version.Count() > 2 ? version[2] : verTW;

            bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
            if (IsExist)
            {
                MsgTP.MsgError("年版已存在！");
                return;
            }

            dt205_Base baseVer = new dt205_Base()
            {
                IdParent = currentData.Id,
                DisplayName = verTW,
                DisplayNameVN = verVN,
                DisplayNameEN = verEN,
                DocType = currentData.DocType,
                Confidential = currentData.Confidential,
                IsFinalNode = true,
                IdDept = currentData.IdDept,
            };

            bool resultAdd = dt205_BaseBUS.Instance.Add(baseVer);

            LoadData();
        }

        // Thêm Node


        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(tlsData))
            {
                baseDatas = dt205_BaseBUS.Instance.GetList();

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
                formName = "節點",
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
            // --- Dịch toàn bộ các mục mặc định ---
            var translations = new Dictionary<string, string>
            {
                { "Expand", "展開" },
                { "Collapse", "收合" },
                { "Full Expand", "全部展開" },
                { "Full Collapse", "全部收合" }
            };

            // Duyệt và dịch caption nếu có trong từ điển
            foreach (DXMenuItem item in e.Menu.Items)
            {
                if (translations.TryGetValue(item.Caption, out var translated))
                {
                    item.Caption = translated;
                }
            }

            // --- Kiểm tra node ---
            TreeList treeList = sender as TreeList;
            if (e.HitInfo?.InRowCell != true || e.HitInfo.Node?.Id < 0)
                return;

            GetFocusData();
            if (currentData == null)
                return;

            // --- Trạng thái node hiện tại ---
            bool isDocument = currentData.CreateDate != null;
            bool isFinalNode = currentData.IsFinalNode == true;

            // --- Kiểm tra node con ---
            dt205_Base firstChildData = null;
            if (currentNode.HasChildren)
                firstChildData = treeList.GetRow(currentNode.FirstNode.Id) as dt205_Base;

            bool haveFinalNode = firstChildData?.IsFinalNode == true;
            bool haveDocument = firstChildData?.CreateDate != null;

            // --- Reset BeginGroup của tất cả item sẵn có ---
            foreach (var item in new[] { itemEditNode, itemDelNode, itemAddNode, itemAddDocs, itemAddVer })
                item.BeginGroup = false;

            // --- Thêm item menu tùy theo loại node ---
            itemEditNode.BeginGroup = true;
            e.Menu.Items.Add(itemEditNode);
            e.Menu.Items.Add(itemDelNode);

            if (isFinalNode)
                return;

            if (isDocument)
            {
                itemAddVer.BeginGroup = true;
                e.Menu.Items.Add(itemAddVer);
                return;
            }

            if (haveDocument)
            {
                itemAddDocs.BeginGroup = true;
                e.Menu.Items.Add(itemAddDocs);
                return;
            }

            itemAddNode.BeginGroup = true;
            e.Menu.Items.Add(itemAddNode);
            e.Menu.Items.Add(itemAddDocs);
        }

        private void tlsData_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "ConfidentialType")
            {
                dt205_Base currentRow = e.Row as dt205_Base;
                if (currentRow == null) return;

                e.Value = currentRow.Confidential ? "機密" : "一般";
            }
        }

        private void tlsData_DoubleClick(object sender, EventArgs e)
        {
            TreeList treeList = sender as TreeList;
            TreeListHitInfo hitInfo = treeList.CalcHitInfo(treeList.PointToClient(MousePosition));
            if (hitInfo.HitInfoType != HitInfoType.Cell)
                return;

            GetFocusData();

            if (currentNode != null && currentNode.Nodes != null)
            {
                if (currentData.IsFinalNode != true)
                {
                    treeList.FocusedNode.Expanded = !treeList.FocusedNode.Expanded;
                    return;
                }

                f205_DocsInfo fInfo = new f205_DocsInfo()
                {
                    Text = currentData.DisplayName,
                    currentData = currentData,
                    parentData = parentData
                };
                fInfo.Show(this);
            }
        }
    }
}
