

    namespace Gizmo.RazorTemplating.TemplateContracts
    {
        public interface IFooterModelInclude<T>:ITemplateModel
        {
            T Footer { get; set; }
        }
    }
