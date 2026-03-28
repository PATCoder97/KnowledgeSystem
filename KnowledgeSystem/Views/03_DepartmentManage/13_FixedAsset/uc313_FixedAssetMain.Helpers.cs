using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._13_FixedAsset
{
    public partial class uc313_FixedAssetMain
    {
        private void ImportAssetsFromExcel()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Excel Files|*.xls;*.xlsx";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != DialogResult.OK) return;

                var parsedRows = new List<dt313_FixedAsset>();
                var errors = new List<string>();
                var duplicateCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                try
                {
                    var table = ReadFirstWorksheet(dialog.FileName);
                    int rowIndex = 1;
                    foreach (DataRow row in table.Rows)
                    {
                        rowIndex++;
                        if (IsRowEmpty(row)) continue;

                        string assetCode = ReadCell(table, row, "AssetCode", "資產編號", "資產编号", "編碼", "Code");
                        string assetNameTw = ReadCell(table, row, "AssetNameTW", "資產中文名稱", "中文名稱");
                        string assetNameVn = ReadCell(table, row, "AssetNameVN", "資產越文名稱", "越文名稱");
                        string deptRaw = ReadCell(table, row, "IdDept", "Dept", "使用部門", "部門");
                        string managerRaw = ReadCell(table, row, "IdManager", "Manager", "管理員", "經辦");
                        string categoryRaw = ReadCell(table, row, "AssetCategory", "Category", "資產分類", "分類");
                        string typeName = ReadCell(table, row, "TypeName", "資產類別", "類別");
                        string location = ReadCell(table, row, "Location", "位置");
                        string brandSpec = ReadCell(table, row, "BrandSpec", "廠牌規格", "規格");
                        string origin = ReadCell(table, row, "Origin", "製造產地", "產地");
                        string acquireRaw = ReadCell(table, row, "AcquireDate", "取得日期");
                        string status = ReadCell(table, row, "Status", "狀態");
                        string remarks = ReadCell(table, row, "Remarks", "備註");

                        if (string.IsNullOrWhiteSpace(assetCode))
                        {
                            errors.Add($"Row {rowIndex}: AssetCode is required.");
                            continue;
                        }

                        if (!duplicateCheck.Add(assetCode.Trim()))
                        {
                            errors.Add($"Row {rowIndex}: duplicated AssetCode {assetCode}.");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(assetNameTw))
                        {
                            errors.Add($"Row {rowIndex}: AssetNameTW is required.");
                            continue;
                        }

                        string deptId = ResolveDeptId(deptRaw);
                        if (string.IsNullOrWhiteSpace(deptId))
                        {
                            errors.Add($"Row {rowIndex}: invalid department {deptRaw}.");
                            continue;
                        }

                        string managerId = ResolveUserId(managerRaw);
                        if (!string.IsNullOrWhiteSpace(managerRaw) && string.IsNullOrWhiteSpace(managerId))
                        {
                            errors.Add($"Row {rowIndex}: invalid manager {managerRaw}.");
                            continue;
                        }

                        DateTime acquireDateValue;
                        DateTime? acquireDate = DateTime.TryParse(acquireRaw, out acquireDateValue)
                            ? acquireDateValue
                            : (DateTime?)null;

                        var existing = fixedAssets.FirstOrDefault(r => string.Equals(r.AssetCode, assetCode.Trim(), StringComparison.OrdinalIgnoreCase));
                        var item = existing != null ? CloneAsset(existing) : new dt313_FixedAsset
                        {
                            CreatedBy = TPConfigs.LoginUser.Id,
                            CreatedDate = DateTime.Now,
                            IsDeleted = false
                        };

                        item.AssetCode = assetCode.Trim();
                        item.AssetNameTW = assetNameTw.Trim();
                        item.AssetNameVN = assetNameVn?.Trim();
                        item.IdDept = deptId;
                        item.IdManager = managerId;
                        item.AssetCategory = NormalizeCategory(categoryRaw);
                        item.TypeName = typeName?.Trim();
                        item.Location = location?.Trim();
                        item.BrandSpec = brandSpec?.Trim();
                        item.Origin = origin?.Trim();
                        item.AcquireDate = acquireDate;
                        item.Status = string.IsNullOrWhiteSpace(status) ? "Active" : status.Trim();
                        item.Remarks = remarks?.Trim();
                        item.UpdatedBy = TPConfigs.LoginUser.Id;
                        item.UpdatedDate = DateTime.Now;

                        parsedRows.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"Excel 讀取失敗\r\n{ex.Message}", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (errors.Count > 0)
                {
                    string errorText = string.Join(Environment.NewLine, errors.Take(20));
                    if (errors.Count > 20)
                    {
                        errorText += $"{Environment.NewLine}...";
                    }

                    XtraMessageBox.Show(errorText, TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (parsedRows.Count == 0)
                {
                    XtraMessageBox.Show("Excel 中沒有可匯入的資料。", TPConfigs.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (XtraMessageBox.Show($"即將匯入/更新 {parsedRows.Count} 筆資產資料，是否繼續？", TPConfigs.SoftNameTW,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                foreach (var item in parsedRows)
                {
                    int id = dt313_FixedAssetBUS.Instance.AddOrUpdateByCode(item);
                    if (id <= 0)
                    {
                        XtraMessageBox.Show($"匯入失敗: {item.AssetCode}", TPConfigs.SoftNameTW,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                ReloadAllData();
                XtraMessageBox.Show("匯入完成。", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private DataTable ReadFirstWorksheet(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLowerInvariant();
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = extension == ".xls"
                ? ExcelReaderFactory.CreateBinaryReader(stream)
                : ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                });

                if (dataSet.Tables.Count == 0)
                {
                    throw new Exception("Excel 中沒有工作表。");
                }

                return dataSet.Tables[0];
            }
        }

        private bool IsRowEmpty(DataRow row)
        {
            return row.ItemArray.All(r => r == null || string.IsNullOrWhiteSpace(r.ToString()));
        }

        private string ReadCell(DataTable table, DataRow row, params string[] aliases)
        {
            string columnName = FindColumnName(table, aliases);
            return string.IsNullOrWhiteSpace(columnName) ? string.Empty : row[columnName]?.ToString().Trim() ?? string.Empty;
        }

        private string FindColumnName(DataTable table, params string[] aliases)
        {
            var normalizedColumns = table.Columns.Cast<DataColumn>()
                .ToDictionary(c => NormalizeHeader(c.ColumnName), c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

            foreach (string alias in aliases)
            {
                string key = NormalizeHeader(alias);
                if (normalizedColumns.ContainsKey(key))
                {
                    return normalizedColumns[key];
                }
            }

            return string.Empty;
        }

        private string NormalizeHeader(string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? string.Empty
                : input.Replace(" ", string.Empty)
                    .Replace("_", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("　", string.Empty)
                    .Trim()
                    .ToLowerInvariant();
        }

        private string ResolveDeptId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string value = raw.Trim();
            if (departmentMap.ContainsKey(value)) return value;

            string token = value.Split(new[] { ' ', '-', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token) && departmentMap.ContainsKey(token))
            {
                return token;
            }

            var match = departments.FirstOrDefault(r =>
                string.Equals(r.DisplayName, value, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.DisplayNameVN, value, StringComparison.OrdinalIgnoreCase));

            return match?.Id ?? string.Empty;
        }

        private string ResolveUserId(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string value = raw.Trim();
            if (userMap.ContainsKey(value)) return value;

            string token = value.Split(new[] { ' ', '-', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(token) && userMap.ContainsKey(token))
            {
                return token;
            }

            var match = users.FirstOrDefault(r =>
                string.Equals(r.DisplayName, value, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r.DisplayNameVN, value, StringComparison.OrdinalIgnoreCase));

            return match?.Id ?? string.Empty;
        }

        private void ExportGrid(DevExpress.XtraGrid.GridControl grid, string filePrefix)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Excel File|*.xlsx";
                dialog.FileName = $"{filePrefix}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                grid.ExportToXlsx(dialog.FileName);
                Process.Start(dialog.FileName);
            }
        }

        private List<LookupItem> GetDepartmentLookupItems(bool includeAll = false)
        {
            var items = departments
                .Where(r => !string.IsNullOrWhiteSpace(r.Id))
                .Select(r => new LookupItem(r.Id, $"{r.Id} {r.DisplayName}"))
                .OrderBy(r => r.Display)
                .ToList();

            if (includeAll)
            {
                return items;
            }

            return items.Where(r => IsDeptAccessible(r.Value) || isManager313).ToList();
        }

        private List<LookupItem> GetUserLookupItems(bool includeAll = false)
        {
            IEnumerable<dm_User> query = users;
            if (!includeAll && !isManager313)
            {
                query = query.Where(r =>
                    string.Equals(r.Id, TPConfigs.LoginUser.Id, StringComparison.OrdinalIgnoreCase) ||
                    IsDeptAccessible(NormalizeDeptId(r.IdDepartment)));
            }

            var items = query
                .Select(r => new LookupItem(r.Id, $"{r.Id} {r.DisplayName}"))
                .OrderBy(r => r.Display)
                .ToList();

            items.Insert(0, new LookupItem(string.Empty, ""));
            return items;
        }

        private string NormalizeCategory(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "General";
            string value = raw.Trim();
            if (value.IndexOf("免", StringComparison.OrdinalIgnoreCase) >= 0 ||
                value.IndexOf("duty", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "DutyFreeImported";
            }

            return "General";
        }

        private dt313_FixedAsset CloneAsset(dt313_FixedAsset asset)
        {
            if (asset == null) return null;
            return new dt313_FixedAsset
            {
                Id = asset.Id,
                AssetCode = asset.AssetCode,
                AssetNameTW = asset.AssetNameTW,
                AssetNameVN = asset.AssetNameVN,
                IdDept = asset.IdDept,
                IdManager = asset.IdManager,
                AssetCategory = asset.AssetCategory,
                TypeName = asset.TypeName,
                Location = asset.Location,
                BrandSpec = asset.BrandSpec,
                Origin = asset.Origin,
                AcquireDate = asset.AcquireDate,
                Status = asset.Status,
                Remarks = asset.Remarks,
                LastMonthlyCheckDate = asset.LastMonthlyCheckDate,
                LastQuarterlyAuditDate = asset.LastQuarterlyAuditDate,
                IsDeleted = asset.IsDeleted,
                DeletedBy = asset.DeletedBy,
                DeletedDate = asset.DeletedDate,
                CreatedBy = asset.CreatedBy,
                CreatedDate = asset.CreatedDate,
                UpdatedBy = asset.UpdatedBy,
                UpdatedDate = asset.UpdatedDate
            };
        }

        private dt313_DepartmentSetting CloneDepartmentSetting(dt313_DepartmentSetting setting)
        {
            if (setting == null) return null;
            return new dt313_DepartmentSetting
            {
                Id = setting.Id,
                IdDept = setting.IdDept,
                QuarterlySampleRate = setting.QuarterlySampleRate,
                IsActive = setting.IsActive,
                UpdatedBy = setting.UpdatedBy,
                UpdatedDate = setting.UpdatedDate
            };
        }

        private dt313_AbnormalCatalog CloneAbnormalCatalog(dt313_AbnormalCatalog catalog)
        {
            if (catalog == null) return null;
            return new dt313_AbnormalCatalog
            {
                Id = catalog.Id,
                Code = catalog.Code,
                DisplayName = catalog.DisplayName,
                SortOrder = catalog.SortOrder,
                IsActive = catalog.IsActive,
                Remarks = catalog.Remarks,
                CreatedBy = catalog.CreatedBy,
                CreatedDate = catalog.CreatedDate
            };
        }
    }
}
