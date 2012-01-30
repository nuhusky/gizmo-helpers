using System.Globalization;
using System.Web.Routing;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Copied from : http://samsaffron.com/archive/2011/10/13/optimising-asp-net-mvc3-routing
    /// Looks for string literal match in the left (root) url segment
    /// </summary>
    internal class LeftMatchingRoute : Route
    {
        private readonly string neededOnTheLeft;

        public LeftMatchingRoute(string url, IRouteHandler handler)
            : base(url, handler)
        {
            int idx = url.IndexOf('{');
            neededOnTheLeft = "~/" + (idx >= 0 ? url.Substring(0, idx) : url).TrimEnd('/');
        }

        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            if (
                !httpContext.Request.AppRelativeCurrentExecutionFilePath.StartsWith(neededOnTheLeft, true,
                                                                                    CultureInfo.InvariantCulture))
                return null;
            return base.GetRouteData(httpContext);
        }
    }
}