using System.Web.Mvc;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Builds MvcUrl Request from passed in enum field
    /// </summary>
    public class MvcRequestUrlBuilder : RequestUrlBuilder<MvcUrlRequest>
    {
        protected readonly UrlHelper _urlHelper;
        public MvcRequestUrlBuilder(UrlHelper urlHelper) 
        {
            Guard.NotNullOrEmpty(urlHelper, "urlHelper");
            _urlHelper = urlHelper;
        }

        public override MvcUrlRequest GetRequest<T>(T actionEnum)
        {
            string action = GetActionName(actionEnum);
            string controller = GetControllerName(actionEnum);
            string area = GetAreaName(actionEnum);
            return new MvcUrlRequest(_urlHelper, action, controller, area);
        }
    }
}