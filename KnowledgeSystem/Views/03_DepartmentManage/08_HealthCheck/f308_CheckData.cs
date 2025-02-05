using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace KnowledgeSystem.Views._03_DepartmentManage._08_HealthCheck
{
    public partial class f308_CheckData : DevExpress.XtraEditors.XtraForm
    {
        public f308_CheckData()
        {
            InitializeComponent();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //MessageBox.Show(radioGroup1.EditValue.ToString());
        }

        private void f308_CheckData_Load(object sender, EventArgs e)
        {
            txbDisease.Properties.Tokens.AddRange(new TokenEditToken[] {
                new TokenEditToken("Bệnh về tuần hoàn / 循環系統相關疾病", "1"),
                new TokenEditToken("Bệnh về hô hấp / 呼吸系統相關疾病", "2"),
                new TokenEditToken("Bệnh về tiêu hóa / 消化系統相關疾病", "3"),
                new TokenEditToken("Bệnh về thận – tiết niệu / 腎臟-泌尿相關疾病", "4"),
                new TokenEditToken("Bệnh về nội tiết / 內分泌相關疾病", "5"),
                new TokenEditToken("Bệnh về cơ xương khớp / 骨關節相關疾病", "6"),
                new TokenEditToken("Bệnh về thần kinh / 神經相關疾病", "7"),
                new TokenEditToken("Bệnh ngoại khoa / 外科相關疾病", "8"),
                new TokenEditToken("Bệnh về mắt / 眼睛相關疾病", "9"),
                new TokenEditToken("Bệnh về tai mũi họng / 耳鼻喉相關疾病", "10"),
                new TokenEditToken("Bệnh về răng hàm mặt / 面部牙齒相關疾病", "11"),
                new TokenEditToken("Bệnh về da liễu / 皮膚相關疾病", "12"),
            });


        }
    }
}