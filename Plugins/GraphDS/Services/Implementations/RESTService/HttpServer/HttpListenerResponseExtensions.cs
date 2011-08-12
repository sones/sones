using System;
using System.Net;
using System.Net.Mime;

namespace sones.GraphDS.Services.RESTService.Networking
{
    public static class HttpListenerResponseExtensions
    {
        public static void SetHttpStatusCode(this HttpListenerResponse myResponse, HttpStatusCode myStatusCode)
        {
            myResponse.StatusCode = (int) myStatusCode;
        }

        public static void SetCacheControl(this HttpListenerResponse myResponse, String myCacheControl)
        {
            myResponse.AddHeader("Cache-Control", myCacheControl);
        }
        
        public static void SetServerName(this HttpListenerResponse myResponse, String myServerName)
        {
            myResponse.AddHeader("Server", myServerName);
            
        }

        public static void SetContentType(this HttpListenerResponse myResponse, ContentType myContentType)
        {
            myResponse.ContentType = myContentType.ToString();
        }


        
    }
}
