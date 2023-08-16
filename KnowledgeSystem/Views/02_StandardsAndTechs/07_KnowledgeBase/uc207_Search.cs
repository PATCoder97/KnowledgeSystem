using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using KnowledgeSystem.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._02_StandardsAndTechs._07_KnowledgeBase
{
    public partial class uc207_Search : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc207_Search()
        {
            InitializeComponent();
            helper = new RefreshHelper(gvData, "Id");
        }

        #region parameters

        BindingSource sourceKnowledge = new BindingSource();

        private class DataDisplay
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string UserRequest { get; set; }
            public string UserRequestName { get; set; }
            public string TypeName { get; set; }
            public string Keyword { get; set; }
            public string UserUploadName { get; set; }
            public DateTime? UploadDate { get; set; }
        }

        #endregion

        #region methods

        private void LoadData()
        {
            string userLogin = TempDatas.LoginId;

            List<User> lsUsers = new List<User>();
            List<dt207_Base> lsKnowledgeBase = new List<dt207_Base>();
            List<KnowledgeType> lsKnowledgeTypes = new List<KnowledgeType>();

            // Kiểm tra xem có lấy Keyword để hiện lên view không
            bool IsSimple = checkUseKeyword.CheckState == CheckState.Unchecked;

            // Lưu thông tin liên quan đến giao diện
            helper.SaveViewInfo();

            using (var db = new DBDocumentManagementSystemEntities())
            {
                // Lấy danh sách Users, lsKnowledgeTypes, lsTypeHisGetFile từ cơ sở dữ liệu
                var lsTypeHisGetFile = db.KnowledgeTypeHisGetFiles.ToList();
                lsKnowledgeTypes = db.KnowledgeTypes.ToList();
                lsUsers = db.Users.ToList();

                // Gán các 說明 quyền vào class temp
                TempDatas.typeViewFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 1);
                TempDatas.typeSaveFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 2);
                TempDatas.typePrintFile = lsTypeHisGetFile.FirstOrDefault(r => r.Id == 3);

                // Lấy danh sách các giá trị IdKnowledgeBase mà IsComplete là false
                var lsIdBaseRemove = db.dt207_DocProgress.Where(r => !r.IsComplete).Select(r => r.IdKnowledgeBase).ToList();

                // Kiểm tra xem phải sysAdmin không
                bool IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
                if (IsSysAdmin)
                {
                    lsKnowledgeBase = db.dt207_Base.Where(r => !lsIdBaseRemove.Contains(r.Id) && !r.IsDelete).ToList();
                    goto GET_DISPLAY_LIST;
                }

                // Danh sách Doc đưa lên hoặc yêu cầu
                var lsDocUserUpload = db.dt207_Base.Where(r => r.UserUpload == userLogin || r.UserRequest == userLogin).ToList();
                var lsIdDocUserUpload = lsDocUserUpload.Select(r => r.Id).ToList();

                // Join group với GroupUser để lấy Prioritize
                var lsGroupP = (from data in db.GroupUsers.ToList()
                                join p in db.Groups.ToList() on data.IdGroup equals p.Id
                                select new
                                {
                                    data.IdGroup,
                                    data.IdUser,
                                    p.Prioritize
                                }).ToList();

                // Danh sách mà user có quyền tìm kiếm
                var lsCanSearchs = (from ks in db.KnowledgeSecurities.ToList()
                                    join gu in lsGroupP on ks.IdGroup equals gu.IdGroup into gj
                                    from subGu in gj.DefaultIfEmpty()
                                    where ks.IdUser == userLogin || (subGu != null ? subGu.IdUser == userLogin : false)
                                    select new
                                    {
                                        ks.IdKnowledgeBase,
                                        ks.IdGroup,
                                        ks.IdUser,
                                        UserG = subGu?.IdUser,
                                        ks.SearchInfo,
                                        Prioritize = subGu?.Prioritize ?? -1
                                    } into dtData
                                    group dtData by dtData.IdKnowledgeBase into dtg
                                    select new
                                    {
                                        dtg.Key,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.Prioritize,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.SearchInfo,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.IdUser,
                                        dtg.OrderBy(r => r.Prioritize).FirstOrDefault()?.UserG,
                                    }).ToList();

                var lsIsCanSearchs = lsCanSearchs.Where(r => r.SearchInfo == true).Select(r => r.Key).ToList();

                // Gom tất cả các Id có quyền tìm kiếm, đưa lên và yêu cầu sau đó xoá đi các Id đang trong quá trình ký
                List<string> lsIdDisplay = new List<string>();

                lsIdDisplay.AddRange(lsIsCanSearchs);
                lsIdDisplay.AddRange(lsIdDocUserUpload);
                lsIdDisplay.RemoveAll(r => lsIdBaseRemove.Contains(r));
                lsIdDisplay = lsIdDisplay.Distinct().ToList();

                lsKnowledgeBase = db.dt207_Base.Where(r => lsIdDisplay.Contains(r.Id) && !r.IsDelete).ToList();
            }

        GET_DISPLAY_LIST:
            // Tạo danh sách các đối tượng DataDisplay bằng cách kết hợp lsKnowledgeBase, lsUsers, và lsKnowledgeTypes
            // Chọn các thuộc tính cụ thể để điền vào các đối tượng DataDisplay
            var lsDataDisplays = (from data in lsKnowledgeBase
                                  join userUpload_ in lsUsers on data.UserUpload equals userUpload_.Id
                                  join userRequest_ in lsUsers on data.UserRequest equals userRequest_.Id
                                  join type_ in lsKnowledgeTypes on data.IdTypes equals type_.Id
                                  select new DataDisplay
                                  {
                                      Id = data.Id,
                                      DisplayName = data.DisplayName,
                                      UserRequest = data.UserRequest,
                                      UserRequestName = userRequest_.DisplayName,
                                      TypeName = type_.DisplayName,
                                      Keyword = IsSimple ? null : data.Keyword,
                                      UserUploadName = userUpload_.DisplayName,
                                      UploadDate = data.UploadDate
                                  })
                                  .ToList();

            // Đặt nguồn dữ liệu của sourceKnowledge là danh sách các đối tượng DataDisplay
            sourceKnowledge.DataSource = lsDataDisplays;

            // Đặt tính hiển thị của cột gvColKeyword dựa trên giá trị của biến IsSimple
            gvColKeyword.Visible = !IsSimple;

            // Điều chỉnh độ rộng các cột của gvData
            gvData.BestFitColumns();

            // Tải thông tin liên quan đến giao diện
            helper.LoadViewInfo();
        }


        #endregion

        private void uc207_Search_Load(object sender, EventArgs e)
        {
            txbKeywords.Properties.NullValuePrompt = "請輸入您要查找的信息";

            gcData.DataSource = sourceKnowledge;

            gvData.ReadOnlyGridView();

            LoadData();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
            gvData.RefreshData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gvData.FindFilterText = txbKeywords.Text.Trim();
        }

        private void txbKeywords_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txbKeywords.Text = string.Empty;
            gvData.FindFilterText = txbKeywords.Text.Trim();
        }

        private void gcData_DoubleClick(object sender, EventArgs e)
        {
            int focusedRow = gvData.FocusedRowHandle;
            if (focusedRow < 0)
                return;

            DataDisplay dataRow = gvData.GetRow(focusedRow) as DataDisplay;
            string idDocuments = dataRow?.Id;

            f207_Document_Info fDocumentInfo = new f207_Document_Info(idDocuments);
            fDocumentInfo.ShowDialog();

            LoadData();
            gcData.RefreshDataSource();
        }
    }
}
