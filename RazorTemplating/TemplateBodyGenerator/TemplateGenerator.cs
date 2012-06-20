using System.Collections.Generic;
using Gizmo.RazorTemplating.TemplateContracts;
using RazorEngine;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    public sealed class TemplateGenerator : ITemplateGenerator
    {
        #region Implementation of IEmailTemplateGenerator

        private static readonly ITemplateGenerator SingletonInstance;
        
        private readonly ITemplateNameGenerator _nameFormatter;

        #region Singleton Accessor

        public static ITemplateGenerator Current
        {
            get { return SingletonInstance; }
        }

        #endregion

        static TemplateGenerator()
        {
            SingletonInstance = new TemplateGenerator();
            CacheTemplates();
        }

        private TemplateGenerator()
        {
            _nameFormatter = new TemplateFullNameFormatter();

        }


        private static void CacheTemplates()
        {
            TemplateLoader loader = new TemplateLoader(new TemplateFullNameFormatter());
            IEnumerable<Template> templates = loader.LoadAll();
            foreach (Template template in templates)
            {
                Razor.Compile(template.Text, template.Model.GetType(), template.Name);
            }

        }

        public string Render<T>(T model) where T : ITemplateModel
        {
            string name = GetFormattedTemplateKey(model);

            string output = Razor.Run(model, name);

            return output;
        }

        private string GetFormattedTemplateKey(ITemplateModel template)
        {
            return _nameFormatter.GetName(template);
        }

        #endregion
    }
}
  
