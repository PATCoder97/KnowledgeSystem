using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using Scriban;
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
        List<dm_Group> groups;
        List<dm_Departments> depts;

        DXMenuItem itemAddNode;
        DXMenuItem itemAddAtt;
        DXMenuItem itemDelNode;
        DXMenuItem itemEditNode;
        DXMenuItem itemAddVer;
        DXMenuItem itemDisable;
        DXMenuItem itemEnable;
        DXMenuItem itemClone;
        DXMenuItem itemSendNote;

        TreeListNode currentNode;
        TreeListNode parentNode;

        dt201_Base currentData;
        dt201_Base parentData;
        List<dt201_Base> childrenDatas;

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
            itemAddNode = CreateMenuItem("新增下級", ItemAddNote_Click, TPSvgimages.Add);
            itemAddVer = CreateMenuItem("新增年版", ItemAddVer_Click, TPSvgimages.Add2);
            itemAddAtt = CreateMenuItem("新增表單", ItemAddAtt_Click, TPSvgimages.Attach);
            itemDelNode = CreateMenuItem("刪除節點", ItemDeleteNote_Click, TPSvgimages.Close);
            itemEditNode = CreateMenuItem("編輯節點", ItemEditNode_Click, TPSvgimages.Edit);
            itemDisable = CreateMenuItem("停用節點", ItemDisable_Click, TPSvgimages.Disable);
            itemEnable = CreateMenuItem("啟用節點", ItemEnable_Click, TPSvgimages.Confirm);
            itemClone = CreateMenuItem("複製節點", ItemClone_Click, TPSvgimages.Copy);
            itemSendNote = CreateMenuItem("通知Note", ItemSendNote_Click, TPSvgimages.EmailSend);
        }

        private void GetFocusData()
        {
            TreeList treeList = tlsData;
            currentNode = treeList.FocusedNode;
            parentNode = currentNode.ParentNode;

            currentData = (treeList.GetRow(currentNode.Id) as dynamic).data as dt201_Base;
            parentData = parentNode != null ? (treeList.GetRow(parentNode.Id) as dynamic).data as dt201_Base : null;
        }

        private void ItemSendNote_Click(object sender, EventArgs e)
        {
            if (TPConfigs.DomainComputer != DomainVNFPG.domainVNFPG)
            {
                string msg = "請使用公司電腦！";
                MsgTP.MsgShowInfomation($"<font='Microsoft JhengHei UI' size=14>{msg}</font>");
                return;
            }

            f00_SendNoteSystem sendNote = new f00_SendNoteSystem();

            using (var handle = SplashScreenManager.ShowOverlayForm(tlsData))
            {
                string dept = "";
                string[] deptSend = currentData.DocCode.Split('.');
                if (deptSend.Count() > 1)
                {
                    dept = deptSend[1].Length >= 4 ? deptSend[1].Substring(0, 4) : "";
                }

                var attsId = dt201_FormsBUS.Instance.GetListByBaseId(currentData.Id)
                    .Where(r => r.IsCancel != true && r.IsDel != true && r.IsProcessing != true)
                    .Select(r => r.AttId).ToList();

                if (attsId.Count <= 0)
                {
                    MsgTP.MsgError("沒有證書！");
                    return;
                }

                // Copy các file báo cáo vào ổ chung 153 để người ta lấy
                string folderCalibCert = Path.Combine(TPConfigs.FolderCalibCert, dept, currentData.DocCode);
                if (!Directory.Exists(folderCalibCert))
                {
                    Directory.CreateDirectory(folderCalibCert);
                }
                else
                {
                    string[] files = Directory.GetFiles(folderCalibCert);
                    foreach (string file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                }

                foreach (var id in attsId)
                {
                    var attDt = dm_AttachmentBUS.Instance.GetItemById(Convert.ToInt32(id));

                    var sourceFile = Path.Combine(TPConfigs.Folder201, attDt.EncryptionName);
                    var destFile = Path.Combine(folderCalibCert, attDt.ActualName);

                    if (File.Exists(sourceFile))
                    {
                        PdfHelper.Instance.ProtechPdf(sourceFile, destFile);
                        //File.Copy(sourceFile, destFile, true);
                    }
                }

                // Thông tin để gửi note
                string cc = TPConfigs.LoginUser.Id;
                string title = $"設備校正完成通知|{currentData.Id}";
                string threadId = "201";

                var templateData = new
                {
                    dept = dept,
                    devicevn = parentData.DisplayNameVN,
                    devicetw = parentData.DisplayName,
                    folder = folderCalibCert,
                    namevn = TPConfigs.LoginUser.DisplayNameVN,
                    nametw = TPConfigs.LoginUser.DisplayName,
                    empid = TPConfigs.LoginUser.Id
                };

                var templateContentSigner = System.IO.File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, $"f201_NotifyCalibCert.html"));
                var templateSigner = Template.Parse(templateContentSigner);

                var pageContent = templateSigner.Render(templateData);

                sendNote = new f00_SendNoteSystem()
                {
                    Cc = cc,
                    Title = title,
                    Subject = pageContent,
                    ThreadId = threadId,
                    Atts = string.Join(",", attsId),
                };
            }

            sendNote.ShowDialog();
        }

        private void ItemClone_Click(object sender, EventArgs e)
        {
            GetFocusData();

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>請輸入文件編號</font>",
                DefaultButtonIndex = 0,
                Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;
            string cloneCode = result.ToString();

            var children = dt201_BaseBUS.Instance.GetAllChildByParentId(currentData.Id);

            var cloneDatas = children.Select(child => new dt201_Base
            {
                IdParent = child.IdParent,
                DocCode = cloneCode,
                DisplayName = child.DisplayName,
                IdDept = child.IdDept,
                IsFinalNode = child.IsFinalNode,
                IsDel = child.IsDel,
                DelTime = child.DelTime,
                IsPaperType = child.IsPaperType,
                NotifyCycle = child.NotifyCycle,
                IsDisable = child.IsDisable,
                DisplayNameVN = child.DisplayNameVN,
                DocType = child.DocType,
                IdRecordCode = child.IdRecordCode,
            }).ToList();

            var cloneParent = cloneDatas.First(r => r.IsFinalNode != true);
            int idCloneParent = dt201_BaseBUS.Instance.Add(cloneParent);

            foreach (var item in cloneDatas)
            {
                if (item.IsFinalNode != true) continue;

                item.IdParent = idCloneParent;
                dt201_BaseBUS.Instance.Add(item);
            }

            LoadData();
        }

        private void ItemEnable_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>請輸入您的工號以確認啟用</font>",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                currentData.IsDisable = false;
                dt201_BaseBUS.Instance.AddOrUpdate(currentData);
            }

            LoadData();
        }

        private void ItemDisable_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>請輸入您的工號以確認停用</font>",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                currentData.IsDisable = true;
                dt201_BaseBUS.Instance.AddOrUpdate(currentData);
            }

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
                    AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                    Prompt = "<font='Microsoft JhengHei UI' size=14>中文與越南文之間用「/」分隔</font>",
                    DefaultButtonIndex = 0,
                    Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                    DefaultResponse = ""
                });

                if (string.IsNullOrEmpty(result?.ToString())) return;
                string[] version = result.ToString().Split('/');

                string verTW = version[0];
                string verVN = version.Count() > 1 ? version[1] : verTW;

                TreeListNode parentNode = tlsData.FocusedNode.ParentNode;
                dt201_Base parentData = (tlsData.GetRow(parentNode.Id) as dynamic).data as dt201_Base;

                bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
                if (IsExist)
                {
                    MsgTP.MsgError("年版已存在！");
                    return;
                }

                currentData.DisplayName = verTW;
                currentData.DisplayNameVN = verVN;
                dt201_BaseBUS.Instance.AddOrUpdate(currentData);
                LoadData();
                return;
            }

            f201_AddNode fAdd = new f201_AddNode()
            {
                eventInfo = EventFormInfo.Update,
                formName = "文件",
                currentData = currentData,
                parentData = parentData
            };

            fAdd.ShowDialog();

            LoadData();
        }

        // Thêm phiên bản theo năm
        private void ItemAddVer_Click(object sender, EventArgs e)
        {
            GetFocusData();

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>中文與越南文之間用「/」分隔</font>",
                DefaultButtonIndex = 0,
                Editor = new TextEdit() { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;
            string[] version = result.ToString().Split('/');

            string verTW = version[0];
            string verVN = version.Count() > 1 ? version[1] : verTW;

            bool IsExist = baseDatas.Any(r => r.IdParent == currentData.Id && r.DisplayName == verTW);
            if (IsExist)
            {
                MsgTP.MsgError("年版已存在！");
                return;
            }

            dt201_Base baseVer = new dt201_Base()
            {
                IdParent = currentData.Id,
                DocCode = currentData.DocCode,
                DisplayName = verTW,
                DisplayNameVN = verVN,
                IsFinalNode = true,
                IdDept = currentData.IdDept,
            };

            dt201_BaseBUS.Instance.Add(baseVer);

            LoadData();
        }

        // Xóa node
        private void ItemDeleteNote_Click(object sender, EventArgs e)
        {
            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                AllowHtmlText = DevExpress.Utils.DefaultBoolean.True,
                Prompt = "<font='Microsoft JhengHei UI' size=14>請輸入您的工號以確認刪除</font>",
                DefaultButtonIndex = 0,
                Editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result) || result != TPConfigs.LoginUser.Id.ToUpper()) return;

            GetFocusData();
            childrenDatas = dt201_BaseBUS.Instance.GetAllChildByParentId(currentData.Id);

            // Cập nhật thuộc tính IsDisable của tất cả các phần tử trong danh sách
            childrenDatas.ForEach(item =>
            {
                item.IsDel = true;
                item.DelTime = DateTime.Now;
            });

            dt201_BaseBUS.Instance.UpdateRange(childrenDatas);

            tlsData.ClearNodes();

            LoadData();
        }

        // Thêm biểu đơn
        private void ItemAddAtt_Click(object sender, EventArgs e)
        {
            GetFocusData();

            f201_AddAttachment fAtt = new f201_AddAttachment()
            {
                eventInfo = EventFormInfo.Create,
                formName = "表單",
                currentData = currentData,
                parentData = parentData
            };
            fAtt.ShowDialog();

            LoadData();
        }

        // Thêm Node
        private void ItemAddNote_Click(object sender, EventArgs e)
        {
            f201_AddNode fAdd = new f201_AddNode();
            fAdd.eventInfo = EventFormInfo.Create;
            fAdd.formName = "文件";
            fAdd.parentData = currentData;
            fAdd.ShowDialog();

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
                baseDatas = dt201_BaseBUS.Instance.GetList().ToList();
                SetIdRecordCodeForFinalNodes(baseDatas);

                var deptsChecked = cbbDepts.Items.Where(r => r.CheckState == CheckState.Checked).Select(r => r.Value).ToList();
                var records = dt201_RecordCodeBUS.Instance.GetList();

                var result = (from data in baseDatas
                              join dept in depts on data.IdDept equals dept.Id
                              join record in records on data.IdRecordCode equals record.Id into recordGroup
                              from record in recordGroup.DefaultIfEmpty()
                              where deptsChecked.Contains(data.IdDept)
                              select new { data, dept, record }).ToList();

                sourceData.DataSource = result;

                tlsData.RefreshDataSource();
                tlsData.Refresh();
                tlsData.BestFitColumns();

                tlsColDept.Visible = deptsChecked.Count > 1;
            }
        }

        private void uc201_AuditISOMain_Load(object sender, EventArgs e)
        {
            tlsData.DataSource = sourceData;
            tlsData.KeyFieldName = "data.Id";
            tlsData.ParentFieldName = "data.IdParent";
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
            // Dịch và xóa các mục menu
            var translations = new Dictionary<string, string> { { "Full Expand", "完全展開" }, { "Full Collapse", "完全折疊" } };
            List<DXMenuItem> itemDefault = new List<DXMenuItem>();
            foreach (var item in e.Menu.Items.ToList())
            {
                // Dịch các mục "Full Expand" và "Full Collapse"
                if (translations.ContainsKey(item.Caption))
                {
                    item.Caption = translations[item.Caption];
                    itemDefault.Add(item);
                }
                // Xóa các mục "Expand" và "Collapse"
                else if (item.Caption == "Expand" || item.Caption == "Collapse")
                {
                    e.Menu.Items.Remove(item);
                }
            }

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

            //// Get first child data
            //dt201_Base firstChildData = (currentNode.HasChildren ? (treeList.GetRow(currentNode.FirstNode.Id) as dynamic)?.data as dt201_Base : null);
            //bool haveFinalNode = firstChildData?.IsFinalNode == true;

            // Get all children data, including children of children (Có thể bỏ đoạn này, Thay bằng đoan firstChild để check có phải là FinalNode)
            List<dt201_Base> allChildrenDatas = new List<dt201_Base>();
            if (currentNode.HasChildren)
            {
                // Use a queue to process nodes in a breadth-first manner
                Queue<TreeListNode> nodeQueue = new Queue<TreeListNode>();
                nodeQueue.Enqueue(currentNode);  // Start with the current node

                while (nodeQueue.Count > 0)
                {
                    // Dequeue a node from the queue
                    TreeListNode currentNodeCheck = nodeQueue.Dequeue();

                    // Process all the immediate children of the current node
                    foreach (TreeListNode childNode in currentNodeCheck.Nodes)
                    {
                        var childNodeRow = treeList.GetRow(childNode.Id);
                        dt201_Base childNodeData = (childNodeRow as dynamic)?.data as dt201_Base;

                        if (childNodeData != null)
                        {
                            allChildrenDatas.Add(childNodeData);
                        }

                        // Enqueue the child node to process its children later
                        if (childNode.HasChildren)
                        {
                            nodeQueue.Enqueue(childNode);
                        }
                    }
                }
            }

            // Get first child data
            dt201_Base firstChildData = allChildrenDatas.FirstOrDefault();
            bool isRootFinalNode = firstChildData?.IsFinalNode == true;
            bool haveChildren = currentNode.HasChildren;

            // Check if the current or any parent node is disabled
            bool isDisable = parentDatas.Any(p => p.IsDisable == true) || currentData.IsDisable == true;

            if (isDisable)
            {
                if (parentData?.IsDisable != true) e.Menu.Items.Add(itemEnable);
                return;
            }

            // Handle final node scenario
            if (currentData.IsFinalNode == true)
            {
                if (parentData != null)
                {
                    // Dùng cái này nếu mà cho sửa toàn bộ năm, sửa luôn disable AuditDoc_Info
                    e.Menu.Items.Add(itemAddAtt);
                    e.Menu.Items.Add(itemEditNode);
                    e.Menu.Items.Add(itemDelNode);

                    if (groups.Select(r => r.IdDept).Contains("7820") && currentData.DisplayName == "校正證書")
                    {
                        itemSendNote.BeginGroup = true;
                        e.Menu.Items.Add(itemSendNote);
                    }
                    //var nodes = dt201_BaseBUS.Instance.GetListByParentId(parentData.Id);
                    //if (nodes.Max(r => r.Id) == currentData.Id)
                    //{
                    //    e.Menu.Items.Add(itemAddAtt);
                    //    e.Menu.Items.Add(itemEditNode);
                    //    e.Menu.Items.Add(itemDelNode);
                    //}
                }
            }
            else if (isRootFinalNode)
            {
                e.Menu.Items.Add(itemAddVer);
                e.Menu.Items.Add(itemEditNode);
                e.Menu.Items.Add(itemClone);
                e.Menu.Items.Add(itemDelNode);
                e.Menu.Items.Add(itemDisable);
            }
            else if (haveChildren)
            {
                e.Menu.Items.Add(itemAddNode);
                e.Menu.Items.Add(itemEditNode);
                e.Menu.Items.Add(itemDelNode);
                e.Menu.Items.Add(itemDisable);
            }
            else
            {
                e.Menu.Items.Add(itemAddVer);
                e.Menu.Items.Add(itemAddNode);
                e.Menu.Items.Add(itemEditNode);
                e.Menu.Items.Add(itemDelNode);
                e.Menu.Items.Add(itemDisable);
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

                f201_AuditDoc_Info fInfo = new f201_AuditDoc_Info()
                {
                    Text = currentData.DisplayName,
                    currentData = currentData,
                    parentData = parentData
                };
                fInfo.Show(this);
            }
        }

        // Thêm Node
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
            tlsData.ClearNodes();
            LoadData();

            tlsData.BestFitColumns();
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

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            tlsData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void tlsData_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "Desc")
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
