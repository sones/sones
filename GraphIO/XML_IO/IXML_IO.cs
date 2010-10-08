/* 
 * IXML_IO
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Xml.Linq;

#endregion

namespace sones.GraphIO.XML
{

    /// <summary>
    /// A transformation interface for user-defined objects into an
    /// application/xml representation and vice versa.
    /// </summary>

    public interface IXML_IO
    {

        /// <summary>
        /// Serialize this object as XML
        /// </summary>
        XElement ToXML();

        /// <summary>
        /// Deserialize the content of this object from the given XML
        /// </summary>
        void FromXML(XElement myXML);

    }

}
