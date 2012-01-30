using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Copied from : http://samsaffron.com/archive/2011/10/13/optimising-asp-net-mvc3-routing
    /// Imporoves route match performance for regex constraints
    /// </summary>
    public class RegexConstraint : IRouteConstraint
    {
        Regex regex;

        public RegexConstraint(string pattern, RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase)
        {
            regex = new Regex(pattern, options);
        }

        public bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object val;
            values.TryGetValue(parameterName, out val);
            string input = Convert.ToString(val, CultureInfo.InvariantCulture);
            return regex.IsMatch(input);
        }

    }
}