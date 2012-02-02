using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Gizmo.Helpers.Html
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Builds a SelectListItem list from an enumeration. If enumeration value is decorated with EnumValueDisplayAttribute then SelectListItem.Text is overridden
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> SelectOptionsFromEnum<T>(this HtmlHelper htmlHelper, T @enum) where T : struct, IConvertible
         {
            Type typeOfEnum = typeof (T);
             if (!typeOfEnum.IsEnum)
             {
                 throw new ArgumentException("T must be an enumerated type");
             }
 
            List<SelectListItem> items = new List<SelectListItem>();
            string desc;
            FieldInfo[] fields=typeOfEnum.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo info in fields)
            {
                
                EnumValueDisplayAttribute attr = info.GetCustomAttributes(typeof(EnumValueDisplayAttribute), false).FirstOrDefault() as EnumValueDisplayAttribute;
                desc = attr == null ? info.Name : attr.Name;

                items.Add(new SelectListItem()
                              {
                                  Text = desc,
                                  Value = Enum.Parse(typeOfEnum, info.Name).ToString()
                              });

            }
                  
             return items;
         }

    }
}