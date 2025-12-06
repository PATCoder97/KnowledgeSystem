using BusinessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._07_Quiz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._12_WireRodEmployeeEval
{
    public partial class uc312_ExamMain : DevExpress.XtraEditors.XtraUserControl
    {
        public uc312_ExamMain()
        {
            InitializeComponent();

            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI14;
        }

        Font fontUI14 = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        BindingSource sourceBases = new BindingSource();

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                var myExams = dt312_ExamUserBUS.Instance.GetListNeedDoByUserId(TPConfigs.LoginUser.Id);
                var bases = dt312_ExamMgmtBUS.Instance.GetList();
                var users = dm_UserBUS.Instance.GetList();
                var settting = dt312_SettingBUS.Instance.GetItemById(1);

                var dataDisplays = (from exam in myExams
                                    join data in bases on exam.ExamId equals data.Id
                                    select new
                                    {
                                        exam,
                                        data,
                                        TestDuration = $"{settting.TestDuration}分鐘",
                                        QuesCount = $"{settting.QuesCount}題目",
                                        PassingScore = $"{settting.PassingScore}/100"
                                    }).ToList();

                sourceBases.DataSource = dataDisplays;
            }
        }

        private void uc312_ExamMain_Load(object sender, EventArgs e)
        {
            LoadData();
            gcData.DataSource = sourceBases;
        }

        private void lvData_DoubleClick(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            LayoutView view = sender as LayoutView;
            LayoutViewHitInfo hi = view.CalcHitInfo(args.Location);
            if (hi.InCard)
            {
                int idExam = Convert.ToInt16(view.GetRowCellValue(hi.RowHandle, "exam.Id").ToString());

                f312_DoExam fDoExam = new f312_DoExam();
                fDoExam.idExamUser = idExam;
                fDoExam.ShowDialog();

                LoadData();
            }
        }
    }
}
