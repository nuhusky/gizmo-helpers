
    namespace Gizmo.RazorTemplating.TemplateContracts
    {
        public interface IHeaderModelInclude<T> : ITemplateModel
        {
            T Header { get; set; }
        }
    }
