using System;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.Samples.Models
{
    public class FooterModel : ITemplateModel
    {
        #region Implementation of IEmailTemplateModel

        public string Text { get; set; }

        public string TemplateName
        {
            get { return "_footer"; }
        }

        public string TemplateType
        {
          get { return String.Empty; } 
        }

        #endregion
    }
}