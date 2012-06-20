

using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    public interface ITemplateNameGenerator
    {
        string GetName(ITemplateModel template);
    }
}
