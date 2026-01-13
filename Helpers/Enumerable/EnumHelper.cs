using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CemSys3.Helpers.Enumerable
{
    public static class EnumHelper
    {
        //para las listas desplegables
        public static List<SelectListItem> GetSelectListItems<T>() where T : Enum
        {
            var items = new List<SelectListItem>();
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (var field in fields)
            {
                // Intenta obtener el atributo DisplayName
                var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
                var name = displayAttribute != null ? displayAttribute.Name : field.Name;
                var value = ((int)field.GetValue(null)).ToString();

                items.Add(new SelectListItem
                {
                    Value = value, // El ID numérico del enum (1 o 2)
                    Text = name    // El texto del atributo DisplayName
                });
            }

            return items;
        }

        //para obtener el nombre del enum por su valor
        public static string GetDisplayNameByValue<T>(int value) where T : Enum
        {
            var type = typeof(T);
            // Convierte el valor numérico al tipo de enum subyacente
            var name = Enum.GetName(type, value);

            if (name == null) return value.ToString(); // Devuelve el valor si no se encuentra el nombre

            var field = type.GetField(name);
            if (field == null) return name;

            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null ? displayAttribute.Name : name;
        }
    }
}
