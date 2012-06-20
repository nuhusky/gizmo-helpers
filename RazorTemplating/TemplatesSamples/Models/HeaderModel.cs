using System;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.Samples.Models
{
    public class HeaderModel:ITemplateModel
    {
        #region Implementation of IEmailTemplateModel

        public string TemplateName
        {
            get { return "_header"; }
        }

        public string TemplateType
        {
            get { return String.Empty; }
        }

        #endregion
    }
}
