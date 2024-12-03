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

            string filePath = @"C:\Users\Dell Alpha\Desktop\aaa.xlsx";

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

                    List<dt201_RecordCode> lists = new List<dt201_RecordCode>();
                    int index = 0;
                    // Duyệt qua các hàng trong DataTable và in ra giá trị
                    foreach (DataRow row in dataTable.Rows)
                    {
                        dt201_RecordCode dt201_RecordCode = new dt201_RecordCode()
                        {
                            Id = index,
                            DisplayName = row[1].ToString(),
                            Code = row[0].ToString(),
                            Articles = row[2].ToString()
                        };

                        lists.Add(dt201_RecordCode);
                        index++;
                    }

                    dt201_RecordCodeBUS.Instance.AddRange(lists);
                }
            }
        }
    }
}
