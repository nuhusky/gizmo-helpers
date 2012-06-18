using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gizmo.Helpers.Routing
{
    /// <summary>
    /// Mvc Url Request object. Wraps UrlHelper.Action call in a chainable interface.
    /// </summary>
    public class MvcUrlRequest
    {
        private const string HTTP = "http";
        private const string HTTPS = "https";
        private readonly string _action;
        private readonly string _controller;
        private readonly UrlHelper _urlHelper;
        private string _area;
        private string _domainName = String.Empty;
        private string _protocol = HTTP;
        private RouteValueDictionary _rvd;

        public MvcUrlRequest(UrlHelper urlHelper, string action, string controller, string areaName)
        {
            Guard.NotNullOrEmpty(action, "action");
            Guard.NotNullOrEmpty(controller, "controller");
            Guard.NotNullOrEmpty(urlHelper,"urlHelper");

            _area = areaName;
            _action = action;
            _controller = controller;
            _urlHelper = urlHelper;
        }


        public MvcUrlRequest WithAreaOverride(string area)
        {
            Guard.NotNullOrEmpty(area, "area");

            _area = area;
            return this;
        }

        public MvcUrlRequest WithRouteValues(object routeValues)
        {
            _rvd = new RouteValueDictionary(routeValues);
            return this;
        }

        public MvcUrlRequest UsingHttps()
        {
            _protocol = HTTPS;
            return this;
        }


        public MvcUrlRequest ForDomain(string domainName)
        {
            Guard.NotNullOrEmpty(domainName, "domainName");
            _domainName = domainName;
            return this;
        }

        public virtual string CreateUrl()
        {
            //Initialize route values if needed
            if (_rvd == null)
            {
                _rvd = new RouteValueDictionary();
            }

            //Add area if present
            if (!String.IsNullOrEmpty(_area))
            {
                _rvd.Add("area", _area);
            }

            return _urlHelper.Action(_action, _controller, _rvd, _protocol, _domainName);
        }

        public override string ToString()
        {
            return CreateUrl();
        }


        
    }
}