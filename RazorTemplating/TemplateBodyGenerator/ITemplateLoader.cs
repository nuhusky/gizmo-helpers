using System.Collections.Generic;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    internal interface ITemplateLoader
    {
        Template LoadFor(ITemplateModel templateModel);
        IEnumerable<Template> LoadAll();

    }
}
