using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web;
using Jampad.Dojo.Rpc.Description;
//+
namespace Jampad.Dojo.Rpc
{
    public abstract class DojoRpcServiceBase : IHttpHandler
    {
        //- $TypeFullName -//
        private String TypeFullName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        //- @IsReusable -//
        public Boolean IsReusable
        {
            get { return false; }
        }

        //- $CallServiceMethod -//
        private Object CallServiceMethod(DojoMessage data)
        {
            Method method = TypeScanner.GetMethod(this.GetType(), data.Method);
            Object[] parameters = TypeScanner.MatchProperties(method, data.Params);
            if (method != null)
            {
                return method.MethodInfo.Invoke(this, parameters);
            }
            else
            {
                return null;
            }
        }

        //- @ProcessRequest -//
        public void ProcessRequest(HttpContext context)
        {
            if (!ReflectionCache.ContainsSpecificMethodCatalog(this.TypeFullName))
            {
                TypeScanner.SetMappedMethods(this.GetType());
            }
            Stream stream = context.Request.InputStream;
            if (context.Request.Url.AbsoluteUri.EndsWith("?smd"))
            {
                if (!ReflectionCache.ContainsSpecificServiceDescription(this.TypeFullName))
                {
                    String url = context.Request.Url.AbsoluteUri;
                    ReflectionCache.AddServiceDescription(this.TypeFullName, DescriptionCreator.CreateDescription(this.GetType(), url));
                }
                DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(DojoServiceDescription));
                s.WriteObject(context.Response.OutputStream, ReflectionCache.GetSpecificServiceDescription(this.TypeFullName));
            }
            else
            {
                if (stream != null && stream.Length > 0)
                {
                    DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(DojoMessage));
                    DojoMessage o = null;
                    try
                    {
                        o = (DojoMessage)s.ReadObject(stream);
                    }
                    catch
                    {
                        context.Response.Write("Error in JSON message");
                    }
                    if (o != null)
                    {
                        Object r = CallServiceMethod(o);
                        s.WriteObject(context.Response.OutputStream, r);
                    }
                }
            }
        }
    }
}