using System;
using System.Collections.Generic;
using System.Linq;

namespace KnowledgeSystem.Helpers
{
    public static class FixedProgressHelper
    {
        public class StepData
        {
            public string IdUsr { get; set; }
            public int IdRole { get; set; }
            public bool HasRoleValue { get; set; }
        }

        public static string Serialize(IEnumerable<StepData> steps)
        {
            if (steps == null)
            {
                return string.Empty;
            }

            return string.Join(";", steps
                .Where(r => !string.IsNullOrWhiteSpace(r.IdUsr))
                .Select(r => r.IdRole > 0
                    ? $"{r.IdUsr}|{r.IdRole}"
                    : r.IdUsr));
        }

        public static List<StepData> Deserialize(string progress)
        {
            var result = new List<StepData>();
            if (string.IsNullOrWhiteSpace(progress))
            {
                return result;
            }

            foreach (var item in progress.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = item.Split(new[] { '|' }, StringSplitOptions.None);
                var step = new StepData
                {
                    IdUsr = parts.FirstOrDefault()?.Trim(),
                    HasRoleValue = parts.Length > 1
                };

                if (step.HasRoleValue && int.TryParse(parts[1], out int idRole) && idRole > 0)
                {
                    step.IdRole = idRole;
                }

                if (!string.IsNullOrWhiteSpace(step.IdUsr))
                {
                    result.Add(step);
                }
            }

            return result;
        }

        public static int NormalizeRoleId(int idRole, IEnumerable<int> validRoleIds)
        {
            if (idRole <= 0 || validRoleIds == null)
            {
                return 0;
            }

            return validRoleIds.Contains(idRole) ? idRole : 0;
        }
    }
}
