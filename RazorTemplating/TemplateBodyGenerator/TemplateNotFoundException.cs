using System;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    public sealed class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException(string templateName):base(String.Format("Could not find template: {0}.",templateName))
        {
        }
    }
}
