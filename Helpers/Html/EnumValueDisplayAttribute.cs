using System;

namespace Gizmo.Helpers.Html
{
    [AttributeUsage(AttributeTargets.Field,Inherited = false,AllowMultiple = false)]
    public sealed class EnumValueDisplayAttribute:Attribute
    {
        public string Name { get; set; }
    }
}