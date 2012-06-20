
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    public interface ITemplateGenerator
    {
        string Render<T>(T model) where T : ITemplateModel;
    }
}
