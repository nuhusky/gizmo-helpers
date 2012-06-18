using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Scans calling application for registered areas and stores names/namespace mapping
    /// </summary>
    public static class AreaNameResolver
    {
        static readonly Dictionary<string, string> AreaNameMap = new Dictionary<string, string>();
        /// <summary>
        /// Scans web application for registered areas. Called from Global.asax Application_Start method.
        /// </summary>
        public static void RegisterAreaNames()
        {
            if (AreaNameMap.Count > 0)
            {
                return; //already initialzed
            }

            Assembly assemblyToSearch = Assembly.GetCallingAssembly();
            var areaRegistrationClasses = from t in assemblyToSearch.GetTypes()
                                          where t.IsSubclassOf(typeof(System.Web.Mvc.AreaRegistration))
                                          select new { Namespace = t.Namespace, AreaRegistrationClassInstance = (AreaRegistration)Activator.CreateInstance(t) };


            foreach (var areaReg in areaRegistrationClasses)
            {
                AreaRegistration ar = areaReg.AreaRegistrationClassInstance;
                string areaName = ar.AreaName;
                string @namespace = areaReg.Namespace;
                if (!AreaNameMap.ContainsKey(@namespace))
                {
                    AreaNameMap.Add(@namespace, areaName);
                }
               
            }
        }

        /// <summary>
        /// Resolves specified area name for passed in namespace
        /// </summary>
        /// <param name="namespace"></param>
        /// <returns>returns string.empty if no area name is found</returns>
        public static string GetAreaNameFor(string @namespace)
        {
            string areaName = String.Empty;
            if (AreaNameMap.ContainsKey(@namespace))
            {
                areaName = AreaNameMap[@namespace];
            }
            return areaName;
        }
    }
}