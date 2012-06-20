using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gizmo.RazorTemplating.TemplateBodyGenerator.Resolvers;
using Gizmo.RazorTemplating.TemplateContracts;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator
{
    class TemplateLoader : ITemplateLoader
    {
        private ITemplateResolver _resolver;
        private readonly ITemplateNameGenerator _nameFormatter;
        public TemplateLoader(ITemplateNameGenerator nameGenerator)
        {
            _nameFormatter = nameGenerator;
        }

        #region Implementation of ITemplateLoader

        public Template LoadFor(ITemplateModel templateModel)
        {
            string text = _resolver.GetTemplate(templateModel.TemplateName);
            if (String.IsNullOrEmpty(text))
            {
                throw new TemplateNotFoundException(templateModel.TemplateName);
            }

            return new Template()
                       {
                           Name = templateModel.TemplateName,
                           Text=text,
                           Model = templateModel
                       };
        }

       
        public IEnumerable<Template> LoadAll()
        {
            //find all models of type IEmailTemplateModel
            List<Template> templates = new List<Template>();

            AppDomain myDomain = AppDomain.CurrentDomain;
            Assembly[] assembliesLoaded = myDomain.GetAssemblies();

            foreach (var assembly in assembliesLoaded)
            {
                Type modelType = typeof(ITemplateModel);
                var foundTypes = Enumerable.Where<Type>(assembly.GetTypes(), t => t.IsClass && modelType.IsAssignableFrom(t));


                foreach (Type foundType in foundTypes)
                {
                    ITemplateModel templateModelInstance = (ITemplateModel)Activator.CreateInstance(foundType);

                    AddValidTemplate(assembly,templates, templateModelInstance);

                }

            }
            return templates;
        }

        private void AddValidTemplate(Assembly assembly,ICollection<Template> templates,  ITemplateModel modelInstance)
        {
            _resolver = new EmbeddedTemplateResolver(assembly);
            Template template = new Template();
            template.Name = _nameFormatter.GetName(modelInstance);
            template.Text = _resolver.GetTemplate(template.Name);
            template.Model = modelInstance;

            if (!String.IsNullOrEmpty(template.Text))
            {
                templates.Add(template);
            }
        }

        

        #endregion
    }
}
