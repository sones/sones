using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;

namespace sones.Networking.HTTP
{

    public static class Extensions
    {

        #region GetMediaType(this myContentType)

        /// <summary>
        /// Returns the mediatype without the subtype
        /// </summary>
        /// <param name="myContentType"></param>
        /// <returns></returns>
        public static String GetMediaType(this ContentType myContentType)
        {
            return myContentType.MediaType.Split(new[] { '/' })[0];
        }

        #endregion

        #region GetMediaSubType(this myContentType)

        /// <summary>
        /// Returns the media subtype
        /// </summary>
        /// <param name="myContentType"></param>
        /// <returns></returns>
        public static String GetMediaSubType(this ContentType myContentType)
        {
            return myContentType.MediaType.Split(new[] { '/' })[1];
        }

        #endregion

    }

}
