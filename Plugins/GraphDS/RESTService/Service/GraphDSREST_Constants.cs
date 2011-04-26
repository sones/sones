#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;

#endregion

namespace sones.Plugins.GraphDS.RESTService
{
    public static class GraphDSREST_Constants
    {
        public static ContentType _JSON = new ContentType("application/json");
        public static ContentType _XML = new ContentType("application/xml");
        public static ContentType _TEXT = new ContentType(MediaTypeNames.Text.Plain);
        public static ContentType _HTML = new ContentType("text/html");
        public static ContentType _GEXF = new ContentType("application/gexf");

        public static ContentType _JSON_UTF8 = new ContentType("application/json") { CharSet = "UTF-8" };
        public static ContentType _XML_UTF8 = new ContentType("application/xml") { CharSet = "UTF-8" };
        public static ContentType _TEXT_UTF8 = new ContentType(MediaTypeNames.Text.Plain) { CharSet = "UTF-8" };
        public static ContentType _HTML_UTF8 = new ContentType("text/html") { CharSet = "UTF-8" };
        public static ContentType _GEXF_UTF8 = new ContentType("application/gexf") { CharSet = "UTF-8" };

        public static ContentType _CSS = new ContentType("text/css");
        public static ContentType _GIF = new ContentType("image/gif");
        public static ContentType _ICO = new ContentType("image/ico");
        public static ContentType _PNG = new ContentType("image/png");
        public static ContentType _JPG = new ContentType("image/jpg");

        public static ContentType _OCTET = new ContentType("application/octet-stream");
    
    }
}
