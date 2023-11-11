using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KnowledgeSystem.Helpers
{
    public class EnumHelper
    {
        public static T GetEnumByDescription<T>(string description) where T : Enum
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new ArgumentException("T must be an enumerated type.");

            foreach (T enumValue in Enum.GetValues(enumType))
            {
                string enumDescription = GetDescription(enumValue);
                if (enumDescription == description)
                    return enumValue;
            }

            throw new ArgumentException("No enum value found with the specified description.");
        }

        public static string GetDescription<T>(T enumValue) where T : Enum
        {
            Type enumType = typeof(T);
            string name = enumValue.ToString();
            FieldInfo field = enumType.GetField(name);
            if (field != null)
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    return attribute.Description;
                }
            }
            return name;
        }
    }
}
