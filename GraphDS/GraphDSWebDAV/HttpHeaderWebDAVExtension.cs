/* <id name="GraphDSWebDAV – HttpHeaderWebDAVExtension" />
 * <copyright file="HttpHeaderWebDAVExtension.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>Some WebDAV specific httpHeader extensions</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Networking.WebDAV;
using sones.Networking.HTTP;

#endregion

namespace sones.GraphDS.Connectors.WebDAV
{
    public static class HttpHeaderWebDAVExtension
    {

        /// <summary>
        /// Depth = "Depth" ":" ("0" | "1" | "infinity")
        /// http://www.webdav.org/specs/rfc2518.html#HEADER_Depth
        /// The Depth header is used with methods executed on resources which could potentially have internal members to indicate whether the method is to be applied only to the resource ("Depth: 0"), to the resource and its immediate children, ("Depth: 1"), or the resource and all its progeny ("Depth: infinity").
        /// The Depth header is only supported if a method's definition explicitly provides for such support.
        /// </summary>        
        public static WebDAVDepth GetDepth(this HTTPHeader httpHeader)
        {
            Int16 depth = (Int16)WebDAVDepth.Infinite;

            if (httpHeader.Headers["Depth"] != null)
            {
                Int16.TryParse(httpHeader.Headers["Depth"], out depth);
            }

            return (WebDAVDepth)depth;
        }

        /// <summary>
        /// Get the full http host
        /// </summary>
        /// <param name="httpHeader"></param>
        /// <returns></returns>
        public static String GetFullHTTPHost(this HTTPHeader httpHeader)
        {
            return String.Concat(httpHeader.ProtocolName, "://", httpHeader.HostName);
        }

        /// <summary>
        /// Get the full http host with the httpHeader.Destination
        /// </summary>
        /// <param name="httpHeader"></param>
        /// <returns></returns>
        public static String FullHTTPDestinationPath(this HTTPHeader httpHeader)
        {
            return GetFullHTTPHost(httpHeader) + httpHeader.Destination;
        }
    }
}
