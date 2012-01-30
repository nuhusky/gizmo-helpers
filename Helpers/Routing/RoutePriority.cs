using System.Text;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Contains values that control when routes are added to the main <see cref="System.Web.Routing.RouteCollection"/>.
    /// </summary>
    /// <remarks>Routes with identical RoutePriority are registered in alphabetical order.  RoutePriority allows for different strata of routes.</remarks>
    public enum RoutePriority
    {
        /// <summary>
        /// A route with Low priority will be registered after routes with Default and High priorities.
        /// </summary>
        Low = 0,
        Default = 1,
        High = 2
    }
}
