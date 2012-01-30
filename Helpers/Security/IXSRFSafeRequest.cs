using System.Reflection;
using System.Web.Mvc;

namespace Gizmo.Helpers.Security
{
    public interface IXSRFSafeRequest
    {
        bool IsSafe(ControllerContext cc, MethodInfo mi);
    }
}