using System;

namespace Gizmo.Helpers
{
    static class Guard
    {
        
        public static void NotNullOrEmpty<T>(T valToCheck, string paramName)
        {
            Type derivedType = typeof (T);
            TypeCode derivedTypeCode = Type.GetTypeCode(derivedType);
            if (derivedTypeCode==TypeCode.String)
            {
                if (String.IsNullOrEmpty(valToCheck as String))
                {
                    throw new ArgumentNullException(paramName);
                }
            } else if (derivedTypeCode==TypeCode.Object)
            {
                if (valToCheck as Object == null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }

           
        }
    }
}