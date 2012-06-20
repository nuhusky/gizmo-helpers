namespace Gizmo.RazorTemplating.TemplateBodyGenerator.Resolvers
{
    /// <summary>
    /// Public contract for resolving templates
    /// </summary>
    public interface ITemplateResolver
    {
        string GetTemplate(string name);
    }
}