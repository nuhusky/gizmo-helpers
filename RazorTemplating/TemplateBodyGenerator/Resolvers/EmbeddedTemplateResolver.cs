using System;
using System.IO;
using System.Reflection;

namespace Gizmo.RazorTemplating.TemplateBodyGenerator.Resolvers
{
    class EmbeddedTemplateResolver : ITemplateResolver
    {
        private readonly Assembly _loadedAssembly;
        #region Implementation of ITemplateResolver

        public EmbeddedTemplateResolver(Assembly assmbly)
        {
            _loadedAssembly = assmbly;
        }

        


        public string GetTemplate(string name)
        {
            
                string rootNamespace = ParseRootNamespace(_loadedAssembly.FullName);
                string templateFullName = String.Format("{0}.Files.{1}.cshtml", rootNamespace, name);
                using (Stream stream = _loadedAssembly.GetManifestResourceStream(templateFullName))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                } 
        }

        private static string ParseRootNamespace(string fullNamespace)
        {
            //HACK: how do get root namespace a better way?
            int cammaIndex = fullNamespace.IndexOf(",");
            return fullNamespace.Substring(0, cammaIndex);
        }
        #endregion
    }
}