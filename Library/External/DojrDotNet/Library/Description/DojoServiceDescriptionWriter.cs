using System;
using System.IO;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Reflection;
//+
namespace Jampad.Dojo.Rpc.Description
{
    internal static class DojoServiceDescriptionWriter
    {
        //- $ScanMethods -//
        private static void ScanMethods(Type type, DojoServiceDescription desc)
        {
            foreach (MethodInfo method in type.GetMethods())
            {
                Object[] os = method.GetCustomAttributes(typeof(DojoOperationAttribute), false);
                if (os.Length > 0)
                {
                    DojoServiceDescriptionMethod m = new DojoServiceDescriptionMethod(method.Name);
                    SearchAttributes(os, m);
                    desc.MethodList.Add(m);
                }
            }
            desc.ServiceType = "JSON-RPC";
        }

        //- $SearchAttributes -//
        private static void SearchAttributes(Object[] os, DojoServiceDescriptionMethod m)
        {
            DojoOperationAttribute doa = null;
            foreach (Object o in os)
            {
                if (o is DojoOperationAttribute)
                {
                    doa = (DojoOperationAttribute)o;
                    break;
                }
            }
            //+
            m.ParameterList.Add(new DojoServiceDescriptionMethodParameter(doa.Name));
        }

        //- ~Write -//
        internal static void Write(DojoRpcServiceBase dojoService)
        {
            DojoServiceDescription desc = new DojoServiceDescription();
            desc.ServiceURL = HttpContext.Current.Request.Url.AbsoluteUri;
            //+
            Type t = dojoService.GetType();
            ScanMethods(t, desc);
            //+
            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(DojoServiceDescription));
            s.WriteObject(HttpContext.Current.Response.OutputStream, desc);
        }
    }
}