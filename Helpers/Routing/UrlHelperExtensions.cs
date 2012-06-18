using System.Web.Mvc;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// UrlHelper extensions class
    /// </summary>
    public static class UrlHelperExtensions
    {
        public static MvcUrlRequest Action<T>(this UrlHelper urlHeler, T val) where T : struct
        {
            MvcRequestUrlBuilder builder = new MvcRequestUrlBuilder(urlHeler);

            return builder.GetRequest(val);
        }
    }
}