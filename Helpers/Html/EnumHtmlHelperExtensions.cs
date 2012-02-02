using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Gizmo.Helpers.Html
{
    public static class EnumHtmlHelperExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression, String.Empty /*optionLabel*/, null /*htmlAttributes*/);
        }
      
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel)
        {
            return EnumDropDownListFor(htmlHelper, expression, optionLabel, null /*htmlAttributes*/);
        }
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, expression, String.Empty /*optionLabel*/, htmlAttributes);
        }

        //Note:http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel,object htmlAttributes)
        {

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            TypeConverter converter = TypeDescriptor.GetConverter(enumType);

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                           {
                               Text = converter.ConvertToString(value),
                               Value = value.ToString(),
                               Selected = value.Equals(metadata.Model)
                           };

            if (metadata.IsNullableValueType)
            {
                items = SingleEmptyItem.Concat(items);
            }

            if (String.IsNullOrEmpty(optionLabel))
            {
                return htmlHelper.DropDownListFor(
                    expression,
                    items,
                    htmlAttributes
                    );
            }
            else
            {
                return htmlHelper.DropDownListFor(
                    expression,
                    items,
                    optionLabel,
                    htmlAttributes
                    );
            }
           
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

    }
}