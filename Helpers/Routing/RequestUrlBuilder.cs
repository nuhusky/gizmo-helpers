using System;
using System.Web.Mvc;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Base class responsible for getting action, controller, and area names;
    /// </summary>
    /// <typeparam name="TR">MvcUrlRequest</typeparam>
    public abstract class RequestUrlBuilder<TR> where TR:MvcUrlRequest
    {
      
        protected string GetActionName<T>(T @fromEnum)
        {
            return @fromEnum.ToString();
        }

        protected string GetControllerName<T>(T @fromEnum)
        {
            EnforceEnumDeclarationConstraint(@fromEnum);
            Type parentType = GetControllerType(@fromEnum);

            // ReSharper disable PossibleNullReferenceException (null checking is done in EnforceEnumDeclarationConstraint)
            string parentTypeName = parentType.Name;
            // ReSharper restore PossibleNullReferenceException

            string controller = parentTypeName.Replace("Controller", "");
            return controller;
        }

        protected string GetAreaName<T>(T @fromEnum)
        {
            Type parentType = GetControllerType(@fromEnum);
            string areaName = String.Empty;
            string controllerNamespace = parentType.Namespace;
            if (!String.IsNullOrEmpty(controllerNamespace))
            {
                string controllerParentNamespace = controllerNamespace.Replace(".Controllers", "");
                areaName = ResolveAreaNameForNamespace(controllerParentNamespace);
            }

            return areaName;
        }

        

        protected virtual string ResolveAreaNameForNamespace(string @namespace)
        {
            return  AreaNameResolver.GetAreaNameFor(@namespace);
        }

        public abstract TR GetRequest<T>(T actionEnum) where T:struct;

        //workers
        private void EnforceEnumDeclarationConstraint<T>(T @enum)
        {
            Type parentType = GetControllerType(@enum);
            if (parentType == null || !parentType.IsSubclassOf(typeof(Controller)))
            {
                throw new Exception("Enumeration must be nested within a System.Web.Mvc.Controller type");
            }
        }
       
        private static Type GetControllerType<T>(T @enum)
        {
            Type parentType = typeof(T).DeclaringType;

            return parentType;
        }

       
    }
}