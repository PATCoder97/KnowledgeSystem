using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class WordHelper
    {
        private static WordHelper instance;

        public static WordHelper Instance
        {
            get { if (instance == null) instance = new WordHelper(); return instance; }
            private set { instance = value; }
        }

        private WordHelper()
        {
        }

        public DataTable ReadTableFromMSWord(string fileName)
        {
            using (var doc = WordprocessingDocument.Open(fileName, false))
            {

                DataTable dt = new DataTable();
                int rowCount = 0;

                Table table = doc.MainDocumentPart.Document.Body.Elements<Table>().First();
                IEnumerable<TableRow> rows = table.Elements<TableRow>();
                foreach (TableRow row in rows)
                {
                    if (rowCount == 0)
                    {
                        foreach (TableCell cell in row.Descendants<TableCell>())
                        {
                            dt.Columns.Add(cell.InnerText);
                        }
                        rowCount += 1;
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        foreach (TableCell cell in row.Descendants<TableCell>())
                        {
                            if (dt.Columns.Count <= i)
                            {
                                dt.Columns.Add($"Col{i}");
                            }

                            dt.Rows[dt.Rows.Count - 1][i] = cell.InnerText;
                            i++;
                        }
                    }
                }

                return dt;
            }
        }
    }
}
