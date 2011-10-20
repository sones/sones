using System;
using MethodCatalog = System.Collections.Generic.Dictionary<System.String, Jampad.Dojo.Rpc.Method>;
//+
namespace Jampad.Dojo.Rpc.Description
{
    internal static class DescriptionCreator
    {
        //- ~CreateDescription -//
        internal static DojoServiceDescription CreateDescription(Type type, String url)
        {
            DojoServiceDescription desc = new DojoServiceDescription();
            desc.ServiceURL = url.Substring(0, url.Length - 4);
            MethodCatalog mc = ReflectionCache.GetSpecificMethodCatalog(type.FullName);
            foreach (String m in mc.Keys)
            {
                DojoServiceDescriptionMethod dm = new DojoServiceDescriptionMethod(m);
                foreach (String p in mc[m].Parameters.Keys)
                {
                    dm.ParameterList.Add(new DojoServiceDescriptionMethodParameter(p));
                }
                desc.MethodList.Add(dm);
            }
            //+
            return desc;
        }
    }
}