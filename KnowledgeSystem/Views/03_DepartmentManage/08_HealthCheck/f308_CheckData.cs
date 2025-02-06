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
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_CheckData : DevExpress.XtraEditors.XtraForm
    {
        public f308_CheckData()
        {
            InitializeComponent();
        }

        public EventFormInfo eventInfo = EventFormInfo.Create;
        public string formName = "";
        public dt308_CheckDetail checkDetail;
        string idDept2word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
        List<dt308_Disease> diseases;
        string sourceScript = File.ReadAllText(Path.Combine(TPConfigs.HtmlPath, "f308_GoogleAppScript.txt"));

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //MessageBox.Show(radioGroup1.EditValue.ToString());
        }

        private void f308_CheckData_Load(object sender, EventArgs e)
        {
            var lsUsers = dm_UserBUS.Instance.GetListByDept(idDept2word).Select(r => new dm_User
            {
                Id = r.Id,
                IdDepartment = r.IdDepartment,
                DisplayName = $"{r.DisplayName} {r.DisplayNameVN}",
                DisplayNameVN = $"LG{r.IdDepartment} {r.DisplayName} {r.DisplayNameVN}",
                JobCode = r.JobCode
            }).ToList();
            cbbUsr.Properties.DataSource = lsUsers;
            cbbUsr.Properties.DisplayMember = "DisplayNameVN";
            cbbUsr.Properties.ValueMember = "Id";
            cbbUsr.Properties.BestFitWidth = 110;

            diseases = dt308_DiseaseBUS.Instance.GetList();
            var diseaseTypes = new[] { 1, 2, 3 };

            foreach (var diseaseType in diseaseTypes)
            {
                var tokens = diseases.Where(r => r.DiseaseType == diseaseType).ToList();
                var tokenEdit = diseaseType == 1 ? txbDisease1.Properties.Tokens :
                                diseaseType == 2 ? txbDisease2.Properties.Tokens :
                                txbDisease3.Properties.Tokens;

                foreach (var item in tokens)
                    tokenEdit.AddToken(new TokenEditToken($"{item.DisplayNameVN} / {item.DisplayNameTW}", item.Id));
            }




        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var diseaseTitles = new List<Tuple<string, string>>
            {
                Tuple.Create("Bạn mắc các bệnh thông thường nào sau đây nào sau đây?", "您患有以下哪些一般疾病？"),
                Tuple.Create("Bạn mắc các bệnh mãn tính nào sau đây nào sau đây?", "您患有以下哪些慢性疾病？"),
                Tuple.Create("Bạn mắc các bệnh nghề nghiệp nào sau đây nào sau đây?", "您患有以下哪些得職業病？")
            };

            var scriptGoogleForm = sourceScript.Replace("{{formname}}", "Khảo sát sức khỏe 2025");

            for (int i = 0; i < diseaseTitles.Count; i++)
            {
                string checkboxName = $"checkboxDiseases{i + 1}";
                string diseasesCode = string.Join(",", diseases
                    .Where(r => r.DiseaseType == i + 1)
                    .Select(r => $"{checkboxName}.createChoice('({r.Id:D2}) {r.DisplayNameVN}/{r.DisplayNameTW}')")
                    .ToList());

                scriptGoogleForm = scriptGoogleForm.Replace($"{{{{diseases{i + 1}}}}}", $"{checkboxName}.setTitle('{diseaseTitles[i].Item1}\\n{diseaseTitles[i].Item2}').setChoices([{diseasesCode}]);");
            }

            Clipboard.SetText(scriptGoogleForm);
            MessageBox.Show("ok");


        }
    }
}