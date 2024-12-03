using BusinessLayer;
using DataAccessLayer;
using ExcelDataReader;
using Logger;
using NotesMail;
using Scriban;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTestService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string filePath = @"C:\Users\Dell Alpha\Desktop\Rác\校正實驗室 - 複製.xlsx";

            // Mở tệp Excel
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Tạo reader từ stream
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Chuyển đổi dữ liệu thành DataSet
                    var result = reader.AsDataSet();

                    // Lấy DataTable đầu tiên
                    var dataTable = result.Tables[0];

                    List<dt201_ReqUpdateDocs> lists = new List<dt201_ReqUpdateDocs>();
                    int index = 0;
                    // Duyệt qua các hàng trong DataTable và in ra giá trị
                    foreach (DataRow row in dataTable.Rows)
                    {
                        dt201_ReqUpdateDocs dt201_RecordCode = dt201_ReqUpdateDocsBUS.Instance.GetItemById(Convert.ToInt16(row[0]));
                        dt201_RecordCode.IdRecordCode = Convert.ToInt16(row[9]);

                        dt201_ReqUpdateDocsBUS.Instance.AddOrUpdate(dt201_RecordCode);
                        index++;
                    }

                    //dt201_RecordCodeBUS.Instance.AddRange(lists);
                }
            }
        }
    }
}
