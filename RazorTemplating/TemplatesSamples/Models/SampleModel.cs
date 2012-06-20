using System;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.Samples.Models
{
    public class SampleModel:ITemplateModel
    {
        public FooterModel Footer { get; set; }
        #region Implementation of IEmailTemplateModel

        
        public string TemplateName
        {
            get { return "sample"; }
        }

        public string TemplateType
        {
            get { return String.Empty; }
        }

        public string Message { get; set; }
        #endregion
    }
}