using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Gizmo.Helpers.Security;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Copied from: http://kevinmontrose.com/2011/07/25/why-i-love-attribute-based-routing/
    /// Allows MVC routing urls to be declared on the action they map to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteAttribute : ActionMethodSelectorAttribute, IComparable<RouteAttribute>
    {

        /// <summary>
        /// Within the calling assembly, looks for any action methods that have the RouteAttribute defined, 
        /// adding the routes to the parameter 'routes' collection.
        /// </summary>
        public static void MapDecoratedRoutes(RouteCollection routes)
        {
            MapDecoratedRoutes(routes, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Looks for any action methods in 'assemblyToSearch' that have the RouteAttribute defined, 
        /// adding the routes to the parameter 'routes' collection.
        /// </summary>
        /// <param name="assemblyToSearch">An assembly containing Controllers with public methods decorated with the RouteAttribute</param>
        public static void MapDecoratedRoutes(RouteCollection routes, Assembly assemblyToSearch)
        {
            var decoratedMethods = from t in assemblyToSearch.GetTypes()
                                   where t.IsSubclassOf(typeof(System.Web.Mvc.Controller))
                                   from m in t.GetMethods()
                                   where m.IsDefined(typeof(RouteAttribute), false)
                                   select m;

            Debug.WriteLine(string.Format("MapDecoratedRoutes - found {0} methods decorated with RouteAttribute", decoratedMethods.Count()));

            var methodsToRegister = new SortedDictionary<RouteAttribute, MethodInfo>(); // sort urls alphabetically via RouteAttribute's IComparable implementation

            // first, collect all the methods decorated with our RouteAttribute
            foreach (var method in decoratedMethods)
            {
                foreach (var attr in method.GetCustomAttributes(typeof(RouteAttribute), false))
                {
                    var ra = (RouteAttribute)attr;
                    if (!methodsToRegister.Any(p => p.Key.Url.Equals(ra.Url)))
                        methodsToRegister.Add(ra, method);
                    else
                        Debug.WriteLine("MapDecoratedRoutes - found duplicate url -> " + ra.Url);
                }
            }

            // now register the unique urls to the Controller.Method that they were decorated upon
            foreach (var pair in methodsToRegister)
            {
                var attr = pair.Key;
                var method = pair.Value;
                var action = method.Name;

                var controllerType = method.ReflectedType;
                var controllerName = controllerType.Name.Replace("Controller", "");
                var controllerNamespace = controllerType.FullName.Replace("." + controllerType.Name, "");

                Debug.WriteLine(string.Format("MapDecoratedRoutes - mapping url '{0}' to {1}.{2}.{3}", attr.Url, controllerNamespace, controllerName, action));

                //determine Area automatically
                if (controllerNamespace.Contains(".Areas."))
                {
                    attr.Area = controllerNamespace.Split('.').SkipWhile(n => n != "Areas").ElementAt(1);
                }

                //register area
                string url = attr.Url;
                bool isAreaPresent = false;
                if (!String.IsNullOrWhiteSpace(attr.Area))
                {
                    isAreaPresent = true;
                    url = String.Format("{0}/{1}", attr.Area.ToLowerInvariant(), url);
                }


                var route = new LeftMatchingRoute(url, new MvcRouteHandler());


                route.Defaults = new RouteValueDictionary(new { controller = controllerName, action = action });

                // optional parameters are specified like: "users/filter/{filter?}"
                if (attr.OptionalParameters != null)
                {
                    foreach (var optional in attr.OptionalParameters)
                        route.Defaults.Add(optional, "");
                }

                // constraints are specified like: @"users/{id:\d+}" or "users/{id:INT}"
                if (attr.Constraints != null)
                {
                    var constraints = new Dictionary<string, IRouteConstraint>();

                    foreach (var constraint in attr.Constraints)
                    {
                        constraints.Add(constraint.Key, new RegexConstraint(constraint.Value));
                    }

                }

                // fully-qualify route to its controller method by adding the namespace; allows multiple assemblies to share controller names/routes
                // e.g. StackOverflow.Controllers.HomeController, StackOverflow.Api.Controllers.HomeController
                route.DataTokens = new RouteValueDictionary(new { namespaces = new[] { controllerNamespace } });

                //register area
                if (isAreaPresent)
                {
                    route.DataTokens.Add("area", attr.Area);
                }



                routes.Add(attr.Name, route);
            }
        }


        /// <summary>
        /// Simple route declaration, with just a url pattern.
        /// </summary>
        /// <param name="url">pattern to match, ex. "user/{id:INT}"; note the lack of leading "/"</param>
        public RouteAttribute(string url)
            : this(url, "", null, RoutePriority.Default)
        {
        }

        /// <summary>
        /// Route declaration, with a url pattern, and a set of permissable HttpVerbs.
        /// </summary>
        /// <param name="url">pattern to match, ex. "user/{id:INT}"; note the lack of leading "/"</param>
        /// <param name="verbs">Verbs the route should be valid for, accepts multiple ex. HttpVerbs.Post | HttpVerbs.Get</param>
        public RouteAttribute(string url, HttpVerbs verbs)
            : this(url, "", verbs, RoutePriority.Default)
        {
        }


        /// <summary>
        /// Route declaration, with a url pattern and a priority.
        /// </summary>
        /// <param name="url">pattern to match, ex. "user/{id:INT}"; note the lack of leading "/"</param>
        /// <param name="priority">priority controls the order of route matching, higher priority routes will be checked against before lower ones.</param>
        public RouteAttribute(string url, RoutePriority priority)
            : this(url, "", null, priority)
        {
        }


        /// <summary>
        /// Route declaration, with a url pattern, an set of pemissable HttpVerbs, and a priority.
        /// </summary>
        /// <param name="url">pattern to match, ex. "user/{id:INT}"; note the lack of leading "/"</param>
        /// <param name="verbs">Verbs the route should be valid for, accepts multiple ex. HttpVerbs.Post | HttpVerbs.Get.</param>
        /// <param name="priority">priority controls the order of route matching, higher priority routes will be checked against before lower ones.</param>
        public RouteAttribute(string url, HttpVerbs verbs, RoutePriority priority)
            : this(url, "", verbs, priority)
        {
        }

        private RouteAttribute(string url, string name, HttpVerbs? verbs, RoutePriority priority)
        {
            Url = url.ToLower();
            Name = name;
            AcceptVerbs = verbs;
            Priority = priority;
        }


        /// <summary>
        /// The explicit verbs that the route will allow.  If null, all verbs are valid.
        /// </summary>
        public HttpVerbs? AcceptVerbs { get; set; }

        /// <summary>
        /// Optional name to allow this route to be referred to later.
        /// </summary>
        public string Name { get; set; }

        private string _url;
        /// <summary>
        /// The request url that will map to the decorated action method.
        /// Specifying optional parameters: "/users/{id}/{name?}" where 'name' may be omitted.
        /// Specifying constraints on parameters: "/users/{id:(\d+)}" where 'id' matches a regex for at least one number
        /// Constraints can also be predefined: "/users/{id:INT}" where 'id' will be constrained to the predefined INT regex <see cref="PredefinedConstraints"/>.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = ParseUrlForConstraints(value); /* side-effects include setting this.OptionalParameters and this.Constraints */ }
        }

        /// <summary>
        /// Determines when this route is registered in the <see cref="System.Web.Routing.RouteCollection"/>.  The higher the priority, the sooner
        /// this route is added to the collection, making it match before other registered routes for a given url.
        /// </summary>
        public RoutePriority Priority { get; set; }

        /// <summary>
        /// Determines the area of the route being registered. It not set, it will automatically be generated from the namespace.
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// If true, ensures that the calling method
        /// </summary>
        public bool EnsureXSRFSafe { get; set; }

        /// <summary>
        /// Gets any optional parameters contained by this Url. Optional parameters are specified with a ?, e.g. "users/{id}/{name?}".
        /// </summary>
        public string[] OptionalParameters { get; private set; }

        /// <summary>
        /// Based on /users/{id:(\d+)(;\d+)*}
        /// </summary>
        public Dictionary<string, string> Constraints { get; private set; }

        /// <summary>
        /// Contains keys that can be used in routes for well-known constraints, e.g. "users/{id:INT}" - this route would ensure the 'id' parameter
        /// would only accept at least one number to match.
        /// </summary>
        public static readonly Dictionary<string, string> PredefinedConstraints = new Dictionary<string, string> 
                                                                                      { 
                                                                                          { "INT",            @"-?\d{1,9}" }, // yes, int32 could have 10 digits, but do we really think we'll have ids that big anytime soon?
                                                                                          { "INTS_DELIMITED", @"-?\d+(;-?\d+)*" },
                                                                                          { "GUID",           @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Za-z0-9]{12}\b" }
                                                                                      };

        public override bool IsValidForRequest(ControllerContext cc, MethodInfo mi)
        {
            bool result = true;

            if (AcceptVerbs.HasValue)
                result = new AcceptVerbsAttribute(AcceptVerbs.Value).IsValidForRequest(cc, mi);

            if (result && EnsureXSRFSafe)
            {
                if (!AcceptVerbs.HasValue || (AcceptVerbs.Value & HttpVerbs.Post) == 0)
                    throw new ArgumentException("When this.XSRFSafe is true, this.AcceptVerbs must include HttpVerbs.Post");

                // XSRF safety depends on your notion of a user, as such this line needs to be customized on a per-project basis -kmontrose
               
                IXSRFSafeRequest xsrfCheck = GlobalFilters.Filters.OfType<IXSRFSafeRequest>().FirstOrDefault();
                if (xsrfCheck==null)
                {
                    Debug.WriteLine("You need to register a IXSRFSafeRequest object using GlobalFilters");
                }
                else
                {
                    //implement IXSRFSafeRequest and register it in GlobalFilters.
                    result = xsrfCheck.IsSafe(cc, mi);
                }

            }

            return result;
        }

        public override string ToString()
        {
            return (AcceptVerbs.HasValue ? AcceptVerbs.Value.ToString().ToUpper() + " " : "") + Url;
        }

        public int CompareTo(RouteAttribute other)
        {
            var result = other.Priority.CompareTo(this.Priority);

            if (result == 0) // sort like priorities in asc alphabetical order
                result = this.Url.CompareTo(other.Url);

            return result;
        }

        private string ParseUrlForConstraints(string url)
        {
            // example url with both optional specifier and a constraint: "posts/{id:INT}/edit-submit/{revisionguid?:GUID}"
            // note that a constraint regex cannot use { } for quantifiers
            var matches = Regex.Matches(url, @"{(?<param>\w+)(?<metadata>(?<optional>\?)?(?::(?<constraint>[^}]*))?)}", RegexOptions.IgnoreCase);

            if (matches.Count == 0) return url; // vanilla route without any parameters, e.g. "home", "users/login"   

            var result = url;
            var optionals = new List<string>();
            var constraints = new Dictionary<string, string>();

            foreach (Match m in matches)
            {
                var metadata = m.Groups["metadata"].Value; // all the extra info after the parameter name
                if (!String.IsNullOrWhiteSpace(metadata)) // we have optional specifier and/or constraints
                {
                    var param = m.Groups["param"].Value; // the name, e.g. 'id' in "/users/{id}"
                    var isOptional = m.Groups["optional"].Success;

                    if (isOptional)
                        optionals.Add(param);

                    var constraint = m.Groups["constraint"].Value;
                    if (!String.IsNullOrWhiteSpace(constraint))
                    {
                        string predefined = null;
                        if (PredefinedConstraints.TryGetValue(constraint.ToUpper(), out predefined))
                            constraint = predefined;

                        if (isOptional)
                            constraint = "(" + constraint + ")?";

                        constraints.Add(param, constraint);
                    }

                    result = result.Replace(metadata, "");
                }
            }

            if (optionals.Count > 0) this.OptionalParameters = optionals.ToArray();
            if (constraints.Count > 0) this.Constraints = constraints;

            return result;
        }
    }
}