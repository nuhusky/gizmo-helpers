using System;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    public class TemplateFullNameFormatter:ITemplateNameGenerator
    {
        #region Implementation of ITemplateNameGenerator

        public string GetName(ITemplateModel template)
        {
            if (String.IsNullOrEmpty(template.TemplateType))
            {
                return template.TemplateName;
            }

            return String.Format("{0}.{1}", template.TemplateName, template.TemplateType);
        }

        #endregion
    }
}
